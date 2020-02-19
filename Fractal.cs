using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace MandelbrotSet
{
    struct Range
    {
        public double Min;
        public double Max;

        public Range(double min, double max)
        {
            this.Min = min;
            this.Max = max;
        }
    }

    struct Coord
    {
        public double X;
        public double Y;

        public Coord(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.X + b.X, a.Y + b.Y);
        }
    }

    class Fractal
    {
        #region members        
        private byte[] ColorMap;
        private int rawBitmapStride;

        private int tileWidth;
        private int tileHeight;
        public Range rx;
        public Range ry;
        private int iterations;
        #endregion

        public Fractal(int height, int width, Range xRange, Range yRange, int iterations = 100, int tileHeight = 100, int tileWidth = 100)
        {            
            rawBitmapStride = width * 3; //rgb            

            Height = height;
            Width = width;

            rx = xRange;
            ry = yRange;

            this.iterations = iterations;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
        }

        #region getters and setters

        public int Height { get; private set; }

        public int Width { get; private set; }

        public int Iterations
        {
            get { return iterations; }
        }

        public Color StartColor { get; set; } = Color.Yellow;
        public Color EndColor { get; set; } = Color.Purple;

        #endregion
        
        public void ProcessInParallel(bool notParallel = false)
        {            
            ColorMap = new byte[Height * rawBitmapStride];
            var taskList = new List<Task>();
            var colorList = ImageUtils.GenerateGradientColors(Iterations, StartColor, EndColor);

            for (int i = 0; i < Height; i += tileHeight)
            {
                for (int j = 0; j < Width; j += tileWidth)
                {
                    int tileh = Math.Min(tileHeight, Height - i);
                    int tilew = Math.Min(tileWidth, Width - j);

                    var xRange = new Range(NormalizeValue(j, 0, Height, rx.Min, rx.Max), NormalizeValue(j + tilew, 0, Height, rx.Min, rx.Max));
                    var yRange = new Range(NormalizeValue(i, 0, Width, ry.Max, ry.Min), NormalizeValue(i + tileh, 0, Width, ry.Max, ry.Min));

                    int ii = i;
                    int jj = j;

                    if (notParallel)
                    {
                        ProcessTile(tilew, tileh, xRange, yRange, Iterations, ii, jj, colorList);
                    }
                    else
                    {
                        taskList.Add(Task.Run(() => ProcessTile(tilew, tileh, xRange, yRange, Iterations, ii, jj, colorList)));
                    }
                }
            }

            Task.WaitAll(taskList.ToArray());
        }

        public void ProcessTile(int width, int height, Range rx, Range ry, int iterations, int ti, int tj, List<Color> colorList)
        {
            var tile = ComputeTile(width, height, rx, ry, Iterations);
            for (int i = 0; i < tile.GetLength(0); i++)
            {
                for (int j = 0; j < tile.GetLength(1); j++)
                {
                    var color = Color.Black;
                    //if(tile[i, j] > 7) //skip first iteratrions that take up most of the screen
                    if (tile[i, j] >= 0) //skip only not escaped values
                    {
                        color = colorList[tile[i, j]];
                    }

                    var imgY = ti + i;
                    var imgX = tj + j;

                    ColorMap[imgY * rawBitmapStride + imgX * 3 + 0] = color.R;
                    ColorMap[imgY * rawBitmapStride + imgX * 3 + 1] = color.G;
                    ColorMap[imgY * rawBitmapStride + imgX * 3 + 2] = color.B;
                }
            }
        }


        public static Coord ApplyFunction(Coord val)
        {
            return new Coord(Math.Pow(val.X, 2) - Math.Pow(val.Y, 2), 2 * (val.Y * val.X));
        }

        public static int[,] ComputeTile(int width, int height, Range rx, Range ry, int iterations)
        {
            var iterMap = new int[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    iterMap[i, j] = -1;
                    var c = new Coord(NormalizeValue(j, 0, width, rx.Min, rx.Max), NormalizeValue(i, 0, height, ry.Min, ry.Max));
                    var current = c; //deep copy    

                    for (int it = 0; it < iterations; it++)
                    {
                        current = ApplyFunction(current) + c;

                        if(Math.Abs(current.X) > 2 || Math.Abs(current.Y) > 2)
                        {
                            iterMap[i, j] = it;
                            break;
                        }
                    }
                }
            }

            return iterMap;
        }

        public static double NormalizeValue(double x, double minx, double maxx, double a, double b)
        {
            return (b - a) * ((x - minx) / (maxx - minx)) + a;
        }
        
        public System.Windows.Media.Imaging.BitmapSource GetBitmapSrc()
        {
            var bitmap = System.Windows.Media.Imaging.BitmapSource.Create(Width, Height,
                96, 96, System.Windows.Media.PixelFormats.Rgb24, null,
                ColorMap, rawBitmapStride);

            return bitmap;
        }
    }
}
