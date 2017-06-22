using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using PublicLibrary.Core.Common_Inner;

namespace VisionDetctor.Contorls
{
    /// <summary>
    /// CameraPnl.xaml 的交互逻辑
    /// </summary>
    public partial class CameraPnl : UserControl
    {
        public CameraPnl()
        {
            InitializeComponent();

            this.Loaded += CameraPnl_Loaded;
            this.Unloaded += CameraPnl_Unloaded;
            AppShutDownEventHandler.AppShutDowningEvent += AppShutDownEventHandler_AppShutDowningEvent;
            this.CameraZm.NewFrame += CameraZm_NewFrame;
        }

        MotionDetector detector = new MotionDetector(
         new TwoFramesDifferenceDetector(),
         new MotionAreaHighlighting());

        public delegate void NewFrameEventHandler(System.Drawing.Bitmap oBitmap);

        public event NewFrameEventHandler NewFrame;


        private Invert SobelEdgeFilter = new Invert();

        private void CameraZm_NewFrame(object sender, ref System.Drawing.Bitmap oBitmap)
        {
            if (NewFrame != null)
                this.NewFrame(oBitmap);

            //lock (this)
            //{
            //    if (detector != null)
            //    {
            //        detector.ProcessFrame(oBitmap);
            //    }
            //}

            //oBitmap = SobelEdgeFilter.Apply(oBitmap);
        }

        private void AppShutDownEventHandler_AppShutDowningEvent()
        {
            this.CloseVideoSource();
        }

        private void CameraPnl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CameraPnl_Loaded;

            this.LoadCameraDevice();
        }

        #region Camera

        private void LoadCameraDevice()
        {
            try
            {
                FilterInfoCollection arrDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                string oDeviceName = arrDevices[1].MonikerString;
                VideoCaptureDevice oCaptureDevice = new VideoCaptureDevice(oDeviceName);
                OpenVideoSource(oCaptureDevice);
            }
            catch { }
        }

        private void OpenVideoSource(IVideoSource source)
        {
            this.Cursor = Cursors.Wait;
            CloseVideoSource();
            CameraZm.VideoSource = new AsyncVideoSource(source);
            CameraZm.Start();
            this.Cursor = Cursors.Arrow;
        }

        private void CloseVideoSource()
        {
            this.Cursor = Cursors.Wait;
            try
            {
                CameraZm.SignalToStop();
                for (int i = 0; (i < 50) && (CameraZm.IsRunning); i++)
                    Thread.Sleep(100);
                if (CameraZm.IsRunning)
                    CameraZm.Stop();
            }
            catch { }
            this.Cursor = Cursors.Arrow;
        }

        #endregion

        private void CameraPnl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= CameraPnl_Unloaded;

            this.CloseVideoSource();
        }
    }
}
