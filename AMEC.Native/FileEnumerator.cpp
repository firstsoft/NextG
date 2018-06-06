#include "FileEnumerator.h"

using namespace AMEC::Native;

IEnumerable<FileInfoEx^>^ DirectoryEx::GetFiles(String^ dir, String^ searchPattern, SearchOption searchOption)
{
	if (dir == nullptr)
	{
		throw gcnew ArgumentNullException("path");
	}

	if (searchPattern == nullptr)
	{
		throw gcnew ArgumentNullException("searchPattern");
	}

	if ((searchOption != SearchOption::TopDirectoryOnly) && (searchOption != SearchOption::AllDirectories))
	{
		throw gcnew ArgumentOutOfRangeException("searchOption");
	}

	if (!Directory::Exists(dir))
	{
		return nullptr;
	}

	String^ fullPath = Path::GetFullPath(dir);

	return gcnew FileEnumerable(fullPath, searchPattern, searchOption);
}

bool FileEnumerator::MoveNext()
{
	bool retval = false;
	WIN32_FIND_DATAW m_win_find_data;

	//If the handle is null, this is first call to MoveNext in the current 
	// directory.  In that case, start a new search.
	if (m_currentContext->SubdirectoriesToProcess == nullptr)
	{
		if (m_hndFindFile == nullptr)
		{
			(gcnew FileIOPermission(FileIOPermissionAccess::PathDiscovery, m_path))->Demand();

			String^ searchPath = Path::Combine(m_path, m_filter);
			pin_ptr<const wchar_t> curSearchPath = PtrToStringChars(searchPath);
			m_hndFindFile = FindFirstFile(curSearchPath, &m_win_find_data);

			retval = m_hndFindFile != INVALID_HANDLE_VALUE;
		}
		else
		{
			//Otherwise, find the next item.
			retval = FindNextFile(m_hndFindFile, &m_win_find_data) == TRUE;
		}
	}

	//If the call to FindNextFile or FindFirstFile succeeded...
	if (retval)
	{
		if (((FileAttributes)m_win_find_data.dwFileAttributes & FileAttributes::Directory) == FileAttributes::Directory)
		{
			//Ignore folders for now.   We call MoveNext recursively here to 
			// move to the next item that FindNextFile will return.
			return MoveNext();
		}
		else
		{
			this->m_FileInfoEx = gcnew FileInfoEx();
			this->m_FileInfoEx->Attributes = (FileAttributes)m_win_find_data.dwFileAttributes;
			this->m_FileInfoEx->CreationTime = DateTime::FromFileTimeUtc(HightLow2Long(m_win_find_data.ftCreationTime.dwHighDateTime, m_win_find_data.ftCreationTime.dwHighDateTime));
			this->m_FileInfoEx->LastAccessTime = DateTime::FromFileTimeUtc(HightLow2Long(m_win_find_data.ftLastAccessTime.dwHighDateTime, m_win_find_data.ftLastAccessTime.dwHighDateTime));
			this->m_FileInfoEx->LastWriteTime = DateTime::FromFileTimeUtc(HightLow2Long(m_win_find_data.ftLastWriteTime.dwHighDateTime, m_win_find_data.ftLastWriteTime.dwHighDateTime));
			this->m_FileInfoEx->Size = HightLow2Long(m_win_find_data.nFileSizeHigh, m_win_find_data.nFileSizeLow);
			this->m_FileInfoEx->Name = gcnew String(m_win_find_data.cFileName);
			this->m_FileInfoEx->ParentDir = m_path;
		}
	}
	else if (m_searchOption == SearchOption::AllDirectories)
	{
		//SearchContext context = new SearchContext(m_hndFindFile, m_path);
		//m_contextStack.Push(context);
		//m_path = Path.Combine(m_path, m_win_find_data.cFileName);
		//m_hndFindFile = null;

		if (m_currentContext->SubdirectoriesToProcess == nullptr)
		{
			array<String^>^ subDirectories = Directory::GetDirectories(m_path);
			m_currentContext->SubdirectoriesToProcess = gcnew Stack<String^>(subDirectories);
		}

		if (m_currentContext->SubdirectoriesToProcess->Count > 0)
		{
			String^ subDir = m_currentContext->SubdirectoriesToProcess->Pop();

			m_contextStack->Push(m_currentContext);
			m_path = subDir;
			m_hndFindFile = nullptr;
			m_currentContext = gcnew SearchContext(m_path);
			return MoveNext();
		}

		//If there are no more files in this directory and we are 
		// in a sub directory, pop back up to the parent directory and
		// continue the search from there.
		if (m_contextStack->Count > 0)
		{
			m_currentContext = m_contextStack->Pop();
			m_path = m_currentContext->Path;
			if (m_hndFindFile != nullptr)
			{
				FindClose(m_hndFindFile);
				m_hndFindFile = nullptr;
			}

			return MoveNext();
		}
	}

	return retval;
}