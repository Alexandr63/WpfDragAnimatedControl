using System;
using System.Windows.Input;

namespace WpfDragAnimatedPanel.Commands
{
    public sealed class DelegateCommand<T> : DelegateCommandBase
    {
        #region Ctor

        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, (x) => true)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base((x) => executeMethod((T)x), (x) => canExecuteMethod((T)x))
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }
        }

        #endregion

        #region Public Methods

        public bool CanExecute(T parameter)
        {
            return base.CanExecute(parameter);
        }

        public void Execute(T parameter)
        {
            base.Execute(parameter);
        }

        #endregion
    }
    
    public sealed class DelegateCommand : DelegateCommandBase
    {
        #region Ctor

        public DelegateCommand(Action executeMethod) : this(executeMethod, () => true)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base((x) => executeMethod(), (x) => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }
        }

        #endregion

        #region Public Methods

        public void Execute()
        {
            Execute(null);
        }

        public bool CanExecute()
        {
            return CanExecute(null);
        }

        #endregion
    }
}
