namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using System.ComponentModel;

/// <summary>
/// Defines an abstract, strongly typed <see cref="ObservableProperty{T}"/> class.
/// </summary>
/// <typeparam name="T">The type of <see cref="ObservableProperty{T}.Value"/>.</typeparam>
public abstract class Setting<T> : ObservableProperty<T>, ISetting
{
    #region Fields

    bool _initialized;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="propertyChanged">The <see cref="ObservableProperty.NotifyPropertyChangedDelegate"/>  delegate to invoke to raised the property chagned event.</param>
    /// <param name="eventArgs">The optional <see cref="PropertyChangedEventArgs"/> to use when the value changes.</param>
    /// <param name="defaultValue">The default <see cref="ObservableProperty{T}.Value"/> of the setting.</param>
    /// <param name="displayName">The <see cref="DisplayName"/> to display in the UI.</param>
    /// <param name="description">The <see cref="Description"/> of the setting.</param>
    /// <param name="comparer">
    /// The optional <see cref="IEqualityComparer{T}"/> to use to compare the <see cref="ObservableProperty{T}.Value"/>.
    /// <para>
    /// The default value is <see cref="EqualityComparer{T}.Default"/>.
    /// </para>
    /// </param>
    protected Setting
    (
        NotifyPropertyChangedDelegate propertyChanged,
        PropertyChangedEventArgs eventArgs,
        T defaultValue,
        string displayName,
        string description,
        IEqualityComparer<T> comparer = null
    )
        : base(propertyChanged, eventArgs, comparer)
    {
        DefaultValue = defaultValue;
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description ?? throw new ArgumentNullException(nameof(description));

        Value = ReadValue();
    }

    #region Properties

    /// <summary>
    /// Gets the name to display in the UI.
    /// </summary>
    public string DisplayName
    {
        get;
    }

    /// <summary>
    /// Gets the description of the setting.
    /// </summary>
    public string Description
    {
        get;
    }

    /// <summary>
    /// Gets the default value for the setting.
    /// </summary>
    public T DefaultValue
    {
        get;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Resets the setting to it's default value.
    /// </summary>
    public void Reset()
    {
        Value = DefaultValue;
    }

    /// <summary>
    /// Notifies the containing object when the <see cref="ObservableProperty{T}.Value"/>
    /// </summary>
    protected override sealed void NotifyPropertyChanged()
    {
        base.NotifyPropertyChanged();
        // NOTE: Don't call WritePreference from the constructor.
        if (_initialized)
        {
            WriteValue(Value);
        }
        else
        {
            _initialized = true;
        }
    }

    #endregion Methods

    #region Abstract Methods

    /// <summary>
    /// Implemented in the derived class to write the value to settings storage.
    /// </summary>
    /// <param name="value">The value to set.</param>
    protected abstract void WriteValue(T value);

    /// <summary>
    /// Implemented in the derived class to get the value from the settings storage.
    /// </summary>
    /// <returns>The value; if found; otherwise, the default value.</returns>
    protected abstract T ReadValue();

    #endregion Abstract Methods
}
