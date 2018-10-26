using FileFormat.Sqlite.Demo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Windows.Input;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class RenamingNodeItemViewModel : ItemViewModelBase
    {
        public RenamingNodeItemViewModel(string name, INodeManager nodeManager) : base(name)
        {
            _newName = name;
            RenameCommand = nodeManager.EndRenameNodeCommand;
        }

        private string _newName;

        public string NewName
        {
            get
            {
                return _newName;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _newName, value);
            }
        }

        public IObservable<(string, string)> ChangedNameObservable => this.WhenAnyValue(r => r.NewName).Select(n => (Name, n));

        public ICommand RenameCommand { get; }
    }
}
