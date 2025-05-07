namespace GlyphViewer.ObjectModel;

using System.ComponentModel;

/// <summary>
/// Provides an abstract base class for classes supporting INotifyPropertyChanged
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    protected ObservableObject()
    {
    }

    #endregion Constructors

    #region INotifyPropertyChanged

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event with a cached <see cref="PropertyChangedEventArgs"/>.
    /// </summary>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> for the event.</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        PropertyChanged?.Invoke(this, e);
    }

    #endregion INotifyPropertyChanged

    #region SetProperty

    /// <summary>
    /// Provides a <see cref="INotifyPropertyChanged"/> event with a cached <see cref="PropertyChangedEventArgs"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> identifying the event.</param>
    /// <returns>true if the property was set; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is a null reference.</exception>
    protected bool SetProperty<T>(ref T field, T newValue, PropertyChangedEventArgs e)
    {
        return SetProperty(ref field, newValue, null, e);
    }

    /// <summary>
    /// Provides a <see cref="INotifyPropertyChanged"/> event with a cached <see cref="PropertyChangedEventArgs"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> identifying the event.</param>
    /// <returns>true if the property was set; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is a null reference.</exception>
    protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        bool areEqual = false;
        if (comparer != null)
        {
            areEqual = comparer.Equals(field, newValue);
        }
        else if (field != null)
        {
            areEqual = field.Equals(newValue);
        }
        else if (newValue == null)
        {
            areEqual = true;
        }
        if (areEqual)
        {
            return false;
        }

        field = newValue;
        OnPropertyChanged(e);
        return true;
    }

    /// <summary>
    /// Provides a default <see cref="IEqualityComparer{T}"/> for comparing reference types.
    /// </summary>
    protected readonly IEqualityComparer<object> ReferenceComparer = ReferenceEqualityComparer.Instance;

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when the property value changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs ValueChangedEventArgs = new("Value");

    #endregion SetProperty
}
