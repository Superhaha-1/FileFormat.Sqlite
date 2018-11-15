using System;
using System.Threading;
using System.Windows;
using FileFormat.Sqlite.Demo.ViewModels;
using FileFormat.Sqlite.Demo.Views;
using ReactiveUI;
using Splat;

namespace FileFormat.Sqlite.Demo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            //转换为英文版（测试用）
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
