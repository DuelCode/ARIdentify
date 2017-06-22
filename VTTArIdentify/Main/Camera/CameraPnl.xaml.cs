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
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using PublicLibrary.Core.Common_Inner;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ZXing.Presentation;

namespace VTTArIdentify.Main.Camera
{
    /// <summary>
    /// CameraPnl.xaml 的交互逻辑
    /// </summary>
    public partial class CameraPnl : UserControl
    {
        private QRCodeDecoder QRImageDecoder;
        private readonly BarcodeReader ZXingDecoder = new BarcodeReader();

        public CameraPnl()
        {
            InitializeComponent();

            this.Loaded += CameraPnl_Loaded;
            this.Unloaded += CameraPnl_Unloaded;
            AppShutDownEventHandler.AppShutDowningEvent += AppShutDownEventHandler_AppShutDowningEvent;
            this.CameraZm.NewFrame += CameraZm_NewFrame;
        }

        public delegate void QrCodeIdentifiedEventHandler(string sResult);

        public event QrCodeIdentifiedEventHandler QrCodeIdentified;

        public bool IsIdentify = false;

        private int FPSCount = -1;

        MotionDetector detector = new MotionDetector(
        new TwoFramesDifferenceDetector(),
        new MotionAreaHighlighting());

        private void CameraZm_NewFrame(object sender, ref System.Drawing.Bitmap oBitmap)
        {
            if (this.IsIdentify)
            {
                FPSCount ++;

                #region 跟踪变化项
                lock (this)
                {
                    if (detector != null)
                    {
                        detector.ProcessFrame(oBitmap);
                    }
                }
                #endregion

                if (FPSCount < 10)
                    return;
                else
                    FPSCount = -1;

                try
                {
                    string sResult = "";

                    // ****************** Failed *************************
                    // 采用ThoughtWorks.QRCode.dll时识别不出，  原始二维码制作成素材可识别。  但是摄像头扫描模式进行识别时识别结果为乱码
                    //QRCodeImage oCodeImage = new QRCodeBitmapImage(oBitmap);
                    //string sResult = QRImageDecoder.decode(oCodeImage);

                    //var result = ZXingDecoder.Decode((BitmapSource)imageBarcode.Source);
                    BitmapSource oBitmapSource = this.ConvertBitmapToBiamapSource(oBitmap);
                    sResult = ZXingDecoder.Decode(oBitmapSource).ToString();

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate()
                    {
                        if (this.QrCodeIdentified != null && !String.IsNullOrEmpty(sResult))
                            this.QrCodeIdentified(sResult);
                    });
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate()
                    {
                        if (this.QrCodeIdentified != null)
                            this.QrCodeIdentified(ex.Message);
                    });
                }
            }
        }

        public BitmapSource ConvertBitmapToBiamapSource(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height, 96, 96, PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        private void AppShutDownEventHandler_AppShutDowningEvent()
        {
            this.CloseVideoSource();
        }

        private void CameraPnl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CameraPnl_Loaded;

            this.LoadCameraDevice();

            QRImageDecoder = new QRCodeDecoder();
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
            catch{}
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
            catch{}
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
