using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Image_Surface_Comparison_System
{
    public class PhotoProcessing
    {
        public static Photo Smoothing(Photo photo)
        {
            Photo patient = new Photo();
            patient.Clone(photo);

            byte alpha = 0;
            uint red, green, blue;
            int index = 0, tmp = 0;

            for (int x = 1; x < patient.width - 1; x++)
            {
                for (int y = 1; y < patient.height - 1; y++)
                {
                    index = patient.GetIndex(x, y);

                    red = 0;
                    green = 0;
                    blue = 0;

                    tmp = patient.GetIndex(x - 1, y - 1);
                    red += (byte)((photo.pixelData[tmp] & 0xff0000) >> 16);
                    green += (byte)((photo.pixelData[tmp] & 0xff00) >> 8);
                    blue += (byte)((photo.pixelData[tmp] & 0xff));

                    tmp = patient.GetIndex(x, y - 1);
                    red += (byte)((photo.pixelData[tmp] & 0xff0000) >> 16);
                    green += (byte)((photo.pixelData[tmp] & 0xff00) >> 8);
                    blue += (byte)((photo.pixelData[tmp] & 0xff));

                    tmp = patient.GetIndex(x + 1, y - 1);
                    red += (byte)((photo.pixelData[tmp] & 0xff0000) >> 16);
                    green += (byte)((photo.pixelData[tmp] & 0xff00) >> 8);
                    blue += (byte)((photo.pixelData[tmp] & 0xff));

                    tmp = patient.GetIndex(x - 1, y);
                    red += (byte)((photo.pixelData[tmp] & 0xff0000) >> 16);
                    green += (byte)((photo.pixelData[tmp] & 0xff00) >> 8);
                    blue += (byte)((photo.pixelData[tmp] & 0xff));

                    tmp = index;
                    red += (byte)((photo.pixelData[tmp] & 0xff0000) >> 16);
                    green += (byte)((photo.pixelData[tmp] & 0xff00) >> 8);
                    blue += (byte)((photo.pixelData[tmp] & 0xff));

                    tmp = patient.GetIndex(x + 1, y);
                    red += (byte)((photo.pixelData[tmp] & 0xff0000) >> 16);
                    green += (byte)((photo.pixelData[tmp] & 0xff00) >> 8);
                    blue += (byte)((photo.pixelData[tmp] & 0xff));

                    tmp = patient.GetIndex(x - 1, y + 1);
                    red += (byte)((photo.pixelData[tmp] & 0xff0000) >> 16);
                    green += (byte)((photo.pixelData[tmp] & 0xff00) >> 8);
                    blue += (byte)((photo.pixelData[tmp] & 0xff));

                    tmp = patient.GetIndex(x, y + 1);
                    red += (byte)((photo.pixelData[tmp] & 0xff0000) >> 16);
                    green += (byte)((photo.pixelData[tmp] & 0xff00) >> 8);
                    blue += (byte)((photo.pixelData[tmp] & 0xff));

                    tmp = patient.GetIndex(x + 1, y + 1);
                    red += (byte)((photo.pixelData[tmp] & 0xff0000) >> 16);
                    green += (byte)((photo.pixelData[tmp] & 0xff00) >> 8);
                    blue += (byte)((photo.pixelData[tmp] & 0xff));

                    patient.pixelData[index] = (uint)((alpha << 24) | ((byte)(red / 9) << 16) | ((byte)(green / 9) << 8) | ((byte)(blue / 9) << 0));

                }
            }
            patient.photo.WritePixels(new Int32Rect(0, 0, photo.width, photo.height), photo.pixelData, photo.widthInByte, 0);
            return patient;
        }

        public static Photo Median(Photo photo)
        {
            Photo patient = new Photo();
            patient.Clone(photo);

            byte alpha = 0;
            int index = 0;

            for (int x = 1; x < photo.width - 1; x++)
            {
                for (int y = 1; y < photo.height - 1; y++)
                {
                    index = photo.GetIndex(x, y);

                    Dictionary<int, int> values = new Dictionary<int, int>();
                    values.Add(0, 0);
                    values.Add(1, 0);
                    values.Add(2, 0);
                    values.Add(3, 0);
                    values.Add(4, 0);
                    values.Add(5, 0);
                    values.Add(6, 0);
                    values.Add(7, 0);
                    values.Add(8, 0);

                    int[] tmp = new int[9];
                    tmp[0] = photo.GetIndex(x - 1, y - 1);

                    values[0] += (byte)((photo.pixelData[tmp[0]] & 0xff0000) >> 16);
                    values[0] += (byte)((photo.pixelData[tmp[0]] & 0xff00) >> 8);
                    values[0] += (byte)((photo.pixelData[tmp[0]] & 0xff));

                    tmp[1] = photo.GetIndex(x, y - 1);
                    values[1] += (byte)((photo.pixelData[tmp[1]] & 0xff0000) >> 16);
                    values[1] += (byte)((photo.pixelData[tmp[1]] & 0xff00) >> 8);
                    values[1] += (byte)((photo.pixelData[tmp[1]] & 0xff));

                    tmp[2] = photo.GetIndex(x + 1, y - 1);
                    values[2] += (byte)((photo.pixelData[tmp[2]] & 0xff0000) >> 16);
                    values[2] += (byte)((photo.pixelData[tmp[2]] & 0xff00) >> 8);
                    values[2] += (byte)((photo.pixelData[tmp[2]] & 0xff));

                    tmp[3] = photo.GetIndex(x - 1, y);
                    values[3] += (byte)((photo.pixelData[tmp[3]] & 0xff0000) >> 16);
                    values[3] += (byte)((photo.pixelData[tmp[3]] & 0xff00) >> 8);
                    values[3] += (byte)((photo.pixelData[tmp[3]] & 0xff));

                    tmp[4] = index;
                    values[4] += (byte)((photo.pixelData[tmp[4]] & 0xff0000) >> 16);
                    values[4] += (byte)((photo.pixelData[tmp[4]] & 0xff00) >> 8);
                    values[4] += (byte)((photo.pixelData[tmp[4]] & 0xff));

                    tmp[5] = photo.GetIndex(x + 1, y);
                    values[5] += (byte)((photo.pixelData[tmp[5]] & 0xff0000) >> 16);
                    values[5] += (byte)((photo.pixelData[tmp[5]] & 0xff00) >> 8);
                    values[5] += (byte)((photo.pixelData[tmp[5]] & 0xff));

                    tmp[6] = photo.GetIndex(x - 1, y + 1);
                    values[6] += (byte)((photo.pixelData[tmp[6]] & 0xff0000) >> 16);
                    values[6] += (byte)((photo.pixelData[tmp[6]] & 0xff00) >> 8);
                    values[6] += (byte)((photo.pixelData[tmp[6]] & 0xff));

                    tmp[7] = photo.GetIndex(x, y + 1);
                    values[7] += (byte)((photo.pixelData[tmp[7]] & 0xff0000) >> 16);
                    values[7] += (byte)((photo.pixelData[tmp[7]] & 0xff00) >> 8);
                    values[7] += (byte)((photo.pixelData[tmp[7]] & 0xff));

                    tmp[8] = photo.GetIndex(x + 1, y + 1);
                    values[8] += (byte)((photo.pixelData[tmp[8]] & 0xff0000) >> 16);
                    values[8] += (byte)((photo.pixelData[tmp[8]] & 0xff00) >> 8);
                    values[8] += (byte)((photo.pixelData[tmp[8]] & 0xff));

                    var tmp2 = values.OrderBy(i => i.Value);
                    int tmp3 = tmp2.ElementAt(4).Key;

                    patient.pixelData[index] = (uint)((alpha << 24) | (photo.pixelData[tmp[tmp3]] << 16) | (photo.pixelData[tmp[tmp3]] << 8) | (photo.pixelData[tmp[tmp3]] << 0));
                }
            }

            patient.photo.WritePixels(new Int32Rect(0, 0, photo.width, photo.height), photo.pixelData, photo.widthInByte, 0);

            return patient;
        }

        public static Photo EdgeDetect(Photo photo)
        {
            Photo patient = new Photo();
            patient.Clone(photo);

            int[][] Gx = new int[][] {
                new int[] {1,2,1},
                new int[] {0,0,0},
                new int[] {-1,-2,-1},
            };
            int[][] Gy = new int[][] {
                new int[] {1,0,-1},
                new int[] {2,0,-2},
                new int[] {1,0,-1},
            };

            byte alpha = 0;

            for (int i = 1; i < photo.width - 1; i++)
            {
                for (int j = 1; j < photo.height - 1; j++)
                {
                    double[] new_x = new double[3];
                    double[] new_y = new double[3];

                    double r, g, b;
                    for (int hw = -1; hw < 2; hw++)
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            int tmp = Gx[hw + 1][wi + 1];
                            r = ((photo.pixelData[photo.GetIndex(i + hw, j + wi)] & 0xff0000) >> 16);
                            new_x[0] += tmp * r;
                            new_y[0] += tmp * r;

                            g = ((photo.pixelData[photo.GetIndex(i + hw, j + wi)] & 0xff00) >> 8);
                            new_x[1] += tmp * g;
                            new_y[1] += tmp * g;

                            b = ((photo.pixelData[photo.GetIndex(i + hw, j + wi)] & 0xff));
                            new_x[2] += tmp * b;
                            new_y[2] += tmp * b;
                        }
                    }
                    byte red = (Convert.ToByte(Convert.ToInt32(Math.Sqrt(new_x[0] * new_x[0] + new_y[0] * new_y[0])) % 255));
                    byte green = (Convert.ToByte(Convert.ToInt32(Math.Sqrt(new_x[1] * new_x[1] + new_y[1] * new_y[1])) % 255));
                    byte blue = (Convert.ToByte(Convert.ToInt32(Math.Sqrt(new_x[2] * new_x[2] + new_y[2] * new_y[2])) % 255));
                    patient.pixelData[photo.GetIndex(i, j)] = (uint)((alpha << 24) | (blue << 16) | (blue << 8) | (blue << 0));
                }
            }

            patient.photo.WritePixels(new Int32Rect(0, 0, photo.width, photo.height), photo.pixelData, photo.widthInByte, 0);

            return patient;
        }

        public static Photo HighPassSharpening(Photo photo)
        {
            Photo patient = new Photo();
            patient.Clone(photo);

            int[][] Gx = new int[][] {
                new int[] {-1,-1,-1},
                new int[] {-1,9,-1},
                new int[] {-1,-1,-1},
            };

            byte alpha = 0;

            for (int i = 1; i < photo.width - 1; i++)
            {
                for (int j = 1; j < photo.height - 1; j++)
                {
                    double[] new_x = new double[3];

                    double r = 0, g = 0, b = 0;
                    double sw = 0;
                    for (int hw = -1; hw < 2; hw++)
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            int tmp = Gx[hw + 1][wi + 1];
                            double tmp2;
                            tmp2 = ((photo.pixelData[photo.GetIndex(i + hw, j + wi)] & 0xff0000) >> 16);
                            sw += tmp;
                            r += tmp * tmp2;

                            tmp2 = ((photo.pixelData[photo.GetIndex(i + hw, j + wi)] & 0xff00) >> 8);
                            g += tmp * tmp2;

                            tmp2 = ((photo.pixelData[photo.GetIndex(i + hw, j + wi)] & 0xff));
                            b += tmp * tmp2;
                        }
                    }
                    if (sw == 0)
                        sw = 1;
                    if (r < 0)
                        r = 0;
                    if (g < 0)
                        g = 0;
                    if (b < 0)
                        b = 0;

                    byte red = (Convert.ToByte(Convert.ToInt32(r / sw) % 255));
                    byte green = (Convert.ToByte(Convert.ToInt32(g / sw) % 255));
                    byte blue = (Convert.ToByte(Convert.ToInt32(b / sw) % 255));
                    patient.pixelData[photo.GetIndex(i, j)] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                }
            }

            patient.photo.WritePixels(new Int32Rect(0, 0, photo.width, photo.height), photo.pixelData, photo.widthInByte, 0);

            return patient;
        }

        public static Photo GaussianBlur(Photo photo)
        {
            Photo patient = new Photo();
            patient.Clone(photo);

            int[][] Gx = new int[][] {
                new int[] {1,2,1},
                new int[] {2,4,2},
                new int[] {1,2,1},
            };

            byte alpha = 0;

            for (int i = 1; i < photo.width - 1; i++)
            {
                for (int j = 1; j < photo.height - 1; j++)
                {
                    double[] new_x = new double[3];

                    double r = 0, g = 0, b = 0;
                    double sw = 0;
                    for (int hw = -1; hw < 2; hw++)
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            int tmp = Gx[hw + 1][wi + 1];
                            double tmp2;
                            tmp2 = ((photo.pixelData[photo.GetIndex(i + hw, j + wi)] & 0xff0000) >> 16);
                            sw += tmp;
                            r += tmp * tmp2;

                            tmp2 = ((photo.pixelData[photo.GetIndex(i + hw, j + wi)] & 0xff00) >> 8);
                            g += tmp * tmp2;

                            tmp2 = ((photo.pixelData[photo.GetIndex(i + hw, j + wi)] & 0xff));
                            b += tmp * tmp2;
                        }
                    }

                    if (sw == 0)
                        sw = 1;

                    byte red = (Convert.ToByte(Convert.ToInt32(r / sw) % 255));
                    byte green = (Convert.ToByte(Convert.ToInt32(g / sw) % 255));
                    byte blue = (Convert.ToByte(Convert.ToInt32(b / sw) % 255));
                    patient.pixelData[photo.GetIndex(i, j)] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                }
            }

            patient.photo.WritePixels(new Int32Rect(0, 0, photo.width, photo.height), photo.pixelData, photo.widthInByte, 0);

            return patient;
        }


        public static Photo Binaryzation(Photo photo, int value)
        {
            Photo patient = new Photo();
            patient.Clone(photo);

            if (photo != null)
            {

                if (value >= 0 && value <= 255)
                {
                    for (int i = 0; i < photo.pixelData.Length; i++)
                    {
                        byte red = (byte)((photo.pixelData[i] & 0xff0000) >> 16);
                        byte green = (byte)((photo.pixelData[i] & 0xff00) >> 8);
                        byte blue = (byte)((photo.pixelData[i] & 0xff));
                        byte alpha = 0;

                        if ((int)(0.114 * blue + 0.587 * green + 0.299 * red) <= value)
                        {
                            red = 0;
                            green = 0;
                            blue = 0;
                        }
                        else
                        {
                            red = 255;
                            green = 255;
                            blue = 255;
                        }

                        patient.pixelData[i] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                    }

                    patient.photo.WritePixels(new Int32Rect(0, 0, photo.width, photo.height), photo.pixelData, photo.widthInByte, 0);
                }
            }

            return patient;
        }
    }
}
