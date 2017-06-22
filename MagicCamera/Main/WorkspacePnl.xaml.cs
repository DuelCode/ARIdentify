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
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging.Textures;

namespace MagicCamera.Main
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
        }

        void WorkspacePnl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= WorkspacePnl_Loaded;


            this.CreateCameraPnl();


            this.BdrNoneZm.MouseLeftButtonUp += BdrNoneZm_MouseLeftButtonUp;
            this.BdrInvertZm.MouseLeftButtonUp += BdrInvertZm_MouseLeftButtonUp;
            this.BdrOilPaintingZm.MouseLeftButtonUp += BdrOilPaintingZm_MouseLeftButtonUp;
            this.BdrRotateChannelsZm.MouseLeftButtonUp += BdrRotateChannelsZm_MouseLeftButtonUp;
            this.BdrSaturationCorrectionZm.MouseLeftButtonUp += BdrSaturationCorrectionZm_MouseLeftButtonUp;
            this.BdrSepiaZm.MouseLeftButtonUp += BdrSepiaZm_MouseLeftButtonUp;
            this.BdrSharpenZm.MouseLeftButtonUp += BdrSharpenZm_MouseLeftButtonUp;
            this.BdrTexturerZm.MouseLeftButtonUp += BdrTexturerZm_MouseLeftButtonUp;
            this.BdrYCbCrFilteringZm.MouseLeftButtonUp += BdrYCbCrFilteringZm_MouseLeftButtonUp;
            this.BdrYCbCrLinearZm.MouseLeftButtonUp += BdrYCbCrLinearZm_MouseLeftButtonUp;
            this.BdrBrightnessCorrectionZm.MouseLeftButtonUp += BdrBrightnessCorrectionZm_MouseLeftButtonUp;
            this.BdrLevelsLinearZm.MouseLeftButtonUp += BdrLevelsLinearZm_MouseLeftButtonUp;
            this.BdrJitterZm.MouseLeftButtonUp += BdrJitterZm_MouseLeftButtonUp;
            this.BdrHueModifierZm.MouseLeftButtonUp += BdrHueModifierZm_MouseLeftButtonUp;
            this.BdrHSLFilteringZm.MouseLeftButtonUp += BdrHSLFilteringZm_MouseLeftButtonUp;
            this.BdrGrayscaleRMYZm.MouseLeftButtonUp += BdrGrayscaleRMYZm_MouseLeftButtonUp;
            this.BdrGrayscaleBT709Zm.MouseLeftButtonUp += BdrGrayscaleBT709Zm_MouseLeftButtonUp;
            this.BdrGaussianBlurZm.MouseLeftButtonUp += BdrGaussianBlurZm_MouseLeftButtonUp;
            this.BdrConvolutionZm.MouseLeftButtonUp += BdrConvolutionZm_MouseLeftButtonUp;
            this.BdrContrastCorrectionZm.MouseLeftButtonUp += BdrContrastCorrectionZm_MouseLeftButtonUp;

            this.BdrSobelEdgeDetectorZm.MouseLeftButtonUp += BdrSobelEdgeDetectorZm_MouseLeftButtonUp;
            this.BdrThresholdZm.MouseLeftButtonUp += BdrThresholdZm_MouseLeftButtonUp;
            this.BdrFloydSteinbergDitheringZm.MouseLeftButtonUp += BdrFloydSteinbergDitheringZm_MouseLeftButtonUp;
            this.BdrOrderedDitheringZm.MouseLeftButtonUp += BdrOrderedDitheringZm_MouseLeftButtonUp;
            this.BdrDifferenceEdgeDetectorZm.MouseLeftButtonUp += BdrDifferenceEdgeDetectorZm_MouseLeftButtonUp;
            this.BdrHomogenityEdgeDetectorZm.MouseLeftButtonUp += BdrHomogenityEdgeDetectorZm_MouseLeftButtonUp;
        }

        void BdrHomogenityEdgeDetectorZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurSpecicalFilter = new HomogenityEdgeDetector();
        }

        void BdrDifferenceEdgeDetectorZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurSpecicalFilter = new DifferenceEdgeDetector();
        }

        void BdrOrderedDitheringZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurSpecicalFilter = new OrderedDithering();
        }

        void BdrFloydSteinbergDitheringZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurSpecicalFilter = new FloydSteinbergDithering();
        }

        void BdrThresholdZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurSpecicalFilter = new Threshold(170);
        }

        private void BdrSobelEdgeDetectorZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurSpecicalFilter = new SobelEdgeDetector();
        }

        void BdrContrastCorrectionZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new ContrastCorrection();
        }

        void BdrConvolutionZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new Convolution(new int[,] {
								{ 1, 2, 3, 2, 1 },
								{ 2, 4, 5, 4, 2 },
								{ 3, 5, 6, 5, 3 },
								{ 2, 4, 5, 4, 2 },
								{ 1, 2, 3, 2, 1 } });
        }

        void BdrGaussianBlurZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new GaussianBlur(2.0, 7);
        }

        void BdrGrayscaleBT709Zm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new GrayscaleBT709();
        }

        void BdrGrayscaleRMYZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new GrayscaleRMY();
        }

        void BdrHSLFilteringZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new HSLFiltering(new IntRange(330, 30), new Range(0, 1), new Range(0, 1));
        }

        void BdrHueModifierZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new HueModifier(50);
        }

        void BdrJitterZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new Jitter();
        }

        void BdrLevelsLinearZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new LevelsLinear()
            {
                InRed = new IntRange(30, 230),
                InGreen = new IntRange(50, 240),
                InBlue = new IntRange(10, 210)
            };
        }

        void BdrBrightnessCorrectionZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new BrightnessCorrection();
        }

        void BdrYCbCrLinearZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new YCbCrLinear() { InCb = new Range(-0.3f, 0.3f) };
        }

        void BdrYCbCrFilteringZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new YCbCrFiltering(new Range(0.2f, 0.9f), new Range(-0.3f, 0.3f), new Range(-0.3f, 0.3f));
        }

        void BdrTexturerZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new Texturer(new TextileTexture(), 1.0, 0.8);
        }

        void BdrSharpenZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new Sharpen();
        }

        void BdrSepiaZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new Sepia();
        }

        void BdrSaturationCorrectionZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new SaturationCorrection(0.15f);
        }

        void BdrRotateChannelsZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new RotateChannels();
        }

        void BdrOilPaintingZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new OilPainting();
        }

        //public IFilter CurFilter = new SaturationCorrection(0.15f); 
        //public IFilter CurFilter = new BrightnessCorrection(); 
        //public IFilter CurFilter = new ContrastCorrection(); 
        //public IFilter CurFilter = new HSLFiltering(new IntRange(330, 30), new Range(0, 1), new Range(0, 1)); 
        //public IFilter CurFilter = new YCbCrLinear() { InCb = new Range(-0.3f, 0.3f) }; 
        //public IFilter CurFilter = new YCbCrFiltering(new Range(0.2f, 0.9f), new Range(-0.3f, 0.3f), new Range(-0.3f, 0.3f)); 

        //public IFilter CurFilter = new Convolution(new int[,] {
        //                        { 1, 2, 3, 2, 1 },
        //                        { 2, 4, 5, 4, 2 },
        //                        { 3, 5, 6, 5, 3 },
        //                        { 2, 4, 5, 4, 2 },
        //                        { 1, 2, 3, 2, 1 } });

        //public IFilter CurFilter = new Sharpen(); 
        //public IFilter CurFilter = new LevelsLinear()
        //{
        //    InRed = new IntRange(30, 230),
        //    InGreen = new IntRange(50, 240),
        //    InBlue = new IntRange(10, 210)
        //}; 

        //public IFilter CurFilter = new Jitter();
        //public IFilter CurFilter = new OilPainting();

        //public IFilter CurFilter = new GaussianBlur(2.0, 7); 

        //public IFilter CurFilter = new Texturer(new TextileTexture(), 1.0, 0.8);

        void BdrInvertZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = new Invert();
        }

        void BdrNoneZm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CameraPanel.CurFilter = null;
        }

        private void CreateCameraPnl()
        {
            CameraPanel = new CameraPnl();
            this.GdCameraZm.Children.Add(CameraPanel);
        }
    }
}
