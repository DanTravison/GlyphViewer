namespace GlyphViewer.Converter;

using GlyphViewer.Settings;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides a <see cref="JsonConverter{UserSettings}"/>.
/// </summary>
internal class UserSettingsJsonConverter : JsonConverter<UserSettings>
{
    /// <summary>
    /// Provides the <see cref="JsonSerializerOptions"/> to use with this converter.
    /// </summary>
    public static readonly JsonSerializerOptions Options;

    /// <summary>
    /// Provides a singleton <see cref="UserSettingsJsonConverter"/> instance.
    /// </summary>
    public static readonly UserSettingsJsonConverter Converter = new();

    static UserSettingsJsonConverter()
    {
        Options = new()
        {
            WriteIndented = true,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IncludeFields = false,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = false,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        Options.Add
        (
            Converter,
            FontFamiliesJsonConverter.Converter
        );
    }

    private UserSettingsJsonConverter()
    {
    }

    /// <summary>
    /// Reads the <see cref="UserSettings"/> from the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to control serialization.</param>
    /// <returns>A new instance of a <see cref="UserSettings"/>.</returns>
    protected override UserSettings OnRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        UserSettings settings = new();
        settings.Read(ref reader, options);
        settings.HasChanges = false;
        return settings;
    }

    /// <summary>
    /// Writes the <see cref="UserSettings"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="settings">The <see cref="UserSettings"/> to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to control serialization.</param>
    protected override void OnWrite(Utf8JsonWriter writer, UserSettings settings, JsonSerializerOptions options)
    {
        settings.Write(writer, options);
        settings.HasChanges = false;
    }
}