using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MandelbrotSet
{
    class Fractal
    {
        private double[,] GeneratedValues;
        private int[,] IterationPassed;

        private double[] xRange;
        private double[] yRange;

        public Fractal(int height, int width, double xL = -1, double xR = 1, double yD = -1, double yU = -1)
        {
            GeneratedValues = new double[height, width];
            IterationPassed = new int[height, width];

            xRange = new double[] { xL, xR };
            yRange = new double[] { yD, yU };

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

        public void InitializeMatrix()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    GeneratedValues[i, j] = NormalizeValue(i, 0, Height, yRange[0], yRange[1]) + NormalizeValue(j, 0, Width, xRange[0], xRange[1]);
                    IterationPassed[i, j] = 0;
                }
            }
        }

        public void Process()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    IterationPassed[i, j] = (int)Math.Floor(Math.Abs(GeneratedValues[i, j] * 100));
                }
            }
        }

        public static double NormalizeValue(double x, double minx, double maxx, double a, double b)
        {
            return (b - a) * ((x - minx) / (maxx - minx)) + a;
        }
        
        public int[,,] GetColorsArray()
        {
            var arr = new int[Height, Width, 3];

            int n = IterationPassed.Cast<int>().ToList().Distinct().Count();
            var colors = ImageUtils.GenerateGradientColors(n, Color.White, Color.Blue);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var col = colors[IterationPassed[i, j]];
                    arr[i, j, 0] = col.R;
                    arr[i, j, 1] = col.G;
                    arr[i, j, 2] = col.B;
                }
            }

            return arr;
        }
    }
}
