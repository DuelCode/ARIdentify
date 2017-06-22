using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
using Microsoft.Win32;
using PublicLibrary;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace QrCode
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseWnd
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "All documents (*.*)|*.*",
                FileName = txtBarcodeImageFile.Text
            };
            if (dlg.ShowDialog(this).GetValueOrDefault(false))
            {
                txtBarcodeImageFile.Text = dlg.FileName;
            }
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {
            //var start = DateTime.Now;
            //var result = reader.Decode((BitmapSource)imageBarcode.Source);
            //labDuration.Content = (DateTime.Now - start).Milliseconds + " ms";
            //if (result != null)
            //{
            //    txtBarcodeType.Text = result.BarcodeFormat.ToString();
            //    txtBarcodeContent.Text = result.Text;
            //}
            //else
            //{
            //    txtBarcodeType.Text = "";
            //    txtBarcodeContent.Text = "No barcode found.";
            //}

            QRCodeDecoder oQrCodeDecoder = new QRCodeDecoder();
            Bitmap oBitmap = new Bitmap(txtBarcodeImageFile.Text);
            QRCodeImage oCodeImage = new QRCodeBitmapImage(oBitmap);
            string sResult = oQrCodeDecoder.decode(oCodeImage);

            if (!String.IsNullOrEmpty(sResult))
                txtBarcodeContent.Text = sResult;
            else
                txtBarcodeContent.Text = "No found";
        }

        private void txtBarcodeImageFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(txtBarcodeImageFile.Text))
                imageBarcode.Source = new BitmapImage(new Uri(txtBarcodeImageFile.Text));
        }

        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource oBitmapImage = null;
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            Bitmap oCodeBitmap = qrCodeEncoder.Encode(txtBarcodeContentEncode.Text, Encoding.UTF8);
            oBitmapImage = ChangeBitmapToBitmapSource(oCodeBitmap);
            if (oBitmapImage != null)
                imageBarcodeEncoder.Source = oBitmapImage;
            else
                imageBarcodeEncoder.Source = null;
        }

        public BitmapSource ChangeBitmapToBitmapSource(Bitmap bmp)
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
    }
}
