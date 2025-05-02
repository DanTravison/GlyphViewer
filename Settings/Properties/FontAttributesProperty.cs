namespace GlyphViewer.Settings.Properties;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using System.ComponentModel;
using System.Text.Json;

/// <summary>
/// Provides a <see cref="SettingProperty{FontAttributes}"/>.
/// </summary>
public sealed class FontAttributesProperty : SettingProperty<FontAttributes>
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="defaultValue">The default <see cref="FontAttributes"/> value.</param>
    public FontAttributesProperty(FontAttributes defaultValue = FontAttributes.None)
        : base(nameof(FontAttributes), defaultValue, Strings.FontAttributesLabel, Strings.FontAttributesDescription)
    {
    }

    #region Overrides

    /// <summary>
    /// Writes the <paramref name="value"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="value">The <see cref="FontAttributes"/> value to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    protected override void WriteValue(Utf8JsonWriter writer, FontAttributes value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Enum.GetName<FontAttributes>(value));
    }

    /// <summary>
    /// Reads the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    /// <returns>The <see cref="FontAttributes"/> value.</returns>
    protected override FontAttributes ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        string value = reader.GetString();
        if (!Enum.TryParse(value, out FontAttributes result))
        {
            result = FontAttributes.None;
        }
        return result;
    }

    #endregion Overrides
}
