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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LWIR
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    [Export]
    public partial class Shell : BaseWnd
    {
        public Shell()
        {
            InitializeComponent();
        }

        [Import]
        ShellViewModel viewmodel
        {
            get { return this.DataContext as ShellViewModel; }
            set { this.DataContext = value; }
        }
    }
}
