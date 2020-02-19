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
        private Rect curRect;

        public MainWindow()
        {
            InitializeComponent();

            //ExperimentWithBitmap();

            ImageControl.MouseWheel += OnMouseScrool;
            curRect = new Rect(new System.Windows.Point(0, 0), new System.Windows.Point(ImageControl.Width, ImageControl.Height));
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

            var xRange = new Range(Fractal.NormalizeValue(curRect.Left, 0, ImageControl.Width, -2, 2), Fractal.NormalizeValue(curRect.Right, 0, ImageControl.Width, -2, 2));
            var yRange = new Range(Fractal.NormalizeValue(curRect.Bottom, 0, ImageControl.Height, -2, 2), Fractal.NormalizeValue(curRect.Top, 0, ImageControl.Height, -2, 2));

            var fractal = new Fractal((int)ImageControl.Height, (int)ImageControl.Width, xRange, yRange, iterations: 50, tileHeight: 50, tileWidth: 50);
            fractal.ProcessInParallel();

            //ImageUtils.SaveImage(bitmap);
            ImageControl.Source = fractal.GetBitmapSrc();

            sw.Stop();
            Console.WriteLine($"Elapsed = {sw.Elapsed}");
        }

        public void OnMouseScrool(object sender, MouseWheelEventArgs e)
        {
            var delta = (double)e.Delta / 1000.0;
            var pos = e.GetPosition(this.ImageControl);

            if(delta > 0)
            {
                var topLeft = curRect.TopLeft;

                topLeft.X += (pos.X - curRect.Left) * 0.1;
                topLeft.Y += (pos.Y - curRect.Top) * 0.1;

                var bottomRight = topLeft;
                bottomRight.X += curRect.Width * 0.9;
                bottomRight.Y += curRect.Height * 0.9;

                curRect = new Rect(topLeft, bottomRight);
            }
            else if (delta < 0 && curRect.Width < ImageControl.Width && curRect.Height < ImageControl.Height)
            {
                var topLeft = curRect.TopLeft;

                topLeft.X -= (pos.X - curRect.Left) * 0.1;
                topLeft.Y -= (pos.Y - curRect.Top) * 0.1;

                var bottomRight = topLeft;
                bottomRight.X += curRect.Width * 1.1;
                bottomRight.Y += curRect.Height * 1.1;

                curRect = new Rect(topLeft, bottomRight);
            }
            
            DrawFractal();
        }
    }
}