using System.Windows.Media.Imaging;

namespace Image_Surface_Comparison_System
{
    public class Photo
    {
        public WriteableBitmap photo;
        public double height;
        public double width;
        public uint[] pixelData;
        public int widthInByte;
        public int selectedPixels;
        public Photo(BitmapSource source)
        {
            photo = new WriteableBitmap(source);
            height = photo.PixelHeight;
            width = photo.PixelWidth;

            pixelData = new uint[(int)(width * height)];
            widthInByte = (int)(4 * width);

            photo.CopyPixels(pixelData, widthInByte, 0);

            selectedPixels = 0;
        }

        public int GetIndex(double x, double y)
        {
            return (int)(y * width + x);
        }

        public Color GetColor(int index)
        {
            return new Color((byte)((pixelData[index] & 0xff0000) >> 0x10), (byte)((pixelData[index] & 0xff00) >> 8), (byte)(pixelData[index] & 0xff));
        }
    }
}