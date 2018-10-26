using System.Windows.Input;

namespace FileFormat.Sqlite.Demo.Interfaces
{
    public interface INodeManager
    {
        ICommand DeleteNodeCommand { get; }

        ICommand EnterNodeCommand { get; }

        ICommand StartRenameNodeCommand { get; }

        ICommand EndRenameNodeCommand { get; }
    }
}
