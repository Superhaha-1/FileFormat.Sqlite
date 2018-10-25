using System.Reactive.Disposables;
using FileFormat.Sqlite.Demo.Interfaces;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class NodeItemViewModel : ItemViewModelBase
    {
        public NodeItemViewModel(string name, INodeManager nodeManager) : base(name)
        {
            this.WhenActivated(d =>
            {
                (EnterCommand = ReactiveCommand.Create(() => nodeManager.EnterNode(name))).DisposeWith(d);
                (RenameCommand = ReactiveCommand.Create(() => IsRenaming = true)).DisposeWith(d);
                (DeleteCommand = ReactiveCommand.Create(() => nodeManager.DeleteNode(name))).DisposeWith(d);
            });
        }

        public ReactiveCommand EnterCommand { get; set; }

        public ReactiveCommand RenameCommand { get; set; }

        public ReactiveCommand DeleteCommand { get; set; }

        private bool _isRenaming;

        public bool IsRenaming
        {
            get
            {
                return _isRenaming;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref _isRenaming, value);
            }
        }
    }
}
