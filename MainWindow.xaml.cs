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
        public MainWindow()
        {
            InitializeComponent();

            var fractal = new Fractal((int)ImageControl.Height * 20, (int)ImageControl.Width * 20, iterations: 100);
            fractal.Process();
            var arr = fractal.GetColorsArray();

            var bitmap = ImageUtils.CreateBitmap(arr);

            ImageUtils.SaveImage(bitmap);

            var imageSource = ImageUtils.ImageSourceFromBitmap(bitmap);

            ImageControl.Source = imageSource;
        }
    }
}