using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using FileFormat.Sqlite.Demo.Interfaces;
using FluentValidation;

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

    public sealed class NameValidator : AbstractValidator<string>
    {
        public static NameValidator Instance { get; } = new NameValidator();

        private NameValidator()
        {
            RuleFor(name => name).NotEmpty();
            RuleFor(name => name).MaximumLength(20);
        }
    }

    public sealed class FluentValidator
    {
        private Dictionary<string, IValidator> Validators { get; }

        private Dictionary<string, IEnumerable> Errors { get; }

        private Action<string> ErrorsChanged { get; }

        public FluentValidator(Action<string> errorsChanged, IDictionary<string, IValidator> validators)
        {
            ErrorsChanged = errorsChanged;
            Validators = new Dictionary<string, IValidator>(validators);
            Errors = new Dictionary<string, IEnumerable>();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null)
                return Errors.Values;
            if (Errors.TryGetValue(propertyName, out var error))
            {
                return error;
            }
            return null;
        }

        public bool HasErrors => Errors.Count > 0;

        public void Validate(string propertyName, object value)
        {
            if (Validators.TryGetValue(propertyName, out var validator))
            {
                var result = validator.Validate(value);
                if (Errors.ContainsKey(propertyName))
                {
                    if (result.IsValid)
                        Errors.Remove(propertyName);
                    else
                        Errors[propertyName] = result.Errors;
                }
                else
                {
                    if (!result.IsValid)
                        Errors.Add(propertyName, result.Errors);
                }
                ErrorsChanged?.Invoke(propertyName);
                return;
            }
            throw new Exception("没有该验证器");
        }
    }
}
