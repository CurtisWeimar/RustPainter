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
using System.ComponentModel;
using System.IO;

namespace Rust_Painter
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Property for the textBlock
        private string _outPut;

        private List<string> outPutList = new List<string>();

        public string outPut
        {
            get { return _outPut; }
            set
            {
                _outPut = value;
                OnPropertyChanged("outPut");
            }
        }

        //Variables
        private List<byte[]> Colors = new List<byte[]>();

        private Uri paletteURI;

        public MainWindow()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
        }

        //private void interpolateColor(List<byte[]> palette, List<byte[]> pixels)
        //{
        //    Console.WriteLine("Interpolating...");

        //    // Loop through every source pixel
        //    int length = pixels.Count;
        //    for (int x = 0; x < pixels.Count; x++)
        //    {
        //        //Get pixel RGB value
        //        byte[] pixel = pixels[x]; // R G B A
        //        //byte[] pixel = ToXYZ(_pixel);
        //        byte red = pixel[0];
        //        byte green = pixel[1];
        //        byte blue = pixel[2];
        //        //byte alpha = pixel[3];  Alpha data not really necessary

        //        int low_diff = 1000; // Lowest integer difference between colors
        //        byte[] newColor = { 0, 0, 0, 255};

        //        // Loop over every palette color and find the closest
        //        for (int y = 0; y < palette.Count; y++)
        //        {
        //            byte[] paletteColor = palette[y];
        //            byte _red = paletteColor[0];
        //            byte _green = paletteColor[1];
        //            byte _blue = paletteColor[2];

        //            // Calculate distance in 3D RGB color space between the two colors
        //            int diff = Math.Abs(((_red - red) ^ 2 + (_green - green) ^ 2 + (_blue - blue) ^ 2) / 2);

        //            //Console.WriteLine("Color Difference: " + diff);
        //            if (y == 0) { low_diff = diff; }
        //            if (diff < low_diff)
        //            {
        //                low_diff = diff;
        //                newColor = paletteColor;
        //                //Console.WriteLine($"New Color: {newColor[0]}, {newColor[1]}, {newColor[2]}");
        //                newColor[3] = 255;
        //            }
        //        }

        //        // Change pixel color data to the palette
        //        Console.WriteLine("Done " + ((double)x / (double)length) * 100 + "%");
        //        pixels[x] = newColor;
        //    }
        //}

        private void generateColors()
        {
            // Clear out any previous colors and load new one into a Bitmap
            Colors.Clear();
            BitmapImage palette = new BitmapImage(paletteURI);

            List<byte[]> colors = grabColorData(palette);

            // For every color returned add it to the list of colors
            foreach(byte[] color in colors)
            {
                if(!checkExists(color))
                {
                    Colors.Add(color);

                    //Temp debug thing
                    Console.WriteLine(String.Format("R: {0} G:{1} B:{2}", color[0], color[1], color[2]));
                }
            }
        }

        public bool checkExists(byte[] color)
        {
            bool exists = false;

            foreach(byte[] colorData in Colors)
            {
                if(colorData[0] == color[0] && colorData[1] == color[1] && colorData[2] == color[2])
                {
                    exists = true;
                }
            }

            return exists;
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
                Uri uri = new Uri(openFileDialog.FileName);
                
                selectedImage = new BitmapImage(uri);
                imgPreview.Source = selectedImage;
            }

            Console.WriteLine("Image found successfully!");
            this.Dispatcher.Invoke(() => DrawImage(selectedImage));
        }

        // TODO: Create basic dithering algorithm
        // TODO: Start with basic mouse drawing operations
        // BUG: If the user exits the image selection it continues trying to draw anyways
            // -- This has been band-aid fixed
            // -- Nvm

        private async void DrawImage(BitmapImage source)
        {
            Console.WriteLine("Getting pixel info...");

            // Temporary WriteabelBitmap
            WriteableBitmap outputMap = new WriteableBitmap(source);

            // Loop throught pixel data
            Console.WriteLine("Checking image data...");

            List<byte[]> colors = grabColorData(source);

            //await interpolateColor(Colors, colors);

            int pixelCount = 0;
            for (int y = 0; y < (int)source.Height; y++)
            {
                for (int x = 0; x < (int)source.Width; x++)
                {
                    byte[] rgb = colors[pixelCount];
                    //Console.WriteLine($"RGB: {rgb[0]}, {rgb[1]}, {rgb[2]}");
                    Int32Rect rect = new Int32Rect(x, y, 1, 1); // Rect that acts as pixel
                    outputMap.WritePixels(rect, rgb, source.PixelWidth * 4, 0);
                    pixelCount++;
                    //Console.WriteLine($"Pixel Count: {pixelCount}");
                }
                imgOutput.Source = outputMap;
                await Task.Delay(500);
            }

            //Int32Rect rect = new Int32Rect(x, y, 1, 1); // Rect that acts as pixel - cringe

            //outputMap.WritePixels(rect, ColorData, 4, 0);

            Console.WriteLine("Finished drawing image...");
            
        }

        // TODO: Cull alpha data - don't think it is necessary

        private List<double[]> grabColorDataDouble(BitmapImage source)
        {
            List<double[]> colors = new List<double[]>();

            // Get pixel info
            // Create pixel array
            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];

            // Copy pixel data
            source.CopyPixels(pixels, stride, 0); // Actual line copying pixels

            int pixelCount = 0;
            // Go down the current column
            for (int y = 0; y < (int)source.Height; y++)
            {
                // Go across the current row
                for (int x = 0; x < (int)source.Width; x++)
                {
                    int index = y * stride + 4 * x;

                    int expected_pixel = (y * (int)source.Width) + x;
                    //Console.WriteLine("Expected pixel: " + expected_pixel + "\n");

                    // Pixel color/alpha data
                    byte red = pixels[index];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    byte[] RawColorData = { red, green, blue, alpha }; // R G B A
                    //double[] ColorData = ToXYZ(RawColorData);

                    //colors.Add(ColorData);

                    pixelCount++;
                }
            }

            return colors;
        }

        private List<byte[]> grabColorData(BitmapImage source)
        {
            List<byte[]> colors = new List<byte[]>();

            // Get pixel info
            // Create pixel array
            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];

            // Copy pixel data
            source.CopyPixels(pixels, stride, 0); // Actual line copying pixels

            int pixelCount = 0;
            // Go down the current column
            Console.WriteLine($"Image width: {source.Width}");
            Console.WriteLine($"Image height: {source.Height}");
            for (int y = 0; y < (int)source.Height; y++)
            {
                // Go across the current row
                for (int x = 0; x < (int)source.Width; x++)
                {
                    int index = y * stride + 4 * x;

                    int expected_pixel = (y * (int)source.Width) + x;
                    //Console.WriteLine("Expected pixel: " + expected_pixel + "\n");

                    // Pixel color/alpha data
                    byte red = pixels[index];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index + 2];
                    byte alpha = pixels[index + 3];
                    
                    byte[] ColorData = { red, green, blue, alpha }; // R G B A

                    colors.Add(ColorData);

                    pixelCount++;
                }
            }

            return colors;
        }

        private byte[] XYZtoRGB(double[] xyz)
        {
            byte[] conv = new byte[4];

            double x = xyz[0];
            double y = xyz[1];
            double z = xyz[2];

            double[] Clinear = new double[3];
            Clinear[0] = x * 3.2410 - y * 1.5374 - z * 0.4986; // red
            Clinear[1] = -x * 0.9692 + y * 1.8760 - z * 0.0416; // green
            Clinear[2] = x * 0.0556 - y * 0.2040 + z * 1.0570; // blue

            for (int i = 0; i < 3; i++)
            {
                Clinear[i] = (Clinear[i] <= 0.0031308) ? 12.92 * Clinear[i] : (1 + 0.055) * Math.Pow(Clinear[i], (1.0 / 2.4)) - 0.055;
            }

            conv[0] = (byte)(x * 255);
            conv[1] = (byte)(y * 255);
            conv[2] = (byte)(z * 255);

            return conv;
        }

        //Helper functions for outputting text to textblock while maintaining previous outputs
        private void outputTextLine(string text)
        {
            outPutList.Add(text + "\n");
            updateOutPutString();
        }

        private void outputText(string text)
        {
            if(outPutList.Count > 0) outPutList[outPutList.Count - 1] = text + "\n";
            else outPutList.Add(text + "\n");
            updateOutPutString();
        }

        private void updateOutPutString()
        {
            outPut = "";
            for(int x = 0; x < outPutList.Count; x++)
            {
                outPut += outPutList[x];
            }
        }

    }
}
