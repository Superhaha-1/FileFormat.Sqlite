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
using System.Collections;
using System.ComponentModel;
using FluentValidation;
using System.Reactive.Subjects;

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
                //(NewNameErrors = new Subject<IEnumerable>()).DisposeWith(d);
                //(_changedName = this.WhenAnyValue(r => r.NewName).Where(newName => NameValidator.Instance.Validate(newName).IsValid).Select(newName => (Name, newName)).ToProperty(this, r => r.ChangedName)).DisposeWith(d);
                //(ChangedName = new Subject<(string, string)>()).DisposeWith(d);
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
                //ChangedName.OnNext((Name, NewName));
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

        //public Subject<IEnumerable> NewNameErrors { get; private set; }

        //public Subject<(string, string)> ChangedName { get; private set; }

        //private ObservableAsPropertyHelper<(string, string)> _changedName;

        //public (string, string) ChangedName { get; private set; }

        //public IObservable<(string, string)> ChangedName => this.WhenAnyValue(r => r.NewName).Where(newName => NameValidator.Instance.Validate(newName).IsValid).Select(newName => (Name, newName));

        public ICommand RenameCommand { get; }
    }
}
