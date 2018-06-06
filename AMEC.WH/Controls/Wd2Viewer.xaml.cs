using LWIR.NET.Common;
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

namespace AMEC.WH.Controls
{
    /// <summary>
    /// Interaction logic for Wd2Viewer.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Wd2Viewer : BaseWnd
    {
        public Wd2Viewer()
        {
            InitializeComponent();
            this.Closed += Wd2Viewer_Closed;
        }

        void Wd2Viewer_Closed(object sender, EventArgs e)
        {
            if (viewmodel == null)
                return;

            viewmodel.Dispose();
        }

        [Import(typeof(Wd2ViewModel))]
        Wd2ViewModel viewmodel
        {
            set { this.DataContext = value; }
            get { return this.DataContext as Wd2ViewModel; }
        }
    }
}
