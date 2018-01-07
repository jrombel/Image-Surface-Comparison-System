using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Image_Surface_Comparison_System
{
    public class Base
    {
        static public List<String> albums { set; get; }
        static public List<String> photos;
        static public String path;

        static public Color selectedColor = new Color(255, 255, 255);

        public static void Save(Photo photo, string shortPath)
        {
            string[] words = shortPath.Split('\\');
            string folder = words[0];
            string file = (words[1].Split('.'))[0] + ".txt";
            string fullPath = Base.path + "\\Data\\" + folder + "\\" + file;

            if (!Directory.Exists(Base.path + "\\Data\\" + folder))
                Directory.CreateDirectory(Base.path + "\\Data\\" + folder);

            long length = 0;
            if (File.Exists(fullPath))
            {
                length = (new FileInfo(fullPath)).Length;
            }
            using (StreamWriter sw = new StreamWriter(fullPath))
            {
                sw.WriteLine(file);
                sw.WriteLine(length);
                sw.WriteLine(photo.selectedPixelsCount);

                for (int y = 0; y < photo.height; y++)
                {
                    for (int x = 0; x < photo.width; x++)
                    {
                        if (photo.selectedPixels[x, y] == false)
                            sw.Write("0 ");
                        else
                            sw.Write("1 ");
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}

