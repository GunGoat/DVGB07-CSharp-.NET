using System;
using System.Windows.Input;

namespace DVGB07_Inventory_App.Commands;

/// <summary>
/// A generic implementation of the ICommand interface that allows 
/// delegates to be used for command execution and availability checking.
/// This class supports commands both with and without parameters.
/// 
/// For more details on implementing ICommand in WPF, see:
/// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.icommand">Microsoft Documentation</see>.
/// <see href="https://www.codeproject.com/Articles/813345/Basic-MVVM-and-ICommand-Usage-Example">CodeProject Tutorial</see>.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object> _executeWithParameter;
    private readonly Action _executeWithoutParameter;
    private readonly Func<object, bool> _canExecuteWithParameter;
    private readonly Func<bool> _canExecuteWithoutParameter;

    /// <summary>
    /// Constructor for commands with parameter.
    /// </summary>
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _executeWithParameter = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecuteWithParameter = canExecute;
    }

    /// <summary>
    /// Constructor for commands without parameter.
    /// </summary>
    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _executeWithoutParameter = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecuteWithoutParameter = canExecute;
    }

    /// <summary>
    /// Determines whether the command can execute.
    /// </summary>
    public bool CanExecute(object parameter)
    {
        if (_canExecuteWithoutParameter != null)
        {
            return _canExecuteWithoutParameter();
        }

        return _canExecuteWithParameter == null || _canExecuteWithParameter(parameter);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    public void Execute(object parameter)
    {
        if (_executeWithoutParameter != null)
        {
            _executeWithoutParameter();
        }
        else
        {
            _executeWithParameter(parameter);
        }
    }

    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Raises the CanExecuteChanged event.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
