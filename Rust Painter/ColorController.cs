using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Rust_Painter
{
    internal class ColorController
    {
        
        public static List<Color> interpXYZ(List<Color> _palette, List<Color> _pixels)
        {
            Console.WriteLine("Interpolating...");

            List<Color> temp = new List<Color>();

            int length = _pixels.Count;

            //Chunk everything into task so UI can update
            for (int x = 0; x < length; x++)
            {
                //Get pixel XYZ value
                Color c = _pixels[x];

                double _x = c.getX();
                double _y = c.getY();
                double _z = c.getZ();

                double lowDiff = 1000.0; //Lowest difference between colors
                Color newColor = new Color();

                //Loop over every color and find the closest
                for (int y = 0; y < _palette.Count; y++)
                {
                    //Grab palette color
                    Color p = _palette[y];
                    double pX = p.getX();
                    double pY = p.getY();
                    double pZ = p.getZ();

                    //Get distance between colors in 3D space
                    double diff = Math.Sqrt(Math.Pow((pX - _x), 2) + Math.Pow((pY - _y), 2) + Math.Pow(pZ - _z, 2));

                    if (y == 0) { lowDiff = diff; }
                    if (diff < lowDiff)
                    {
                        lowDiff = diff;
                        newColor = p;
                    }
                }

                //Change pixel color data to the palette
                temp.Add(newColor);
            }

            return temp;
        }

        public static async void interpolateXYZ(List<Color> _palette, List<Color> _pixels)
        {
            Console.WriteLine("Interpolating...");

            int length = _pixels.Count;
            
            //Chunk everything into task so UI can update
            for (int x = 0; x < length; x++)
            {
                //Get pixel XYZ value
                Color c = _pixels[x];

                double _x = c.getX();
                double _y = c.getY();
                double _z = c.getZ();

                double lowDiff = 1000.0; //Lowest difference between colors
                Color newColor = new Color();

                //Loop over every color and find the closest
                for (int y = 0; y < _palette.Count; y++)
                {
                    //Grab palette color
                    Color p = _palette[y];
                    double pX = p.getX();
                    double pY = p.getY();
                    double pZ = p.getZ();

                    //Get distance between colors in 3D space
                    double diff = Math.Sqrt(Math.Pow((pX - _x), 2) + Math.Pow((pY - _y), 2) + Math.Pow(pZ - _z, 2));

                    if (y == 0) { lowDiff = diff; }
                    if (diff < lowDiff)
                    {
                        lowDiff = diff;
                        newColor = p;
                    }
                }

                //Change pixel color data to the palette
                _pixels[x] = newColor;
            }
        }

        public static double[] ToXYZ(double red, double green, double blue, double alpha)
        {
            double[] converted;

            double rLinear = (double)(red / 255.0);
            double gLinear = (double)(blue / 255.0);
            double bLinear = (double)(green / 255.0);

            double r = (rLinear > 0.04045) ? Math.Pow((rLinear + 0.055) / (1 + 0.055), 2.2) : (rLinear / 12.92);
            double g = (gLinear > 0.04045) ? Math.Pow((gLinear + 0.055) / (1 + 0.055), 2.2) : (gLinear / 12.92);
            double b = (bLinear > 0.04045) ? Math.Pow((bLinear + 0.055) / (1 + 0.055), 2.2) : (bLinear / 12.92);

            converted = new double[4];
            converted[0] = (r * 0.4124 + g * 0.3576 + b * 0.1805);
            converted[1] = (r * 0.2126 + g * 0.7152 + b * 0.0722);
            converted[2] = (r * 0.0193 + g * 0.1192 + b * 0.9505);
            converted[3] = alpha;

            return converted;
        }

        public static double[] XYZtoRGB(double x, double y, double z)
        {
            double[] conv = new double[4];

            double[] Clinear = new double[3];
            Clinear[0] = x * 3.2410 - y * 1.5374 - z * 0.4986; // red
            Clinear[1] = -x * 0.9692 + y * 1.8760 - z * 0.0416; // green
            Clinear[2] = x * 0.0556 - y * 0.2040 + z * 1.0570; // blue

            for (int i = 0; i < 3; i++)
            {
                Clinear[i] = (Clinear[i] <= 0.0031308) ? 12.92 * Clinear[i] : (1 + 0.055) * Math.Pow(Clinear[i], (1.0 / 2.4)) - 0.055;
            }

            conv[0] = (double)(x * 255);
            conv[1] = (double)(y * 255);
            conv[2] = (double)(z * 255);

            return conv;
        }

        private static bool checkExists(Color color, List<Color> array)
        {
            foreach(Color c in array)
            {
                double[] _c = c.getRGB();
                double[] _color = color.getRGB();
                if(_c[0] == _color[0] && _c[1] == _color[1] && _c[2] == _color[2])
                {
                    return true;
                }
            }

            return false;
        }

        public static List<Color> grabColorDataUnique(BitmapImage source)
        {
            List<Color> colors = new List<Color>();

            //Get pixel info
            //Create pixel array
            //BROOOOOOO THREADS SUCK ASS FUCK
            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
             byte[] pixels = new byte[size];

            //Copy pixel data
            source.CopyPixels(pixels, stride, 0);

            //Go down current column
            for (int y = 0; y < (int)source.Height; y++)
            {
                //Go across the current row
                for (int x = 0; x < (int)source.Width; x++)
                {
                    int index = y * stride + 4 * x;

                    //Pixel color/alpha data
                    double red = (double)pixels[index];
                    double green = (double)pixels[index + 1];
                    double blue = (double)pixels[index + 2];
                    double alpha = (double)pixels[index + 3];

                    Color c = new Color(red, green, blue);

                    if (!checkExists(c, colors)) colors.Add(c);

                }
            }

            Console.WriteLine("Colors found: " + colors.Count);
            return colors;
        }

        public static List<Color> grabColorData(BitmapImage source)
        {
            List<Color> colors = new List<Color>();

            //Get pixel info
                //Create pixel array
            //BROOOOOOO THREADS SUCK ASS FUCK
            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];

            //Copy pixel data
            source.CopyPixels(pixels, stride, 0);

            //Go down current column
            for(int y = 0; y < (int)source.Height; y++)
            {
                //Go across the current row
                for(int x = 0; x < (int)source.Width; x++)
                {
                    int index = y * stride + 4 * x;

                    //Pixel color/alpha data
                    double red = (double)pixels[index];
                    double green = (double)pixels[index + 1];
                    double blue = (double)pixels[index + 2];
                    double alpha = (double)pixels[index + 3];

                    Color c = new Color(red, green, blue);
                    colors.Add(c);
                }
            }
            Console.WriteLine("Colors found: " + colors.Count);
            return colors;
        }
    }
}
