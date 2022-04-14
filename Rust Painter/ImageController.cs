using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Rust_Painter
{
    internal class ImageController
    {
        private BitmapImage paletteImage;
        private BitmapImage sourceImage;
        private WriteableBitmap finalImage;

        int sourceHeight;
        int sourceWidth;

        private List<Color> palette;
        private List<Color> pixels;
        private List<Color> uniquePixels;

        public ImageController()
        {

        }

        public ImageController(List<Color> palatte, List<Color> pixels)
        {
            this.palette = palatte;
            this.pixels = pixels;
        }

        // Getters and setters
        public BitmapImage getPalette() { return paletteImage; }
        public List<Color> getPaletteList() { return palette; }
        public void setPalette(BitmapImage palette) //Made async to try to keep UI from hanging
        {
            this.paletteImage = palette.Clone();
            this.palette = ColorController.grabColorDataUnique(paletteImage);
            Console.WriteLine("Palatte Processed!");
        }
        public BitmapImage getSourceImage() { return sourceImage; }
        public List<Color> getSourceList() { return pixels; }
        public void setSourceImage(BitmapImage sourceImage)
        {
            this.sourceImage = sourceImage.Clone();
            this.sourceHeight = (int)sourceImage.Height;
            this.sourceWidth = (int)sourceImage.Width;
            this.pixels = ColorController.grabColorData(this.sourceImage);
            this.uniquePixels = ColorController.grabColorDataUnique(this.sourceImage);
            Console.WriteLine("Source Processed!");
        }
        public WriteableBitmap getFinalImage() { return finalImage; }
        //"setFinalImage" is less of a traditional setter. Used to ensure that finalImage is created from sourceImage
        public void setFinalImage(WriteableBitmap source) { finalImage = source; }

        //Methods
        public async void drawImage()
        {
            //if(paletteImage == null || finalImage == null || sourceImage == null)
            //{
            //    Console.WriteLine($"Palette: {paletteImage != null}, Final: {finalImage != null}, Source: {sourceImage != null}");
            //    Console.WriteLine("One or both images not set!");
            //    return;
            //}

            //Hopefully this works lmao
            ColorController.interpolateXYZ(palette, pixels);
            //List<Color> colors = ColorController.interpXYZ(palette, uniquePixels);

            Console.WriteLine($"Access: {finalImage.Dispatcher.CheckAccess()}");
            Console.WriteLine($"Access through invoke: {finalImage.Dispatcher.Invoke(() => finalImage.Dispatcher.CheckAccess())}");

            int pixelCount = 0;
            for(int y = 0; y < sourceHeight; y++)
            {
                for(int x = 0; x < sourceWidth; x++)
                {
                    double[] rgb = pixels[pixelCount].getRGB();
                    byte[] _rgb = { (byte)rgb[0], (byte)rgb[1], (byte)rgb[2], 255 };

                    //for (int i = 0; i < uniquePixels.Count; i++)
                    //{
                    //    double[] c = uniquePixels[i].getRGB();
                    //    if(rgb[0] == c[0] && rgb[1] == c[1] && rgb[2] == c[2])
                    //    {
                    //        rgb = c;
                    //        break;
                    //    }
                    //}

                    Int32Rect rect = new Int32Rect(x, y, 1, 1);

                    Dispatcher disp = finalImage.Dispatcher;
                    //disp.Invoke(() => 
                    disp.Invoke(() => finalImage.WritePixels(rect, _rgb, sourceImage.PixelWidth * 4, 0));
                    await Task.Delay(10);
                    pixelCount++;
                }
            }

            Console.WriteLine("Finished drawing image...");
        }

        //Checks to see if all pixels in the line are the same color
        public int checkConnectedLine(int pixelCount, int x)
        {
            int output = 1;
            for(int i = 0; i < sourceImage.Width - x; i++)
            {
                double[] c1 = pixels[pixelCount].getRGB();
                double[] c2 = pixels[pixelCount + i].getRGB();
                if (c1[0] == c2[0] && c1[1] == c2[1] && c1[2] == c2[2])
                {
                    output++;
                } else
                {
                    break;
                }
            }
            Console.WriteLine($"Found {output} colors in a row");
            return output;
        }
    }
}
