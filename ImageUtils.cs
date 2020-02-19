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
using System.Diagnostics;

namespace MandelbrotSet
{
    class ImageUtils
    {
        public static int[,,] GerenateColorsArray(int width, int height)
        {
            var arr = new int[height, width, 3];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    arr[i, j, 0] = 255;
                    arr[i, j, 1] = 0;
                    arr[i, j, 2] = 0;

                    if (i == j)
                    {
                        arr[i, j, 0] = 0;
                    }
                }
            }

            return arr;
        }

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

        public static Bitmap CreateBitmap(Color[,] colorArray)
        {
            var height = colorArray.GetLength(0);
            var width = colorArray.GetLength(1);            

            Bitmap bitmap = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bitmap.SetPixel(i, j, colorArray[j, i]);
                }
            }

            return bitmap;
        }

        public static Bitmap CreateBitmap(int[,,] values)
        {
            var height = values.GetLength(0);
            var width = values.GetLength(1);

            Bitmap bitmap = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var color = System.Drawing.Color.FromArgb(values[j, i, 0], values[j, i, 1], values[j, i, 2]);
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
            //bitmap.Save(filePath, ImageFormat.Jpeg);
            bitmap.Save(filePath, ImageFormat.Png);
            Console.WriteLine("Image saved.");
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

        public static Color GetGradientColors(int coloIndex, int gradSize, Color initColor, Color endColor)
        {
            var rAverage = initColor.R + (int)((endColor.R - initColor.R) * coloIndex / gradSize);
            var gAverage = initColor.G + (int)((endColor.G - initColor.G) * coloIndex / gradSize);
            var bAverage = initColor.B + (int)((endColor.B - initColor.B) * coloIndex / gradSize);

            return Color.FromArgb(rAverage, gAverage, bAverage);
        }

        public static System.Windows.Media.GradientStopCollection GetRainboxGradient(int iterations)
        {
            var colors = new List<Color>();
            
            colors.Add(Color.Purple);
            colors.Add(Color.Blue);
            colors.Add(Color.Teal);
            colors.Add(Color.Green);
            colors.Add(Color.Yellow);
            colors.Add(Color.Orange);
            colors.Add(Color.Red);


            var fullGrad = new List<Color>();

            int repeat = (int)((double)iterations / (double)colors.Count);
            for (int i=0;i< repeat; i++)
            {
                fullGrad.AddRange(colors);
            }

            var grsc = new System.Windows.Media.GradientStopCollection(fullGrad.Count);

            double offset = 0.0;
            foreach(var color in fullGrad)
            {
                offset += Math.Min(1.0 / (double)fullGrad.Count, 1.0);
                grsc.Add(new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B), offset));                
            }

            return grsc;
        }

        public static System.Windows.Media.GradientStopCollection GetTwoColorGradient()
        {
            var grsc = new System.Windows.Media.GradientStopCollection(2);

            grsc.Add(new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromRgb(Color.Purple.R, Color.Purple.G, Color.Purple.B), 0));
            grsc.Add(new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromRgb(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B), 1.0));

            return grsc;
        }

        public static Color GetColorByOffset(System.Windows.Media.GradientStopCollection collection, double offset)
        {
            System.Windows.Media.GradientStop[] stops = collection.OrderBy(x => x.Offset).ToArray();
            if (offset <= 0)
            {
                return Color.FromArgb(stops[0].Color.A, stops[0].Color.R, stops[0].Color.G, stops[0].Color.B);
            }
            if (offset >= 1)
            {
                var i = stops.Length - 1;
                return Color.FromArgb(stops[i].Color.A, stops[i].Color.R, stops[i].Color.G, stops[i].Color.B);
            }
            System.Windows.Media.GradientStop left = stops[0], right = null;
            foreach (System.Windows.Media.GradientStop stop in stops)
            {
                if (stop.Offset >= offset)
                {
                    right = stop;
                    break;
                }
                left = stop;
            }
            Debug.Assert(right != null);
            offset = Math.Round((offset - left.Offset) / (right.Offset - left.Offset), 2);

            byte a = (byte)((right.Color.A - left.Color.A) * offset + left.Color.A);
            byte r = (byte)((right.Color.R - left.Color.R) * offset + left.Color.R);
            byte g = (byte)((right.Color.G - left.Color.G) * offset + left.Color.G);
            byte b = (byte)((right.Color.B - left.Color.B) * offset + left.Color.B);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
