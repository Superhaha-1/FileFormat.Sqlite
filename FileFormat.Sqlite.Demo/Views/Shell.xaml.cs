using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
using FileFormat.Sqlite.Demo.ViewModels;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Shell : Window, IViewFor<ShellViewModel>
    {
        public Shell()
        {
            InitializeComponent();
            ViewModel = new ShellViewModel();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.LoadFileCommand, v => v.Button_LoadFile.Command).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.NodeNames, v => v.ListBox_Nodes.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedNodeName, v => v.ListBox_Nodes.SelectedItem).DisposeWith(d);
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
