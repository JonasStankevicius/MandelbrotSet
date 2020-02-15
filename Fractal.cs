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
    class Fractal
    {
        private double[,,] GeneratedValues;
        private double[,,] ConstantsMap;
        private bool[,] StopTrack;
        private Color[,] ColorMap;

        private double[] xRange;
        private double[] yRange;
        private int iterations;

        public Fractal(int height, int width, double xL = -2, double xR = 2, double yD = -2, double yU = 2, int iterations = 50)
        {
            GeneratedValues = new double[height, width, 2];
            ConstantsMap = new double[height, width, 2];
            StopTrack = new bool[height, width];
            ColorMap = new Color[height, width];

            xRange = new double[] { xL, xR };
            yRange = new double[] { yD, yU };

            this.iterations = iterations;

            InitializeMatrix();
        }

        public int Height
        {
            get { return GeneratedValues.GetLength(0); }
        }

        public int Width
        {
            get { return GeneratedValues.GetLength(1); }
        }

        public int Iterations
        {
            get { return iterations; }
        }

        public Color StartColor { get; set; } = Color.Red;
        public Color EndColor { get; set; } = Color.Blue;

        public void InitializeMatrix()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    ConstantsMap[i, j, 0] = NormalizeValue(i, 0, Height, yRange[0], yRange[1]); //y
                    ConstantsMap[i, j, 1] = NormalizeValue(j, 0, Width, xRange[0], xRange[1]); //x

                    GeneratedValues[i, j, 0] = 0.0;
                    GeneratedValues[i, j, 1] = 0.0;

                    ColorMap[i, j] = Color.Black;

                    StopTrack[i, j] = false;
                }
            }
        }

        public void Process()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var gradColor = ImageUtils.GetRainboxGradient(Iterations);

            Console.WriteLine($"Process fractal ({Height}x{Width})");

            for (int i = 0; i < Iterations; i++)
            {
                Console.WriteLine($"Iteration: {i}/{Iterations}");
                ApplyFunction(i, gradColor);
            }

            sw.Stop();
            Console.WriteLine($"Elapsed={sw.Elapsed}");
        }

        public void ApplyFunction(int iteration, System.Windows.Media.GradientStopCollection gradientStops)
        {
            var color = ImageUtils.GetColorByOffset(gradientStops, NormalizeValue(iteration, 0, Iterations, 0, 1));

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if(StopTrack[i, j])
                    {
                        continue;
                    }

                    var y = GeneratedValues[i, j, 0];
                    var x = GeneratedValues[i, j, 1];                    

                    GeneratedValues[i, j, 0] = ConstantsMap[i, j, 0] + 2*(y * x); //y
                    GeneratedValues[i, j, 1] = ConstantsMap[i, j, 1] + (Math.Pow(x, 2) - Math.Pow(y, 2)); //x

                    if (Math.Abs(GeneratedValues[i, j, 0]) > 2 || Math.Abs(GeneratedValues[i, j, 1]) > 2)
                    {
                        StopTrack[i, j] = true;
                        ColorMap[i, j] = color;
                    }
                }
            }
        }

        public static double NormalizeValue(double x, double minx, double maxx, double a, double b)
        {
            return (b - a) * ((x - minx) / (maxx - minx)) + a;
        }
        
        public Color[,] GetColorsArray()
        {
            return ColorMap;
        }
    }
}
