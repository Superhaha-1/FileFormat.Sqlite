using System.Reactive.Disposables;
using System.Windows;
using FileFormat.Sqlite.Demo.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System;

namespace FileFormat.Sqlite.Demo.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(Shell))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class Shell : IViewFor<ShellViewModel>
    {
        [ImportingConstructor]
        private Shell(ShellViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DialogParticipation.SetRegister(this, ViewModel);
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.LoadFileCommand, v => v.MenuItem_LoadFile).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.UpCommand, v => v.Button_Up).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CreateNodeCommand, v => v.MenuItem_CreateNode).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.HasFile, v => v.Grid_Content.Visibility).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.NodeNames, v => v.ListBox_Nodes.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedNodeIndex, v => v.ListBox_Nodes.SelectedIndex).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ItemViewModels, v => v.ListBox_BrowseItem.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedItemIndex, v => v.ListBox_BrowseItem.SelectedIndex).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.KeyBinding_Save).DisposeWith(d);
            });
        }

        #region 实现IViewFor<ShellViewModel>

        object IViewFor.ViewModel
        {
            get
            {
                return ViewModel;
            }

            set
            {
                ViewModel = (ShellViewModel)value;
            }
        }

        public ShellViewModel ViewModel
        {
            get
            {
                return (ShellViewModel)GetValue(ViewModelProperty);
            }

            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ShellViewModel), typeof(Shell));

        #endregion
    }
}
