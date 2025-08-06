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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrickBreakerss
{
    /// <summary>
    /// Logica di interazione per Classifica.xaml
    /// </summary>
    public partial class Classifica : UserControl
    {
        public Classifica()
        {
            InitializeComponent();
            this.DataContext = FileManager.Leggi("classifica.bin");
            lv.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/Podio.jpg", UriKind.RelativeOrAbsolute))
            };
        }

        private void home(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)Window.GetWindow(this);
            mw.contenitore.Visibility = Visibility.Visible;
            //cl_contenitore.Children.Clear();
            //cl_contenitore.RowDefinitions.Clear();
            mw.cont_main.Children.Remove(this);
        }
    }
}
