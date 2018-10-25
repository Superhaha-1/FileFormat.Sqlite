using System.Reactive.Disposables;
using FileFormat.Sqlite.Demo.Interfaces;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class DataItemViewModel : ItemViewModelBase
    {
        public DataItemViewModel(string name, IDataManager dataManager) : base(name)
        {
            this.WhenActivated(d=>
            {
                (DeleteCommand = ReactiveCommand.Create(() => dataManager.DeleteData(name))).DisposeWith(d);
            });
        }

        public ReactiveCommand DeleteCommand { get; set; }
    }
}
