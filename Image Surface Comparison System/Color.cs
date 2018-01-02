using System;

namespace Image_Surface_Comparison_System
{
    public class Color
    {
        byte red;
        byte green;
        byte blue;

        public Color(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public Color(Color tmp)
        {
            this.red = tmp.red;
            this.green = tmp.green;
            this.blue = tmp.blue;
        }

        public String ToString()
        {
            return "#" + red.ToString("X2") + green.ToString("X2") + blue.ToString("X2");
        }

        public bool Difference(Color color, byte difference)
        {
            if (Math.Abs(color.red - this.red) <= difference && Math.Abs(color.green - this.green) <= difference && Math.Abs(color.blue - this.blue) <= difference)
                return true;
            else
                return false;
        }

        public static int Difference(Color a, Color b)
        {
            int distance = (int)Math.Sqrt(Math.Pow(b.red - a.red, 2) + Math.Pow(b.green - a.green, 2) + Math.Pow(b.blue - a.blue, 2));
            return distance;
        }

        public static int Difference(byte oR, byte oG, byte oB, byte cR, byte cG, byte cB)
        {
            int distance = (int)Math.Sqrt(Math.Pow(cR - oR, 2) + Math.Pow(cG - oG, 2) + Math.Pow(cB - oB, 2));
            return distance;
        }
    }
}