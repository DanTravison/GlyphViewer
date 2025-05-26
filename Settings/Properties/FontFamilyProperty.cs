namespace GlyphViewer.Settings.Properties;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using GlyphViewer.Text;
using System.Text.Json;

/// <summary>
/// Provides a <see cref="SettingProperty{FontFamily}"/> for a font family name.
/// </summary>
public sealed class FontFamilyProperty : SettingProperty<FontFamily>
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="defaultValue">The <see cref="NamedValue{FontFamily}.DefaultValue"/>.
    /// <para>
    /// The default value is <see cref="App.DefaultFontFamily"/>.
    /// </para></param>
    public FontFamilyProperty(FontFamily defaultValue = null)
        : base(nameof(FontSetting.FontFamily), defaultValue ?? App.DefaultFontFamily, Strings.FamilyNameLabel, Strings.FamilyNameDescription)
    {
    }

    /// <summary>
    /// Reads the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    /// <returns>The <see cref="FontFamily"/> value.</returns>
    protected override FontFamily ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        string familyName = reader.GetString();
        return new FontFamily(familyName);
    }

    /// <summary>
    /// Writes the <paramref name="value"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="value">The <see cref="FontFamily"/> value to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    protected override void WriteValue(Utf8JsonWriter writer, FontFamily value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
