using FileFormat.Sqlite.Demo.Interfaces;
using System;
using ReactiveUI;
using System.Windows.Input;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Collections;
using FileFormat.Sqlite.Demo.Validators;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class RenamingNodeItemViewModel : ItemViewModelBase
    {
        public RenamingNodeItemViewModel(string name, INodeManager nodeManager) : base(name)
        {
            _newName = name;
            RenameCommand = nodeManager.EndRenameNodeCommand;
            this.WhenActivated(d =>
            {
                this.WhenAnyValue(r => r.NewName).Subscribe(newName =>
                {
                    var result = NameValidator.Instance.Validate(newName);
                    if (result.IsValid)
                    {
                        ChangedName = (Name, newName);
                        NewNameErrors = null;
                    }
                    else
                    {
                        NewNameErrors = result.Errors;
                    }
                }).DisposeWith(d);
            });
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

        private IEnumerable _newNameErrors;

        public IEnumerable NewNameErrors
        {
            get
            {
                return _newNameErrors;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref _newNameErrors, value);
            }
        }

        private (string, string) _changedName;

        public (string, string) ChangedName
        {
            get
            {
                return _changedName;
            }

            private set
            {
                this.RaiseAndSetIfChanged(ref _changedName, value);
            }
        }

        public ICommand RenameCommand { get; }
    }
}
