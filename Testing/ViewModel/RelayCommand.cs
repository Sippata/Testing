using System;
using System.Windows.Input;

namespace Testing.ViewModel
{   
    public class RelayCommand : ICommand
    {
        #region Fields

        private Action<object> _execute;
        private Predicate<object> _canExecute;

        #endregion // Fields

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        #region ICommand Members
        
        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion // ICommand Members
    }
}