using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Image = System.Drawing.Image;

namespace SequencePresenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _index = 0;
        private bool _inShowState = false;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            Image.Visibility = Visibility.Hidden;
            Background = new SolidColorBrush(Colors.Red);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        private void TimerOnTick(object? sender, EventArgs e)
        {
            if (_inShowState)
            {
                _inShowState = false;
                Image.Visibility = Visibility.Hidden;
                Background = new SolidColorBrush(Colors.Red);
                return;

            }
            _inShowState = true;
            Image.Visibility = Visibility.Visible;
            Background = new SolidColorBrush(Colors.White);
            string file = System.IO.Path.Combine(App.DirectoryToTraverse, $"{_index++}.bmp");
            
            if (!File.Exists(file))
            {
                _timer.Stop();
                Image.Visibility = Visibility.Hidden;
                Background = new SolidColorBrush(Colors.Red);
                return;
            }

            Image fromFile = System.Drawing.Image.FromFile(file);
            BitmapImage bitmapToImageSource = BitmapToImageSource(fromFile);
            Image.Source = bitmapToImageSource;

        }

        BitmapImage BitmapToImageSource(Image bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
