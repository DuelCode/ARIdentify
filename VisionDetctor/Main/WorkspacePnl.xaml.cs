using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using VisionDetctor.Contorls;
using Rectangle = System.Drawing.Rectangle;

namespace VisionDetctor.Main
{
    /// <summary>
    /// WorkspacePnl.xaml 的交互逻辑
    /// </summary>
    public partial class WorkspacePnl : UserControl
    {
        private CameraPnl CameraPanel;
        
        ColorFiltering colorFilter = new ColorFiltering();
        GrayscaleBT709 grayFilter = new GrayscaleBT709();  // 灰度化
        BlobCounter blobCounter1 = new BlobCounter();

        public WorkspacePnl()
        {
            InitializeComponent();

            this.Loaded += WorkspacePnl_Loaded;
            this.Unloaded += WorkspacePnl_Unloaded;
        }

        private Thread trackingThread = null;

        private void WorkspacePnl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= WorkspacePnl_Loaded;

            this.CameraPanel = new CameraPnl();
            this.CameraPanel.NewFrame += CameraPanel_NewFrame;
            this.CameraPanel.CameraZm.MouseUp += CameraZm_MouseUp;
            this.GdCameraZm.Children.Add(this.CameraPanel);

            colorFilter.Red = new IntRange(0, 255);
            colorFilter.Green = new IntRange(0, 255);
            colorFilter.Blue = new IntRange(0, 255);

            blobCounter1.MinWidth = 10;
            blobCounter1.MinHeight = 10;
            blobCounter1.FilterBlobs = true;
            blobCounter1.ObjectsOrder = ObjectsOrder.Size;
        }

        private void CameraZm_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            colorFilter.Red = new IntRange(0, 70);
            colorFilter.Blue = new IntRange(0, 41);
            colorFilter.Green = new IntRange(70, 255);
        }

        private void CameraPanel_NewFrame(System.Drawing.Bitmap oBitmap)
        {
            #region 去除绿色以外其他色、绿色区域绘制边框
            Bitmap objectImage = colorFilter.Apply(oBitmap);
            BitmapData objectData = objectImage.LockBits(new Rectangle(0, 0, oBitmap.Width, oBitmap.Height),
                ImageLockMode.ReadOnly, oBitmap.PixelFormat);
            UnmanagedImage grayImage = grayFilter.Apply(new UnmanagedImage(objectData));
            objectImage.UnlockBits(objectData);

            blobCounter1.ProcessImage(grayImage);
            Rectangle[] rects = blobCounter1.GetObjectsRectangles();

            if (rects.Length > 0)
            {
                Rectangle objectRect = rects[0];
                Graphics g = Graphics.FromImage(oBitmap);
                using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(160, 255, 160), 3))
                {
                    g.DrawRectangle(pen, objectRect);
                }
                g.Dispose();
            }

            UpdateObjectPicture(objectImage);
            #endregion


            //#region 去除绿色以外其他色
            //Bitmap objectImage = colorFilter.Apply(oBitmap);
            //UpdateObjectPicture(objectImage);
            //#endregion

            //Bitmap objectImage = SobelEdgeFilter.Apply(oBitmap);
            //UpdateObjectPicture(objectImage);
        }

        private Invert SobelEdgeFilter = new Invert();

        private void UpdateObjectPicture(Bitmap picture)
        {
            try
            {
                System.Drawing.Image oldPicture = null;
                oldPicture = this.ImgMainZm.Image;
                ImgMainZm.Image = picture;
                if (oldPicture != null)
                    oldPicture.Dispose();
            }
            catch {}
        }




        #region

        private Bitmap CloneImage(BitmapData sourceData)
        {
            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            // create new image
            Bitmap destination = new Bitmap(width, height, sourceData.PixelFormat);

            // lock destination bitmap data
            BitmapData destinationData = destination.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, destination.PixelFormat);

            AForge.SystemTools.CopyUnmanagedMemory(destinationData.Scan0, sourceData.Scan0, height * sourceData.Stride);

            // unlock destination image
            destination.UnlockBits(destinationData);

            return destination;
        }

        private byte[] BitmapSourceToArray(BitmapSource bitmapSource)
        {
            int height = bitmapSource.PixelHeight;
            int width = bitmapSource.PixelWidth;
            int stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            byte[] bits = new byte[height * stride + 12];
            bitmapSource.CopyPixels(bits, stride, 0);
            bits[bits.Length - 12] = (byte)(width >> 24);
            bits[bits.Length - 11] = (byte)((width << 8) >> 24);
            bits[bits.Length - 10] = (byte)((width << 16) >> 24);
            bits[bits.Length - 9] = (byte)(width & 255);

            bits[bits.Length - 8] = (byte)(height >> 24);
            bits[bits.Length - 7] = (byte)((height << 8) >> 24);
            bits[bits.Length - 6] = (byte)((height << 16) >> 24);
            bits[bits.Length - 5] = (byte)(height & 255);

            bits[bits.Length - 4] = (byte)(stride >> 24);
            bits[bits.Length - 3] = (byte)((stride << 8) >> 24);
            bits[bits.Length - 2] = (byte)((stride << 16) >> 24);
            bits[bits.Length - 1] = (byte)(stride & 255);
            return bits;
        }

        private BitmapSource ChangeBitmapToBitmapSource(Bitmap bmp)
        {
            BitmapSource returnSource;
            try
            {
                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;
        }

        private void WorkspacePnl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= WorkspacePnl_Unloaded;
        }

        #endregion
    }
}
