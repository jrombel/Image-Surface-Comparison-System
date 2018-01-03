using System.Windows.Media.Imaging;

namespace Image_Surface_Comparison_System
{
    public class Photo
    {
        public WriteableBitmap photo;
        public int height;
        public int width;
        public uint[] pixelData;
        public int widthInByte;
        public bool[,] selectedPixels;
        public int selectedPixelsCount;

        public Photo(BitmapSource source)
        {
            photo = new WriteableBitmap(source);
            height = photo.PixelHeight;
            width = photo.PixelWidth;

            pixelData = new uint[width * height];
            widthInByte = 4 * width;

            photo.CopyPixels(pixelData, widthInByte, 0);

            selectedPixels = new bool[width, height];
            selectedPixelsCount = 0;
        }

        public int GetIndex(int x, int y)
        {
            return y * width + x;
        }

        public Color GetColor(int index)
        {
            return new Color((byte)((pixelData[index] & 0xff0000) >> 0x10), (byte)((pixelData[index] & 0xff00) >> 8), (byte)(pixelData[index] & 0xff));
        }
    }
}