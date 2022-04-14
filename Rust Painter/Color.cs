using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Painter
{
    internal class Color
    {

        private double red;
        private double green;
        private double blue;
        private double alpha = 255.0;

        private double x;
        private double y;
        private double z;

        public Color()
        {
            setRed(0);
            setGreen(0);
            setBlue(0);

            setX(0);
            setY(0);
            setZ(0);
        }

        public Color(double red, double green, double blue)
        {
            setRed(red);
            setGreen(green);
            setBlue(blue);

            double[] xyz = ColorController.ToXYZ(red, green, blue, 255); //Temp array
            setX(xyz[0]);
            setY(xyz[1]);
            setZ(xyz[2]);
        }
        
        //TODO: Find way to make overload which takes individual variables intead of array ?
        public Color(double[] xyz)
        {
            setX(xyz[0]);
            setY(xyz[1]);
            setZ(xyz[2]);

            double[] rgb = ColorController.XYZtoRGB(x, y, z);

            setRed(rgb[0]);
            setGreen(rgb[1]);
            setBlue(rgb[2]);
        }

        //Getters and Setters
        //Array getters and setters
        public double[] getRGB() { return new double[] {red, green, blue}; }
        public void setRGB(double[] rgb) { setRed(rgb[0]); setGreen(rgb[1]); setBlue(rgb[2]); }
        public double[] getXYZ() { return new double[] { x, y, z }; }
        public void setXYZ(double[] xyz) { setX(xyz[0]); setY(xyz[1]); setZ(xyz[2]); }

        //RGB Getters and Setters
        public double getRed() { return red; }
        public void setRed(double red) { if(red < 0) red = 0; if (red > 255) red = 255; this.red = red; }
        public double getGreen() { return green; }
        public void setGreen(double green) { if (green < 0) green = 0; if (green > 255) green = 255; this.green = green; }
        public double getBlue(double blue) { return blue; }
        public void setBlue(double blue) { if(blue < 0) blue = 0; if (blue > 255) blue = 255; this.blue = blue;}
        public double getAlpha() { return alpha; }
        public void setAlpha(double alpha) { if(alpha < 0) alpha = 0; if (alpha > 255) alpha = 255; this.alpha = alpha; }

        //XYZ Getters and Setters
        public double getX() { return x; }
        public void setX(double x) { this.x = x; }
        public double getY() { return y; }
        public void setY(double y) { this.y = y; }
        public double getZ() { return z; }
        public void setZ(double z) { this.z = z; }

    }
}
