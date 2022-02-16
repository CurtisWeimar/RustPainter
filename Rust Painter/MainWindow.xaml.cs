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
        List<byte[]> Colors = new List<byte[]>();

        Uri paletteURI;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void generateColors()
        {
            // Clear out any previous colors and load new one into a Bitmap
            Colors.Clear();
            BitmapImage palette = new BitmapImage(paletteURI);

            List<byte[]> colors = grabColorData(palette);

            // For every color returned add it to the list of colors
            foreach(byte[] color in colors)
            {
                if(!Colors.Contains(color))
                {
                    Colors.Add(color);
                }
            }
        }

        // TODO: Save URI for which pallete is being used
        // CRITICAL: Find out why this method is hanging
        private void paletteClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                paletteURI = new Uri(openFileDialog.FileName);
            }

            generateColors();
        }

        // TODO: Do checks to make ensure an image is selected
        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage selectedImage = new BitmapImage();

            // Standard windows file selection dialog box
            OpenFileDialog openFileDialog =  new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName == null) return; // Band-aid fix
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
        // BUG: If the user exits the image selection it continues trying to draw anyways
            // -- This has been band-aid fixed
        private void DrawImage(BitmapImage source)
        {
            Console.WriteLine("Getting pixel info...");
            

            // Temporary WriteabelBitmap
            WriteableBitmap outputMap = new WriteableBitmap(source);

            // Loop throught pixel data
            Console.WriteLine("Drawing new image...");

            List<byte[]> colors = grabColorData(source);

            int pixelCount = 0;
            for(int y = 0; y < (int)source.Height; y++)
            {
                for(int x = 0; x < (int)source.Width; x++)
                {
                    if(colors.Count >= pixelCount)
                    {
                        Console.WriteLine("Length: " + colors.Count);
                        Console.WriteLine("Pixel Count: " + pixelCount);
                    }
                    byte[] rgb = colors[pixelCount];
                    Int32Rect rect = new Int32Rect(x, y, 1, 1); // Rect that acts as pixel - cringe
                    outputMap.WritePixels(rect, rgb, 4, 0);
                    pixelCount++;
                }
            }

            //Int32Rect rect = new Int32Rect(x, y, 1, 1); // Rect that acts as pixel - cringe

            //outputMap.WritePixels(rect, ColorData, 4, 0);

            Console.WriteLine("Finished drawing image...");
            imgOutput.Source = outputMap;
        }

        // TODO: Write this method so that it has two different modes
            // -- One for returning every pixel
            // -- One for returning every unique pixel
        private List<byte[]> grabColorData(BitmapImage source)
        {
            List<byte[]> colors = new List<byte[]>();

            // Get pixel info
            // Create pixel array
            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];

            // Copy pixel data
            Console.WriteLine("Pixels Array Length: " + pixels.Length);
            Console.WriteLine("Copying pixel data...");
            source.CopyPixels(pixels, stride, 0); // Actual line copying pixels
            Console.WriteLine("Pixel data copied!");
            textOutput.Text = "Pixels Array Length: " + pixels.Length / 4;

            int pixelCount = 0;
            // Go down the current column
            for (int y = 0; y < (int)source.Height; y++)
            {
                // Go across the current row
                for (int x = 0; x < (int)source.Width; x++)
                {
                    // TODO: Understand how this works
                    int index = y * stride + 4 * x;

                    //Console.WriteLine("Index: " + index);
                    //Console.WriteLine("Index / 4: " + index / 4);
                    int expected_pixel = (y * (int)source.Width) + x;
                    //Console.WriteLine("Expected pixel: " + expected_pixel + "\n");

                    // Pixel color/alpha data
                    byte red = pixels[index];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    // TODO: Write pixel data to WriteableBitmap directly
                    byte[] ColorData = { red, green, blue, alpha }; // R G B A

                    colors.Add(ColorData);

                    pixelCount++;
                }
            }

            return colors;
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
