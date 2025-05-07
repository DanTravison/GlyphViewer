namespace GlyphViewer.Settings.Properties;

using GlyphViewer.ObjectModel;
using System.Text.Json;

/// <summary>
/// Provides a <see cref="SettingProperty{String}"/>.
/// </summary>
public class StringProperty : SettingProperty<string>
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="name">The property <see cref="NamedValue{String}.Name"/>.</param>
    /// <param name="defaultValue">The <see cref="NamedValue{String}.DefaultValue"/>.</param>
    /// <param name="displayName">The <see cref="SettingProperty{String}.DisplayName"/>.</param>
    /// <param name="description">The <see cref="SettingProperty{String}.Description"/>.</param>
    /// <param name="comparer">
    /// The optional <see cref="IEqualityComparer{String}"/> to use to compare the value.
    /// <para>
    /// The default value is <see cref="EqualityComparer{String}.Default"/>.
    /// </para>
    /// </param>
    public StringProperty
    (
        string name,
        string defaultValue,
        string displayName,
        string description,
        IEqualityComparer<string> comparer = null
    )
        : base(name, defaultValue, displayName, description, comparer)
    {
    }

    #region Overrides

    /// <summary>
    /// Writes the <paramref name="value"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="value">The <see cref="string"/> value to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    protected override void WriteValue(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }

    /// <summary>
    /// Reads the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    /// <returns>The <see cref="string"/> value.</returns>
    protected override string ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

    #endregion Overrides
}
