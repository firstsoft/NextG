using LWIR.NET.Framework.Core.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LWIR.Window
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AboutWindow : BaseWnd
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.Closed += AboutWindow_Closed;
        }

        void AboutWindow_Closed(object sender, EventArgs e)
        {
            if (viewmodel == null)
                return;

            viewmodel.Dispose();
        }

        [Import(typeof(AboutViewModel))]
        AboutViewModel viewmodel
        {
            set { this.DataContext = value; }
            get { return this.DataContext as AboutViewModel; }
        }
    }
}
