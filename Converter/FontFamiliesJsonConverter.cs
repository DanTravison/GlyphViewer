namespace GlyphViewer.Converter;

using GlyphViewer.Text;
using System.Text.Json;

/// <summary>
/// Provides methods to read and write <see cref="IReadOnlyList{FontFamily}"/> to and from JSON.
/// </summary>
internal class FontFamiliesJsonConverter : JsonConverter<List<FontFamily>>
{
    const string PathPrefix = "file:";

    /// <summary>
    /// Gets the singleton instance of the <see cref="FontFamiliesJsonConverter"/>.
    /// </summary>
    public static readonly FontFamiliesJsonConverter Converter = new();

    private FontFamiliesJsonConverter()
    {
    }

    /// <summary>
    /// Writes the <see cref="IReadOnlyList{FontFamily}"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="families">The <see cref="List{FontFamily}"/> to write.</param>
    protected override void OnWrite(Utf8JsonWriter writer, List<FontFamily> families, JsonSerializerOptions options)
    {
        if (families.Count > 0)
        {
            List<string> values = [];
            foreach (FontFamily family in families)
            {
                if (family is FileFontFamily file && file.Exists)
                {
                    values.Add($"{PathPrefix}{file.FilePath}");
                }
                else
                {
                    values.Add(family.Name);
                }
            }
            System.Text.Json.JsonSerializer.Serialize(writer, values, options);
            values.Clear();
        }
    }

    /// <summary>
    /// Reads a <see cref="List{FontFamily}"/> from the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to control serialization.</param>
    /// <returns>A new instance of a <see cref="List{FontFamily}"/> containing zero or more elements.</returns>
    protected override List<FontFamily> OnRead(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        List<FontFamily> families = [];
        List<string> values = JsonSerializer.Deserialize<List<string>>(ref reader, options);

        foreach (string value in values)
        {
            if (value.StartsWith(PathPrefix, StringComparison.Ordinal))
            {
                string filePath = value.Substring(PathPrefix.Length);
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    families.Add(new FileFontFamily(fileInfo));
                }
            }
            else
            {
                families.Add(new FontFamily(value));
            }
        }
        return families;
    }
}
