using System;
using System.Windows;

namespace Image_Surface_Comparison_System
{
    public static class BrushTool
    {
        private static int size;
        private static int startX;
        private static int startY;
        private static int shape;

        public static void Brush(int _startX, int _startY, int _size, int _shape)
        {
            size = _size;
            startX = _startX;
            startY = _startY;
            shape = _shape;

            if (Base.selectedMode == 0)
            {
                Base.photo.pixelData = (uint[])Base.photoOrginal.pixelData.Clone();
                Array.Clear(Base.photo.selectedPixels, 0, Base.photo.selectedPixels.Length);
                Base.photo.selectedPixelsCount = 0;
            }

            int x, y;

            if (startX - (size / 2) < 0)
                x = 0;
            else if (startX + (size / 2) >= Base.photo.width)
                x = (int)Base.photo.width - 1;
            else
                x = startX - (size / 2);

            if (startY - (size / 2) < 0)
                y = 0;
            else if (startY + (size / 2) >= Base.photo.height)
                y = (int)Base.photo.height - 1;
            else
                y = startY - (size / 2);

            if (shape == 0)
                DrawCircle(startX, startY, size);
            else
                DrawSquare(startX, startY, size);
            
            Base.photo.photo.WritePixels(new Int32Rect(0, 0, (int)Base.photo.width, (int)Base.photo.height), Base.photo.pixelData, Base.photo.widthInByte, 0);
        }

        private static void DrawSquare(int startX, int startY, int size)
        {
            int x = startX - size / 2 + 1;
            int y = startY - size / 2 + 1;

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    DrawPixel(x + i, y + j);
        }

        private static void DrawCircle(int x0, int y0, int size)
        {
            int radius = (size / 2) - 1;
            int x = radius;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (radius << 1);

            while (x >= y)
            {
                DrawPixel(x0 + x, y0 + y);
                DrawPixel(x0 + y, y0 + x);
                DrawPixel(x0 - y, y0 + x);
                DrawPixel(x0 - x, y0 + y);
                DrawPixel(x0 - x, y0 - y);
                DrawPixel(x0 - y, y0 - x);
                DrawPixel(x0 + y, y0 - x);
                DrawPixel(x0 + x, y0 - y);

                DrawLine(x0 + x, x0 - x, y0 + y);
                DrawLine(x0 + y, x0 - y, y0 + x);
                DrawLine(x0 - x, x0 + x, y0 - y);
                DrawLine(x0 - y, x0 + y, y0 - x);

                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }
                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += dx - (radius << 1);
                }
            }
            radius--;
        }

        private static void DrawPixel(int x, int y)
        {
            if (x >= 0 && x < Base.photo.width && y >= 0 && y < Base.photo.height)
            {
                int index;
                index = Base.photo.GetIndex(x, y);
                if (Base.selectedMode != 2)
                {
                    if (Base.photo.selectedPixels[x, y] == false)
                    {
                        Base.photo.pixelData[index] = Color.ToUint(Base.selectedColor);
                        Base.photo.selectedPixels[x, y] = true;
                        Base.photo.selectedPixelsCount++;
                    }
                }
                else
                {
                    if (Base.photo.selectedPixels[x, y] == true)
                    {
                        Base.photo.pixelData[index] = Base.photoOrginal.pixelData[index];
                        Base.photo.selectedPixels[x, y] = false;
                        Base.photo.selectedPixelsCount--;
                    }
                }
            }
        }

        private static void DrawLine(int x1, int x2, int y)
        {
            if (x1 < x2)
                for (; x1 < x2; x1++)
                    DrawPixel(x1, y);
            else
                for (; x2 < x1; x2++)
                    DrawPixel(x2, y);
        }
    }
}