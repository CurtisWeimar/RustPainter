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
            outputText("Processing palatte...");
            selectedImage.Freeze();
            await Task.Run(() => controller.setPalette(selectedImage));
            outputText("Palatte done processing!");
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
                outputText("Processing source image...");
                for (int y = 0; y < (int)finalImage.Height; y++)
                {
                    for(int x = 0; x < (int)finalImage.Width; x++)
                    {
                        Int32Rect rect = new Int32Rect(x, y, 1, 1);
                        byte[] _rgb = { 255, 255, 255, 255 };
                        finalImage.WritePixels(rect, _rgb, finalImage.PixelWidth * 4, 0);
                    }
                }
            } else
            {
                //Janky solution so it is always initialized
                finalImage = new WriteableBitmap(selectedImage);
            }

            selectedImage.Freeze();
            controller.setSourceImage(selectedImage);
            controller.setFinalImage(finalImage);
            outputText("Source image finished processing!");

            outputText("Drawing image...");
            await controller.getFinalImage().Dispatcher.InvokeAsync(() => controller.drawImage());
            imgOutput.Source = controller.getFinalImage();
            outputText("Finished drawing image!");
        }

        private async void RedrawButton_Click(object sender, RoutedEventArgs e)
        {
            await controller.getFinalImage().Dispatcher.InvokeAsync(() => controller.drawImage());
        }

        // TODO: Create basic dithering algorithm
        // TODO: Start with basic mouse drawing operations
        // BUG: If the user exits the image selection it continues trying to draw anyways

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
