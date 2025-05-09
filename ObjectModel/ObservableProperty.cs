namespace GlyphViewer.ObjectModel;

using System.ComponentModel;

/// <summary>
/// Provides a delegate on the containing object to invoke to raise the 
/// <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the property.
/// </summary>
/// <param name="e">The <see cref="PropertyChangedEventArgs"/> with the event details.</param>
public delegate void NotifyPropertyChangedDelegate(PropertyChangedEventArgs e);

/// <summary>
/// Provides an observable <see cref="NamedValue{T}"/>.
/// </summary>
/// <typeparam name="T">The type of <see cref="NamedValue{T}.Value"/>.</typeparam>
/// <remarks>
/// This class is designed to support two use cases:
/// <para>
/// 1: Implementing a strongly typed property on a class that implements <see cref="INotifyPropertyChanged"/>.
/// This is similar to a <see cref="BindableProperty"/> on a <see cref="BindableObject"/>.
/// </para>
/// <para>
/// 2: Provide a DataTemplate driven view of an object sets as a Settings viewer/editor.
/// For this case, instead of binding to various properties on the containing object, the view is bound to a
/// collection of observable poperties and a <see cref="DataTemplateSelector"/> is used
/// to define the view of each property.
/// </para>
/// </remarks>
public class ObservableProperty<T> : NamedValue<T>
{
    #region Fields

    readonly NotifyPropertyChangedDelegate _propertyChanged;
    readonly PropertyChangedEventArgs _eventArgs;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="propertyChanged">
    /// The <see cref="NotifyPropertyChangedDelegate"/> to invoke to raise
    /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </param>
    /// <param name="eventArgs">The <see cref="PropertyChangedEventArgs"/> to pass to <paramref name="propertyChanged"/>.</param>
    /// <param name="comparer">
    /// The optional <see cref="IEqualityComparer{T}"/> to use to compare the <see cref="NamedValue{T}.Value"/>.
    /// <para>
    /// The default value is <see cref="EqualityComparer{T}.Default"/>.
    /// </para>
    /// </param>
    public ObservableProperty
    (
        NotifyPropertyChangedDelegate propertyChanged,
        PropertyChangedEventArgs eventArgs,
        T defaultValue,
        IEqualityComparer<T> comparer = null
    )
        : base(eventArgs.PropertyName, defaultValue, comparer)
    {
        ArgumentNullException.ThrowIfNull(propertyChanged, nameof(propertyChanged));
        ArgumentNullException.ThrowIfNull(eventArgs, nameof(eventArgs));

        _propertyChanged = propertyChanged;
        _eventArgs = eventArgs;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Notifies the containing object that the <see cref="NamedValue{T}.Value"/> changed.
    /// </summary>
    /// <remarks>
    /// When overriding this method, be sure to call the base class implementation,
    /// </remarks>
    protected override void OnValueChanged()
    {
        _propertyChanged(_eventArgs);
    }

    #endregion Methods
}

