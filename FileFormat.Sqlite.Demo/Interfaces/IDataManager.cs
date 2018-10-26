using System.Windows.Input;

namespace FileFormat.Sqlite.Demo.Interfaces
{
    public interface IDataManager
    {
        ICommand DeleteDataCommand { get; }
    }
}
