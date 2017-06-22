using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
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

namespace EmguMagicCamera.Main
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

        public FilterType CurFilterType = FilterType.ftNormal;


        //private string[] GFilterStr =
        //{
        //    "Normal", "Gray", "SmallGray", "SmoothGray", "Canny", "Winter", "Summer",
        //    "Spring", "Rainbow", "Pink", "Ocean", "Jet", "Hsv", "Hot" , "Cool" , "Bone" , "Autumn" 
        //    ,"BitwiseNot"
        //};

        public void SuccessSample()
        {
            Mat frame = new Mat();
            Mat newFrame = new Mat();

            CvInvoke.EdgePreservingFilter(frame, newFrame);
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat frame = new Mat();
            _capture.Retrieve(frame, 0);

            Mat newFrame = new Mat();

            
           
            //CvInvoke.EdgePreservingFilter(frame, newFrame);
            //CvInvoke.Dct(frame, newFrame, DctType.Forward);
            //CvInvoke.CornerHarris(frame, newFrame, 4);
            //CvInvoke.ConvertPointsToHomogeneous(frame, newFrame);
            //CvInvoke.ConvertPointsFromHomogeneous(frame, newFrame);
            //CvInvoke.Blur(frame, newFrame, new System.Drawing.Size(100,100), new System.Drawing.Point(100,50));
            //CvInvoke.BoxFilter(frame, newFrame, DepthType.Default, new System.Drawing.Size(100, 100), new System.Drawing.Point(100, 50));
            //this.ImgMainZm.Image = newFrame;
            //return;

            if (this.CurFilterType == FilterType.ftNormal)
            {
                this.ImgMainZm.Image = frame;
            }
            else if (this.CurFilterType == FilterType.ftGray)
            {
                newFrame = new Mat();
                CvInvoke.CvtColor(frame, newFrame, ColorConversion.Bgr2Gray);
                this.ImgMainZm.Image = newFrame;
            }
            else if (this.CurFilterType == FilterType.ftSmallGray)
            {
                Mat grayFrame = new Mat();
                CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);

                Mat smallGrayFrame = new Mat();
                CvInvoke.PyrDown(grayFrame, smallGrayFrame);

                this.ImgMainZm.Image = smallGrayFrame;
            }
            else if (this.CurFilterType == FilterType.ftSmoothGray)
            {
                Mat grayFrame = new Mat();
                CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);

                Mat smallGrayFrame = new Mat();
                CvInvoke.PyrDown(grayFrame, smallGrayFrame);

                Mat smoothedGrayFrame = new Mat();
                CvInvoke.PyrUp(smallGrayFrame, smoothedGrayFrame);

                this.ImgMainZm.Image = smoothedGrayFrame;
            }
            else if (this.CurFilterType == FilterType.ftCanny)
            {
                Mat grayFrame = new Mat();
                CvInvoke.CvtColor(frame, grayFrame, ColorConversion.Bgr2Gray);

                Mat smallGrayFrame = new Mat();
                CvInvoke.PyrDown(grayFrame, smallGrayFrame);

                Mat smoothedGrayFrame = new Mat();
                CvInvoke.PyrUp(smallGrayFrame, smoothedGrayFrame);

                Mat cannyFrame = new Mat();
                CvInvoke.Canny(smoothedGrayFrame, cannyFrame, 100, 60);

                this.ImgMainZm.Image = cannyFrame;
            }
             else if (this.CurFilterType == FilterType.ftAutumn)
             {
                newFrame = new Mat();
                CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Autumn);
                this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftCool)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Cool);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftBone)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Bone);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftHot)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Hot);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftHsv)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Hsv);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftJet)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Jet);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftOcean)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Ocean);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftPink)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Pink);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftRainbow)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Rainbow);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftSpring)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Spring);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftSummer)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Summer);
                 this.ImgMainZm.Image = newFrame;
             }
             else if (this.CurFilterType == FilterType.ftWinter)
             {
                 newFrame = new Mat();
                 CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Winter);
                 this.ImgMainZm.Image = newFrame;
             }
            else if (this.CurFilterType == FilterType.ftWinter)
            {
                newFrame = new Mat();
                CvInvoke.ApplyColorMap(frame, newFrame, ColorMapType.Winter);
                this.ImgMainZm.Image = newFrame;
            }
            else if (this.CurFilterType == FilterType.ftBitwiseNot)
            {
                newFrame = new Mat();
                CvInvoke.BitwiseNot(frame, newFrame);
                this.ImgMainZm.Image = newFrame;
            }
            else if (this.CurFilterType == FilterType.ftEdgePreservingFilter)
            {
                newFrame = new Mat();
                CvInvoke.EdgePreservingFilter(frame, newFrame);
                this.ImgMainZm.Image = newFrame;
            }
        }

        private void ReleaseData()
        {
            if (_capture != null)
                _capture.Dispose();
        }
    }
}
