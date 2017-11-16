using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImgOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ControlPanel cp = new ControlPanel();

        public MainWindow()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            InitializeComponent();
        }

        public void LoadImage(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("The selected image file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var img = new BitmapImage();
            try
            {
                img.BeginInit();
                    img.UriSource = new Uri(path);
                img.EndInit();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading image. Perhaps its format is unsupported?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DisplayImage.Source = img;
        }

        public void ChangeOpacity(float opacity)
        {
            DisplayImage.Opacity = opacity;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cp.Owner = this;
            cp.Show();
            cp.Closed += (o, ev) =>
            {
                this.Close();
            };
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }
    }
}
