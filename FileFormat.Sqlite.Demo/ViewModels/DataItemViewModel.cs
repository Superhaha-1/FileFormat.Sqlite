using System.Reactive.Disposables;
using System.Windows.Input;
using FileFormat.Sqlite.Demo.Interfaces;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class DataItemViewModel : ItemViewModelBase
    {
        public DataItemViewModel(string name, IDataManager dataManager) : base(name)
        {
            DeleteCommand = dataManager.DeleteDataCommand;
            //this.WhenActivated(d=>
            //{
            //});
        }

        public ICommand DeleteCommand { get; }
    }
}
