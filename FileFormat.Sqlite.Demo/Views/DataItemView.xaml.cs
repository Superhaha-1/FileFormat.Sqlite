﻿using System.Reactive.Disposables;
using System.Windows;
using FileFormat.Sqlite.Demo.ViewModels;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.Views
{
    /// <summary>
    /// NodeItemView.xaml 的交互逻辑
    /// </summary>
    public partial class DataItemView : IViewFor<DataItemViewModel>
    {
        public DataItemView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.TextBlock_Name.Text).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.DeleteCommand, v => v.MenuItem_Delete).DisposeWith(d);
            });
        }

        #region 实现IViewFor<DataItemViewModel>

        object IViewFor.ViewModel
        {
            get
            {
                return ViewModel;
            }

            set
            {
                ViewModel = (DataItemViewModel)value;
            }
        }

        public DataItemViewModel ViewModel
        {
            get
            {
                return (DataItemViewModel)GetValue(ViewModelProperty);
            }

            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(DataItemViewModel), typeof(DataItemView));

        #endregion
    }
}
