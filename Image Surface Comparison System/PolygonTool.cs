using System;
using System.Collections.Generic;
using System.Windows;

namespace Image_Surface_Comparison_System
{
    public static class PolygonTool
    {
        private static List<Point> points;
        private static double actualWidth;
        private static double actualHeight;
        private static bool[,] visitedPixels;

        public static void Polygon(List<Point> _points, double _actualWidth, double _actualHeight)
        {
            points = new List<Point>();

            actualWidth = _actualWidth;
            actualHeight = _actualHeight;

            foreach (Point tmp in _points)
                points.Add(new Point((tmp.X * (double)(Base.photo.width) / actualWidth), (tmp.Y * (double)(Base.photo.height) / actualHeight)));

            
            visitedPixels = new bool[Base.photo.width, Base.photo.height];

            if (Base.selectedMode == 0)
            {
                Base.photo.pixelData = (uint[])Base.photoOrginal.pixelData.Clone();
                Base.photo.selectedPixelsCount = 0;

                Array.Clear(Base.photo.selectedPixels, 0, Base.photo.selectedPixels.Length);
                Array.Clear(visitedPixels, 0, visitedPixels.Length);
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (i + 1 != points.Count)
                    Line(points[i], points[i + 1]);
                else
                    Line(points[i], points[0]);
            }

            bool last = false;
            int amount;
            bool exit = false;

            for (int y = 0; y < Base.photo.height; y++)
            {
                amount = 0;
                for (int x = 0; x < Base.photo.width; x++)
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

            Base.photo.photo.WritePixels(new Int32Rect(0, 0, (int)Base.photo.width, (int)Base.photo.height), Base.photo.pixelData, Base.photo.widthInByte, 0);
        }

        private static void Line(Point p1, Point p2)
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
                index = Base.photo.GetIndex(x, y);

                visitedPixels[x, y] = true;

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

        private static void Fill(int xx, int yy)
        {
            Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>();
            stack.Push(new Tuple<int, int>(xx, yy));

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                int x = current.Item1;
                int y = current.Item2;


                int index;
                index = Base.photo.GetIndex(x, y);

                visitedPixels[x, y] = true;
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

                if (x - 1 >= 0 && visitedPixels[x - 1, y] == false)
                    stack.Push(new Tuple<int, int>(x - 1, y));
                if (x + 1 < Base.photo.width && visitedPixels[x + 1, y] == false)
                    stack.Push(new Tuple<int, int>(x + 1, y));
                if (y - 1 >= 0 && visitedPixels[x, y - 1] == false)
                    stack.Push(new Tuple<int, int>(x, y - 1));
                if (y + 1 < Base.photo.height && visitedPixels[x, y + 1] == false)
                    stack.Push(new Tuple<int, int>(x, y + 1));
            }
        }
    }
}