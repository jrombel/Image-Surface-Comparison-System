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

        public BrushTool(Photo patient, Photo orginal, int mode, int startX, int startY, int size)
        {
            this.patient = patient;
            this.orginal = orginal;
            this.mode = mode;
            this.size = size;
            this.startX = startX;
            this.startY = startY;
        }

        public Photo Brush()
        {
            if (mode == 0)
                patient.pixelData = (uint[])orginal.pixelData.Clone();

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

            int index;

            DrawCircle(startX, startY, size);
            //for (int i = 0; i < size; i++)
            //{
            //    for (int j = 0; j < size; j++)
            //    {
            //        index = patient.GetIndex(x + i, y + j);
            //        if (mode != 2)
            //            patient.pixelData[index] = 0xffff0000;
            //        else
            //            patient.pixelData[index] = orginal.pixelData[index];
            //    }
            //}






            patient.photo.WritePixels(new Int32Rect(0, 0, (int)patient.width, (int)patient.height), patient.pixelData, patient.widthInByte, 0);
            return patient;
        }

        private void DrawCircle(int x0, int y0, int radius)
        {
            int x = radius - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (radius << 1);

            while (x >= y)
            {
                PutPixel(x0 + x, y0 + y);
                PutPixel(x0 + y, y0 + x);
                PutPixel(x0 - y, y0 + x);
                PutPixel(x0 - x, y0 + y);
                PutPixel(x0 - x, y0 - y);
                PutPixel(x0 - y, y0 - x);
                PutPixel(x0 + y, y0 - x);
                PutPixel(x0 + x, y0 - y);

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
        }

        private void PutPixel(int x, int y)
        {
            int index;
            index = patient.GetIndex(x, y);
            if (mode != 2)
                patient.pixelData[index] = 0xffff0000;
            else
                patient.pixelData[index] = orginal.pixelData[index];
        }

    }
}