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
using System.Threading;
using System.Collections.Concurrent;

namespace Rust_Painter
{
    public partial class MainWindow : Window
    {
        ConcurrentQueue<WriteableBitmap> maps = new ConcurrentQueue<WriteableBitmap>();
        ImageController controller = new ImageController();

        private List<string> outPutList = new List<string>();

        //Variables
        private List<byte[]> Colors = new List<byte[]>();

        private Uri paletteURI;

        public MainWindow()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            RenderOptions.SetEdgeMode(mainGrid, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(mainGrid, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(imgOutput, BitmapScalingMode.NearestNeighbor);
        }

        // TODO: Save URI for which pallete is being used
        private async void paletteClick(object sender, RoutedEventArgs e)
        {
            BitmapImage selectedImage = new BitmapImage();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                paletteURI = new Uri(openFileDialog.FileName);
                selectedImage = new BitmapImage(paletteURI);
                palatteImg.Source = selectedImage;
            }

            //Set image to image controller
            //System.Windows.Threading.Dispatcher.Invoke(() => controller.setPalette(selectedImage));
            outputTextLine("Processing palatte...");
            selectedImage.Freeze();
            await Task.Run(() => controller.setPalette(selectedImage));
            outputTextLine("Palatte done processing!");
        }

        // TODO: Do checks to make ensure an image is selected
        private async void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage selectedImage = new BitmapImage();
            WriteableBitmap finalImage;

            // Standard windows file selection dialog box
            OpenFileDialog openFileDialog =  new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                Uri uri = new Uri(openFileDialog.FileName);
                
                selectedImage = new BitmapImage(uri);
                imgPreview.Source = selectedImage;
                finalImage = new WriteableBitmap(selectedImage);
            } else
            {
                //Janky solution so it is always initialized
                finalImage = new WriteableBitmap(selectedImage);
            }
            outputTextLine("Processing source image...");
            selectedImage.Freeze();
            controller.setSourceImage(selectedImage);
            controller.setFinalImage(finalImage);
            outputTextLine("Source image finished processing!");

            outputTextLine("Drawing image...");
            await controller.getFinalImage().Dispatcher.InvokeAsync(() => controller.drawImage());
            imgOutput.Source = controller.getFinalImage();
            outputTextLine("Finished drawing image!");
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
            string outPut = "";
            for(int x = 0; x < outPutList.Count; x++)
            {
                outPut += outPutList[x];
            }
            textOutput.Text = outPut;
        }

    }
}
