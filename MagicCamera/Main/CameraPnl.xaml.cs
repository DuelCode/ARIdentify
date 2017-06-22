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
using System.Windows.Threading;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging.Textures;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using PublicLibrary.Core.Common_Inner;

namespace MagicCamera.Main
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

        AForge.Vision.Motion.MotionDetector detector = new AForge.Vision.Motion.MotionDetector(
        new TwoFramesDifferenceDetector(),
        new MotionAreaHighlighting());

        private IFilter _CurFilter = null;

        public IFilter CurFilter
        {
            get { return _CurFilter; }
            set
            {
                _CurFilter = value;
                this._CurSpecicalFilter = null;
            }
        }

        private IFilter _CurSpecicalFilter = null;
        public IFilter CurSpecicalFilter
        {
            get { return _CurSpecicalFilter; }
            set
            {
                _CurSpecicalFilter = value;
                this._CurFilter = null;
            }
        }

        private void CameraZm_NewFrame(object sender, ref System.Drawing.Bitmap oBitmap)
        {
            //#region 跟踪变化项
            //lock (this)
            //{
            //    if (detector != null)
            //    {
            //        detector.ProcessFrame(oBitmap);
            //    }
            //}
            //#endregion

            if (this.CurFilter != null)
                oBitmap = CurFilter.Apply(oBitmap);
            else if (this.CurSpecicalFilter != null)
            {
                Bitmap oFilterBitmap = Grayscale.CommonAlgorithms.RMY.Apply(oBitmap);
                oBitmap = CurSpecicalFilter.Apply(oFilterBitmap);
            }
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
