using System;
using System.Windows;

namespace Image_Surface_Comparison_System
{
    class BrushTool
    {
        Photo patient;
        Photo orginal;
        int mode;
        int size;
        int startX;
        int startY;
        int shape;

        public BrushTool(Photo patient, Photo orginal, int mode, int startX, int startY, int size, int shape)
        {
            this.patient = patient;
            this.orginal = orginal;
            this.mode = mode;
            this.size = size;
            this.startX = startX;
            this.startY = startY;
            this.shape = shape;
        }

        public Photo Brush()
        {
            if (mode == 0)
            {
                patient.pixelData = (uint[])orginal.pixelData.Clone();
                Array.Clear(patient.selectedPixels, 0, patient.selectedPixels.Length);
                patient.selectedPixelsCount = 0;
                mode = 1;
            }

            int x, y;

            if (startX - (size / 2) < 0)
                x = 0;
            else if (startX + (size / 2) >= patient.width)
                x = (int)patient.width - 1;
            else
                x = startX - (size / 2);

            if (startY - (size / 2) < 0)
                y = 0;
            else if (startY + (size / 2) >= patient.height)
                y = (int)patient.height - 1;
            else
                y = startY - (size / 2);

            if (shape == 0)
                DrawCircle(startX, startY, size);
            else
                DrawSquare(startX, startY, size);


            patient.photo.WritePixels(new Int32Rect(0, 0, (int)patient.width, (int)patient.height), patient.pixelData, patient.widthInByte, 0);
            return patient;
        }

        private void DrawSquare(int startX, int startY, int size)
        {
            int x = startX - size / 2 + 1;
            int y = startY - size / 2 + 1;

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    DrawPixel(x + i, y + j);
        }

        private void DrawCircle(int x0, int y0, int size)
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

        private void DrawPixel(int x, int y)
        {
            if (x >= 0 && x < patient.width && y >= 0 && y < patient.height)
            {
                int index;
                index = patient.GetIndex(x, y);
                if (mode != 2)
                {
                    if (patient.selectedPixels[x, y] == false)
                    {
                        patient.pixelData[index] = Color.ToUint(Base.selectedColor);
                        patient.selectedPixels[x, y] = true;
                        patient.selectedPixelsCount++;
                    }
                }
                else
                {
                    if (patient.selectedPixels[x, y] == true)
                    {
                        patient.pixelData[index] = orginal.pixelData[index];
                        patient.selectedPixels[x, y] = false;
                        patient.selectedPixelsCount--;
                    }
                }
            }
        }

        private void DrawLine(int x1, int x2, int y)
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