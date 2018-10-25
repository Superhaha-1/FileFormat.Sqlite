using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using FileFormat.Sqlite.Demo.ViewModels;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.Views
{
    /// <summary>
    /// NodeItemView.xaml 的交互逻辑
    /// </summary>
    public partial class NodeItemView : IViewFor<NodeItemViewModel>
    {
        public NodeItemView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.EnterCommand, v => v.UserControl_Main, nameof(MouseDoubleClick)).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RenameCommand, v => v.MenuItem_Rename).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteCommand, v => v.MenuItem_Delete).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.TextBlock_Name.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.IsRenaming, v => v.TextBox_Rename.Visibility).DisposeWith(d);
            });
        }

        #region 实现IViewFor<NodeItemViewModel>

        object IViewFor.ViewModel
        {
            get
            {
                return ViewModel;
            }

            set
            {
                ViewModel = (NodeItemViewModel)value;
            }
        }

        public NodeItemViewModel ViewModel
        {
            get
            {
                return (NodeItemViewModel)GetValue(ViewModelProperty);
            }

            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(NodeItemViewModel), typeof(NodeItemView));

        #endregion
    }
}
