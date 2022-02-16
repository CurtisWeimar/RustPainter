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

        // TODO: Do checks to make ensure an image is selected
        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage selectedImage = new BitmapImage();

            // Standard windows file selection dialog box
            OpenFileDialog openFileDialog =  new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                Uri uri = new Uri(openFileDialog.FileName);
                selectedImage = new BitmapImage(uri);
                imgPreview.Source = selectedImage;
            }

            Console.WriteLine("Image found successfully!");
            DrawImage(selectedImage);
        }

        // TODO: Implement Rust color palette for output Bitmap
        // TODO: Create basic dithering algorithm
        // TODO: Start with basic mouse drawing operations
        private void DrawImage(BitmapImage source)
        {
            Console.WriteLine("Getting pixel info...");
            // Get pixel info
            // Create pixel array
            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];

            // Copy pixel data
            Console.WriteLine("Pixels Array Length: " + pixels.Length);
            Console.WriteLine("Copying pixel data...");
            source.CopyPixels(pixels, stride, 0);
            Console.WriteLine("Pixel data copied!");
            textOutput.Text += "Pixels Array Length: " + pixels.Length;

            // Temporary WriteabelBitmap
            WriteableBitmap outputMap = new WriteableBitmap(source);

            // Loop throught pixel data
            Console.WriteLine("Drawing new image...");
            int pixelCount = 0;
            // Go down the current column
            for(int y = 0; y < source.Height; y++)
            {
                // Go across the current row
                for(int x = 0; x < source.Width; x++)
                {
                    // TODO: Understand how this works
                    int index = y * stride + 4 * x;

                    // Pixel color/alpha data
                    byte red = pixels[index];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    // TODO: Write pixel data to WriteableBitmap directly
                    byte[] ColorData = { blue, green, red, alpha }; // B G R A ?

                    Int32Rect rect = new Int32Rect(x, y, 1, 1); // Rect that acts as pixel - cringe

                    outputMap.WritePixels(rect, ColorData, 4, 0);

                    // Debug tools
                    Console.WriteLine("Pixel " + pixelCount + ": ");
                    Console.WriteLine("Red: " + red.ToString());
                    Console.WriteLine("Green: " + green.ToString());
                    Console.WriteLine("Blue: " + blue.ToString());
                    Console.WriteLine("Alpha: " + alpha.ToString());

                    pixelCount++;
                }
            }

            Console.WriteLine("Finished drawing image...");
            imgOutput.Source = outputMap;
        }

        private void outputTextLine(String text)
        {
            textOutput.Text += text;
        }

        private void outputText(String text)
        {
            textOutput.Text = text;
        }
    }
}
