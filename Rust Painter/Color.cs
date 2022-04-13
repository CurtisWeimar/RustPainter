using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Painter
{
    internal class Color
    {

        double red;
        double green;
        double blue;
        double alpha = 255.0;

        double x;
        double y;
        double z;

        private static async Task<Color> interpolateXYZ(List<Color> _palette, List<Color> _pixels)
        {
            Console.WriteLine("Interpolating...");

            int length = _pixels.Count;

            for(int x = 0; x < _pixels.Count; x++)
            {
                //Get pixel color
                Color _c = _pixels[x];
                Color c = ToXYZ(_c);
                
            }

            for (int x = 0; x < _pixels.Count; x++)
            {
                //Get pixel XYZ value
                byte[] _pixel = _pixels[x]; // R G B A
                //Console.WriteLine($"RGB: {_pixel[0]}, {_pixel[1]}, {_pixel[2]}");
                double[] pixel = ToXYZ(_pixel); // Convert it to XYZ
                //Console.WriteLine($"XYZ: {pixel[0]}, {pixel[1]}, {pixel[2]}");

                double red = pixel[0];
                double green = pixel[1];
                double blue = pixel[2];
                //byte alpha = pixel[3];  Alpha data not really necessary

                double low_diff = 1000.0; // Lowest integer difference between colors
                double[] newColor = { 0, 0, 0, 255 };

                // Loop over every palette color and find the closest
                for (int y = 0; y < _palette.Count; y++)
                {


                    double[] paletteColor = ToXYZ(_palette[y]); // Convert it to XYZ
                    double _red = paletteColor[0];
                    double _green = paletteColor[1];
                    double _blue = paletteColor[2];

                    // Calculate distance in 3D RGB color space between the two colors
                    double diff = Math.Sqrt(Math.Pow((_red - red), 2) + Math.Pow((_green - green), 2) + Math.Pow((_blue - blue), 2));

                    //Console.WriteLine("Color Difference: " + diff);
                    if (y == 0) { low_diff = diff; diffs.Add(diff); }
                    if (diff < low_diff)
                    {
                        low_diff = diff;
                        diffs.Add(diff);
                        newColor = paletteColor;
                        //Console.WriteLine($"New Color: {newColor[0]}, {newColor[1]}, {newColor[2]}");
                        newColor[3] = 255;
                    }
                }
            }
        }

        private static double[] ToXYZ(byte[] pixel)
        {
            double[] converted;

            double rLinear = (double)(pixel[0] / 255.0);
            double gLinear = (double)(pixel[1] / 255.0);
            double bLinear = (double)(pixel[2] / 255.0);

            double r = (rLinear > 0.04045) ? Math.Pow((rLinear + 0.055) / (1 + 0.055), 2.2) : (rLinear / 12.92);
            double g = (gLinear > 0.04045) ? Math.Pow((gLinear + 0.055) / (1 + 0.055), 2.2) : (gLinear / 12.92);
            double b = (bLinear > 0.04045) ? Math.Pow((bLinear + 0.055) / (1 + 0.055), 2.2) : (bLinear / 12.92);

            converted = new double[4];
            converted[0] = (r * 0.4124 + g * 0.3576 + b * 0.1805);
            converted[1] = (r * 0.2126 + g * 0.7152 + b * 0.0722);
            converted[2] = (r * 0.0193 + g * 0.1192 + b * 0.9505);
            converted[3] = pixel[3];

            return converted;
        }

        private static async Task<Task> interpolateColor(List<byte[]> _palette, List<byte[]> _pixels)
        {
            Console.WriteLine("Interpolating...");
            //Console.WriteLine($"Lenght of array: {_pixels.Count()}");

            List<double> diffs = new List<double>();

            // Loop through every source pixel
            int length = _pixels.Count;
            for (int x = 0; x < _pixels.Count; x++)
            {
                //Get pixel XYZ value
                byte[] _pixel = _pixels[x]; // R G B A
                //Console.WriteLine($"RGB: {_pixel[0]}, {_pixel[1]}, {_pixel[2]}");
                double[] pixel = ToXYZ(_pixel); // Convert it to XYZ
                //Console.WriteLine($"XYZ: {pixel[0]}, {pixel[1]}, {pixel[2]}");

                double red = pixel[0];
                double green = pixel[1];
                double blue = pixel[2];
                //byte alpha = pixel[3];  Alpha data not really necessary

                double low_diff = 1000.0; // Lowest integer difference between colors
                double[] newColor = { 0, 0, 0, 255 };

                // Loop over every palette color and find the closest
                for (int y = 0; y < _palette.Count; y++)
                {


                    double[] paletteColor = ToXYZ(_palette[y]); // Convert it to XYZ
                    double _red = paletteColor[0];
                    double _green = paletteColor[1];
                    double _blue = paletteColor[2];

                    // Calculate distance in 3D RGB color space between the two colors
                    double diff = Math.Sqrt(Math.Pow((_red - red), 2) + Math.Pow((_green - green), 2) + Math.Pow((_blue - blue), 2));

                    //Console.WriteLine("Color Difference: " + diff);
                    if (y == 0) { low_diff = diff; diffs.Add(diff); }
                    if (diff < low_diff)
                    {
                        low_diff = diff;
                        diffs.Add(diff);
                        newColor = paletteColor;
                        //Console.WriteLine($"New Color: {newColor[0]}, {newColor[1]}, {newColor[2]}");
                        newColor[3] = 255;
                    }
                }

                // Change pixel color data to the palette
                Console.WriteLine("Done " + ((double)x / (double)length) * 100 + "%");
                await Task.Run(() => outputText("Done " + ((double)x / (double)length) * 100 + "%"));
                _pixels[x] = XYZtoRGB(newColor);
            }

            //Average difference between colors
            double sum = 0;
            for (int i = 0; i < diffs.Count(); i++)
            {
                sum += diffs[i];
            }
            //Console.WriteLine($"Average Color Difference: {sum/diffs.Count()}");
            return Task.CompletedTask;
        }
    }
}
