using System;
using System.Collections.Generic;
using System.IO;
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

namespace FileFormat.Sqlite.Wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Connection = new FileConnection("Test.db");
        }

        private FileConnection Connection { get; }

        private void SetImage(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                Image_Local.Source = bitmapImage;
            }
        }

        private void Button_LoadImage_Click(object sender, RoutedEventArgs e)
        {
            Image_Local.Source = new BitmapImage(new Uri("Test.jpg", UriKind.Relative));
        }

        private async void Button_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            //using (BinaryReader br = new BinaryReader(this.get))
            //{
            //    await Connection.SaveData("TestImage", br.ReadBytes((int)stream.Length));
            //}
        }

        private void Button_ReadImage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
