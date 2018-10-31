using System.Windows.Input;
using FileFormat.Sqlite.Demo.Interfaces;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class NodeItemViewModel : ItemViewModelBase
    {
        public NodeItemViewModel(string name, INodeManager nodeManager) : base(name)
        {
            DeleteCommand = nodeManager.DeleteNodeCommand;
            EnterCommand = nodeManager.EnterNodeCommand;
            RenameNodeCommand = nodeManager.StartRenameNodeCommand;
        }

        public ICommand DeleteCommand { get; }

        public ICommand EnterCommand { get; }

        public ICommand RenameNodeCommand { get; }
    }
}
