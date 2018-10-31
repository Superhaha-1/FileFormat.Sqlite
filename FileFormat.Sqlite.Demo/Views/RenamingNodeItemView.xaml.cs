using FileFormat.Sqlite.Demo.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace FileFormat.Sqlite.Demo.Views
{
    /// <summary>
    /// RenamingNodeItemView.xaml 的交互逻辑
    /// </summary>
    public partial class RenamingNodeItemView : IViewFor<RenamingNodeItemViewModel>
    {
        public RenamingNodeItemView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                ((KeyBinding_Enter.Command = ReactiveCommand.Create(() => Grid_Main.Focus())) as IDisposable).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.NewName, v => v.TextBox_Name.Text).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.RenameCommand, v => v.TextBox_Name, vm => vm.ChangedName, nameof(TextBox_Name.LostKeyboardFocus)).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.NewNameErrors, v => v.ShowErrorsBehavior_TextBox_Name.Errors).DisposeWith(d);
                TextBox_Name.Focus();
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
                ViewModel = (RenamingNodeItemViewModel)value;
            }
        }

        public RenamingNodeItemViewModel ViewModel
        {
            get
            {
                return (RenamingNodeItemViewModel)GetValue(ViewModelProperty);
            }

            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(RenamingNodeItemViewModel), typeof(RenamingNodeItemView));

        #endregion
    }
}
