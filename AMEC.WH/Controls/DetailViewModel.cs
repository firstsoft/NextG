using AMEC.Native;
using AMEC.WH.Model;
using LWIR.NET.Common.Entity;
using LWIR.NET.Common.Event;
using LWIR.NET.Common.Windows;
using LWIR.NET.Framework.Enum;
using LWIR.NET.Framework.Interface;
using LWIR.NET.Framework.MVVM;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace AMEC.WH.Controls
{
    [Export(typeof(DetailViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DetailViewModel : ViewModelBase
    {
        private IRegionManager inRegionManager;
        private IEventAggregator inEventAggregator;

        #region Collection
        private ObservableCollection<SH2Info> sh2Collection = new ObservableCollection<SH2Info>();
        public ObservableCollection<SH2Info> Sh2Collection
        {
            get { return sh2Collection; }
            set { Set(() => Sh2Collection, ref sh2Collection, value); }
        }

        private ObservableCollection<WaferHistoryInfo> wh3Collection = new ObservableCollection<WaferHistoryInfo>();
        public ObservableCollection<WaferHistoryInfo> Wh3Collection
        {
            get { return wh3Collection; }
            set { Set(() => Wh3Collection, ref wh3Collection, value); }
        }

        private SeqWaferInfo[] wh3SummaryCollection = null;
        public SeqWaferInfo[] Wh3SummaryCollection
        {
            get { return wh3SummaryCollection; }
            set { Set(() => Wh3SummaryCollection, ref wh3SummaryCollection, value); }
        }

        private SH2Info curSH2Info = null;
        public SH2Info CurSH2Info
        {
            get { return curSH2Info; }
            set
            {
                Set(() => CurSH2Info, ref curSH2Info, value);

                Wh3SummaryCollection = curSH2Info == null ? null : curSH2Info.SeqWaferInfos;
            }
        }

        private SeqWaferInfo curSeqWaferInfo = null;
        public SeqWaferInfo CurSeqWaferInfo
        {
            get { return curSeqWaferInfo; }
            set
            {
                Set(() => CurSeqWaferInfo, ref curSeqWaferInfo, value);

                if (curSeqWaferInfo == null)
                    return;

                CurWaferHistoryInfo = FileModel.Instance.GetWaferHistoryInfo(curSeqWaferInfo.WaferHistoryFullfileName);
            }
        }

        private WaferHistoryInfo curWaferHistoryInfo = null;
        public WaferHistoryInfo CurWaferHistoryInfo
        {
            get { return curWaferHistoryInfo; }
            set { Set(() => CurWaferHistoryInfo, ref curWaferHistoryInfo, value); }
        }

        #endregion

        public RelayCommand ShowAllCommand { private set; get; }
        public RelayCommand DataCollectionViewCommand { private set; get; }

        [ImportingConstructor]
        public DetailViewModel(IRegionManager _inRegionManager, IEventAggregator _inEventAggregator)
        {
            inRegionManager = _inRegionManager;
            inEventAggregator = _inEventAggregator;

            ShowAllCommand = new RelayCommand(ShowAll_Callback);
            DataCollectionViewCommand = new RelayCommand(DataCollectionView_Callback);
            inEventAggregator.GetEvent<HistoryFolderEvent>().Subscribe(ReceiveHistoryFolder_Callback);
            inEventAggregator.GetEvent<SearchConditionsEvent>().Subscribe(SearchWaferHistory_Callback);
            FileModel.Instance.PostSH2InfoEvent += PostSH2Info_Callback;
            FileModel.Instance.PostWaferHistoryInfoEvent += PostWaferHistoryInfo_Callback;
            FileModel.Instance.PostFilesCountEvent += PostFilesCount_Callback;

            //Load event message
            EventModel.Instance.Load();
        }

        private void DataCollectionView_Callback()
        {
            if (CurWaferHistoryInfo == null)
            {
                goto Wd2ViewFailed;
            }

            if (CurWaferHistoryInfo.WaferDataBlocks == null || CurWaferHistoryInfo.WaferDataBlocks.All(s => s.SubWaferDataBlocks == null))
            {
                goto Wd2ViewFailed;
            }

            string[] wd2Files = CurWaferHistoryInfo.WaferDataBlocks.SelectMany(w => w.SubWaferDataBlocks.Select(s => s.CurRecipeInfo.DataCollectionFullName)).ToArray();

            if (wd2Files == null || wd2Files.Length == 0)
            {
                goto Wd2ViewFailed;
            }

            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(Wd2Viewer))
                {
                    w.Close();
                }
            }

            Wd2Viewer viewer = (Wd2Viewer)ServiceLocator.Current.GetInstance(typeof(Wd2Viewer));
            viewer.Show();

            inEventAggregator.GetEvent<Wd2FilesEvent>().Publish(wd2Files);
            return;

        Wd2ViewFailed:
            MessageBoxEx.ShowDialog(DialogType.Hint, "Hint", "There's no *.wd2 file to show.");
        }

        private void ShowAll_Callback()
        {
            Wh3Collection = new ObservableCollection<WaferHistoryInfo>(FileModel.Instance.Wh3Collection);
        }

        private void SearchWaferHistory_Callback(SearchConditions conditions)
        {
            CurWaferHistoryInfo = null;
            Wh3Collection.Clear();

            CRecipeType curRecipeType = (CRecipeType)System.Enum.Parse(typeof(CRecipeType), conditions.RecipeType);
            CMOD_ID curModuleId = (CMOD_ID)System.Enum.Parse(typeof(CMOD_ID), conditions.ModuleId);

            IEnumerable<WaferHistoryInfo> curSearch = FileModel.Instance.Wh3Collection.Where(w => w.CurWaferInfo.WaferCreateTime >= conditions.DateFrom && w.CurWaferInfo.WaferCreateTime <= conditions.DateTo);

            if (!string.IsNullOrEmpty(conditions.FoupId))
            {
                curSearch = curSearch.Where(w => w.CurWaferInfo.FoupId.Contains(conditions.FoupId));
            }

            if (!string.IsNullOrEmpty(conditions.LotId))
            {
                curSearch = curSearch.Where(w => w.CurWaferInfo.LotId.Contains(conditions.LotId));
            }

            if (!string.IsNullOrEmpty(conditions.WaferId))
            {
                curSearch = curSearch.Where(w => w.CurWaferInfo.WaferId.Contains(conditions.WaferId));
            }

            if (!string.IsNullOrEmpty(conditions.RecipeName))
            {
                curSearch = curSearch.Where(w => w.WaferDataBlocks != null
                    && w.WaferDataBlocks.Any(b => b.SubWaferDataBlocks != null
                        && b.SubWaferDataBlocks.Any(sb => sb.CurRecipeInfo.RecipeName.Contains(conditions.RecipeName) && sb.CurRecipeInfo.RecipeType == curRecipeType)));
            }
            else
            {
                curSearch = curSearch.Where(w => w.WaferDataBlocks != null
                    && w.WaferDataBlocks.Any(b => b.SubWaferDataBlocks != null
                        && b.SubWaferDataBlocks.Any(sb => sb.CurRecipeInfo.RecipeType == curRecipeType)));
            }

            if (curModuleId != CMOD_ID.MOD_ID_ALL)
            {
                curSearch = curSearch.Where(w => w.WaferDataBlocks != null &&
                    w.WaferDataBlocks.Any(b => b.CurMoveData != null && b.CurMoveData.MoveInfoCount > 0 && b.CurMoveData.MoveInfos.Any(m => m.MOD_ID == curModuleId)));
            }

            Wh3Collection = new ObservableCollection<WaferHistoryInfo>(curSearch.ToList());
        }

        private void PostSH2Info_Callback()
        {
            Sh2Collection = new ObservableCollection<SH2Info>(FileModel.Instance.Sh2Collection);
        }

        private void PostWaferHistoryInfo_Callback()
        {
            Wh3Collection = new ObservableCollection<WaferHistoryInfo>(FileModel.Instance.Wh3Collection);
        }


        private void ReceiveHistoryFolder_Callback(string historyFolder)
        {
            Sh2Collection.Clear();
            FileModel.Instance.ReceiveHistoryFolder(historyFolder);
        }

        private void PostFilesCount_Callback(long sh2Count, long wh3Count)
        {
            inEventAggregator.GetEvent<FilesCountEvent>().Publish(new FilesCount() { Sh2Count = sh2Count, Wh3Count = wh3Count });
        }

        public override void Dispose()
        {
            inEventAggregator.GetEvent<HistoryFolderEvent>().Unsubscribe(ReceiveHistoryFolder_Callback);
            inEventAggregator.GetEvent<SearchConditionsEvent>().Unsubscribe(SearchWaferHistory_Callback);
            FileModel.Instance.PostSH2InfoEvent -= PostSH2Info_Callback;
            FileModel.Instance.PostWaferHistoryInfoEvent -= PostWaferHistoryInfo_Callback;
            FileModel.Instance.PostFilesCountEvent -= PostFilesCount_Callback;
        }
    }
}
