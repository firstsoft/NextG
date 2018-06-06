#pragma once
#define _X86_

#include <stdio.h>
#include <vcclr.h> 
#include <Windows.h>
#include "Common.h"

typedef struct __WH_EVENT_INTERNAL
{
	unsigned short      wMsgLenth;      //message string length    
	BYTE                EventType;      //event type
	BYTE                EventGroup;     //event group
	unsigned short      wEventID;       //event ID
	unsigned short      wModuleID;      //module ID
	unsigned short      wSubModuleID;   //submodule ID
	BYTE                DataType[4];	//data type
	BYTE                UnitType[4];    //unit type
	float               fData[4];       //float value
	int                 nData[4];       //int value 
	SYSTEMTIME          stTime;         //system time when event occur
	TCHAR               szMsg[160];     //event string
}WH_EVENT_INTERNAL;


namespace AMEC
{
	namespace Native
	{
		using namespace System;
		using namespace System::IO;
		using namespace System::ComponentModel;
		using namespace System::Collections::Generic;
		using namespace System::Security::Permissions;
		using namespace System::Windows::Media;
		using namespace System::ComponentModel;
		using namespace LWIR::NET::Entity;

		public ref class Notify :INotifyPropertyChanged
		{
		public:
			virtual event PropertyChangedEventHandler^ PropertyChanged;

			virtual void RaisePropertyChanged(String^ propertyName)
			{
				PropertyChanged(this, gcnew PropertyChangedEventArgs(propertyName));
			}
		};

		public enum class EventType
		{
			DEBUG = 0,
			DEBUG_ENABLE = 1,
			INFO = 2,
			WARNING = 3,
			ALARM = 4,
		};

		public enum class GroupType
		{
			SYSTEM = 0,
			EFEM = 1,
			LL = 2,
			BF = 3,
			TM = 4,
			PM = 5,
			FA = 6,
			FDC = 7,
			GAS = 8,
			DP = 9,
			PULSE = 10,
			VCE = 11,
		};

		public enum class FileType
		{
			SH2,
			WH3,
			CH3,
			UNKNOWN
		};

		public enum class AttrType
		{
			Int,
			String,
			Float
		};

		public enum class DataType
		{
			None = 0,
			Int = 1,
			Float = 2,
			String = 3,
			Attribute = 4,
			Enum = 5,
			IntHex = 6
		};

		public enum class Function
		{
			None,
			Gas,
			GasSplitter,
			ESCTemperature,
			Enum
		};

		public ref class FileInfoEx :Notify
		{
		private:
			long size;
			String^ name;
			String^ fullfile;
			String^ parentDir;
		public:
			FileAttributes Attributes;
			DateTime CreationTime;
			DateTime LastAccessTime;
			DateTime LastWriteTime;

			AMEC::Native::FileType GetFileType()
			{
				String^ ext = System::IO::Path::GetExtension(this->Name);

				ext = ext == nullptr ? "" : ext;

				AMEC::Native::FileType fileType = AMEC::Native::FileType::UNKNOWN;
				if (ext->Equals(".sh2"))
				{
					fileType = AMEC::Native::FileType::SH2;
				}
				else if (ext->Equals(".wh3"))
				{
					fileType = AMEC::Native::FileType::WH3;
				}
				else if (ext->Equals(".ch3"))
				{
					fileType = AMEC::Native::FileType::CH3;
				}

				return fileType;
			}

			property long Size
			{
				long get(){ return size; }
				void set(long value)
				{
					size = value;
					RaisePropertyChanged("Size");
				}
			}

			property String^ Name
			{
				String^ get(){ return name; }
				void set(String^ value)
				{
					name = value;
					RaisePropertyChanged("Name");
				}
			}

			property String^ ParentDir
			{
				String^ get()
				{
					return parentDir;
				}

				void set(String^ value)
				{
					parentDir = value;
					RaisePropertyChanged("ParentDir");
					RaisePropertyChanged("Fullfile");
				}
			}

			property String^ Fullfile
			{
				String^ get()
				{
					return String::IsNullOrWhiteSpace(parentDir) ? nullptr : Path::Combine(this->parentDir, this->name);
				}
			}

			property bool IsExists
			{
				bool get()
				{
					if (Fullfile == nullptr)
						return false;

					return File::Exists(Fullfile);
				}
			}
		};

		public ref class FileHeader :Notify
		{
		private:
			unsigned short majorVersion;
			unsigned short minorVersion;
		public:
			unsigned int FileHeaderFlag;

			property String^ FileHeaderName
			{
				String^ get()
				{
					array<unsigned char, 1>^ tempBuffer = BitConverter::GetBytes(FileHeaderFlag);
					wchar_t tempStr[5];
					tempStr[0] = tempBuffer[0];
					tempStr[1] = tempBuffer[1];
					tempStr[2] = tempBuffer[2];
					tempStr[3] = tempBuffer[3];
					tempStr[4] = '\0';

					return gcnew String(tempStr);
				}
			}

			property unsigned short MajorVersion
			{
				unsigned short get(){ return majorVersion; }
				void set(unsigned short value)
				{
					majorVersion = value;
					RaisePropertyChanged("MajorVersion");
				}
			}

			property unsigned short MinorVersion
			{
				unsigned short get(){ return minorVersion; }
				void set(unsigned short value)
				{
					minorVersion = value;
					RaisePropertyChanged("MinorVersion");
				}
			}
		};

		public ref class SeqInfo :Notify
		{
		private:
			CMOD_ID mOD_ID;
			int waferCount;
			int reserved;
			DateTime seqStartTime;
			DateTime seqEndTime;
			String^ sequenceFullName;

		public:
			property CMOD_ID MOD_ID
			{
				CMOD_ID get(){ return mOD_ID; }
				void set(CMOD_ID value)
				{
					mOD_ID = value;
					RaisePropertyChanged("MOD_ID");
				}
			}

			property int WaferCount
			{
				int get(){ return waferCount; }
				void set(int value)
				{
					waferCount = value;
					RaisePropertyChanged("WaferCount");
				}
			}

			property int Reserved
			{
				int get(){ return reserved; }
				void set(int value){ reserved = value; }
			}

			property DateTime SeqStartTime
			{
				DateTime get(){ return seqStartTime; }
				void set(DateTime value)
				{
					seqStartTime = value;
					RaisePropertyChanged("SeqStartTime");
				}
			}

			property DateTime SeqEndTime
			{
				DateTime get(){ return seqEndTime; }
				void set(DateTime value)
				{
					seqEndTime = value;
					RaisePropertyChanged("SeqEndTime");
				}
			}

			property String^ SequenceName
			{
				String^ get()
				{
					if (String::IsNullOrEmpty(sequenceFullName))
						return String::Empty;

					return System::IO::Path::GetFileName(sequenceFullName);
				}
			}

			property String^ SequenceFullName
			{
				String^ get(){ return sequenceFullName; }
				void set(String^ value)
				{
					sequenceFullName = value;
					RaisePropertyChanged("SequenceFullName");
					RaisePropertyChanged("SequenceName");
				}
			}
		};

		public ref class SeqWaferInfo :Notify
		{
		private:
			unsigned int uniqueNumber;
			int slotId;
			int reserved;
			DateTime waferCreateTime;
			String^ waferHistoryFullfileName;
		public:
			property unsigned int UniqueNumber
			{
				unsigned int get(){ return uniqueNumber; }
				void set(unsigned int value)
				{
					uniqueNumber = value;
					RaisePropertyChanged("UniqueNumber");
				}
			}

			property int SlotId
			{
				int get(){ return slotId; }
				void set(int value)
				{
					slotId = value;
					RaisePropertyChanged("SlotId");
				}
			}

			property int Reserved
			{
				int get(){ return reserved; }
				void set(int value){ reserved = value; }
			}

			property DateTime WaferCreateTime
			{
				DateTime get(){ return waferCreateTime; }
				void set(DateTime value)
				{
					waferCreateTime = value;
					RaisePropertyChanged("WaferCreateTime");
				}
			}

			property String^ WaferHistoryFullfileName
			{
				String^ get(){ return waferHistoryFullfileName; }
				void set(String^ value)
				{
					waferHistoryFullfileName = value;
					RaisePropertyChanged("WaferHistoryFullfileName");
					RaisePropertyChanged("WaferHistoryFileName");
				}
			}

			property String^ WaferHistoryFileName
			{
				String^ get()
				{
					if (String::IsNullOrEmpty(waferHistoryFullfileName))
						return String::Empty;

					return System::IO::Path::GetFileName(waferHistoryFullfileName);
				}
			}
		};

		public ref class SH2Info :Notify
		{
		private:
			FileInfoEx^ fileInfoEx = nullptr;
			FileHeader^ header = gcnew FileHeader();
			SeqInfo^ sequenceInfo = gcnew SeqInfo();
			array<SeqWaferInfo^>^ seqWaferInfos = nullptr;
			bool isInfoIntegrity = false;
		public:
			property FileInfoEx^ CurFileInfoEx
			{
				FileInfoEx^ get(){ return fileInfoEx; }
				void set(FileInfoEx^ value)
				{
					fileInfoEx = value;
					RaisePropertyChanged("CurFileInfoEx");
				}
			}

			property FileHeader^ Header
			{
				FileHeader^ get(){ return header; }
				void set(FileHeader^ value)
				{
					header = value;
					RaisePropertyChanged("Header");
				}
			}

			property SeqInfo^ SequenceInfo
			{
				SeqInfo^ get(){ return sequenceInfo; }
				void set(SeqInfo^ value)
				{
					sequenceInfo = value;
					RaisePropertyChanged("SequenceInfo");
				}
			}

			property array<SeqWaferInfo^>^ SeqWaferInfos
			{
				array<SeqWaferInfo^>^ get(){ return seqWaferInfos; }
				void set(array<SeqWaferInfo^>^ value)
				{
					seqWaferInfos = value;
					RaisePropertyChanged("SeqWaferInfos");
				}
			}

			property bool IsInfoIntegrity
			{
				bool get(){ return isInfoIntegrity; }
				void set(bool value)
				{
					isInfoIntegrity = value;
					RaisePropertyChanged("IsInfoIntegrity");
				}
			}

			void FillInfos();
		};

		public ref class BaseReader abstract
		{
		public:
			BaseReader();
			virtual ~BaseReader();
			!BaseReader();
			bool Open(String^ fullfile);
			void Close();

		protected:
			FILE* fp;
			String^ curFullfile = String::Empty;
			String^ historyFolder = String::Empty;
			unsigned long long lastValidPos;
			bool ReadHeader(FileHeader^ info);
			virtual String^ FileChangeParentDir(String^ oldfullfile);
			virtual String^ RecipeChangeParentDir(String^ oldfullfile);
		};

		public ref class SH2Reader :BaseReader
		{
		public:
			void Read(SH2Info^ info);

		private:
			void ReadSeqInfo(SeqInfo^ info);
			void ReadSeqWaferInfos(SH2Info^ info);
		};



		public ref class WaferInfo :Notify
		{
		private:
			unsigned int uniqueNumber;
			int reserved;
			int waferStatus;
			CMOD_ID mOD_ID;
			int slotId;
			DateTime waferCreateTime;
			int loadedTimes;
			String^ waferId = String::Empty;
			String^ lotId = String::Empty;
			String^ foupId = String::Empty;
		public:
			property unsigned int UniqueNumber
			{
				unsigned int get(){ return uniqueNumber; }
				void set(unsigned int value)
				{
					uniqueNumber = value;
					RaisePropertyChanged("UniqueNumber");
				}
			}

			property int Reserved
			{
				int get(){ return reserved; }
				void set(int value){ reserved = value; }
			}

			property int WaferStatus
			{
				int get(){ return waferStatus; }
				void set(int value)
				{
					waferStatus = value;
					RaisePropertyChanged("WaferStatus");
				}
			}

			property CMOD_ID MOD_ID
			{
				CMOD_ID get(){ return mOD_ID; }
				void set(CMOD_ID value)
				{
					mOD_ID = value;
					RaisePropertyChanged("MOD_ID");
				}
			}

			property int SlotId
			{
				int get(){ return slotId; }
				void set(int value)
				{
					slotId = value;
					RaisePropertyChanged("SlotId");
				}
			}

			property DateTime WaferCreateTime
			{
				DateTime get(){ return waferCreateTime; }
				void set(DateTime value)
				{
					waferCreateTime = value;
					RaisePropertyChanged("WaferCreateTime");
				}
			}

			property int LoadedTimes
			{
				int get(){ return loadedTimes; }
				void set(int value)
				{
					loadedTimes = value;
					RaisePropertyChanged("LoadedTimes");
				}
			}

			property String^ WaferId
			{
				String^ get(){ return waferId; }
				void set(String^ value)
				{
					waferId = value;
					RaisePropertyChanged("WaferId");
				}
			}

			property String^ LotId
			{
				String^ get(){ return lotId; }
				void set(String^ value)
				{
					lotId = value;
					RaisePropertyChanged("LotId");
				}
			}

			property String^ FoupId
			{
				String^ get(){ return foupId; }
				void set(String^ value)
				{
					foupId = value;
					RaisePropertyChanged("FoupId");
				}
			}
		};

		public ref class RecipeItem :Notify
		{
		private:
			unsigned int attributeIndex;
			AttrType attributeType;
			String^ unit;
			String^ attributeName;

		public:
			property unsigned int AttributeIndex
			{
				unsigned int get(){ return attributeIndex; }
				void set(unsigned int value)
				{
					attributeIndex = value;
					RaisePropertyChanged("AttributeIndex");
				}
			}

			property AttrType AttributeType
			{
				AttrType get(){ return attributeType; }
				void set(AttrType value)
				{
					attributeType = value;
					RaisePropertyChanged("AttributeType");
				}
			}

			property String^ Unit
			{
				String^ get(){ return unit; }
				void set(String^ value)
				{
					unit = value;
					RaisePropertyChanged("Unit");
				}
			}

			property String^ AttributeName
			{
				String^ get(){ return attributeName; }
				void set(String^ value)
				{
					attributeName = value;
					RaisePropertyChanged("AttributeName");
				}
			}
		};

		public ref class RecipeInfoHeader :Notify
		{
		private:
			unsigned int processTimes;
			int reserved;
		public:
			property unsigned int ProcessTimes
			{
				unsigned int get(){ return processTimes; }
				void set(unsigned int value)
				{
					processTimes = value;
					RaisePropertyChanged("ProcessTimes");
				}
			}

			property int Reserved
			{
				int get(){ return reserved; }
				void set(int value){ reserved = value; }
			}
		};

		public ref class RecipeInfo :Notify
		{
		private:
			CMOD_ID mOD_ID;
			CRecipeType recipeType;
			int recipeItemsCount;
			int processResult;
			int eventNumber;
			float waferAngle;
			DateTime recipeStartTime;
			DateTime recipeEndTime;
			short totalStepCount;
			short completedStepCount;
			short perStepNumber;
			short postStepNumber;
			String^ recipeName;
			String^ perRecipeName;
			String^ postRecipeName;
			String^ dataCollectionFileName;
			String^ endpointRawDataFileName;
			int rf;

		public:
			property CMOD_ID MOD_ID
			{
				CMOD_ID get(){ return mOD_ID; }
				void set(CMOD_ID value)
				{
					mOD_ID = value;
					RaisePropertyChanged("MOD_ID");
				}
			}

			property CRecipeType RecipeType
			{
				CRecipeType get(){ return recipeType; }
				void set(CRecipeType value)
				{
					recipeType = value;
					RaisePropertyChanged("RecipeType");
				}
			}

			property int RecipeItemsCount
			{
				int get(){ return recipeItemsCount; }
				void set(int value)
				{
					recipeItemsCount = value;
					RaisePropertyChanged("RecipeItemsCount");
				}
			}

			property int ProcessResult
			{
				int get(){ return processResult; }
				void set(int value)
				{
					processResult = value;
					RaisePropertyChanged("ProcessResult");
				}
			}

			property int EventNumber
			{
				int get(){ return eventNumber; }
				void set(int value)
				{
					eventNumber = value;
					RaisePropertyChanged("EventNumber");
				}
			}

			property float WaferAngle
			{
				float get(){ return waferAngle; }
				void set(float value)
				{
					waferAngle = value;
					RaisePropertyChanged("WaferAngle");
				}
			}

			property DateTime RecipeStartTime
			{
				DateTime get(){ return recipeStartTime; }
				void set(DateTime value)
				{
					recipeStartTime = value;
					RaisePropertyChanged("RecipeStartTime");
				}
			}

			property DateTime RecipeEndTime
			{
				DateTime get(){ return recipeEndTime; }
				void set(DateTime value)
				{
					recipeEndTime = value;
					RaisePropertyChanged("RecipeEndTime");
				}
			}

			property short TotalStepCount
			{
				short get(){ return totalStepCount; }
				void set(short value)
				{
					totalStepCount = value;
					RaisePropertyChanged("TotalStepCount");
				}
			}

			property short CompletedStepCount
			{
				short get(){ return completedStepCount; }
				void set(short value)
				{
					completedStepCount = value;
					RaisePropertyChanged("CompletedStepCount");
				}
			}

			property short PerStepNumber
			{
				short get(){ return perStepNumber; }
				void set(short value)
				{
					perStepNumber = value;
					RaisePropertyChanged("PerStepNumber");
				}
			}

			property short PostStepNumber
			{
				short get(){ return postStepNumber; }
				void set(short value)
				{
					postStepNumber = value;
					RaisePropertyChanged("PostStepNumber");
				}
			}

			property String^ RecipeFullName
			{
				String^ get(){ return recipeName; }
				void set(String^ value)
				{
					recipeName = value;
					RaisePropertyChanged("RecipeName");
					RaisePropertyChanged("RecipeFullName");
				}
			}

			property String^ PerRecipeFullName
			{
				String^ get(){ return perRecipeName; }
				void set(String^ value)
				{
					perRecipeName = value;
					RaisePropertyChanged("PerRecipeName");
					RaisePropertyChanged("PerRecipeFullName");
				}
			}

			property String^ PostRecipeFullName
			{
				String^ get(){ return postRecipeName; }
				void set(String^ value)
				{
					postRecipeName = value;
					RaisePropertyChanged("PostRecipeName");
					RaisePropertyChanged("PostRecipeFullName");
				}
			}

			property String^ DataCollectionFullName
			{
				String^ get(){ return dataCollectionFileName; }
				void set(String^ value)
				{
					dataCollectionFileName = value;
					RaisePropertyChanged("DataCollectionFileName");
					RaisePropertyChanged("DataCollectionFullName");
				}
			}

			property String^ EndpointRawDataFullName
			{
				String^ get(){ return endpointRawDataFileName; }
				void set(String^ value)
				{
					endpointRawDataFileName = value;
					RaisePropertyChanged("EndpointRawDataFileName");
					RaisePropertyChanged("EndpointRawDataFullName");
				}
			}

			property String^ RecipeName
			{
				String^ get()
				{
					return Path::GetFileName(RecipeFullName);
				}
			}

			property String^ PerRecipeName
			{
				String^ get()
				{
					return Path::GetFileName(PerRecipeFullName);
				}
			}

			property String^ PostRecipeName
			{
				String^ get()
				{
					return Path::GetFileName(PostRecipeFullName);
				}
			}

			property String^ DataCollectionFileName
			{
				String^ get()
				{
					return Path::GetFileName(DataCollectionFullName);
				}
			}

			property String^ EndpointRawDataFileName
			{
				String^ get()
				{
					return Path::GetFileName(EndpointRawDataFullName);
				}
			}

			property int RF
			{
				int get(){ return rf; }
				void set(int value)
				{
					rf = value;
					RaisePropertyChanged("RF");
				}
			}
		};

		public ref class RecipeStepInfo :Notify
		{
		private:
			int stepNumber;
			CStepType stepType;
			unsigned char stepResult;
			unsigned char auxiliaryCount;
			unsigned char eventCount;
			DateTime stepStartTime;
			DateTime stepEndTime;
			int rf;
			int processDataCount;
		public:
			property int StepNumber
			{
				int get(){ return stepNumber; }
				void set(int value)
				{
					stepNumber = value;
					RaisePropertyChanged("StepNumber");
				}
			}

			property CStepType StepType
			{
				CStepType get(){ return stepType; }
				void set(CStepType value)
				{
					stepType = value;
					RaisePropertyChanged("StepType");
				}
			}

			property unsigned char StepResult
			{
				unsigned char get(){ return stepResult; }
				void set(unsigned char value)
				{
					stepResult = value;
					RaisePropertyChanged("StepResult");
				}
			}

			property unsigned char AuxiliaryCount
			{
				unsigned char get(){ return auxiliaryCount; }
				void set(unsigned char value)
				{
					auxiliaryCount = value;
					RaisePropertyChanged("AuxiliaryCount");
				}
			}

			property unsigned char EventCount
			{
				unsigned char get(){ return eventCount; }
				void set(unsigned char value)
				{
					eventCount = value;
					RaisePropertyChanged("EventCount");
				}
			}

			property DateTime StepStartTime
			{
				DateTime get(){ return stepStartTime; }
				void set(DateTime value)
				{
					stepStartTime = value;
					RaisePropertyChanged("StepStartTime");
				}
			}

			property DateTime StepEndTime
			{
				DateTime get(){ return stepEndTime; }
				void set(DateTime value)
				{
					stepEndTime = value;
					RaisePropertyChanged("StepEndTime");
				}
			}

			property int RF
			{
				int get(){ return rf; }
				void set(int value)
				{
					rf = value;
					RaisePropertyChanged("RF");
				}
			}

			property int ProcessDataCount
			{
				int get(){ return processDataCount; }
				void set(int value)
				{
					processDataCount = value;
					RaisePropertyChanged("ProcessDataCount");
				}
			}
		};

#pragma region AUX
		public ref class AuxData abstract :Notify
		{

		};

		public ref class AuxEnum :AuxData
		{
		private:
			short enumId;
		public:
			property short EnumId
			{
				short get(){ return enumId; }
				void set(short value)
				{
					enumId = value;
					RaisePropertyChanged("EnumId");
				}
			}
		};

		public ref class AuxGasSplitter :AuxData
		{
		private:
			int flowRatio;
		public:
			property int FlowRatio
			{
				int get(){ return flowRatio; }
				void set(int value)
				{
					flowRatio = value;
					RaisePropertyChanged("FlowRatio");
				}
			}
		};

		public ref class AuxGasPulse :AuxData
		{
		private:
			CGasZone zone;
			short ctrlMode;
			float levelAFlux;
			float levelBFlux;
			unsigned int cycleTime;
			unsigned int levelATime;
		public:
			property CGasZone Zone
			{
				CGasZone get(){ return zone; }
				void set(CGasZone value)
				{
					zone = value;
					RaisePropertyChanged("Zone");
				}
			}

			property short CtrlMode
			{
				short get(){ return ctrlMode; }
				void set(short value)
				{
					ctrlMode = value;
					RaisePropertyChanged("CtrlMode");
				}
			}

			property float LevelAFlux
			{
				float get(){ return levelAFlux; }
				void set(float value)
				{
					levelAFlux = value;
					RaisePropertyChanged("LevelAFlux");
				}
			}

			property float LevelBFlux
			{
				float get(){ return levelBFlux; }
				void set(float value)
				{
					levelBFlux = value;
					RaisePropertyChanged("LevelBFlux");
				}
			}

			property unsigned int CycleTime
			{
				unsigned int get(){ return cycleTime; }
				void set(unsigned int value)
				{
					cycleTime = value;
					RaisePropertyChanged("CycleTime");
				}
			}

			property unsigned int LevelATime
			{
				unsigned int get(){ return levelATime; }
				void set(unsigned int value)
				{
					levelATime = value;
					RaisePropertyChanged("LevelATime");
				}
			}
		};

		public ref class AuxGasRamp :AuxData
		{
		private:
			CGasZone zone;
			short ctrlMode;
			CRampMode rampMode;
			short reserved;
			unsigned int unitTime;
			float startingFlux;
			float endingFlux;
			float middlePointFlux;
			unsigned int middlePointTime;

		public:
			property CGasZone Zone
			{
				CGasZone get(){ return zone; }
				void set(CGasZone value)
				{
					zone = value;
					RaisePropertyChanged("Zone");
				}
			}

			property short CtrlMode
			{
				short get(){ return ctrlMode; }
				void set(short value)
				{
					ctrlMode = value;
					RaisePropertyChanged("CtrlMode");
				}
			}

			property CRampMode RampMode
			{
				CRampMode get(){ return rampMode; }
				void set(CRampMode value)
				{
					rampMode = value;
					RaisePropertyChanged("RampMode");
				}
			}

			property short Reserved
			{
				short get(){ return reserved; }
				void set(short value){ reserved = value; }
			}

			property unsigned int UnitTime
			{
				unsigned int get(){ return unitTime; }
				void set(unsigned int value)
				{
					unitTime = value;
					RaisePropertyChanged("UnitTime");
				}
			}

			property float StartingFlux
			{
				float get(){ return startingFlux; }
				void set(float value)
				{
					startingFlux = value;
					RaisePropertyChanged("StartingFlux");
				}
			}

			property float EndingFlux
			{
				float get(){ return endingFlux; }
				void set(float value)
				{
					endingFlux = value;
					RaisePropertyChanged("EndingFlux");
				}
			}

			property float MiddlePointFlux
			{
				float get(){ return middlePointFlux; }
				void set(float value)
				{
					middlePointFlux = value;
					RaisePropertyChanged("MiddlePointFlux");
				}
			}

			property unsigned int MiddlePointTime
			{
				unsigned int get(){ return middlePointTime; }
				void set(unsigned int value)
				{
					middlePointTime = value;
					RaisePropertyChanged("MiddlePointTime");
				}
			}
		};

		public union ValueConvert
		{
		public:
			unsigned int UIntValue;
			float FloatValue;
			unsigned char Buffer[4];

		public:
			float getFloat(unsigned char* curBuffer)
			{
				memcpy_s(Buffer, 4, curBuffer, 4);
				return FloatValue;
			}

			unsigned int getInt(unsigned char* curBuffer)
			{
				memcpy_s(Buffer, 4, curBuffer, 4);
				return UIntValue;
			}
		};
#pragma endregion

		public ref class RecipeParam :RecipeItem
		{
		private:
			float setpoint;
			float minValue;
			float maxValue;
			float average;
			float std;
			Function curFunction;
			int auxiliaryDataIndex;
			AuxData^ curAuxData = nullptr;

			bool isSelected = false;
		public:
			property float Setpoint
			{
				float get(){ return setpoint; }
				void set(float value)
				{
					setpoint = value;
					RaisePropertyChanged("Setpoint");
				}
			}

			property float MinValue
			{
				float get(){ return minValue; }
				void set(float value)
				{
					minValue = value;
					RaisePropertyChanged("MinValue");
				}
			}

			property float MaxValue
			{
				float get(){ return maxValue; }
				void set(float value)
				{
					maxValue = value;
					RaisePropertyChanged("MaxValue");
				}
			}

			property float Average
			{
				float get(){ return average; }
				void set(float value)
				{
					average = value;
					RaisePropertyChanged("Average");
				}
			}

			property float Std
			{
				float get(){ return std; }
				void set(float value)
				{
					std = value;
					RaisePropertyChanged("Std");
				}
			}

			property Function CurFunction
			{
				Function get(){ return curFunction; }
				void set(Function value)
				{
					curFunction = value;
					RaisePropertyChanged("CurFunction");
				}
			}

			property int AuxiliaryDataIndex
			{
				int get(){ return auxiliaryDataIndex; }
				void set(int value)
				{
					auxiliaryDataIndex = value;
					RaisePropertyChanged("AuxiliaryDataIndex");
				}
			}

			property AuxData^ CurAuxData
			{
				AuxData^ get(){ return curAuxData; }
				void set(AuxData^ value)
				{
					curAuxData = value;
					RaisePropertyChanged("CurAuxData");
				}
			}

			property bool IsSelected
			{
				bool get(){ return isSelected; }
				void set(bool value)
				{
					isSelected = value;
					RaisePropertyChanged("IsSelected");
				}
			}
		};

		public ref class EventData :Notify
		{
		private:
			int msgLength; //message string length    
			EventType eventType; //event type
			GroupType eventGroup; //event group
			unsigned short eventID; //event ID
			unsigned short moduleID; //module ID
			unsigned short subModuleID; //submodule ID
			array<DataType>^ dataType = gcnew array<DataType>(4); //data type
			array<unsigned char>^ unitType = gcnew array<unsigned char>(4); //unit type
			array<float>^ fData = gcnew array<float>(4); //float value
			array<int>^ nData = gcnew array<int>(4); //int value 
			DateTime sysTime; //system time when event occure 
			String^ eventStr; //event string

		public:
			property int MsgLength
			{
				int get(){ return msgLength; }
				void set(int value)
				{
					msgLength = value;
					RaisePropertyChanged("MsgLength");
				}
			}

			property EventType CurEventType
			{
				EventType get(){ return eventType; }
				void set(EventType value)
				{
					eventType = value;
					RaisePropertyChanged("CurEventType");
				}
			}

			property GroupType EventGroup
			{
				GroupType get(){ return eventGroup; }
				void set(GroupType value)
				{
					eventGroup = value;
					RaisePropertyChanged("EventGroup");
				}
			}

			property unsigned short EventID
			{
				unsigned short get(){ return eventID; }
				void set(unsigned short value)
				{
					eventID = value;
					RaisePropertyChanged("EventID");
				}
			}

			property unsigned short ModuleID
			{
				unsigned short get(){ return moduleID; }
				void set(unsigned short value)
				{
					moduleID = value;
					RaisePropertyChanged("ModuleID");
				}
			}

			property unsigned short SubModuleID
			{
				unsigned short get(){ return subModuleID; }
				void set(unsigned short value)
				{
					subModuleID = value;
					RaisePropertyChanged("SubModuleID");
				}
			}

			property array<DataType>^ CurDataType
			{
				array<DataType>^ get(){ return dataType; }
				void set(array<DataType>^ value)
				{
					dataType = value;
					RaisePropertyChanged("CurDataType");
					RaisePropertyChanged("DataTypeString");
				}
			}

			property String^ DataTypeString
			{
				String^ get()
				{
					return String::Join<DataType>(",", dataType);
				}
			}

			property array<unsigned char>^ UnitType
			{
				array<unsigned char>^ get(){ return unitType; }
				void set(array<unsigned char>^ value)
				{
					unitType = value;
					RaisePropertyChanged("UnitType");
				}
			}

			property array<float>^ FloatData
			{
				array<float>^ get(){ return fData; }
				void set(array<float>^ value)
				{
					fData = value;
					RaisePropertyChanged("FloatData");
				}
			}

			property array<int>^ IntData
			{
				array<int>^ get(){ return nData; }
				void set(array<int>^ value)
				{
					nData = value;
					RaisePropertyChanged("IntData");
				}
			}

			property DateTime SystemTime
			{
				DateTime get(){ return sysTime; }
				void set(DateTime value)
				{
					sysTime = value;
					RaisePropertyChanged("SystemTime");
				}
			}

			property String^ EventStr
			{
				String^ get(){ return eventStr; }
				void set(String^ value)
				{
					eventStr = value;
					RaisePropertyChanged("EventStr");
				}
			}
		};

#pragma region
		public ref class MoveInfo :Notify
		{
		private:
			CMOD_ID mOD_ID;
			int slotId;
			DateTime happenTime;
		public:
			property CMOD_ID MOD_ID
			{
				CMOD_ID get(){ return mOD_ID; }
				void set(CMOD_ID value)
				{
					mOD_ID = value;
					RaisePropertyChanged("MOD_ID");
				}
			}

			property int SlotId
			{
				int get(){ return slotId; }
				void set(int value)
				{
					slotId = value;
					RaisePropertyChanged("SlotId");
				}
			}

			property DateTime HappenTime
			{
				DateTime get(){ return happenTime; }
				void set(DateTime value)
				{
					happenTime = value;
					RaisePropertyChanged("HappenTime");
				}
			}
		};

		public ref class ShiftInfo :Notify
		{
		private:
			CMOD_ID moduleIdFrom;
			CMOD_ID moduleIdTo;
			int robotArm;
			int reserved;
			float shiftX;
			float shiftY;
			float distance;
		public:
			property CMOD_ID ModuleIdFrom
			{
				CMOD_ID get(){ return moduleIdFrom; }
				void set(CMOD_ID value)
				{
					moduleIdFrom = value;
					RaisePropertyChanged("ModuleIdFrom");
				}
			}

			property CMOD_ID ModuleIdTo
			{
				CMOD_ID get(){ return moduleIdTo; }
				void set(CMOD_ID value)
				{
					moduleIdTo = value;
					RaisePropertyChanged("ModuleIdTo");
				}
			}

			property int RobotArm
			{
				int get(){ return robotArm; }
				void set(int value)
				{
					robotArm = value;
					RaisePropertyChanged("RobotArm");
				}
			}

			property int Reserved
			{
				int get(){ return reserved; }
				void set(int value){ reserved = value; }
			}

			property float ShiftX
			{
				float get(){ return shiftX; }
				void set(float value)
				{
					shiftX = value;
					RaisePropertyChanged("ShiftX");
				}
			}

			property float ShiftY
			{
				float get(){ return shiftY; }
				void set(float value)
				{
					shiftY = value;
					RaisePropertyChanged("ShiftY");
				}
			}

			property float Distance
			{
				float get(){ return distance; }
				void set(float value)
				{
					distance = value;
					RaisePropertyChanged("Distance");
				}
			}
		};

		public ref class MoveData :Notify
		{
		private:
			int angleInfoCount;
			array<float>^ angleInfos = nullptr;
			int shiftInfoCount;
			array<ShiftInfo^>^ shiftInfos = nullptr;
			int moveInfoCount;
			array<MoveInfo^>^ moveInfos = nullptr;
		public:
			property int AngleInfoCount
			{
				int get(){ return angleInfoCount; }
				void set(int value)
				{
					angleInfoCount = value;
					RaisePropertyChanged("AngleInfoCount");
				}
			}

			property array<float>^ AngleInfos
			{
				array<float>^ get(){ return angleInfos; }
				void set(array<float>^ value)
				{
					angleInfos = value;
					RaisePropertyChanged("AngleInfos");
				}
			}

			property int ShiftInfoCount
			{
				int get(){ return shiftInfoCount; }
				void set(int value)
				{
					shiftInfoCount = value;
					RaisePropertyChanged("ShiftInfoCount");
				}
			}

			property array<ShiftInfo^>^ ShiftInfos
			{
				array<ShiftInfo^>^ get(){ return shiftInfos; }
				void set(array<ShiftInfo^>^ value)
				{
					shiftInfos = value;
					RaisePropertyChanged("ShiftInfos");
				}
			}

			property int MoveInfoCount
			{
				int get(){ return moveInfoCount; }
				void set(int value)
				{
					moveInfoCount = value;
					RaisePropertyChanged("MoveInfoCount");
				}
			}

			property array<MoveInfo^>^ MoveInfos
			{
				array<MoveInfo^>^ get(){ return moveInfos; }
				void set(array<MoveInfo^>^ value)
				{
					moveInfos = value;
					RaisePropertyChanged("MoveInfos");
				}
			}
		};
#pragma endregion

		public ref class StepInfo :Notify
		{
		private:
			RecipeStepInfo^ recipeStepInfo = gcnew RecipeStepInfo();
			array<RecipeParam^>^ recipeParams = nullptr;
			array<AuxData^>^ auxData = nullptr;
			array<EventData^>^ eventDatas = nullptr;
		public:
			property RecipeStepInfo^ CurRecipeStepInfo
			{
				RecipeStepInfo^ get(){ return recipeStepInfo; }
				void set(RecipeStepInfo^ value)
				{
					recipeStepInfo = value;
					RaisePropertyChanged("CurRecipeStepInfo");
				}
			}

			property array<RecipeParam^>^ RecipeParams
			{
				array<RecipeParam^>^ get(){ return recipeParams; }
				void set(array<RecipeParam^>^ value)
				{
					recipeParams = value;
					RaisePropertyChanged("RecipeParams");
				}
			}

			property array<AuxData^>^ CurAuxData
			{
				array<AuxData^>^ get(){ return auxData; }
				void set(array<AuxData^>^ value)
				{
					auxData = value;
					RaisePropertyChanged("CurAuxData");
				}
			}

			property array<EventData^>^ CurEventData
			{
				array<EventData^>^ get(){ return eventDatas; }
				void set(array<EventData^>^ value)
				{
					eventDatas = value;
					RaisePropertyChanged("CurEventData");
				}
			}
		};

		public ref class SubWaferDataBlock :Notify
		{
		private:
			RecipeInfo^ recipeInfo = gcnew RecipeInfo();
			array<RecipeItem^>^ recipeItems = nullptr;
			array<StepInfo^>^ stepInfos = nullptr;
		public:
			property RecipeInfo^ CurRecipeInfo
			{
				RecipeInfo^ get(){ return recipeInfo; }
				void set(RecipeInfo^ value)
				{
					recipeInfo = value;
					RaisePropertyChanged("CurRecipeInfo");
				}
			}

			property array<RecipeItem^>^ RecipeItems
			{
				array<RecipeItem^>^ get(){ return recipeItems; }
				void set(array<RecipeItem^>^ value)
				{
					recipeItems = value;
					RaisePropertyChanged("RecipeItems");
				}
			}

			property array<StepInfo^>^ StepInfos
			{
				array<StepInfo^>^ get(){ return stepInfos; }
				void set(array<StepInfo^>^ value)
				{
					stepInfos = value;
					RaisePropertyChanged("StepInfos");
				}
			}
		};

		public ref class WaferDataBlock :Notify
		{
		private:
			RecipeInfoHeader^ recipeInfoHeader = gcnew RecipeInfoHeader();
			array<SubWaferDataBlock^>^ subWaferDataBlocks = nullptr;
			MoveData^ moveData = nullptr;

		public:
			property RecipeInfoHeader^ CurRecipeInfoHeader
			{
				RecipeInfoHeader^ get(){ return recipeInfoHeader; }
				void set(RecipeInfoHeader^ value)
				{
					recipeInfoHeader = value;
					RaisePropertyChanged("CurRecipeInfoHeader");
				}
			}

			property array<SubWaferDataBlock^>^ SubWaferDataBlocks
			{
				array<SubWaferDataBlock^>^ get(){ return subWaferDataBlocks; }
				void set(array<SubWaferDataBlock^>^ value)
				{
					subWaferDataBlocks = value;
					RaisePropertyChanged("SubWaferDataBlocks");
				}
			}

			property MoveData^ CurMoveData
			{
				MoveData^ get(){ return moveData; }
				void set(MoveData^ value)
				{
					moveData = value;
					RaisePropertyChanged("CurMoveData");
				}
			}
		};

		public ref class WaferHistoryInfo :Notify
		{
		private:
			FileInfoEx^ fileInfoEx = nullptr;
			FileHeader^ header = gcnew FileHeader();
			WaferInfo^ waferInfo = gcnew WaferInfo();
			array<WaferDataBlock^>^ waferDataBlocks = nullptr;
			bool isInfoIntegrity = false;
		public:
			property FileInfoEx^ CurFileInfoEx
			{
				FileInfoEx^ get(){ return fileInfoEx; }
				void set(FileInfoEx^ value)
				{
					fileInfoEx = value;
					RaisePropertyChanged("CurFileInfoEx");
				}
			}

			property FileHeader^ Header
			{
				FileHeader^ get(){ return header; }
				void set(FileHeader^ value)
				{
					header = value;
					RaisePropertyChanged("Header");
				}
			}

			property WaferInfo^ CurWaferInfo
			{
				WaferInfo^ get(){ return waferInfo; }
				void set(WaferInfo^ value)
				{
					waferInfo = value;
					RaisePropertyChanged("CurWaferInfo");
				}
			}

			property array<WaferDataBlock^>^ WaferDataBlocks
			{
				array<WaferDataBlock^>^ get(){ return waferDataBlocks; }
				void set(array<WaferDataBlock^>^ value)
				{
					waferDataBlocks = value;
					RaisePropertyChanged("WaferDataBlocks");
				}
			}

			property bool IsInfoIntegrity
			{
				bool get(){ return isInfoIntegrity; }
				void set(bool value)
				{
					isInfoIntegrity = value;
					RaisePropertyChanged("IsInfoIntegrity");
				}
			}

			void FillInfos();
		};


		public ref class WH3Reader :BaseReader
		{
		public:
			void Read(WaferHistoryInfo^ info);


		private:
			void ReadWaferInfo(WaferInfo^ info);
			void ReadWaferDataBlocks(WaferHistoryInfo^ info);


			void ReadRecipeInfoHeader(RecipeInfoHeader^ info);
			void ReadRecipeInfo(RecipeInfo^ info, String^ parentDir);
			void ReadRecipeItem(RecipeItem^ item);
			void ReadRecipeStepInfo(RecipeStepInfo^ info);
			void ReadRecipeParam(RecipeParam^ curParam);
			AuxData^ ReadAuxData(Function curFunction);
			void ReadEventData(EventData^ eventData);
			void ReadMoveData(MoveData^ moveData);
		};

		public ref class WD2RecipeItem :CurvesData
		{
		private:
			bool isSelected = false;
			unsigned int attributeIndex;
			AttrType attributeType;
			String^ unit;
			String^ attributeName;

		public:
			property bool IsSelected
			{
				bool get(){ return isSelected; }
				void set(bool value)
				{
					isSelected = value;
					RaisePropertyChanged("IsSelected");
				}
			}

			property unsigned int AttributeIndex
			{
				unsigned int get(){ return attributeIndex; }
				void set(unsigned int value)
				{
					attributeIndex = value;
					RaisePropertyChanged("AttributeIndex");
				}
			}

			property AttrType AttributeType
			{
				AttrType get(){ return attributeType; }
				void set(AttrType value)
				{
					attributeType = value;
					RaisePropertyChanged("AttributeType");
				}
			}

			property String^ Unit
			{
				String^ get(){ return unit; }
				void set(String^ value)
				{
					unit = value;
					RaisePropertyChanged("Unit");
				}
			}

			property String^ AttributeName
			{
				String^ get(){ return attributeName; }
				void set(String^ value)
				{
					attributeName = value;
					RaisePropertyChanged("AttributeName");
				}
			}
		};

		public ref class WD2RecipeInfo :Notify
		{
		private:
			unsigned int uniqueWaferNumber;
			CMOD_ID mOD_ID;
			int reserved;
			int totalStepCount;
			int completedStepCount;
			int samplingInterval;
			DateTime recipeStartTime;
			DateTime recipeEndTime;
			String^ waferId;
			String^ recipeFullName;
			String^ recipeName;
			unsigned int samplingItemCount;
			unsigned int samplingCount;

		public:
			property unsigned int UniqueWaferNumber
			{
				unsigned int get(){ return uniqueWaferNumber; }
				void set(unsigned int value)
				{
					uniqueWaferNumber = value;
					RaisePropertyChanged("UniqueWaferNumber");
				}
			}

			property CMOD_ID MOD_ID
			{
				CMOD_ID get(){ return mOD_ID; }
				void set(CMOD_ID value)
				{
					mOD_ID = value;
					RaisePropertyChanged("MOD_ID");
				}
			}

			property int Reserved
			{
				int get(){ return reserved; }
				void set(int value)
				{
					reserved = value;
				}
			}

			property int TotalStepCount
			{
				int get(){ return totalStepCount; }
				void set(int value)
				{
					totalStepCount = value;
					RaisePropertyChanged("TotalStepCount");
				}
			}

			property int CompletedStepCount
			{
				int get(){ return completedStepCount; }
				void set(int value)
				{
					completedStepCount = value;
					RaisePropertyChanged("CompletedStepCount");
				}
			}

			property int SamplingInterval
			{
				int get(){ return samplingInterval; }
				void set(int value)
				{
					samplingInterval = value;
					RaisePropertyChanged("SamplingInterval");
				}
			}

			property DateTime RecipeStartTime
			{
				DateTime get(){ return recipeStartTime; }
				void set(DateTime value)
				{
					recipeStartTime = value;
					RaisePropertyChanged("RecipeStartTime");
				}
			}

			property DateTime RecipeEndTime
			{
				DateTime get(){ return recipeEndTime; }
				void set(DateTime value)
				{
					recipeEndTime = value;
					RaisePropertyChanged("RecipeEndTime");
				}
			}

			property String^ WaferId
			{
				String^ get(){ return waferId; }
				void set(String^ value)
				{
					waferId = value;
					RaisePropertyChanged("WaferId");
				}
			}

			property String^ RecipeFullName
			{
				String^ get(){ return recipeFullName; }
				void set(String^ value)
				{
					recipeFullName = value;
					RaisePropertyChanged("RecipeName");
					RaisePropertyChanged("RecipeFullName");
				}
			}

			property String^ RecipeName
			{
				String^ get()
				{
					return String::IsNullOrWhiteSpace(RecipeFullName) ? nullptr : Path::GetFileName(recipeFullName);
				}
			}

			property unsigned int SamplingItemCount
			{
				unsigned int get(){ return samplingItemCount; }
				void set(unsigned int value)
				{
					samplingItemCount = value;
					RaisePropertyChanged("SamplingItemCount");
				}
			}

			property unsigned int SamplingCount
			{
				unsigned int get(){ return samplingCount; }
				void set(unsigned int value)
				{
					samplingCount = value;
					RaisePropertyChanged("SamplingCount");
				}
			}
		};

		public ref class WD2StepInfo :Notify
		{
		private:
			DateTime stepStartTime;
			DateTime stepEndTime;
		public:
			property DateTime StepStartTime
			{
				DateTime get(){ return stepStartTime; }
				void set(DateTime value)
				{
					stepStartTime = value;
					RaisePropertyChanged("StepStartTime");
				}
			}

			property DateTime StepEndTime
			{
				DateTime get(){ return stepEndTime; }
				void set(DateTime value)
				{
					stepEndTime = value;
					RaisePropertyChanged("StepEndTime");
				}
			}
		};

		public ref class WD2Info :Notify
		{
		private:
			FileInfoEx^ fileInfoEx = nullptr;
			FileHeader^ header = gcnew FileHeader();
			WD2RecipeInfo^ recipeInfo = gcnew WD2RecipeInfo();
			array<WD2RecipeItem^>^ recipeItems = nullptr;
			array<WD2StepInfo^>^ wD2StepInfos = nullptr;
			bool isInfoIntegrity = false;

		public:
			property FileInfoEx^ CurFileInfoEx
			{
				FileInfoEx^ get(){ return fileInfoEx; }
				void set(FileInfoEx^ value)
				{
					fileInfoEx = value;
					RaisePropertyChanged("CurFileInfoEx");
				}
			}

			property FileHeader^ Header
			{
				FileHeader^ get(){ return header; }
				void set(FileHeader^ value)
				{
					header = value;
					RaisePropertyChanged("Header");
				}
			}

			property WD2RecipeInfo^ CurRecipeInfo
			{
				WD2RecipeInfo^ get(){ return recipeInfo; }
				void set(WD2RecipeInfo^ value)
				{
					recipeInfo = value;
					RaisePropertyChanged("CurRecipeInfo");
				}
			}

			property array<WD2RecipeItem^>^ RecipeItems
			{
				array<WD2RecipeItem^>^ get(){ return recipeItems; }
				void set(array<WD2RecipeItem^>^ value)
				{
					recipeItems = value;
					RaisePropertyChanged("RecipeItems");
				}
			}

			property array<WD2StepInfo^>^ WD2StepInfos
			{
				array<WD2StepInfo^>^ get(){ return wD2StepInfos; }
				void set(array<WD2StepInfo^>^ value)
				{
					wD2StepInfos = value;
					RaisePropertyChanged("WD2StepInfos");
				}
			}

			property bool IsInfoIntegrity
			{
				bool get(){ return isInfoIntegrity; }
				void set(bool value)
				{
					isInfoIntegrity = value;
					RaisePropertyChanged("IsInfoIntegrity");
				}
			}

			void FillInfos();
		};

		public ref class WD2Reader :BaseReader
		{
		public:
			void Read(WD2Info^ info);

		private:
			void ReadRecipeInfo(WD2RecipeInfo^ recipeInfo);
			void ReadRecipeItems(array<WD2RecipeItem^>^ recipeItems, unsigned int samplingCount);
			void ReadStepInfos(unsigned int totalCount, unsigned int completedCount, array<WD2StepInfo^>^ WD2StepInfos);
			void ReadRecords(array<WD2RecipeItem^>^ recipeItems, unsigned int samplingCount, WD2RecipeInfo^ recipeInfo);
		};
	}
}