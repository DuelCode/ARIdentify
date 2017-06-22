using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using PublicLibrary.Core.Common_Inner;

namespace EmguSharpIdentify.Main
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

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat frame = new Mat();
            _capture.Retrieve(frame, 0);
            this.ImgMainZm.Image = frame;


            //Image<Bgr, Byte> img =
            //   new Image<Bgr, byte>()
            //   .Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);


            UMat uimage = new UMat();
            CvInvoke.CvtColor(frame, uimage, ColorConversion.Bgr2Gray);
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);

            this.ImgAfterHandlerZm.Image = uimage;

            #region circle detection
            Stopwatch watch = Stopwatch.StartNew();
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 120;
            CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);

            watch.Stop();
            //msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
            #endregion

            #region draw circles
            Mat circleImage = new Mat(frame.Size, DepthType.Cv8U, 3);
            circleImage.SetTo(new MCvScalar(0));
            foreach (CircleF circle in circles)
                CvInvoke.Circle(circleImage, System.Drawing.Point.Round(circle.Center), (int)circle.Radius, new Bgr(System.Drawing.Color.Brown).MCvScalar, 2);

            this.ImgResultZm.Image = circleImage;
            #endregion
        }

        private void ReleaseData()
        {
            if (_capture != null)
                _capture.Dispose();
        }
    }
}
