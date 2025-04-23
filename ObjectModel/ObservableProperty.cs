namespace GlyphViewer.ObjectModel;

using System.ComponentModel;

/// <summary>
/// Provides an abstract base class for an observable property.
/// </summary>
public abstract class ObservableProperty : ObservableObject
{
    /// <summary>
    /// Provides a delegate on the containing object to invoke to raise the 
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the property.
    /// </summary>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> with the event details.</param>
    public delegate void NotifyPropertyChangedDelegate(PropertyChangedEventArgs e);

    #region Fields

    readonly NotifyPropertyChangedDelegate _propertyChanged;
    readonly PropertyChangedEventArgs _eventArgs;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="propertyChanged">
    /// The <see cref="NotifyPropertyChangedDelegate"/> to raise the <see cref="INotifyPropertyChanged.PropertyChanged"/> event
    /// on the containing object.
    /// </param>
    /// <param name="eventArgs">The <see cref="PropertyChangedEventArgs"/> to pass to <paramref name="propertyChanged"/>.</param>
    protected ObservableProperty(NotifyPropertyChangedDelegate propertyChanged, PropertyChangedEventArgs eventArgs)
    {
        ArgumentNullException.ThrowIfNull(propertyChanged, nameof(propertyChanged));
        ArgumentNullException.ThrowIfNull(eventArgs, nameof(eventArgs));

        _propertyChanged = propertyChanged;
        _eventArgs = eventArgs;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the <see cref="PropertyChangedEventArgs"/> to pass to the <see cref="NotifyPropertyChangedDelegate"/> delegate.
    /// </summary>
    public PropertyChangedEventArgs EventArgs
    {
        get => _eventArgs;
    }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string Name
    {
        get => _eventArgs.PropertyName;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Notifies the containing object that the property value changed.
    /// </summary>
    /// <remarks>
    /// When overriding this method, be sure to call the base class implementation,
    /// </remarks>
    protected virtual void NotifyPropertyChanged()
    {
        _propertyChanged(_eventArgs);
    }

    #endregion Methods
}

/// <summary>
/// Provides a strongly typed <see cref="ObservableProperty"/>.
/// </summary>
/// <typeparam name="T">The type of the property <see cref="Value"/>.</typeparam>
/// <remarks>
/// This class is designed to support two use cases:
/// <para>
/// 1: Implementing a strongly typed property on a class that implements <see cref="INotifyPropertyChanged"/>.
/// This is similar to a <see cref="BindableProperty"/> on a <see cref="BindableObject"/>.
/// </para>
/// <para>
/// 2: Provide a DataTemplate driven view of an object sets as a Settings viewer/editor.
/// For this case, instead of binding to various properties on the containing object, the view is bound to a
/// collection of <see cref="ObservableProperty"/> objects and a <see cref="DataTemplateSelector"/> is used
/// to define the view of each property.
/// </para>
/// </remarks>
public class ObservableProperty<T> : ObservableProperty
{
    #region Fields

    T _value;
    IEqualityComparer<T> _comparer;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="propertyChanged">
    /// The <see cref="ObservableProperty.NotifyPropertyChangedDelegate"/> to invoke to raise
    /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </param>
    /// <param name="eventArgs">The <see cref="PropertyChangedEventArgs"/> to pass to <paramref name="propertyChanged"/>.</param>
    /// <param name="comparer">
    /// The optional <see cref="IEqualityComparer{T}"/> to use to compare the <see cref="Value"/>.
    /// <para>
    /// The default value is <see cref="EqualityComparer{T}.Default"/>.
    /// </para>
    /// </param>
    public ObservableProperty
    (
        NotifyPropertyChangedDelegate propertyChanged,
        PropertyChangedEventArgs eventArgs,
        IEqualityComparer<T> comparer = null
    )
        : base(propertyChanged, eventArgs)
    {
        _comparer = comparer;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets or sets the value of the property.
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            // Raise the INotifyPropertyChanged.PropertyChanged for Value on this instance.
            if (SetProperty(ref _value, value, _comparer, ValueChangedEventArgs))
            {
                // Raise the INotifyPropertyChanged.PropertyChagned(EventArgs) on the containing object.
                NotifyPropertyChanged();
            }
        }
    }

    #endregion Properties

    /// <summary>
    /// Defines the <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="Value"/> changes.
    /// </summary>
    static readonly PropertyChangedEventArgs ValueChangedEventArgs = new(nameof(Value));
}

