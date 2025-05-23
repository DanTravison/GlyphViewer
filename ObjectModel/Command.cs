﻿namespace GlyphViewer.ObjectModel;

using System.ComponentModel;
using System.Windows.Input;

/// <summary>
/// Provides an <see cref="ICommand"/> implementation.
/// </summary>
public class Command : ObservableObject, ICommand
{
    #region Fields

    readonly Action _action;
    bool _isEnabled = true;


    /// <summary>
    /// Provides an action that does nothing.
    /// </summary>
    /// <remarks>
    /// This method is used for cases where the derived class overrides <see cref="Execute"/>.
    /// </remarks>
    protected static void NopAction()
    {
    }

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="action">The <see cref="Action"/> to invoke.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="action"/> is a null reference.</exception>"
    public Command(Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        _action = action;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the value indicating if the command is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (SetProperty(ref _isEnabled, value, IsEnabledChangedEventArgs))
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets the parameter passed to the command when it is executed.
    /// </summary>
    public object Parameter
    {
        get;
        private set;
    }

    #endregion Properties

    /// <summary>
    /// Occurs when <see cref="IsEnabled"/> changes.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    #region Methods

    /// <summary>
    /// Gest the value indicating if the command can be executed.
    /// </summary>
    /// <param name="parameter">Not used.</param>
    /// <returns></returns>
    public virtual bool CanExecute(object parameter)
    {
        return _isEnabled;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">The parameter for the command.</param>
    public virtual void Execute(object parameter)
    {
        Parameter = parameter;
        if (_isEnabled)
        {
            _action?.Invoke();
        }
    }

    #endregion Methods

    #region PropertyChangedEventArgs

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="IsEnabled"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs IsEnabledChangedEventArgs = new(nameof(IsEnabled));

    #endregion PropertyChangedEventArgs
}
