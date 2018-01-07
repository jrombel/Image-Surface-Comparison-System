using System;
using System.Collections.Generic;
using System.Windows;

namespace Image_Surface_Comparison_System
{
    class PolygonTool
    {
        Photo patient;
        Photo orginal;
        int mode;
        List<Point> points;

        double actualWidth;
        double actualHeight;

        bool[,] visitedPixels;

        public PolygonTool(Photo patient, Photo orginal, int mode, List<Point> points, double actualWidth, double actualHeight)
        {
            this.patient = patient;
            this.orginal = orginal;
            this.mode = mode;

            this.points = new List<Point>();
            foreach (Point tmp in points)
                this.points.Add(new Point((tmp.X * (double)(patient.width) / actualWidth), (tmp.Y * (double)(patient.height) / actualHeight)));

            this.actualWidth = actualWidth;
            this.actualHeight = actualHeight;
            visitedPixels = new bool[(int)patient.width, (int)patient.height];
        }

        public Photo Polygon()
        {
            if (mode == 0)
            {
                patient.pixelData = (uint[])orginal.pixelData.Clone();
                patient.selectedPixelsCount = 0;

                Array.Clear(patient.selectedPixels, 0, patient.selectedPixels.Length);
                Array.Clear(visitedPixels, 0, visitedPixels.Length);
            }
            
            for (int i = 0; i < points.Count; i++)
            {
                if (i + 1 != points.Count)
                    line(points[i], points[i + 1]);
                else
                    line(points[i], points[0]);
            }
            
            bool last = false;
            int amount;
            bool exit = false;

            for (int y = 0; y < patient.height; y++)
            {
                amount = 0;
                for (int x = 0; x < patient.width; x++)
                {
                    if (visitedPixels[x, y] != last)
                        amount++;

                    last = visitedPixels[x, y];

                    if (y > 0)
                    {
                        if ((amount / 2) % 2 == 1 && amount > 0 && visitedPixels[x, y] == false && visitedPixels[x, y - 1] == true)
                        {
                            Fill(x, y);
                            exit = true;
                            break;
                        }
                    }
                }
                if (exit == true)
                    break;
            }

            patient.photo.WritePixels(new Int32Rect(0, 0, (int)patient.width, (int)patient.height), patient.pixelData, patient.widthInByte, 0);
            return patient;
        }

        private void line(Point p1, Point p2)
        {
            int w = (int)(p2.X - p1.X);
            int h = (int)(p2.Y - p1.Y);
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            int x = (int)p1.X;
            int y = (int)p1.Y;
            for (int i = 0; i <= longest; i++)
            {
                int index;
                index = patient.GetIndex(x, y);

                visitedPixels[x, y] = true;

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
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        private void Fill(int xx, int yy)
        {
            Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>();
            stack.Push(new Tuple<int, int>(xx, yy));

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                int x = current.Item1;
                int y = current.Item2;


                int index;
                index = patient.GetIndex(x, y);

                visitedPixels[x, y] = true;
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

                if (x - 1 >= 0 && visitedPixels[x - 1, y] == false)
                    stack.Push(new Tuple<int, int>(x - 1, y));
                if (x + 1 < patient.width && visitedPixels[x + 1, y] == false)
                    stack.Push(new Tuple<int, int>(x + 1, y));
                if (y - 1 >= 0 && visitedPixels[x, y - 1] == false)
                    stack.Push(new Tuple<int, int>(x, y - 1));
                if (y + 1 < patient.height && visitedPixels[x, y + 1] == false)
                    stack.Push(new Tuple<int, int>(x, y + 1));
            }
        }
    }
}
