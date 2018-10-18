using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private string Path { get; } = @"Test.jpg";

        private string Key { get; } = "Images.Images.TestImage";

        private void ShowImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                Image_Local.Source = bitmapImage;
            }
        }

        private void Button_LoadImage_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            byte[] data = null;
            stopwatch.Start();
            using (var stream = new FileStream(Path, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    data = br.ReadBytes((int)stream.Length);
                }
            }
            stopwatch.Stop();
            TextBox_LoadImage.Text = stopwatch.ElapsedMilliseconds.ToString();
            ShowImage(data);
        }

        private async void Button_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            using (var stream = new FileStream(Path, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    await Connection.SaveDataAsync(Key, br.ReadBytes((int)stream.Length));
                    stopwatch.Stop();
                    TextBox_SaveImage.Text = stopwatch.ElapsedMilliseconds.ToString();
                }
            }
        }

        private async void Button_ReadImage_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var data = await Connection.ReadDataAsync(Key);
            stopwatch.Stop();
            TextBox_ReadImage.Text = stopwatch.ElapsedMilliseconds.ToString();
            ShowImage(data);
        }

        private async void Button_Test_Click(object sender, RoutedEventArgs e)
        {
            //byte[] data = null;
            //using (var stream = new FileStream(Path, FileMode.Open))
            //{
            //    using (BinaryReader br = new BinaryReader(stream))
            //    {
            //        data = br.ReadBytes((int)stream.Length);
            //    }
            //}
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //var keys = Connection.GetChildrenKeys("TestGroup");
            //for (int i = 0; i < 100; i++)
            //{
            //    await Connection.SaveDataAsync($@"TestGroup\{i}", data);
            //}
            //stopwatch.Stop();
            //TextBox_Test.Text = stopwatch.ElapsedMilliseconds.ToString();
        }
    }
}
