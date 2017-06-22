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
using EmguSamples.Main;

namespace EmguSamples
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

            CameraPanel = new CameraPnl();
            this.GdRootZm.Children.Add(this.CameraPanel);
        }
    }
}
