using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
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

        public static System.Windows.Media.ImageSource ImageSourceFromBitmap(Bitmap bmp)
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

        public static List<Color> GenerateGradientColors(int n, Color initColor, Color endColor)
        {
            int rMax = initColor.R;
            int rMin = endColor.R;

            int gMax = initColor.G;
            int gMin = endColor.G;

            int bMax = initColor.B;
            int bMin = endColor.B;

            var colorList = new List<Color>();
            for (int i = 0; i < n; i++)
            {
                var rAverage = rMin + (int)((rMax - rMin) * i / n);
                var gAverage = gMin + (int)((gMax - gMin) * i / n);
                var bAverage = bMin + (int)((bMax - bMin) * i / n);
                colorList.Add(Color.FromArgb(rAverage, gAverage, bAverage));
            }

            return colorList;
        }

    }
}
