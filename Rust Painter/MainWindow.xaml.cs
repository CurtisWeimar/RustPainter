using Microsoft.Win32;
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
using System.Drawing;

namespace Rust_Painter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage selectedImage = new BitmapImage();

            OpenFileDialog openFileDialog =  new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                Uri uri = new Uri(openFileDialog.FileName);
                selectedImage = new BitmapImage(uri);
                imgPreview.Source = selectedImage;
            }

            DrawImage(selectedImage);
        }

        private void DrawImage(BitmapImage source)
        {
            // TODO: Rewrite this so that we put all pixel info into arrays instead
            // Go to next row down
            // Get pixel info
            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];
            source.CopyPixels(pixels, stride, 0);
            textOutput.Text += "Pixels Array Length: " + pixels.Length;
            int pixelCount = 0; 
            foreach (byte pixel in pixels)
            {
                textOutput.Text += "Pixel " + pixelCount + ": " + pixel.ToString() + "\n";
                pixelCount++;
            }

            //for (int y = 0; y < source.Width; y++)
            //{
            //    // Go across the current row
            //    for(int x = 0; x < source.Height; x++)
            //    {
                    
            //        // Output it to new window
            //    }
            //}
        }
    }
}
