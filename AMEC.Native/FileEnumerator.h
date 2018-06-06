#pragma once
#define _X86_

#include <stdio.h>
#include <vcclr.h> 
#include <windef.h>
#include <windows.h>
#include "WHReader.h"
#define HightLow2Long(high,low) (((long long)high) << 0x20) | low

namespace AMEC
{
	namespace Native
	{
		private ref class SearchContext
		{
		public:
			initonly String^ Path;
			Stack<String^>^ SubdirectoriesToProcess;

			SearchContext(String^ path)
			{
				this->Path = path;
			}
		};

		private ref class FileEnumerator : IEnumerator<FileInfoEx^>
		{
		private:
			String^ m_path;
			String^ m_filter;
			SearchOption m_searchOption;
			HANDLE m_hndFindFile;
			Stack<SearchContext^>^ m_contextStack;
			SearchContext^ m_currentContext;
			FileInfoEx^ m_FileInfoEx = nullptr;

		public:
			FileEnumerator(String^ path, String^ filter, SearchOption searchOption)
			{
				m_path = path;
				m_filter = filter;
				m_searchOption = searchOption;

				m_currentContext = gcnew SearchContext(path);

				if (m_searchOption == SearchOption::AllDirectories)
				{
					m_contextStack = gcnew Stack<SearchContext^>();
				}
			}

			~FileEnumerator()
			{
				this->!FileEnumerator();
			}

			!FileEnumerator()
			{
				if (m_hndFindFile != nullptr)
				{
					FindClose(m_hndFindFile);
					m_hndFindFile = nullptr;
				}
			}

			virtual property FileInfoEx^ Current
			{
				FileInfoEx^ get() = IEnumerator<FileInfoEx^>::Current::get
				{ return m_FileInfoEx; }
			}

			virtual property Object^ Current1
			{
				Object^ get() = System::Collections::IEnumerator::Current::get
				{ return this->Current; }
			}

			virtual void Reset()
			{
				this->!FileEnumerator();
			}

			virtual bool MoveNext();
		};

		private ref class FileEnumerable : public IEnumerable<FileInfoEx^>
		{
		private:
			initonly String^ m_path;
			initonly String^ m_filter;
			initonly SearchOption m_searchOption;
		public:
			FileEnumerable(String^ path, String^ filter, SearchOption searchOption)
			{
				m_path = path;
				m_filter = filter;
				m_searchOption = searchOption;
			}

			virtual System::Collections::Generic::IEnumerator<FileInfoEx^>^ GetEnumerator() = System::Collections::Generic::IEnumerable<FileInfoEx^>::GetEnumerator
			{
				return gcnew FileEnumerator(m_path, m_filter, m_searchOption);
			}

			virtual System::Collections::IEnumerator^ GetEnumeratorOld() = System::Collections::IEnumerable::GetEnumerator
			{
				return gcnew FileEnumerator(m_path, m_filter, m_searchOption);
			}
		};


		public ref class DirectoryEx
		{
		public:
			static IEnumerable<FileInfoEx^>^ GetFiles(String^ path, String^ searchPattern, SearchOption searchOption);
		};
	}
}