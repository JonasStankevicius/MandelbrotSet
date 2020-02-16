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
        private Bitmap ColorMap;
        private Color[,] Colors;

        private int tileWidth;
        private int tileHeight;
        private Range rx;
        private Range ry;
        private int iterations;
        #endregion

        public Fractal(int height, int width, Range xRange, Range yRange, int iterations = 100, int tileHeight = 1000, int tileWidth = 1000)
        {
            ColorMap = new Bitmap(width, height);
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

        public void Process()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"Computing fractal ({Height}x{Width})");

            var tiles = GenerateTiles();

            Console.WriteLine($"{tiles.Count} tiles ({tileHeight}x{tileWidth})");

            sw.Stop();
            Console.WriteLine($"Elapsed = {sw.Elapsed}");
            sw.Start();

            //var gradColor = ImageUtils.GetTwoColorGradient();
            var colorList = ImageUtils.GenerateGradientColors(Iterations, StartColor, EndColor);
            foreach (var (coord, tile) in tiles)
            {
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

                        var imgY = coord.Item1 + i;
                        var imgX = coord.Item2 + j;

                        ColorMap.SetPixel(imgX, imgY, color);
                    }
                }
            }

            sw.Stop();
            Console.WriteLine($"Bitmap creation took = {sw.Elapsed}");
        }

        public List<Tuple<Tuple<int, int>, int[,]>> GenerateTiles()
        {
            var tiles = new List<Tuple<Tuple<int, int>, int[,]>>();

            for (int i = 0; i < Height; i += tileHeight)
            {
                for (int j = 0; j < Width; j += tileWidth)
                {
                    int tileh = Math.Min(tileHeight, Height - i);
                    int tilew = Math.Min(tileWidth, Width - j);

                    var xRange = new Range(NormalizeValue(j, 0, Height, rx.Min, rx.Max), NormalizeValue(j + tilew, 0, Height, rx.Min, rx.Max));
                    var yRange = new Range(NormalizeValue(i, 0, Width, ry.Max, ry.Min), NormalizeValue(i + tileh, 0, Width, ry.Max, ry.Min));

                    var tile = ComputeTile(tilew, tileh, xRange, yRange, Iterations);

                    tiles.Add(Tuple.Create(Tuple.Create(i, j), tile));
                }
            }

            return tiles;
        }

        public void ProcessInParallel()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"Computing fractal ({Height}x{Width})");

            Colors = new Color[Width, Height];
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

                    taskList.Add(Task.Run(() => ProcessTile(tilew, tileh, xRange, yRange, Iterations, ii, jj, colorList)));
                }
            }

            Task.WaitAll(taskList.ToArray());

            sw.Stop();
            Console.WriteLine($"Elapsed = {sw.Elapsed}");
            sw.Start();

            for (int i = 0; i < Height; i ++)
            {
                for (int j = 0; j < Width; j ++)
                {
                    ColorMap.SetPixel(i, j, Colors[i, j]);
                }
            }

            sw.Stop();
            Console.WriteLine($"Bitmap creation took = {sw.Elapsed}");
        }

        public void ProcessTile(int width, int height, Range rx, Range ry, int iterations, int ti, int tj, List<Color> colorList)
        {
            //Console.WriteLine($"Tile index ({ti},{tj})");

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

                    //ColorMap.SetPixel(imgX, imgY, color);
                    Colors[imgX, imgY] = color;
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
        
        public System.Windows.Media.Imaging.WriteableBitmap GetBitMap()
        {
            return ColorMap;
        }
    }
}
