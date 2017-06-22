using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using PublicLibrary;
using ZXing;

using BarcodeReader = ZXing.Presentation.BarcodeReader;
using BarcodeWriter = ZXing.Presentation.BarcodeWriter;
using BarcodeWriterGeometry = ZXing.Presentation.BarcodeWriterGeometry;

namespace ZXingCode
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseWnd
    {
        private readonly BarcodeReader reader = new BarcodeReader();

        public MainWindow()
        {
            InitializeComponent();

            foreach (var format in MultiFormatWriter.SupportedWriters)
                cmbEncoderType.Items.Add(format);
            cmbEncoderType.SelectedItem = BarcodeFormat.QR_CODE;

            cmbRendererType.Items.Add("WriteableBitmap");
            cmbRendererType.Items.Add("XAML Geometry");
            cmbRendererType.SelectedItem = "WriteableBitmap";
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
            var start = DateTime.Now;
            var result = reader.Decode((BitmapSource)imageBarcode.Source);
            labDuration.Content = (DateTime.Now - start).Milliseconds + " ms";
            if (result != null)
            {
                txtBarcodeType.Text = result.BarcodeFormat.ToString();
                txtBarcodeContent.Text = result.Text;
            }
            else
            {
                txtBarcodeType.Text = "";
                txtBarcodeContent.Text = "No barcode found.";
            }
        }

        private void txtBarcodeImageFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(txtBarcodeImageFile.Text))
            {
                imageBarcode.Source = new BitmapImage(new Uri(txtBarcodeImageFile.Text));
            }
        }

        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imageBarcodeEncoder.Visibility = Visibility.Hidden;
                imageBarcodeEncoderGeometry.Visibility = Visibility.Hidden;

                switch (cmbRendererType.SelectedItem.ToString())
                {
                    case "WriteableBitmap":
                        {
                            var writer = new BarcodeWriter
                            {
                                Format = (BarcodeFormat)cmbEncoderType.SelectedItem,
                                Options = new ZXing.Common.EncodingOptions
                                {
                                    Height = (int)imageBarcodeEncoder.Height,
                                    Width = (int)imageBarcodeEncoder.Width,
                                    Margin = 0
                                }
                            };
                            var image = writer.Write(txtBarcodeContentEncode.Text);
                            imageBarcodeEncoder.Source = image;
                            imageBarcodeEncoder.Visibility = Visibility.Visible;
                        }
                        break;
                    case "XAML Geometry":
                        {
                            var writer = new BarcodeWriterGeometry
                            {
                                Format = (BarcodeFormat)cmbEncoderType.SelectedItem,
                                Options = new ZXing.Common.EncodingOptions
                                {
                                    Height = (int)imageBarcodeEncoder.Height,
                                    Width = (int)imageBarcodeEncoder.Width,
                                    Margin = 0
                                }
                            };
                            var image = writer.Write(txtBarcodeContentEncode.Text);
                            imageBarcodeEncoderGeometry.Data = image;
                            imageBarcodeEncoderGeometry.Visibility = Visibility.Visible;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
