using System;

namespace Image_Surface_Comparison_System
{
    public class Color
    {
        public byte red;
        public byte green;
        public byte blue;

        public double CieL;
        public double CieA;
        public double CieB;

        public Color(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;

            RGBtoLAB();
        }

        public Color(Color tmp)
        {
            this.red = tmp.red;
            this.green = tmp.green;
            this.blue = tmp.blue;

            RGBtoLAB();
        }

        //public bool Difference(Color color, byte difference)
        //{
        //    if (Math.Abs(color.red - this.red) <= difference && Math.Abs(color.green - this.green) <= difference && Math.Abs(color.blue - this.blue) <= difference)
        //        return true;
        //    else
        //        return false;
        //}

        public static int Difference(Color a, Color b)
        {
            double distance = Math.Sqrt(Math.Pow((a.CieL - b.CieL), 2) + Math.Pow((a.CieA - b.CieA), 2) + Math.Pow((a.CieB - b.CieB), 2));
            return Convert.ToInt16(Math.Round(distance));
        }

        public void RGBtoLAB()
        {
            double R = Convert.ToDouble(red) / 255.0;
            double G = Convert.ToDouble(green) / 255.0;
            double B = Convert.ToDouble(blue) / 255.0;

            if (R > 0.04045)
                R = Math.Pow(((R + 0.055) / 1.055), 2.4);
            else
                R = R / 12.92;

            if (G > 0.04045)
                G = Math.Pow(((G + 0.055) / 1.055), 2.4);
            else
                G = G / 12.92;

            if (B > 0.04045)
                B = Math.Pow(((B + 0.055) / 1.055), 2.4);
            else
                B = B / 12.92;

            R *= 100;
            G *= 100;
            B *= 100;

            double X = R * 0.4124 + G * 0.3576 + B * 0.1805 / 95.047;
            double Y = R * 0.2126 + G * 0.7152 + B * 0.0722 / 100.000;
            double Z = R * 0.0193 + G * 0.1192 + B * 0.9505 / 108.883;

            if (X > 0.008856)
                X = Math.Pow(X, (1 / 3.0));
            else
                X = (7.787 * X) + (16 / 116.0);

            if (Y > 0.008856)
                Y = Math.Pow(Y, (1 / 3.0));
            else
                Y = (7.787 * Y) + (16 / 116.0);

            if (Z > 0.008856)
                Z = Math.Pow(Z, (1 / 3.0));
            else
                Z = (7.787 * Z) + (16 / 116.0);

            CieL = (116 * Y) - 16;
            CieA = 500 * (X - Y);
            CieB = 200 * (Y - Z);
        }

        public static uint ToUint(byte red, byte green, byte blue)
        {
            byte alpha = 0;
            return (uint)(((alpha << 24) | (red << 16) | (green << 8) | blue) & 0xffffffffL);
        }

        public static uint ToUint(Color color)
        {
            byte alpha = 0;
            return (uint)(((alpha << 24) | (color.red << 16) | (color.green << 8) | color.blue) & 0xffffffffL);
        }

        public String ToString()
        {
            return "#" + red.ToString("X2") + green.ToString("X2") + blue.ToString("X2");
        }
    }
}