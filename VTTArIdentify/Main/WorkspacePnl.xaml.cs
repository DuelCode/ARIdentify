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
using System.Windows.Threading;
using VTTArIdentify.Main.Camera;

namespace VTTArIdentify.Main
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

            this.CreateCameraPnl();

            this.BdrStartZm.MouseLeftButtonUp += BdrStartZm_MouseLeftButtonUp;
            this.BdrPauseZm.MouseLeftButtonUp += BdrPauseZm_MouseLeftButtonUp;
            this.BdrClearZm.MouseLeftButtonUp += BdrClearZm_MouseLeftButtonUp;
        }

        void BdrPauseZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CameraPanel != null)
                this.CameraPanel.IsIdentify = false;            
        }

        void BdrStartZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CameraPanel != null)
                this.CameraPanel.IsIdentify = true;
        }

        private void BdrClearZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.LbxResultZm.Items.Clear();
            this.LbxResultErrorZm.Items.Clear();
        }

        private void CreateCameraPnl()
        {
            CameraPanel = new CameraPnl();
            this.CameraPanel.QrCodeIdentified += CameraPanel_QrCodeIdentified;
            this.GdCameraZm.Children.Add(CameraPanel);
        }

        private DispatcherTimer IdentifyTimer;
        private string CurIdentifyResult = "";
        private int IdentifyCount = 0;

        private void CreateIdentifyTimer()
        {
            IdentifyTimer = new DispatcherTimer();
            IdentifyTimer.Interval = TimeSpan.FromMilliseconds(100);
            IdentifyTimer.Tick += IdentifyTimer_Tick;
        }

        private void IdentifyTimer_Tick(object sender, EventArgs e)
        {
            
        }

        private void CameraPanel_QrCodeIdentified(string sResult)
        {
            DispatcherTimer oDelayTimer = new DispatcherTimer();
            oDelayTimer.Interval = TimeSpan.FromMilliseconds(1);
            oDelayTimer.Tick += (s1, e1) =>
            {
                oDelayTimer.Stop();
                oDelayTimer.Tick -= (s2, e2) => { };
                oDelayTimer = null;

                Border oBorder = new Border() { Height = 16 };
                TextBlock oTextBlock = new TextBlock() { VerticalAlignment = VerticalAlignment.Center };
              
                oBorder.Child = oTextBlock;

                if (sResult.Contains("Invalid number of Finder Pattern detected") || sResult.Contains(
                        "This method must be called after QRCodeImageReader.getQRCodeSymbol() called")
                    || sResult.Contains("未将对象引用设置到对象的实例"))
                {
                    oTextBlock.Text = DateTime.Now.ToLongTimeString() + "  识别结果： " + sResult;
                    this.LbxResultErrorZm.Items.Add(oBorder);
                    this.SvResultErrorZm.ScrollToEnd();

                    IdentifyCount++;
                    if (IdentifyCount == 3)
                    {
                        this.CurIdentifyResult = "";
                        IdentifyCount = 0;
                        // Identify Nothing
                        this.TbkResultZm.Text = "未识别";
                    }
                }
                else
                {
                    oTextBlock.Text = DateTime.Now.ToLongTimeString() + "  识别结果： " + sResult;
                    this.LbxResultZm.Items.Add(oBorder);
                    this.SvResultZm.ScrollToEnd();

                    if (this.CurIdentifyResult == "")
                    {
                        this.CurIdentifyResult = sResult;
                        this.TbkResultZm.Text = sResult;
                    }
                    else
                    {
                        if (this.CurIdentifyResult == sResult)
                            IdentifyCount = 0;
                        else
                        {
                            IdentifyCount ++;
                            if (IdentifyCount == 3)
                            {
                                this.CurIdentifyResult = sResult;
                                IdentifyCount = 0;

                                // New Target
                                this.TbkResultZm.Text = sResult;
                            }
                        }
                    }
                }
            };
           oDelayTimer.Start();
        }

        private void WorkspacePnl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= WorkspacePnl_Unloaded;

            this.GdCameraZm.Children.Clear();
            this.CameraPanel = null;
        }
    }
}
