using System;
using System.Windows.Input;

namespace FileFormat.Sqlite.Demo
{
    public sealed class DelegateCommand : ICommand
    {
        public DelegateCommand(Action action)
        {
            Action = action ?? throw new Exception("Action为空");
        }

        private Action Action { get; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
                throw new Exception("parameter不为空");
            Action.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}
