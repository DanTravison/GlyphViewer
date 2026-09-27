namespace GlyphViewer.Settings.Properties;

using GlyphViewer.Converter;
using GlyphViewer.ObjectModel;
using System.Text.Json;

/// <summary>
/// Provides an abstract base class for a <see cref="Setting"/> property.
/// </summary>
/// <typeparam name="T">The type of property value.</typeparam>
public abstract class SettingProperty<T> : NamedValue<T>, ISettingProperty
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="name">The property <see cref="NamedValue{T}.Name"/>.</param>
    /// <param name="defaultValue">The default <see cref="NamedValue{T}.Value"/> of the setting.</param>
    /// <param name="displayName">The <see cref="DisplayName"/> to display in the UI.</param>
    /// <param name="description">The <see cref="Description"/> of the setting.</param>
    /// <param name="comparer">
    /// The optional <see cref="IEqualityComparer{T}"/> to use to compare the <see cref="NamedValue{T}.Value"/>.
    /// <para>
    /// The default value is <see cref="EqualityComparer{T}.Default"/>.
    /// </para>
    /// </param>
    protected SettingProperty
    (
        string name,
        T defaultValue,
        string displayName,
        string description,
        IEqualityComparer<T> comparer = null
    )
        : base(name, defaultValue, comparer)
    {
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
    /// Gets the value indicating if the instance is user editable.
    /// </summary>
    public bool CanEdit
    {
        get;
        init;
    } = true;

    #endregion Properties

    #region Serialization

    /// <summary>
    /// Reads the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    public void Read(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        reader.ReadPropertyName();
        Value = ReadValue(ref reader, options);
    }

    /// <summary>
    /// Writes the value to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    public void Write(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WritePropertyName(Name);
        WriteValue(writer, Value, options);
    }

    /// <summary>
    /// Implement in the derived class to read the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    /// <returns>The <typeparamref name="T"/> value.</returns>
    protected abstract T ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options);

    /// <summary>
    /// Implemented in the derived class to write the <paramref name="value"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="value">The <typeparamref name="T"/> value to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    protected abstract void WriteValue(Utf8JsonWriter writer, T value, JsonSerializerOptions options);

    #endregion Serialization
}
