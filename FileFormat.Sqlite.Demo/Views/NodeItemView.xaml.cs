using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows;
using FileFormat.Sqlite.Demo.ViewModels;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.Views
{
    /// <summary>
    /// NodeItemView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IViewFor<NodeItemViewModel>))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class NodeItemView : IViewFor<NodeItemViewModel>
    {
        [ImportingConstructor]
        private NodeItemView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.EnterCommand, v => v.UserControl_Local, vm => vm.Name, nameof(MouseDoubleClick)).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RenameNodeCommand, v => v.MenuItem_Rename, vm => vm.Name).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteCommand, v => v.MenuItem_Delete, vm => vm.Name).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.TextBlock_Name.Text).DisposeWith(d);


                new Disposable(ViewModel.Name).DisposeWith(d);
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
