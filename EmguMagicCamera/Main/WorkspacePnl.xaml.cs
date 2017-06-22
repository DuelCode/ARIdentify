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

namespace EmguMagicCamera.Main
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

            this.CameraPanel = new CameraPnl();
            this.GdCameraZm.Children.Add(this.CameraPanel);
        }

        private void BdrCameraFilter_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border oBorder = sender as Border;
            string sTag = oBorder.Tag.ToString();
            FilterType oFilterType = FilterUtils.ConvertStrToFilterType(sTag);
            if (this.CameraPanel != null)
                this.CameraPanel.CurFilterType = oFilterType;
        }
    }
}
