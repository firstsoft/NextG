#include "WHReader.h"
using namespace AMEC::Native;


BaseReader::BaseReader()
{
}

BaseReader::~BaseReader()
{
	this->!BaseReader();
}

BaseReader::!BaseReader()
{
	Close();
}

bool BaseReader::Open(String^ fullfile)
{
	if (String::IsNullOrWhiteSpace(fullfile))
		return false;

	pin_ptr<const wchar_t> tempFile = PtrToStringChars(fullfile);
	pin_ptr<FILE*> tempFp = &fp;
	_wfopen_s(tempFp, tempFile, L"rb");

	int historyIndex = fullfile->IndexOf("History");
	historyIndex = historyIndex < 0 ? 0 : historyIndex;
	this->historyFolder = fullfile->Substring(0, historyIndex);
	this->curFullfile = fullfile;

	return *tempFp != NULL;
}

void BaseReader::Close()
{
	if (fp != NULL)
	{
		fclose(fp);
		fp = NULL;
	}
}

bool BaseReader::ReadHeader(FileHeader^ info)
{
	fseek(fp, 0, SEEK_END);
	long endPos = ftell(fp);

	if (endPos <= 4)
	{
		return false;
	}

	// read tail
	fseek(fp, endPos - sizeof(unsigned int), SEEK_SET);

	unsigned int tail;
	fread_s(&tail, sizeof(unsigned int), sizeof(unsigned int), 1, fp);

	array<unsigned char, 1>^ tempBuffer = BitConverter::GetBytes(tail);
	wchar_t tempStr[5];
	tempStr[0] = tempBuffer[0];
	tempStr[1] = tempBuffer[1];
	tempStr[2] = tempBuffer[2];
	tempStr[3] = tempBuffer[3];
	tempStr[4] = '\0';

	bool isFullfile = wcscmp(tempStr, L"WHED") == 0;
	lastValidPos = isFullfile ? endPos - sizeof(unsigned int) : endPos;

	//read header
	fseek(fp, 0, SEEK_SET);
	unsigned int tempFileHeaderFlag;
	unsigned short tempMajorVersion;
	unsigned short tempMinorVersion;

	fread_s(&tempFileHeaderFlag, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
	fread_s(&tempMajorVersion, sizeof(unsigned short), sizeof(unsigned short), 1, fp);
	fread_s(&tempMinorVersion, sizeof(unsigned short), sizeof(unsigned short), 1, fp);

	info->FileHeaderFlag = tempFileHeaderFlag;
	info->MajorVersion = tempMajorVersion;
	info->MinorVersion = tempMinorVersion;

	return isFullfile;
}

String^ BaseReader::FileChangeParentDir(String^ oldfullfile)
{
	if (String::IsNullOrWhiteSpace(oldfullfile))
	{
		return String::Empty;
	}

	int historyIndex = oldfullfile->LastIndexOf("History");
	String^ subStr = String::Empty;
	if (historyIndex < 0)
	{
		subStr = Path::GetFileName(oldfullfile);
	}
	else
	{
		subStr = oldfullfile->Substring(historyIndex);
	}

	return Path::Combine(this->historyFolder, subStr);
}

String^ BaseReader::RecipeChangeParentDir(String^ oldfullfile)
{
	if (String::IsNullOrWhiteSpace(oldfullfile))
	{
		return String::Empty;
	}

	int recipeIndex = oldfullfile->IndexOf("Recipe");
	String^ subStr = String::Empty;
	if (recipeIndex < 0)
	{
		subStr = Path::GetFileName(oldfullfile);
	}
	else
	{
		subStr = oldfullfile->Substring(recipeIndex);
	}

	return Path::Combine(this->historyFolder, subStr);
}


void SH2Info::FillInfos()
{
	if (fileInfoEx == nullptr)
		return;

	SH2Reader^ sh2;
	sh2 = gcnew SH2Reader();
	sh2->Open(this->fileInfoEx->Fullfile);
	sh2->Read(this);
	sh2->Close();
}

void WaferHistoryInfo::FillInfos()
{
	if (fileInfoEx == nullptr)
		return;

	WH3Reader^ wh3;
	wh3 = gcnew WH3Reader();
	wh3->Open(this->fileInfoEx->Fullfile);
	wh3->Read(this);
	wh3->Close();
}




void SH2Reader::Read(SH2Info^ info)
{
	info = info == nullptr ? gcnew SH2Info() : info;

	try
	{
		info->IsInfoIntegrity = ReadHeader(info->Header);

		if (info->Header->FileHeaderName->Equals("AMEC"))
		{
			ReadSeqInfo(info->SequenceInfo);
			ReadSeqWaferInfos(info);
		}
	}
	catch (...)
	{
		info->IsInfoIntegrity = false;
	}
}

void SH2Reader::ReadSeqInfo(SeqInfo^ info)
{
	CMOD_ID ModId;
	int WaferCount;
	int Reserved;
	unsigned long long StartTime;
	unsigned long long EndTime;
	wchar_t SequenceName[MAX_PATH];

	fread_s(&ModId, sizeof(int), sizeof(int), 1, fp);
	fread_s(&WaferCount, sizeof(int), sizeof(int), 1, fp);
	fread_s(&Reserved, sizeof(int), sizeof(int), 1, fp);
	fread_s(&StartTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
	fread_s(&EndTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
	fread_s(SequenceName, sizeof(wchar_t)*MAX_PATH, sizeof(wchar_t), MAX_PATH, fp);

	info->MOD_ID = ModId;
	info->WaferCount = WaferCount;
	info->Reserved = Reserved;
	info->SeqStartTime = DateTime::FromFileTimeUtc(StartTime);
	info->SeqEndTime = DateTime::FromFileTimeUtc(EndTime);

	info->SequenceFullName = gcnew String(SequenceName);
	info->SequenceFullName = this->RecipeChangeParentDir(info->SequenceFullName);
}

void SH2Reader::ReadSeqWaferInfos(SH2Info^ info)
{
	info->SeqWaferInfos = gcnew array<SeqWaferInfo^>(info->SequenceInfo->WaferCount);

	for (int num = 0; num < info->SequenceInfo->WaferCount; num++)
	{
		unsigned int WaferIndex = 0;
		int SoltId = 0;
		int Reserved = 0;
		unsigned long long WaferCreateTime;
		wchar_t WaferHistoryFileName[MAX_PATH];

		fread_s(&WaferIndex, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
		fread_s(&SoltId, sizeof(int), sizeof(int), 1, fp);
		fread_s(&Reserved, sizeof(int), sizeof(int), 1, fp);
		fread_s(&WaferCreateTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
		fread_s(WaferHistoryFileName, sizeof(wchar_t)*MAX_PATH, sizeof(wchar_t), MAX_PATH, fp);

		SeqWaferInfo^ tempInfo = gcnew SeqWaferInfo();
		tempInfo->UniqueNumber = WaferIndex;
		tempInfo->SlotId = SoltId;
		tempInfo->Reserved = Reserved;
		tempInfo->WaferCreateTime = DateTime::FromFileTimeUtc(WaferCreateTime);

		tempInfo->WaferHistoryFullfileName = gcnew String(WaferHistoryFileName);
		tempInfo->WaferHistoryFullfileName = this->FileChangeParentDir(tempInfo->WaferHistoryFullfileName);

		info->SeqWaferInfos[num] = tempInfo;
	}
}



void WH3Reader::Read(WaferHistoryInfo^ info)
{
	info = info == nullptr ? gcnew  WaferHistoryInfo() : info;

	try
	{
		info->IsInfoIntegrity = ReadHeader(info->Header);

		if (info->Header->FileHeaderName->Equals("AMEC"))
		{
			ReadWaferInfo(info->CurWaferInfo);
			ReadWaferDataBlocks(info);
		}
	}
	catch (...)
	{
		info->IsInfoIntegrity = false;
	}
}

void WH3Reader::ReadWaferInfo(WaferInfo^ info)
{
	unsigned int uniqueNumber;
	int reserved;
	int waferStatus;
	CMOD_ID mOD_ID;
	int slotId;
	unsigned long long waferCreateTime;
	int loadedTimes;
	wchar_t waferId[32];
	wchar_t lotId[32];
	wchar_t foupId[32];

	fread_s(&uniqueNumber, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
	fread_s(&reserved, sizeof(int), sizeof(int), 1, fp);
	fread_s(&waferStatus, sizeof(int), sizeof(int), 1, fp);
	fread_s(&mOD_ID, sizeof(int), sizeof(int), 1, fp);
	fread_s(&slotId, sizeof(int), sizeof(int), 1, fp);
	fread_s(&waferCreateTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
	fread_s(&loadedTimes, sizeof(int), sizeof(int), 1, fp);
	fread_s(waferId, sizeof(wchar_t) << 5, sizeof(wchar_t), 32, fp);
	fread_s(lotId, sizeof(wchar_t) << 5, sizeof(wchar_t), 32, fp);
	fread_s(foupId, sizeof(wchar_t) << 5, sizeof(wchar_t), 32, fp);

	info->UniqueNumber = uniqueNumber;
	info->Reserved = reserved;
	info->WaferStatus = waferStatus;
	info->MOD_ID = mOD_ID;
	info->SlotId = slotId;
	info->WaferCreateTime = DateTime::FromFileTimeUtc(waferCreateTime);
	info->LoadedTimes = loadedTimes;
	info->WaferId = gcnew String(waferId);
	info->LotId = gcnew String(lotId);
	info->FoupId = gcnew String(foupId);
}

void WH3Reader::ReadWaferDataBlocks(WaferHistoryInfo^ info)
{
	info->WaferDataBlocks = gcnew array<WaferDataBlock^>(info->CurWaferInfo->LoadedTimes);

	for (int num = 0; num < info->CurWaferInfo->LoadedTimes; num++)
	{
		WaferDataBlock^ waferDataBlock = gcnew WaferDataBlock();
		ReadRecipeInfoHeader(waferDataBlock->CurRecipeInfoHeader);
		waferDataBlock->SubWaferDataBlocks = gcnew array<SubWaferDataBlock^>(waferDataBlock->CurRecipeInfoHeader->ProcessTimes);

		for (unsigned int pTimes = 0; pTimes < waferDataBlock->CurRecipeInfoHeader->ProcessTimes; pTimes++)
		{
			SubWaferDataBlock^ subWaferDataBlock = gcnew SubWaferDataBlock();

#pragma region read RecipeItems
			ReadRecipeInfo(subWaferDataBlock->CurRecipeInfo, info->CurFileInfoEx->ParentDir);
			subWaferDataBlock->RecipeItems = gcnew array<RecipeItem^>(subWaferDataBlock->CurRecipeInfo->RecipeItemsCount);

			for (int num1 = 0; num1 < subWaferDataBlock->CurRecipeInfo->RecipeItemsCount; num1++)
			{
				RecipeItem^ recipeItem = gcnew RecipeItem();
				ReadRecipeItem(recipeItem);

				subWaferDataBlock->RecipeItems[num1] = recipeItem;
			}
#pragma endregion

			subWaferDataBlock->StepInfos = gcnew array<StepInfo^>(subWaferDataBlock->CurRecipeInfo->CompletedStepCount);
			for (int stepIndex = 0; stepIndex < subWaferDataBlock->CurRecipeInfo->CompletedStepCount; stepIndex++)
			{
				StepInfo^ stepInfo = gcnew StepInfo();

#pragma region read StepInfo
				ReadRecipeStepInfo(stepInfo->CurRecipeStepInfo);

				stepInfo->RecipeParams = gcnew array<RecipeParam^>(stepInfo->CurRecipeStepInfo->ProcessDataCount);
				for (int num1 = 0; num1 < stepInfo->CurRecipeStepInfo->ProcessDataCount; num1++)
				{
					RecipeParam^ curParam = gcnew RecipeParam();
					ReadRecipeParam(curParam);

					RecipeItem^ curRecipeItem = (RecipeItem^)subWaferDataBlock->RecipeItems[num1];
					curParam->AttributeIndex = curRecipeItem->AttributeIndex;
					curParam->AttributeType = curRecipeItem->AttributeType;
					curParam->Unit = curRecipeItem->Unit;
					curParam->AttributeName = curRecipeItem->AttributeName;

					stepInfo->RecipeParams[num1] = curParam;
				}

				stepInfo->CurAuxData = gcnew array<AuxData^>(stepInfo->CurRecipeStepInfo->AuxiliaryCount);

				if (stepInfo->CurRecipeStepInfo->AuxiliaryCount > 0)
				{
					for (int num1 = 0; num1 < stepInfo->CurRecipeStepInfo->ProcessDataCount; num1++)
					{
						RecipeParam^ tempRecipeParam = ((RecipeParam^)stepInfo->RecipeParams[num1]);

						if (tempRecipeParam->CurFunction == Function::None || tempRecipeParam->CurFunction == Function::ESCTemperature)
							continue;

						AuxData^ curAuxData = ReadAuxData(tempRecipeParam->CurFunction);

						if (curAuxData != nullptr)
						{
							tempRecipeParam->CurAuxData = curAuxData;
							stepInfo->CurAuxData[tempRecipeParam->AuxiliaryDataIndex] = curAuxData;
						}
					}
				}

				stepInfo->CurEventData = gcnew array<EventData^>(stepInfo->CurRecipeStepInfo->EventCount);
				for (int eventIndex = 0; eventIndex < stepInfo->CurRecipeStepInfo->EventCount; eventIndex++)
				{
					EventData^ eventData = gcnew EventData();
					ReadEventData(eventData);
					stepInfo->CurEventData[eventIndex] = eventData;
				}
#pragma endregion

				subWaferDataBlock->StepInfos[stepIndex] = stepInfo;
			}

			waferDataBlock->SubWaferDataBlocks[pTimes] = subWaferDataBlock;
		}

		//read movedata
		/*if (waferDataBlock->SubWaferDataBlocks->Length == 0 ||
			((SubWaferDataBlock^)waferDataBlock->SubWaferDataBlocks[0])->CurRecipeInfo->RecipeType != CRecipeType::Clean)
			{*/
		long curPos = ftell(fp);
		if (curPos < lastValidPos)
		{
			waferDataBlock->CurMoveData = gcnew MoveData();
			ReadMoveData(waferDataBlock->CurMoveData);

			if (waferDataBlock->CurMoveData->AngleInfoCount +
				waferDataBlock->CurMoveData->ShiftInfoCount +
				waferDataBlock->CurMoveData->MoveInfoCount == 0)
			{
				waferDataBlock->CurMoveData = nullptr;
			}
		}
		//}

		info->WaferDataBlocks[num] = waferDataBlock;
	}
}

void WH3Reader::ReadRecipeInfoHeader(RecipeInfoHeader^ recipeInfoHeader)
{
	unsigned int processTimes;
	int reserved;

	fread_s(&processTimes, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
	fread_s(&reserved, sizeof(int), sizeof(int), 1, fp);

	recipeInfoHeader->ProcessTimes = processTimes;
	recipeInfoHeader->Reserved = reserved;
}

void WH3Reader::ReadRecipeInfo(RecipeInfo^ info, String^ parentDir)
{
	CMOD_ID mOD_ID;
	int recipeType;
	int recipeItemsCount;
	int processResult;
	int eventNumber;
	float waferAngle;
	unsigned long long recipeStartTime;
	unsigned long long recipeEndTime;
	short totalStepCount;
	short completedStepCount;
	short perStepNumber;
	short postStepNumber;
	wchar_t recipeName[MAX_PATH];
	wchar_t perRecipeName[MAX_PATH >> 1];
	wchar_t postRecipeName[MAX_PATH >> 1];
	wchar_t dataCollectionFileName[MAX_PATH];
	wchar_t endpointRawDataFileName[MAX_PATH];
	int rf[4];

	fread_s(&mOD_ID, sizeof(int), sizeof(int), 1, fp);
	fread_s(&recipeType, sizeof(int), sizeof(int), 1, fp);
	fread_s(&recipeItemsCount, sizeof(int), sizeof(int), 1, fp);
	fread_s(&processResult, sizeof(int), sizeof(int), 1, fp);
	fread_s(&eventNumber, sizeof(int), sizeof(int), 1, fp);
	fread_s(&waferAngle, sizeof(float), sizeof(float), 1, fp);
	fread_s(&recipeStartTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
	fread_s(&recipeEndTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
	fread_s(&totalStepCount, sizeof(short), sizeof(short), 1, fp);
	fread_s(&completedStepCount, sizeof(short), sizeof(short), 1, fp);
	fread_s(&perStepNumber, sizeof(short), sizeof(short), 1, fp);
	fread_s(&postStepNumber, sizeof(short), sizeof(short), 1, fp);
	fread_s(recipeName, sizeof(wchar_t)*MAX_PATH, sizeof(wchar_t), MAX_PATH, fp);
	fread_s(perRecipeName, sizeof(wchar_t)*(MAX_PATH >> 1), sizeof(wchar_t), (MAX_PATH >> 1), fp);
	fread_s(postRecipeName, sizeof(wchar_t)*(MAX_PATH >> 1), sizeof(wchar_t), (MAX_PATH >> 1), fp);
	fread_s(dataCollectionFileName, sizeof(wchar_t)*MAX_PATH, sizeof(wchar_t), MAX_PATH, fp);
	fread_s(endpointRawDataFileName, sizeof(wchar_t)*MAX_PATH, sizeof(wchar_t), MAX_PATH, fp);
	fread_s(rf, sizeof(int) << 2, sizeof(int), 4, fp);

	info->MOD_ID = mOD_ID;
	info->RecipeType = (CRecipeType)recipeType;
	info->RecipeItemsCount = recipeItemsCount;
	info->ProcessResult = processResult;
	info->EventNumber = eventNumber;
	info->WaferAngle = waferAngle;
	info->RecipeStartTime = DateTime::FromFileTimeUtc(recipeStartTime);
	info->RecipeEndTime = DateTime::FromFileTimeUtc(recipeEndTime);
	info->TotalStepCount = totalStepCount;
	info->CompletedStepCount = completedStepCount;
	info->PerStepNumber = perStepNumber;
	info->PostStepNumber = postStepNumber;
	info->RecipeFullName = gcnew String(recipeName);

	info->PerRecipeFullName = gcnew String(perRecipeName);
	info->PostRecipeFullName = gcnew String(postRecipeName);

	if (wcslen(dataCollectionFileName) > 0)
	{
		String^ dataCollectionNameStr = Path::GetFileName(gcnew String(dataCollectionFileName));
		info->DataCollectionFullName = Path::Combine(parentDir, dataCollectionNameStr);
	}
	else
	{
		info->DataCollectionFullName = nullptr;
	}

	info->EndpointRawDataFullName = gcnew String(endpointRawDataFileName);
	info->RF = rf[0];
}

void WH3Reader::ReadRecipeItem(RecipeItem^ item)
{
	unsigned int attributeIndex;
	AttrType attributeType;
	wchar_t unit[32];
	wchar_t attributeName[128];

	fread_s(&attributeIndex, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
	fread_s(&attributeType, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
	fread_s(unit, sizeof(wchar_t) << 5, sizeof(wchar_t), 32, fp);
	fread_s(attributeName, sizeof(wchar_t) << 7, sizeof(wchar_t), 128, fp);

	item->AttributeIndex = attributeIndex;
	item->AttributeType = attributeType;
	item->Unit = gcnew String(unit);
	item->AttributeName = gcnew String(attributeName);
}

void WH3Reader::ReadRecipeStepInfo(RecipeStepInfo^ info)
{
	int stepNumber;
	unsigned char stepType;
	unsigned char stepResult;
	unsigned char auxiliaryCount;
	unsigned char eventCount;
	unsigned long long stepStartTime;
	unsigned long long stepEndTime;
	int rf[4];
	int processDataCount;

	fread_s(&stepNumber, sizeof(int), sizeof(int), 1, fp);
	fread_s(&stepType, sizeof(unsigned char), sizeof(unsigned char), 1, fp);
	fread_s(&stepResult, sizeof(unsigned char), sizeof(unsigned char), 1, fp);
	fread_s(&auxiliaryCount, sizeof(unsigned char), sizeof(unsigned char), 1, fp);
	fread_s(&eventCount, sizeof(unsigned char), sizeof(unsigned char), 1, fp);
	fread_s(&stepStartTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
	fread_s(&stepEndTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
	fread_s(rf, sizeof(int) << 2, sizeof(int), 4, fp);
	fread_s(&processDataCount, sizeof(int), sizeof(int), 1, fp);

	info->StepNumber = stepNumber;
	info->StepType = (CStepType)stepType;
	info->StepResult = stepResult;
	info->AuxiliaryCount = auxiliaryCount;
	info->EventCount = eventCount;
	info->StepStartTime = DateTime::FromFileTimeUtc(stepStartTime);
	info->StepEndTime = DateTime::FromFileTimeUtc(stepEndTime);
	info->RF = rf[0];
	info->ProcessDataCount = processDataCount;
}

void WH3Reader::ReadRecipeParam(RecipeParam^ curParam)
{
	float setpoint;
	float minValue;
	float maxValue;
	float average;
	float std;
	int flag;

	fread_s(&setpoint, sizeof(float), sizeof(float), 1, fp);
	fread_s(&minValue, sizeof(float), sizeof(float), 1, fp);
	fread_s(&maxValue, sizeof(float), sizeof(float), 1, fp);
	fread_s(&average, sizeof(float), sizeof(float), 1, fp);
	fread_s(&std, sizeof(float), sizeof(float), 1, fp);
	fread_s(&flag, sizeof(int), sizeof(int), 1, fp);

	curParam->Setpoint = setpoint;
	curParam->MinValue = minValue;
	curParam->MaxValue = maxValue;
	curParam->Average = average;
	curParam->Std = std;

	curParam->CurFunction = (Function)(flag >> 24);
	curParam->AuxiliaryDataIndex = (flag << 16) >> 16;
}

AuxData^ WH3Reader::ReadAuxData(Function curFunction)
{
	unsigned char buffer[32];
	fread_s(buffer, sizeof(unsigned char) << 5, sizeof(unsigned char), 32, fp);

	if (curFunction == Function::Enum)
	{
		AuxEnum^ auxEnum = gcnew AuxEnum();
		auxEnum->EnumId = (short)((buffer[1] << 8) | buffer[0]);
		return auxEnum;
	}

	if (curFunction == Function::GasSplitter)
	{
		AuxGasSplitter^ auxGasSplitter = gcnew AuxGasSplitter();
		auxGasSplitter->FlowRatio = (int)((buffer[3] << 24) | (buffer[2] << 16) | (buffer[1] << 8) | buffer[0]);
		return auxGasSplitter;
	}

	if (curFunction == Function::Gas)
	{
		short zone = (short)((buffer[1] << 8) | buffer[0]);
		short ctrlMode = (short)((buffer[3] << 8) | buffer[2]);

		if (ctrlMode == 1)
		{
			// gas ramp
			ValueConvert valueConvert;
			AuxGasRamp^ auxGasRamp = gcnew AuxGasRamp();
			auxGasRamp->Zone = (CGasZone)zone;
			auxGasRamp->CtrlMode = ctrlMode;
			auxGasRamp->RampMode = (CRampMode)((buffer[5] << 8) | buffer[4]);
			auxGasRamp->Reserved = (short)((buffer[7] << 8) | buffer[6]);

			auxGasRamp->UnitTime = valueConvert.getInt(buffer + 8);
			auxGasRamp->StartingFlux = valueConvert.getFloat(buffer + 12);
			auxGasRamp->EndingFlux = valueConvert.getFloat(buffer + 16);
			auxGasRamp->MiddlePointFlux = valueConvert.getFloat(buffer + 20);
			auxGasRamp->MiddlePointTime = valueConvert.getInt(buffer + 24);

			return auxGasRamp;
		}
		else if (ctrlMode == 2)
		{
			//gas pulse
			ValueConvert valueConvert;
			AuxGasPulse^ auxGasPulse = gcnew AuxGasPulse();
			auxGasPulse->Zone = (CGasZone)zone;
			auxGasPulse->CtrlMode = ctrlMode;
			auxGasPulse->LevelAFlux = valueConvert.getFloat(buffer + 4);
			auxGasPulse->LevelBFlux = valueConvert.getFloat(buffer + 8);
			auxGasPulse->CycleTime = valueConvert.getInt(buffer + 12);
			auxGasPulse->LevelATime = valueConvert.getInt(buffer + 16);

			return auxGasPulse;
		}
	}

	return nullptr;
}

void WH3Reader::ReadEventData(EventData^ eventData)
{
	WH_EVENT_INTERNAL eventData1 = { 0 };
	fread_s(&eventData1, sizeof(WH_EVENT_INTERNAL), sizeof(WH_EVENT_INTERNAL), 1, fp);

	eventData->MsgLength = eventData1.wMsgLenth;
	eventData->CurEventType = (EventType)eventData1.EventType;
	eventData->EventGroup = (GroupType)eventData1.EventGroup;
	eventData->EventID = eventData1.wEventID;
	eventData->ModuleID = eventData1.wModuleID;
	eventData->SubModuleID = eventData1.wSubModuleID;

	for (int num = 0; num < 4; num++)
	{
		eventData->CurDataType[num] = (DataType)eventData1.DataType[num];
		eventData->UnitType[num] = eventData1.UnitType[num];
		eventData->FloatData[num] = eventData1.fData[num];
		eventData->IntData[num] = eventData1.nData[num];
	}

	FILETIME fileTime;
	SystemTimeToFileTime(&eventData1.stTime, &fileTime);
	eventData->SystemTime = DateTime::FromFileTimeUtc((((unsigned long long)fileTime.dwHighDateTime) << 32) + fileTime.dwLowDateTime);

	eventData->EventStr = gcnew String(eventData1.szMsg);
}

void WH3Reader::ReadMoveData(MoveData^ moveData)
{
	int angleInfoCount = 0;
	int shiftInfoCount = 0;
	int moveInfoCount = 0;

	// angle info
	fread_s(&angleInfoCount, sizeof(int), sizeof(int), 1, fp);
	moveData->AngleInfoCount = angleInfoCount;
	moveData->AngleInfos = gcnew array<float>(angleInfoCount);

	for (int num = 0; num < angleInfoCount; num++)
	{
		float angle;
		fread_s(&angle, sizeof(float), sizeof(float), 1, fp);
		moveData->AngleInfos[num] = angle;
	}

	//shift info
	fread_s(&shiftInfoCount, sizeof(int), sizeof(int), 1, fp);
	moveData->ShiftInfoCount = shiftInfoCount;
	moveData->ShiftInfos = gcnew array<ShiftInfo^>(shiftInfoCount);

	for (int num = 0; num < shiftInfoCount; num++)
	{
		ShiftInfo^ shiftInfo = gcnew ShiftInfo();

		CMOD_ID moduleIdFrom;
		CMOD_ID moduleIdTo;
		int robotArm;
		int reserved;
		float shiftX;
		float shiftY;
		float distance;

		fread_s(&moduleIdFrom, sizeof(int), sizeof(int), 1, fp);
		fread_s(&moduleIdTo, sizeof(int), sizeof(int), 1, fp);
		fread_s(&robotArm, sizeof(int), sizeof(int), 1, fp);
		fread_s(&reserved, sizeof(int), sizeof(int), 1, fp);
		fread_s(&shiftX, sizeof(float), sizeof(float), 1, fp);
		fread_s(&shiftY, sizeof(float), sizeof(float), 1, fp);
		fread_s(&distance, sizeof(float), sizeof(float), 1, fp);

		shiftInfo->ModuleIdFrom = moduleIdFrom;
		shiftInfo->ModuleIdTo = moduleIdTo;
		shiftInfo->RobotArm = robotArm;
		shiftInfo->Reserved = reserved;
		shiftInfo->ShiftX = shiftX;
		shiftInfo->ShiftY = shiftY;
		shiftInfo->Distance = distance;

		moveData->ShiftInfos[num] = shiftInfo;
	}

	//move info
	fread_s(&moveInfoCount, sizeof(int), sizeof(int), 1, fp);

	/*if (this->curFullfile->IndexOf("testLot_AMECMADEC_20170405_101623496_7444_01") >= 0)
	{
	Console::WriteLine();
	return;
	}*/

	moveData->MoveInfoCount = moveInfoCount;
	moveData->MoveInfos = gcnew array<MoveInfo^>(moveInfoCount);

	for (int num = 0; num < moveInfoCount; num++)
	{
		MoveInfo^ moveInfo = gcnew MoveInfo();

		CMOD_ID mOD_ID;
		int slotId;
		unsigned long long happenTime;

		fread_s(&mOD_ID, sizeof(int), sizeof(int), 1, fp);
		fread_s(&slotId, sizeof(int), sizeof(int), 1, fp);
		fread_s(&happenTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);

		moveInfo->MOD_ID = mOD_ID;
		moveInfo->SlotId = slotId;
		moveInfo->HappenTime = DateTime::FromFileTimeUtc(happenTime);

		moveData->MoveInfos[num] = moveInfo;
	}
}



void WD2Info::FillInfos()
{
	if (fileInfoEx == nullptr)
		return;

	WD2Reader^ wd2;
	wd2 = gcnew WD2Reader();
	wd2->Open(this->fileInfoEx->Fullfile);
	wd2->Read(this);
	wd2->Close();
}

void WD2Reader::Read(WD2Info^ info)
{
	info = info == nullptr ? gcnew  WD2Info() : info;

	try
	{
		info->IsInfoIntegrity = ReadHeader(info->Header);

		if (info->Header->FileHeaderName->Equals("AMEC"))
		{
			ReadRecipeInfo(info->CurRecipeInfo);

			if (info->CurRecipeInfo->SamplingItemCount > 0)
			{
				info->RecipeItems = gcnew array<WD2RecipeItem^>(info->CurRecipeInfo->SamplingItemCount);
				ReadRecipeItems(info->RecipeItems, info->CurRecipeInfo->SamplingCount);
			}

			if (info->CurRecipeInfo->TotalStepCount > 0)
			{
				info->WD2StepInfos = info->CurRecipeInfo->CompletedStepCount > 0 ?
					gcnew array<WD2StepInfo^>(info->CurRecipeInfo->CompletedStepCount) : nullptr;

				ReadStepInfos(info->CurRecipeInfo->TotalStepCount, info->CurRecipeInfo->CompletedStepCount, info->WD2StepInfos);
			}

			if (info->CurRecipeInfo->SamplingCount > 0)
			{
				ReadRecords(info->RecipeItems, info->CurRecipeInfo->SamplingCount, info->CurRecipeInfo);
			}
		}
	}
	catch (...)
	{
		info->IsInfoIntegrity = false;
	}
}

void WD2Reader::ReadRecipeInfo(WD2RecipeInfo^ recipeInfo)
{
	unsigned int uniqueWaferNumber;
	CMOD_ID mOD_ID;
	int reserved;
	int totalStepCount;
	int completedStepCount;
	int samplingInterval;
	unsigned long long recipeStartTime;
	unsigned long long recipeEndTime;
	wchar_t waferId[32];
	wchar_t recipeName[MAX_PATH];
	unsigned int samplingItemCount;
	unsigned int samplingCount;

	fread_s(&uniqueWaferNumber, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
	fread_s(&mOD_ID, sizeof(int), sizeof(int), 1, fp);
	fread_s(&reserved, sizeof(int), sizeof(int), 1, fp);
	fread_s(&totalStepCount, sizeof(int), sizeof(int), 1, fp);
	fread_s(&completedStepCount, sizeof(int), sizeof(int), 1, fp);
	fread_s(&samplingInterval, sizeof(int), sizeof(int), 1, fp);
	fread_s(&recipeStartTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
	fread_s(&recipeEndTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
	fread_s(waferId, sizeof(wchar_t)* 32, sizeof(wchar_t), 32, fp);
	fread_s(recipeName, sizeof(wchar_t)*MAX_PATH, sizeof(wchar_t), MAX_PATH, fp);
	fread_s(&samplingItemCount, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
	fread_s(&samplingCount, sizeof(unsigned int), sizeof(unsigned int), 1, fp);

	recipeInfo->UniqueWaferNumber = uniqueWaferNumber;
	recipeInfo->MOD_ID = mOD_ID;
	recipeInfo->Reserved = reserved;
	recipeInfo->TotalStepCount = totalStepCount;
	recipeInfo->CompletedStepCount = completedStepCount;
	recipeInfo->SamplingInterval = samplingInterval;
	recipeInfo->RecipeStartTime = DateTime::FromFileTimeUtc(recipeStartTime);
	recipeInfo->RecipeEndTime = DateTime::FromFileTimeUtc(recipeEndTime);
	recipeInfo->WaferId = gcnew String(waferId);
	recipeInfo->RecipeFullName = gcnew String(recipeName);
	recipeInfo->SamplingItemCount = samplingItemCount;
	recipeInfo->SamplingCount = samplingCount;
}

void WD2Reader::ReadRecipeItems(array<WD2RecipeItem^>^ recipeItems, unsigned int samplingCount)
{
	unsigned int attributeIndex;
	AttrType attributeType;
	wchar_t unit[32];
	wchar_t attributeName[128];

	for (int num = 0; num < recipeItems->Length; num++)
	{
		WD2RecipeItem^ item = gcnew WD2RecipeItem();
		fread_s(&attributeIndex, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
		fread_s(&attributeType, sizeof(unsigned int), sizeof(unsigned int), 1, fp);
		fread_s(unit, sizeof(wchar_t) << 5, sizeof(wchar_t), 32, fp);
		fread_s(attributeName, sizeof(wchar_t) << 7, sizeof(wchar_t), 128, fp);

		item->AttributeIndex = attributeIndex;
		item->AttributeType = attributeType;
		item->Unit = gcnew String(unit);
		item->AttributeName = gcnew String(attributeName);
		item->DataPoints = gcnew List<DataPoint^>(samplingCount);
		recipeItems[num] = item;
	}
}

void WD2Reader::ReadStepInfos(unsigned int totalCount, unsigned int completedCount, array<WD2StepInfo^>^ WD2StepInfos)
{
	unsigned long long stepStartTime;
	unsigned long long stepEndTime;

	for (unsigned int num = 0; num < totalCount; num++)
	{
		fread_s(&stepStartTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);
		fread_s(&stepEndTime, sizeof(unsigned long long), sizeof(unsigned long long), 1, fp);

		if (num < completedCount)
		{
			WD2StepInfo^ info = gcnew WD2StepInfo();
			info->StepStartTime = DateTime::FromFileTimeUtc(stepStartTime);
			info->StepEndTime = DateTime::FromFileTimeUtc(stepEndTime);

			WD2StepInfos[num] = info;
		}
	}
}

void WD2Reader::ReadRecords(array<WD2RecipeItem^>^ recipeItems, unsigned int samplingCount, WD2RecipeInfo^ recipeInfo)
{
	if (recipeItems == nullptr || recipeItems->Length == 0)
		return;

	unsigned int elapsed;
	float floatData;
	int intData;
	for (unsigned int num = 0; num < samplingCount; num++)
	{
		fread_s(&elapsed, sizeof(unsigned int), sizeof(unsigned int), 1, fp);

		for (int index = 0; index < recipeItems->Length; index++)
		{
			WD2RecipeItem^ item = recipeItems[index];
			DataPoint^ data = gcnew DataPoint();
			item->DataPoints->Add(data);

			data->Elapsed = elapsed;
			data->X = recipeInfo->RecipeStartTime.AddMilliseconds(elapsed);

			if (item->AttributeType == AttrType::Float)
			{
				fread_s(&floatData, sizeof(float), sizeof(float), 1, fp);
				data->Y = floatData;
			}
			else // Int
			{
				fread_s(&intData, sizeof(int), sizeof(int), 1, fp);
				data->Y = intData;
			}
		}
	}
}

