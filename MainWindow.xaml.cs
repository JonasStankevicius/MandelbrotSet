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

            var arr = GerenateColorsArray((int)ImageControl.Width, (int)ImageControl.Height);

            //var bitmap = ImageUtils.CreateBitmap((int)ImageControl.Width, (int)ImageControl.Height, new int[] { 255, 0, 0 });
            var bitmap = ImageUtils.CreateBitmap(arr);
            //ImageUtils.SaveImage(bitmap);

            var imageSource = ImageUtils.ImageSourceFromBitmap(bitmap);

            ImageControl.Source = imageSource;
        }

        public static int[,,] GerenateColorsArray(int width, int height)
        {
            var arr = new int[height, width, 3];

            for(int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
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
    }
}
