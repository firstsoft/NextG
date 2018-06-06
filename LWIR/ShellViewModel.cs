using LWIR.NET.Common;
using LWIR.NET.Common.Entity;
using LWIR.NET.Common.Enum;
using LWIR.NET.Common.Event;
using LWIR.NET.Common.Model;
using LWIR.NET.Common.Windows;
using LWIR.NET.Framework.Enum;
using LWIR.NET.Framework.Interface;
using LWIR.NET.Framework.MVVM;
using LWIR.Window;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace LWIR
{
    [Export]
    public class ShellViewModel : ViewModelBase
    {
        public RelayCommand MouseDownCommand { private set; get; }

        public RelayCommand<CancelEventArgs> ClosingCommand { private set; get; }

        public RelayCommand ContentRenderedCommand { private set; get; }

        public RelayCommand SelectFolderCommand { private set; get; }

        public RelayCommand SearchCommand { private set; get; }

        public RelayCommand AutoSetFolderCommand { private set; get; }

        public RelayCommand AboutMenuClickCommand { private set; get; }

        private IRegionManager inRegionManager;
        private IEventAggregator inEventAggregator;

        #region 属性
        private bool isMenuOpen = false;
        /// <summary>
        /// 语言切换按钮菜单是否展开
        /// </summary>
        public bool IsMenuOpen
        {
            get { return isMenuOpen; }
            set { Set(() => IsMenuOpen, ref isMenuOpen, value); }
        }

        private bool isChinese = false;
        /// <summary>
        /// 当前是否是中文
        /// </summary>
        public bool IsChinese
        {
            get { return isChinese; }
            set
            {
                Set(() => IsChinese, ref isChinese, value);

                IsMenuOpen = false;
                if (isChinese)
                {
                    LanguageModel.Instance.SelectLanguage(NET.Common.Enum.LanguageEnum.Chinese);
                }
            }
        }

        private bool isEnglish = false;
        /// <summary>
        /// 当前是否是英文
        /// </summary>
        public bool IsEnglish
        {
            get { return isEnglish; }
            set
            {
                Set(() => IsEnglish, ref isEnglish, value);

                IsMenuOpen = false;
                if (isEnglish)
                {
                    LanguageModel.Instance.SelectLanguage(NET.Common.Enum.LanguageEnum.English);
                }
            }
        }

        private string curSelectFolder = string.Empty;
        /// <summary>
        /// Current history path
        /// </summary>
        public string CurSelectFolder
        {
            get { return curSelectFolder; }
            set
            {
                Set(() => CurSelectFolder, ref curSelectFolder, value);

                ScanFilesToDisplay();
            }
        }

        private DateTime dateFrom = DateTime.Now.AddDays(-7);
        public DateTime DateFrom
        {
            get { return dateFrom; }
            set { Set(() => DateFrom, ref dateFrom, value); }
        }

        private DateTime dateTo = DateTime.Now;
        public DateTime DateTo
        {
            get { return dateTo; }
            set { Set(() => DateTo, ref dateTo, value); }
        }

        private string foupId = string.Empty;
        public string FoupId
        {
            get { return foupId; }
            set { Set(() => FoupId, ref foupId, value); }
        }

        private string lotId = string.Empty;
        public string LotId
        {
            get { return lotId; }
            set { Set(() => LotId, ref lotId, value); }
        }

        private string waferId = string.Empty;
        public string WaferId
        {
            get { return waferId; }
            set { Set(() => WaferId, ref waferId, value); }
        }

        private string recipeName = string.Empty;
        public string RecipeName
        {
            get { return recipeName; }
            set { Set(() => RecipeName, ref recipeName, value); }
        }

        private Dictionary<MOD_ID, string> modList = new Dictionary<MOD_ID, string>();
        public Dictionary<MOD_ID, string> ModList
        {
            get { return modList; }
            set { Set(() => ModList, ref modList, value); }
        }

        private Dictionary<RecipeType, string> recipeTypeList = new Dictionary<RecipeType, string>();
        public Dictionary<RecipeType, string> RecipeTypeList
        {
            get { return recipeTypeList; }
            set { Set(() => RecipeTypeList, ref recipeTypeList, value); }
        }

        private MOD_ID selectedMod = MOD_ID.MOD_ID_ALL;
        public MOD_ID SelectedMod
        {
            get { return selectedMod; }
            set { Set(() => SelectedMod, ref selectedMod, value); }
        }

        private RecipeType selectedRecipeType = RecipeType.Process;
        public RecipeType SelectedRecipeType
        {
            get { return selectedRecipeType; }
            set { Set(() => SelectedRecipeType, ref selectedRecipeType, value); }
        }

        private long msgCount = 0L;
        public long MsgCount
        {
            get { return msgCount; }
            set { Set(() => MsgCount, ref msgCount, value); }
        }

        private long sh2Count = 0L;
        public long Sh2Count
        {
            get { return sh2Count; }
            set { Set(() => Sh2Count, ref sh2Count, value); }
        }

        private long wh3Count = 0L;
        public long Wh3Count
        {
            get { return wh3Count; }
            set { Set(() => Wh3Count, ref wh3Count, value); }
        }

        #endregion

        [ImportingConstructor]
        public ShellViewModel(IRegionManager InRegionManager, IEventAggregator InEventAggregator)
        {
            inRegionManager = InRegionManager;
            inEventAggregator = InEventAggregator;

            MouseDownCommand = new RelayCommand(MouseDown_Callback);
            ClosingCommand = new RelayCommand<CancelEventArgs>(Closing_Callback);
            ContentRenderedCommand = new RelayCommand(ContentRendered_Callback);
            SelectFolderCommand = new RelayCommand(SelectFolder_Callback);
            SearchCommand = new RelayCommand(Search_Callback);
            AutoSetFolderCommand = new RelayCommand(AutoSetFolder_Callback);
            AboutMenuClickCommand = new RelayCommand(AboutMenuClicked);

            //默认语言选择
            IsChinese = LanguageModel.Instance.CurLanguage.CurLanguage == NET.Common.Enum.LanguageEnum.Chinese;
            IsEnglish = LanguageModel.Instance.CurLanguage.CurLanguage == NET.Common.Enum.LanguageEnum.English;

            ModList.Add(MOD_ID.MOD_ID_ALL, "ALL");
            ModList.Add(MOD_ID.MOD_ID_LOADLOCK_A, "LL A");
            ModList.Add(MOD_ID.MOD_ID_LOADLOCK_B, "LL B");
            ModList.Add(MOD_ID.MOD_ID_PROCESS_MODULE_A1, "A1");
            ModList.Add(MOD_ID.MOD_ID_PROCESS_MODULE_A2, "A2");
            ModList.Add(MOD_ID.MOD_ID_PROCESS_MODULE_B1, "B1");
            ModList.Add(MOD_ID.MOD_ID_PROCESS_MODULE_B2, "B2");
            ModList.Add(MOD_ID.MOD_ID_PROCESS_MODULE_C1, "C1");
            ModList.Add(MOD_ID.MOD_ID_PROCESS_MODULE_C2, "C2");

            RecipeTypeList.Add(RecipeType.Process, "Process");
            RecipeTypeList.Add(RecipeType.Clean, "Clean");

            LWIR.NET.Framework.Utility.ThreadPool.Instance.LastMsgCountEvent += LastMsgCount_Callback;
            inEventAggregator.GetEvent<FilesCountEvent>().Subscribe(ReceiveFilesCount);

            if(Properties.Settings.Default.ShowTips)
            {
                AboutMenuClicked();
            }
        }
        
        private void AboutMenuClicked()
        {
            foreach (System.Windows.Window w in Application.Current.Windows)
            {
                if (w.GetType() == typeof(AboutWindow))
                {
                    w.Close();
                }
            }

            AboutWindow aboutWnd = (AboutWindow)ServiceLocator.Current.GetInstance(typeof(AboutWindow));
            aboutWnd.Show();
        }

        private void Search_Callback()
        {
            if (DateTo.Date <= DateFrom.Date)
            {
                string yesText = (string)Application.Current.FindResource("DLG_BUTTON_YES");
                string noText = (string)Application.Current.FindResource("DLG_BUTTON_NO");
                string titleText = "Hint";
                string infoText = "The DateFrom should be earlier than DateTo.";

                MessageBoxEx.ShowDialog(DialogType.Warning, titleText, infoText);
                return;
            }

            SearchConditions conditions = new SearchConditions();
            conditions.DateFrom = DateFrom.Date;
            conditions.DateTo = DateTo.Date;
            conditions.FoupId = FoupId.Trim();
            conditions.LotId = LotId.Trim();
            conditions.WaferId = WaferId.Trim();
            conditions.RecipeName = RecipeName.Trim();
            conditions.RecipeType = SelectedRecipeType.ToString();
            conditions.ModuleId = SelectedMod.ToString();

            inEventAggregator.GetEvent<SearchConditionsEvent>().Publish(conditions);
        }

        private void ReceiveFilesCount(FilesCount filesCount)
        {
            Application.Current.Dispatcher.BeginInvoke((ThreadStart)(() =>
            {
                Sh2Count = filesCount.Sh2Count;
                Wh3Count = filesCount.Wh3Count;
            }));
        }

        private void LastMsgCount_Callback(long count)
        {
            Application.Current.Dispatcher.BeginInvoke((ThreadStart)(() => { MsgCount = count; }));
        }

        private void ContentRendered_Callback()
        {
            inRegionManager.NavigateTo(RegionNames.MainRegion, ViewManagerModel.DetailView);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void Closing_Callback(CancelEventArgs e)
        {
            string yesText = (string)Application.Current.FindResource("DLG_BUTTON_YES");
            string noText = (string)Application.Current.FindResource("DLG_BUTTON_NO");
            string cancelText = (string)Application.Current.FindResource("DLG_BUTTON_IGORE");
            string titleText = (string)Application.Current.FindResource("DLG_TITLE_SHUTDOWN_SYSTEM");
            string infoText = (string)Application.Current.FindResource("DLG_TITLE_SHUTDOWN_SYSTEM_INFO");

            if (!QuestionDialogBox.ShowDialog(titleText, infoText, yesText, noText, cancelText))
            {
                e.Cancel = true;
                return;
            }

            LWIR.NET.Framework.Utility.ThreadPool.Instance.Stop();
        }

        /// <summary>
        /// Select foloder for history
        /// </summary>
        private void SelectFolder_Callback()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CurSelectFolder = dialog.SelectedPath;
                }
            }
        }

        private void AutoSetFolder_Callback()
        {
            //read environment
            string SH_HOME = Environment.GetEnvironmentVariable("SH_HOME");

            if (!string.IsNullOrEmpty(SH_HOME))
            {
                CurSelectFolder = Path.Combine(SH_HOME, "Data", "History");
            }
            else
            {
                CurSelectFolder = AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// 鼠标点击事件，关闭语言菜单
        /// </summary>
        private void MouseDown_Callback()
        {
            IsMenuOpen = false;
        }


        private void ScanFilesToDisplay()
        {
            if (!Directory.Exists(CurSelectFolder))
            {
                MessageBoxEx.ShowDialog(DialogType.Hint, "Hint", "The folder does not exist.");

                return;
            }

            inEventAggregator.GetEvent<HistoryFolderEvent>().Publish(CurSelectFolder);
        }

        public override void Dispose()
        {
            LWIR.NET.Framework.Utility.ThreadPool.Instance.LastMsgCountEvent -= LastMsgCount_Callback;
            inEventAggregator.GetEvent<FilesCountEvent>().Unsubscribe(ReceiveFilesCount);
        }
    }
}
