using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.VFW;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;

namespace MotionDetector
{
    public partial class Form1 : Form
    {
        private IVideoSource videoSource = null;
        private List<float> motionHistory = new List<float>();
        private int detectedObjectsCount = -1;
        // statistics length
        private const int statLength = 15;
        // current statistics index
        private int statIndex = 0;
        // ready statistics values
        private int statReady = 0;
        private int motionDetectionType = 1;
        private int motionProcessingType = 1;
        AForge.Vision.Motion.MotionDetector detector = new AForge.Vision.Motion.MotionDetector(
          new TwoFramesDifferenceDetector(),
          new MotionAreaHighlighting());

        private int[] statCount = new int[statLength];

        public Form1()
        {
            InitializeComponent();

            Application.Idle += new EventHandler(Application_Idle);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            objectsCountLabel.Text = (detectedObjectsCount < 0) ? string.Empty : "Objects: " + detectedObjectsCount;
        }
     
        private void CloseVideoSource()
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            videoSourcePlayer1.SignalToStop();

            // wait 2 seconds until camera stops
            for (int i = 0; (i < 50) && (videoSourcePlayer1.IsRunning); i++)
            {
                Thread.Sleep(100);
            }
            if (videoSourcePlayer1.IsRunning)
                videoSourcePlayer1.Stop();

            // stop timers
            timer.Stop();
            alarmTimer.Stop();

            motionHistory.Clear();

            // reset motion detector
            if (detector != null)
                detector.Reset();

            videoSourcePlayer1.BorderColor = Color.Black;
            this.Cursor = Cursors.Default;
        }

        private void localVideoCaptureDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterInfoCollection arrDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            string oDeviceName = arrDevices[1].MonikerString;
            VideoCaptureDevice oCaptureDevice = new VideoCaptureDevice(oDeviceName);

            OpenVideoSource(oCaptureDevice);
        }

        private void OpenVideoSource(IVideoSource source)
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // close previous video source
            CloseVideoSource();

            // start new video source
            videoSourcePlayer1.VideoSource = new AsyncVideoSource(source);
            videoSourcePlayer1.Start();

            // reset statistics
            statIndex = statReady = 0;

            // start timers
            timer.Start();
            alarmTimer.Start();

            videoSource = source;

            this.Cursor = Cursors.Default;
        }

        private void twoFramesDifferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            motionDetectionType = 1;
            SetMotionDetectionAlgorithm(new TwoFramesDifferenceDetector());
        }

        private void SetMotionDetectionAlgorithm(IMotionDetector detectionAlgorithm)
        {
            lock (this)
            {
                detector.MotionDetectionAlgorithm = detectionAlgorithm;
                motionHistory.Clear();

                if (detectionAlgorithm is TwoFramesDifferenceDetector)
                {
                    if (
                        (detector.MotionProcessingAlgorithm is MotionBorderHighlighting) ||
                        (detector.MotionProcessingAlgorithm is BlobCountingObjectsProcessing))
                    {
                        motionProcessingType = 1;
                        SetMotionProcessingAlgorithm(new MotionAreaHighlighting());
                    }
                }
            }
        }

        private void SetMotionProcessingAlgorithm(IMotionProcessing processingAlgorithm)
        {
            lock (this)
            {
                detector.MotionProcessingAlgorithm = processingAlgorithm;
            }
        }

        private void motionAreaHighlightingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            motionProcessingType = 1;
            SetMotionProcessingAlgorithm(new MotionAreaHighlighting());
        }

        private void noneToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            motionDetectionType = 0;
            SetMotionDetectionAlgorithm(null);
        }

        private void simpleBackgroundModelingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            motionDetectionType = 2;
            SetMotionDetectionAlgorithm(new SimpleBackgroundModelingDetector(true, true));
        }

        private void noneToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            motionProcessingType = 0;
            SetMotionProcessingAlgorithm(null);
        }

        private void motionBorderHighlightingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            motionProcessingType = 2;
            SetMotionProcessingAlgorithm(new MotionBorderHighlighting());
        }

        private void blobCountingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            motionProcessingType = 3;
            SetMotionProcessingAlgorithm(new BlobCountingObjectsProcessing());
        }

        private void gridMotionAreaProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            motionProcessingType = 4;
            SetMotionProcessingAlgorithm(new GridMotionAreaProcessing(32, 32));
        }

        private void defineMotionregionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (videoSourcePlayer1.VideoSource != null)
            {
                //Bitmap currentVideoFrame = videoSourcePlayer1.GetCurrentVideoFrame();

                //if (currentVideoFrame != null)
                //{
                //    MotionRegionsForm form = new MotionRegionsForm();
                //    form.VideoFrame = currentVideoFrame;
                //    form.MotionRectangles = detector.MotionZones;

                //    // show the dialog
                //    if (form.ShowDialog(this) == DialogResult.OK)
                //    {
                //        Rectangle[] rects = form.MotionRectangles;

                //        if (rects.Length == 0)
                //            rects = null;

                //        detector.MotionZones = rects;
                //    }

                //    return;
                //}
            }

            //MessageBox.Show("It is required to start video source and receive at least first video frame before setting motion zones.",
            //    "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void showMotionHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showMotionHistoryToolStripMenuItem.Checked = !showMotionHistoryToolStripMenuItem.Checked;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            IVideoSource videoSource = videoSourcePlayer1.VideoSource;

            if (videoSource != null)
            {
                // get number of frames for the last second
                statCount[statIndex] = videoSource.FramesReceived;

                // increment indexes
                if (++statIndex >= statLength)
                    statIndex = 0;
                if (statReady < statLength)
                    statReady++;

                float fps = 0;

                // calculate average value
                for (int i = 0; i < statReady; i++)
                {
                    fps += statCount[i];
                }
                fps /= statReady;

                statCount[statIndex] = 0;

                fpsLabel.Text = fps.ToString("F2") + " fps";
            }
        }


        private int flash = 0;
        private void alarmTimer_Tick(object sender, EventArgs e)
        {
            if (flash != 0)
            {
                videoSourcePlayer1.BorderColor = (flash % 2 == 1) ? Color.Black : Color.Red;
                flash--;
            }
        }

        private float motionAlarmLevel = 0.015f;

        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            lock (this)
            {
                if (detector != null)
                {
                    float motionLevel = detector.ProcessFrame(image);

                    if (motionLevel > motionAlarmLevel)
                    {
                        // flash for 2 seconds
                        flash = (int)(2 * (1000 / alarmTimer.Interval));
                    }

                    // check objects' count
                    if (detector.MotionProcessingAlgorithm is BlobCountingObjectsProcessing)
                    {
                        BlobCountingObjectsProcessing countingDetector = (BlobCountingObjectsProcessing)detector.MotionProcessingAlgorithm;
                        detectedObjectsCount = countingDetector.ObjectsCount;
                    }
                    else
                    {
                        detectedObjectsCount = -1;
                    }

                    // accumulate history
                    motionHistory.Add(motionLevel);
                    if (motionHistory.Count > 300)
                    {
                        motionHistory.RemoveAt(0);
                    }

                    if (showMotionHistoryToolStripMenuItem.Checked)
                        DrawMotionHistory(image);
                }
            }
        }

        private void DrawMotionHistory(Bitmap image)
        {
            Color greenColor = Color.FromArgb(128, 0, 255, 0);
            Color yellowColor = Color.FromArgb(128, 255, 255, 0);
            Color redColor = Color.FromArgb(128, 255, 0, 0);

            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadWrite, image.PixelFormat);

            int t1 = (int)(motionAlarmLevel * 500);
            int t2 = (int)(0.075 * 500);

            for (int i = 1, n = motionHistory.Count; i <= n; i++)
            {
                int motionBarLength = (int)(motionHistory[n - i] * 500);

                if (motionBarLength == 0)
                    continue;

                if (motionBarLength > 50)
                    motionBarLength = 50;

                Drawing.Line(bitmapData,
                    new IntPoint(image.Width - i, image.Height - 1),
                    new IntPoint(image.Width - i, image.Height - 1 - motionBarLength),
                    greenColor);

                if (motionBarLength > t1)
                {
                    Drawing.Line(bitmapData,
                        new IntPoint(image.Width - i, image.Height - 1 - t1),
                        new IntPoint(image.Width - i, image.Height - 1 - motionBarLength),
                        yellowColor);
                }

                if (motionBarLength > t2)
                {
                    Drawing.Line(bitmapData,
                        new IntPoint(image.Width - i, image.Height - 1 - t2),
                        new IntPoint(image.Width - i, image.Height - 1 - motionBarLength),
                        redColor);
                }
            }

            image.UnlockBits(bitmapData);
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
