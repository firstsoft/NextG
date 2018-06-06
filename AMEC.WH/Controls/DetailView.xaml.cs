using LWIR.NET.Common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMEC.WH.Controls
{
    /// <summary>
    /// Interaction logic for DetailView.xaml
    /// </summary>
    [Export(ViewManagerModel.DetailView, typeof(DetailView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DetailView : UserControl
    {
        public DetailView()
        {
            InitializeComponent();
        }

        [Import(typeof(DetailViewModel))]
        DetailViewModel viewmodel
        {
            set { this.DataContext = value; }
            get { return this.DataContext as DetailViewModel; }
        }
    }
}
