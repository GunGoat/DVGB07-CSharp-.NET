using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DVGB07_Text_Editor_App.Commands;

/// <summary>
/// A command that relays its execution logic to other objects.
/// Base structure for the class:
/// <list type="bullet">
///     <item><description>https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/relaycommand#how-they-work</description></item>
///     <item><description>https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.icommand?view=net-9.0</description></item>
/// </list>
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The logic for determining if the command can execute. Default is null.</param>
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. This can be null.</param>
    /// <returns>True if the command can execute, otherwise false.</returns>
    public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command. This can be null.</param>
    public void Execute(object parameter) => _execute(parameter);

    /// <summary>
    /// Occurs when changes occur that affect whether the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged"/> event.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
