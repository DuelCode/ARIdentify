using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using Emgu.CV;
using Emgu.CV.CvEnum;
using PublicLibrary.Core.Common_Inner;

namespace EmguSamples.Main
{
    /// <summary>
    /// CameraPnl.xaml 的交互逻辑
    /// </summary>
    public partial class CameraPnl : UserControl
    {
        private Capture _capture = null;
        private bool _captureInProgress;

        public CameraPnl()
        {
            InitializeComponent();

            CvInvoke.UseOpenCL = false;
            try
            {
                _capture = new Capture();
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }

            this.Loaded += CameraPnl_Loaded;

            AppShutDownEventHandler.AppShutDowningEvent += AppShutDownEventHandler_AppShutDowningEvent;
        }

        void AppShutDownEventHandler_AppShutDowningEvent()
        {
            this.ReleaseData();
        }

        private void CameraPnl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CameraPnl_Loaded;

            if (_capture != null)
            {
                _capture.Start();
            }
        }

        /// <summary>
        /// Delete a GDI object
        /// </summary>
        /// <param name="o">The poniter to the GDI object to be deleted</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat frame = new Mat();
            _capture.Retrieve(frame, 0);

            Mat grayFrame = new Mat();
            CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);

            Mat smallGrayFrame = new Mat();
            CvInvoke.PyrDown(grayFrame, smallGrayFrame);

            Mat smoothedGrayFrame = new Mat();
            CvInvoke.PyrUp(smallGrayFrame, smoothedGrayFrame);

            Mat cannyFrame = new Mat();
            CvInvoke.Canny(smoothedGrayFrame, cannyFrame, 100, 60);

            this.ImgNormalZm.Image = frame;
            this.ImgGrayZm.Image = grayFrame;
            this.ImgSmoothedGrayZm.Image = smoothedGrayFrame;
            this.ImgCannyZm.Image = cannyFrame;
        }

        private void ReleaseData()
        {
            if (_capture != null)
                _capture.Dispose();
        }

    }
}
