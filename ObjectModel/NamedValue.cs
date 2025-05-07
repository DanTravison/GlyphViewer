namespace GlyphViewer.ObjectModel;

/// <summary>
/// Provides a strongly typed named value.
/// </summary>
/// <typeparam name="T">The typeof the <see cref="Value"/>.</typeparam>
public class NamedValue<T> : ObservableObject
{
    T _value;

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="name">The <see cref="Name"/></param>
    /// <param name="defaultValue">The associated <typeparamref name="T"/> <see cref="Value"/>.</param>
    /// <param name="comparer">
    /// The optional <see cref="IEqualityComparer{T}"/> to use to compare the <see cref="Value"/>.
    /// <para>
    /// The default value is <see cref="EqualityComparer{T}.Default"/>.
    /// </para>
    /// </param>
    public NamedValue(string name, T defaultValue, IEqualityComparer<T> comparer = null)
    {
        Name = name;
        Comparer = comparer;
        Value = defaultValue;
        DefaultValue = defaultValue;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name
    {
        get;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <remarks>
    /// Raises <see cref="ObservableObject.PropertyChanged"/> with <see cref="ObservableObject.ValueChangedEventArgs"/>
    /// when the value changes.
    /// </remarks>
    public T Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value, Comparer, ValueChangedEventArgs))
            {
                OnValueChanged();
            }
        }
    }

    /// <summary>
    /// Gets the default <see cref="NamedValue{T}.Value"/> for the property.
    /// </summary>
    public T DefaultValue
    {
        get;
    }

    /// <summary>
    /// Gets the value indicating the current <see cref="NamedValue{T}.Value"/> is the <see cref="DefaultValue"/>.
    /// </summary>
    public bool IsDefault
    {
        get => AreEqual(DefaultValue);
    }

    /// <summary>
    /// Gets the <see cref="IEqualityComparer{T}"/> used to compare the <see cref="Value"/>.
    /// </summary>
    protected IEqualityComparer<T> Comparer
    {
        get;
    }

    /// <summary>
    /// Called when the <see cref="Value"/> changes.
    /// </summary>
    protected virtual void OnValueChanged()
    {
    }

    /// <summary>
    /// Compares the <see cref="Value"/> to the specified value using the <see cref="IEqualityComparer{T}"/> instance.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <returns>true if <paramref name="value"/> is equal to <see cref="Value"/>; otherwise, false.</returns>
    protected bool AreEqual(T value)
    {
        return Comparer.Equals(_value, value);
    }

    /// <summary>
    /// Resets the <see cref="Value"/> to the <see cref="DefaultValue"/>.
    /// </summary>
    public void Reset()
    {
        Value = DefaultValue;
    }
}

/// <summary>
/// Provides a <see cref="NamedValue{Object}"/>
/// </summary>
public class NamedValue : NamedValue<object>
{
    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="name">The value's name</param>
    /// <param name="value">The associated value.</param>
    /// <param name="comparer">
    /// The optional <see cref="IEqualityComparer{Object}"/> to use to compare the <see cref="NamedValue{Object}.Value"/>.
    /// <para>
    /// The default value is <see cref="EqualityComparer{Object}.Default"/>.
    /// </para>
    /// </param>
    public NamedValue(string name, object value, IEqualityComparer<object> comparer = null)
        : base(name, value, comparer)
    {
    }
}
