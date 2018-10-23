using System.Reactive.Disposables;
using FileFormat.Sqlite.Demo.ViewModels;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.Views
{
    /// <summary>
    /// NodeItemView.xaml 的交互逻辑
    /// </summary>
    public partial class NodeItemView : ReactiveUserControl<NodeItemViewModel>
    {
        public NodeItemView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.TextBlock_Name.Text).DisposeWith(d);
            });
        }
    }
}
