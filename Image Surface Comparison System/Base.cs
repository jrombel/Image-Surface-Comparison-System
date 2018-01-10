using System;
using System.Collections.Generic;
using System.IO;

namespace Image_Surface_Comparison_System
{
    public class Base
    {
        static public List<String> albums { set; get; }
        static public List<String> photos;
        static public String path;

        static public Photo photo;
        static public Photo photoOrginal;
        static public int selectedTool;
        static public int selectedMode;

        static public Color selectedColor = new Color(255, 255, 255);

        static public string lastFilename = "";

        public static void Save()
        {
            if (lastFilename != "")
            {
                string[] words = lastFilename.Split('\\');
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
                    sw.WriteLine(words[1]);
                    sw.WriteLine(length);
                    sw.WriteLine(photo.selectedPixelsCount);
                    sw.WriteLine(photo.pixelData.Length);

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
}