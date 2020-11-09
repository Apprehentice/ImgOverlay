using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImgOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ControlPanel cp = new ControlPanel();

        public MainWindow()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            InitializeComponent();

            this.SizeChanged += Window_SizeChanged;
        }

        public void LoadImage(string path)
        {
            if (System.IO.Directory.Exists(path))
            {
                MessageBox.Show("Cannot open folders.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
            SetImageSize();
        }

        public void ChangeOpacity(float opacity)
        {
            DisplayImage.Opacity = opacity;
        }

        public void ChangeRotation(float angle)
        {
            // Create a transform to rotate the button
            RotateTransform myRotateTransform = new RotateTransform();

            // Set the rotation of the transform.
            myRotateTransform.Angle = angle;

            // Create a TransformGroup to contain the transforms
            // and add the transforms to it.
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myRotateTransform);

            Container.RenderTransformOrigin = new Point(0.5, 0.5);
            // Associate the transforms to the button.
            Container.RenderTransform = myTransformGroup;
        }

        private void SetImageSize()
        {
            if (DisplayImage.Source != null)
            {
                // Set image size so that corners stay in the window when rotating
                var w = DisplayImage.Source.Width;
                var h = DisplayImage.Source.Height;
                var diag = Math.Sqrt(w * w + h * h);

                var scale = Math.Min(this.Width, this.Height) / diag;
                Container.MaxWidth = w * scale;
                Container.MaxHeight = h * scale;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetImageSize();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowMovePreventer.AddWindow(this);

            cp.Owner = this;
            cp.Show();
            cp.Closed += (o, ev) =>
            {
                this.Close();
            };
        }
    }
}

public static class WindowMovePreventer
{
    /// <summary>
    /// Prevent Windows from moving window back to top of screen if window goes above top of screen.
    /// https://stackoverflow.com/questions/328127/how-do-i-move-a-wpf-window-into-a-negative-top-value
    /// </summary>
    public struct WINDOWPOS
    {
        public IntPtr hwnd;
        public IntPtr hwndInsertAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public UInt32 flags;
    };

    public static void AddWindow(Window window)
    {
        HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
        source.AddHook(new HwndSourceHook(WndProc));
    }

    private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case 0x46://WM_WINDOWPOSCHANGING
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                {
                    WINDOWPOS wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    wp.flags = wp.flags | 2; //SWP_NOMOVE
                    Marshal.StructureToPtr(wp, lParam, false);
                }
                break;
        }
        return IntPtr.Zero;
    }
}