using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace MandelbrotSet
{
    class ImageUtils
    {
        public static Bitmap CreateBitmap(int width, int height, int[] rgb)
        {
            Bitmap bitmap = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var color = System.Drawing.Color.FromArgb(rgb[0], rgb[1], rgb[2]);
                    bitmap.SetPixel(i, j, color);
                }
            }

            return bitmap;
        }

        public static Bitmap CreateBitmap(int[,,] values)
        {
            var height = values.GetLength(0);
            var width = values.GetLength(1);            

            Bitmap bitmap = new Bitmap(width, height);

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    var color = System.Drawing.Color.FromArgb(values[j,i,0], values[j,i,1], values[j,i,2]);
                    bitmap.SetPixel(i, j, color);
                }
            }

            return bitmap;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public static void SaveImage(Bitmap bitmap, string filePath = "test.png")
        {
            bitmap.Save(filePath, ImageFormat.Png);
        }
    }
}
