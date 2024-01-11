using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Erp.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
    public class AsyncCommand<TParameter> : IAsyncCommand
    {
        private bool _isExecuting = false;

        private readonly Func<TParameter, Task> _execute;
        private readonly Func<TParameter, bool> _canExecute;

        public AsyncCommand(Func<TParameter, Task> execute, Func<TParameter, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute ?? (_ => true);
        }

        private bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

        public bool CanExecute(object parameter)
        {
            return !IsExecuting && _canExecute((TParameter)parameter);
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            IsExecuting = true;

            try
            {
                await _execute((TParameter)parameter);
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }

    public sealed class AsyncCommand : AsyncCommand<object>
    {
        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null) : base(async _ => await execute(), _ => canExecute?.Invoke() != false)
        {
        }
    }
}
