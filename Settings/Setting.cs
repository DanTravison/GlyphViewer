namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using System.ComponentModel;
using System.Text.Json;

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
    /// <param name="settings">The containing <see cref="SettingCollection"/>.</param>
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
        SettingCollection settings,
        PropertyChangedEventArgs eventArgs,
        T defaultValue,
        string displayName,
        string description,
        IEqualityComparer<T> comparer = null
    )
        : base(settings.NotifyPropertyChanged, eventArgs, comparer)
    {
        Value = defaultValue;
        DefaultValue = defaultValue;
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description ?? throw new ArgumentNullException(nameof(description));
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

    /// <summary>
    /// Gets the value indicating the current value is the <see cref="DefaultValue"/>.
    /// </summary>
    public bool IsDefault
    {
        get => AreEqual(DefaultValue);
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
        // NOTE: Don't notify when called from the constructor.
        if (_initialized)
        {
            base.NotifyPropertyChanged();
        }
        else
        {
            _initialized = true;
        }
    }

    #endregion Methods

    #region Serialization

    /// <summary>
    /// Reads the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    public virtual void ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        Value = OnReadValue(ref reader, options);
    }

    /// <summary>
    /// Writes the value to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    public virtual void WriteValue(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        OnWriteValue(writer, Value, options);
    }

    /// <summary>
    /// Implement in the derived class to read the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    /// <returns>The <typeparamref name="T"/> value.</returns>
    protected abstract T OnReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options);

    /// <summary>
    /// Implemented in the derived class to write the <paramref name="value"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="value">The <typeparamref name="T"/> value to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    protected abstract void OnWriteValue(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
    
    #endregion Serialization
}
