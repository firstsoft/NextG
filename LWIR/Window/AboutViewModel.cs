using LWIR.NET.Framework.Interface;
using LWIR.NET.Framework.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace LWIR.Window
{
    [Export(typeof(AboutViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AboutViewModel : ViewModelBase
    {
        public RelayCommand EmailBtnCommand { private set; get; }
        private IRegionManager inRegionManager;
        private IEventAggregator inEventAggregator;

        [ImportingConstructor]
        public AboutViewModel(IRegionManager InRegionManager, IEventAggregator InEventAggregator)
        {
            IsTipsShow = Properties.Settings.Default.ShowTips;
            EmailBtnCommand = new RelayCommand(EmailBtn_Click);
        }

        private void EmailBtn_Click()
        {
            var url = "mailto:SudiFan@amecnsh.com?subject=LWIR&body=";
            System.Diagnostics.Process.Start(url);
        }

        private bool m_IsTipsShow = false;
        public bool IsTipsShow
        {
            get { return m_IsTipsShow; }
            set
            {
                Set(() => IsTipsShow, ref m_IsTipsShow, value);

                Properties.Settings.Default.ShowTips = m_IsTipsShow;
                Properties.Settings.Default.Save();
            }
        }

        public override void Dispose()
        {

        }
    }
}
