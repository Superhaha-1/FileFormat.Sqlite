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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Locator.CurrentMutable.Register(() => new NodeItemView(), typeof(IViewFor<NodeItemViewModel>));
            Locator.CurrentMutable.Register(() => new RenamingNodeItemView(), typeof(IViewFor<RenamingNodeItemViewModel>));
            Locator.CurrentMutable.Register(() => new DataItemView(), typeof(IViewFor<DataItemViewModel>));
        }
    }
}
