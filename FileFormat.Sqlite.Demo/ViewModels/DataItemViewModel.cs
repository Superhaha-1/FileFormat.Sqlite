using System.Windows.Input;
using FileFormat.Sqlite.Demo.Interfaces;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class DataItemViewModel : ItemViewModelBase
    {
        public DataItemViewModel(string name, IDataManager dataManager) : base(name)
        {
            DeleteCommand = dataManager.DeleteDataCommand;
        }

        public ICommand DeleteCommand { get; }
    }
}
