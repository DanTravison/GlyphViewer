using System.Text.Json;

namespace GlyphViewer.Settings.Properties;

/// <summary>
/// Provide an <see cref="ISettingProperty"/> for an enum value.
/// </summary>
/// <typeparam name="T">The type of enum.</typeparam>
public abstract class EnumProperty<T> : SettingProperty<T> 
    where T : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="defaultValue">The default <typeparamref name="T"/> value of the setting.</param>
    /// <param name="displayName">The <see cref="ISettingProperty.DisplayName"/> to display in the UI.</param>
    /// <param name="description">The <see cref="ISettingProperty.Description"/> of the setting.</param>
    protected EnumProperty(string name, T defaultValue, string displayName, string description)
        : base(name,  defaultValue, displayName, description)
    {
    }

    #region Overrides

    /// <summary>
    /// Writes the <paramref name="value"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="value">The <see cref="FontAttributes"/> value to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    protected override void WriteValue(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Enum.GetName<T>(value));
    }

    /// <summary>
    /// Reads the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    /// <returns>The <see cref="FontAttributes"/> value.</returns>
    protected override T ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        string value = reader.GetString();
        if (!Enum.TryParse(value, out T result))
        {
            result = DefaultValue;
        }
        return result;
    }

    #endregion Overrides
}
