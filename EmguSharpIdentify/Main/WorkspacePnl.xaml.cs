using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmguSharpIdentify.Main
{
    /// <summary>
    /// WorkspacePnl.xaml 的交互逻辑
    /// </summary>
    public partial class WorkspacePnl : UserControl
    {
        private CameraPnl CameraPanel;

        public WorkspacePnl()
        {
            InitializeComponent();

            this.Loaded += WorkspacePnl_Loaded;
            this.Unloaded += WorkspacePnl_Unloaded;
        }

        private void WorkspacePnl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= WorkspacePnl_Loaded;

            CameraPanel = new CameraPnl();
            this.GdCameraZm.Children.Add(this.CameraPanel);
        }

        private void WorkspacePnl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= WorkspacePnl_Unloaded;
        }
    }
}
