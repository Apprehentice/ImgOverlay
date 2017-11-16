using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImgOverlay
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : Window
    {
        public ControlPanel()
        {
            InitializeComponent();
        }

        private void DragButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner == null)
                return;

            var opaque = (sender as ToggleButton).IsChecked.Value;
            Owner.IsHitTestVisible = opaque;

            var hwnd = new WindowInteropHelper(Owner).Handle;
            if (opaque)
            {
                WindowsServices.SetWindowExOpaque(hwnd);
            }
            else
            {
                WindowsServices.SetWindowExTransparent(hwnd);
            }

            e.Handled = true;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                (Owner as MainWindow)?.LoadImage(openDialog.FileName);
            }
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (Owner as MainWindow)?.ChangeOpacity((float)e.NewValue);
        }
    }
}
