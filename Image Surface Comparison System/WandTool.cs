using System;
using System.Collections.Generic;
using System.Windows;

namespace Image_Surface_Comparison_System
{
    public static class WandTool
    {
        private static bool[,] visitedPixels;
        private static int startX;
        private static int startY;
        private static int index;
        private static Color comparative;
        private static byte degree;

        public static void Wand(int _startX, int _startY, Color _comparative, byte _degree)
        {
            visitedPixels = new bool[(int)Base.photo.width, (int)Base.photo.height];
            startX = _startX;
            startY = _startY;
            visitedPixels[startX, startY] = true;
            comparative = _comparative;
            degree = _degree;

            if (Base.selectedMode == 0)
            {
                Base.photo.pixelData = (uint[])Base.photoOrginal.pixelData.Clone();
                Base.photo.selectedPixelsCount = 0;

                Array.Clear(Base.photo.selectedPixels, 0, Base.photo.selectedPixels.Length);
                Array.Clear(visitedPixels, 0, visitedPixels.Length);
            }
            Selected((ushort)startX, (ushort)startY);

            Base.photo.photo.WritePixels(new Int32Rect(0, 0, (int)Base.photo.width, (int)Base.photo.height), Base.photo.pixelData, Base.photo.widthInByte, 0);
        }

        private static void Selected(ushort x, ushort y)
        {
            Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>();
            stack.Push(new Tuple<int, int>(x, y));

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                int x2 = current.Item1;
                int y2 = current.Item2;

                index = Base.photoOrginal.GetIndex(x2, y2);
                if (Base.selectedMode != 2)
                {
                    if (Base.photo.selectedPixels[x2, y2] == false)
                    {
                        Base.photo.pixelData[index] = Color.ToUint(Base.selectedColor);
                        Base.photo.selectedPixelsCount++;
                        Base.photo.selectedPixels[x2, y2] = true;
                    }
                }
                else
                {
                    if (Base.photo.selectedPixels[x2, y2] == true)
                    {
                        Base.photo.pixelData[index] = Base.photoOrginal.pixelData[index];
                        Base.photo.selectedPixelsCount--;
                        Base.photo.selectedPixels[x2, y2] = false;
                    }
                }

                visitedPixels[x2, y2] = true;

                if (x2 - 1 >= 0 && visitedPixels[x2 - 1, y2] == false)
                {
                    index = Base.photoOrginal.GetIndex(x2 - 1, y2);
                    Color tmp = Base.photoOrginal.GetColor(index);
                    if (Color.Difference(tmp, comparative) < degree)
                        stack.Push(new Tuple<int, int>(x2 - 1, y2));
                }

                if (y2 - 1 >= 0 && visitedPixels[x2, y2 - 1] == false)
                {
                    index = Base.photoOrginal.GetIndex(x2, y2 - 1);
                    Color tmp = Base.photoOrginal.GetColor(index);
                    if (Color.Difference(tmp, comparative) < degree)
                        stack.Push(new Tuple<int, int>(x2, y2 - 1));
                }

                if (x2 + 1 < Base.photoOrginal.width && visitedPixels[x2 + 1, y2] == false)
                {
                    index = Base.photoOrginal.GetIndex(x2 + 1, y2);
                    Color tmp = Base.photoOrginal.GetColor(index);
                    if (Color.Difference(tmp, comparative) < degree)
                        stack.Push(new Tuple<int, int>(x2 + 1, y2));
                }

                if (y2 + 1 < Base.photoOrginal.height && visitedPixels[x2, y2 + 1] == false)
                {
                    index = Base.photoOrginal.GetIndex(x2, y2 + 1);
                    Color tmp = Base.photoOrginal.GetColor(index);
                    if (Color.Difference(tmp, comparative) < degree)
                        stack.Push(new Tuple<int, int>(x2, y2 + 1));
                }
            }
        }
    }
}