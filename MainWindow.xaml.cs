using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MandelbrotSet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Range xRange;
        private Range yRange;
        private double scroolStep = 0.05;

        public MainWindow()
        {
            InitializeComponent();

            //ExperimentWithBitmap();

            ImageControl.MouseWheel += OnMouseScrool;
            xRange = new Range(-2, 2);
            yRange = new Range(-2, 2);
            DrawFractal();
        }

        public void ExperimentWithBitmap()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int width = (int)ImageControl.Width;
            int height = (int)ImageControl.Height;

            int rawStride = width * 3; //rgb (8+8+8) = 3 bytes
            byte[] rawImage = new byte[rawStride * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {                    
                    rawImage[i * rawStride + j * 3 + 0] = (byte)255;
                    rawImage[i * rawStride + j * 3 + 1] = (byte)0;
                    rawImage[i * rawStride + j * 3 + 2] = (byte)0;

                    if (i == j)
                    {
                        rawImage[i * rawStride + j * 3 + 0] = (byte)0;
                    }
                }
            }

            // Create a BitmapSource.
            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, PixelFormats.Rgb24, null,
                rawImage, rawStride);

            sw.Stop();
            Console.WriteLine($"Elapsed = {sw.Elapsed}");

            ImageControl.Source = bitmap;
        }

        public void DrawFractal()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var fractal = new Fractal((int)ImageControl.Height, (int)ImageControl.Width, xRange, yRange, iterations: 50, tileHeight: 50, tileWidth: 50);
            fractal.ProcessInParallel();

            //ImageUtils.SaveImage(bitmap);
            ImageControl.Source = fractal.GetBitmapSrc();

            sw.Stop();
            Console.WriteLine($"Elapsed = {sw.Elapsed}");
        }

        public void OnMouseScrool(object sender, MouseWheelEventArgs e)
        {
            double topLeftFactor = 0;
            double widthFactor = 0;
            if (e.Delta > 0)
            {
                topLeftFactor = scroolStep;
                widthFactor = 1 - scroolStep;
            }
            else
            {
                topLeftFactor = -1 * scroolStep;
                widthFactor = 1 + scroolStep;
            }

            var pos = e.GetPosition(this.ImageControl);
            var mouseX = Fractal.NormalizeValue(pos.X, 0, ImageControl.Width, -2, 2);
            var mouseY = Fractal.NormalizeValue(pos.Y, 0, ImageControl.Height, 2, -2);

            var xRangeMin = xRange.Min + (mouseX - xRange.Min) * topLeftFactor;
            var yRangeMax = yRange.Max + (mouseY - yRange.Max) * topLeftFactor;

            var xRangeMax = xRangeMin + xRange.Width * widthFactor;
            var yRangeMin = yRangeMax - yRange.Width * widthFactor;

            xRange = new Range(xRangeMin, xRangeMax);
            yRange = new Range(yRangeMin, yRangeMax);

            DrawFractal();
        }
    }
}