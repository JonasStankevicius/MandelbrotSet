using System;
using System.Collections.Generic;
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
            ImageControl.MouseWheel += OnMouseScrool;

            curRect = new Rect(new System.Windows.Point(0, 0), new System.Windows.Point(ImageControl.Width, ImageControl.Height));
            DrawFractal();
        }

        public void DrawFractal()
        {
            var xRange = new Range(Fractal.NormalizeValue(curRect.Left, 0, ImageControl.Width, -2, 2), Fractal.NormalizeValue(curRect.Right, 0, ImageControl.Width, -2, 2));
            var yRange = new Range(Fractal.NormalizeValue(curRect.Bottom, 0, ImageControl.Height, -2, 2), Fractal.NormalizeValue(curRect.Top, 0, ImageControl.Height, -2, 2));

            var fractal = new Fractal((int)ImageControl.Height*25, (int)ImageControl.Width*25, xRange, yRange, iterations: 100, tileHeight: 100, tileWidth: 100);
            //fractal.Process();
            fractal.ProcessInParallel();
            var bitmap = fractal.GetBitMap();

            ImageUtils.SaveImage(bitmap);
            ImageControl.Source = ImageUtils.ImageSourceFromBitmap(bitmap);
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