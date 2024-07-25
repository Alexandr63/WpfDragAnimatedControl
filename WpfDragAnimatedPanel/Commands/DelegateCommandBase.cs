using System;
using System.Windows.Input;

namespace WpfDragAnimatedPanel.Commands
{
    public abstract class DelegateCommandBase : ICommand
    {
        #region Private Fields

        private readonly Func<object, bool> _canExecuteMethod;
        private readonly Action<object> _executeMethod;

        #endregion

        #region Ctor

        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        #endregion

        #region Protected Methods

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handlers = CanExecuteChanged;
            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }
        }

        protected void Execute(object parameter)
        {
            _executeMethod(parameter);
        }

        protected bool CanExecute(object parameter)
        {
            return _canExecuteMethod == null || _canExecuteMethod(parameter);
        }

        #endregion

        #region ICommand Members

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
