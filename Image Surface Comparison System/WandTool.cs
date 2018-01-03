using System;
using System.Collections.Generic;
using System.Windows;

namespace Image_Surface_Comparison_System
{
    public class WandTool
    {
        Photo patient;
        Photo orginal;
        int mode;
        bool[,] visitedPixels;
        int startX;
        int startY;
        private int index;
        bool adjacent;
        Color comparative;
        byte degree;

        public WandTool(Photo patient, Photo orginal, int mode, int startX, int startY, bool adjacent, Color comparative, byte degree)
        {
            this.patient = patient;
            this.orginal = orginal;
            this.mode = mode;
            visitedPixels = new bool[(int)patient.width, (int)patient.height];
            this.startX = startX;
            this.startY = startY;
            visitedPixels[startX, startY] = true;
            this.adjacent = adjacent;
            this.comparative = comparative;
            this.degree = degree;
        }

        public Photo Wand()
        {
            index = patient.GetIndex(startX, startY);
            if (mode == 0)
            {
                patient.pixelData = (uint[])orginal.pixelData.Clone();
                patient.selectedPixelsCount = 0;

                Array.Clear(patient.selectedPixels, 0, patient.selectedPixels.Length);
                Array.Clear(visitedPixels, 0, visitedPixels.Length);
            }
            Selected((ushort)startX, (ushort)startY);

            patient.photo.WritePixels(new Int32Rect(0, 0, (int)patient.width, (int)patient.height), patient.pixelData, patient.widthInByte, 0);
            return patient;
        }

        private void Selected(ushort x, ushort y)
        {
            Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>();
            stack.Push(new Tuple<int, int>(x, y));

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                int x2 = current.Item1;
                int y2 = current.Item2;

                index = orginal.GetIndex(x2, y2);
                if (mode != 2)
                {
                    if (patient.selectedPixels[x2, y2] == false)
                    {
                        patient.pixelData[index] = Color.ToUint(Base.selectedColor);
                        patient.selectedPixelsCount++;
                        patient.selectedPixels[x2, y2] = true;
                    }
                }
                else
                {
                    if (patient.selectedPixels[x2, y2] == true)
                    {
                        patient.pixelData[index] = orginal.pixelData[index];
                        patient.selectedPixelsCount--;
                        patient.selectedPixels[x2, y2] = false;
                    }
                }

                visitedPixels[x2, y2] = true;

                if (adjacent)
                {
                    comparative = orginal.GetColor(index);
                    index = orginal.GetIndex(x2, y2);
                }

                if (x2 - 1 >= 0 && visitedPixels[x2 - 1, y2] == false)
                {
                    index = orginal.GetIndex(x2 - 1, y2);
                    Color tmp = orginal.GetColor(index);
                    if (Color.Difference(tmp, comparative) < degree)
                        stack.Push(new Tuple<int, int>(x2 - 1, y2));
                }

                if (y2 - 1 >= 0 && visitedPixels[x2, y2 - 1] == false)
                {
                    index = orginal.GetIndex(x2, y2 - 1);
                    Color tmp = orginal.GetColor(index);
                    if (Color.Difference(tmp, comparative) < degree)
                        stack.Push(new Tuple<int, int>(x2, y2 - 1));
                }

                if (x2 + 1 < orginal.width && visitedPixels[x2 + 1, y2] == false)
                {
                    index = orginal.GetIndex(x2 + 1, y2);
                    Color tmp = orginal.GetColor(index);
                    if (Color.Difference(tmp, comparative) < degree)
                        stack.Push(new Tuple<int, int>(x2 + 1, y2));
                }

                if (y2 + 1 < orginal.height && visitedPixels[x2, y2 + 1] == false)
                {
                    index = orginal.GetIndex(x2, y2 + 1);
                    Color tmp = orginal.GetColor(index);
                    if (Color.Difference(tmp, comparative) < degree)
                        stack.Push(new Tuple<int, int>(x2, y2 + 1));
                }

                //if (x2 - 1 >= 0 && y2 - 1 >= 0 && visitedPixels[x2 - 1, y2 - 1] == false)
                //{
                //    index = orginal.GetIndex(x2 - 1, y2 - 1);
                //    Color tmp = orginal.GetColor(index);
                //    if (Color.Difference(tmp, comparative) < degree)
                //        stack.Push(new Tuple<int, int>(x2 - 1, y2 - 1));
                //}

                //if (x2 + 1 < orginal.width && y2 - 1 >= 0 && visitedPixels[x2 + 1, y2 - 1] == false)
                //{
                //    index = orginal.GetIndex(x2 + 1, y2 - 1);
                //    Color tmp = orginal.GetColor(index);
                //    if (Color.Difference(tmp, comparative) < degree)
                //        stack.Push(new Tuple<int, int>(x2 + 1, y2 - 1));
                //}

                //if (x2 - 1 >= 0 && y2 + 1 < orginal.height && visitedPixels[x2 - 1, y2 + 1] == false)
                //{
                //    index = orginal.GetIndex(x2 - 1, y2 + 1);
                //    Color tmp = orginal.GetColor(index);
                //    if (Color.Difference(tmp, comparative) < degree)
                //        stack.Push(new Tuple<int, int>(x2 - 1, y2 + 1));
                //}

                //if (x2 + 1 < orginal.width && y2 + 1 < orginal.height && visitedPixels[x2 + 1, y2 + 1] == false)
                //{
                //    index = orginal.GetIndex(x2 + 1, y2 + 1);
                //    Color tmp = orginal.GetColor(index);
                //    if (Color.Difference(tmp, comparative) < degree)
                //        stack.Push(new Tuple<int, int>(x2 + 1, y2 + 1));
                //}
            }
        }
    }
}