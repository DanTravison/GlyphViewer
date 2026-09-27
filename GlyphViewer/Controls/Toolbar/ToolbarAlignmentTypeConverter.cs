namespace GlyphViewer.Controls;

using GlyphViewer.Converter;
using System.Globalization;

/// <summary>
/// Provides a <see cref="IValueConverter"/> for converting between <see cref="ToolbarAlignment"/> values and their string representations.
/// </summary>
public sealed class ToolbarAlignmentTypeConverter : TypeConverter<ToolbarAlignment, String>
{
    /// <summary>
    /// Defines an instance of a <see cref="ToolbarAlignmentTypeConverter"/>.
    /// </summary>
    public static readonly ToolbarAlignmentTypeConverter Converter = new();

    /// <summary>
    /// Converts a string representation of an <see cref="ToolbarAlignment"/> value to its corresponding enum value.
    /// </summary>
    /// <param name="culture">Not used.</param>
    /// <param name="value">The string value to convert.</param>
    /// <returns>An <see cref="ToolbarAlignment"/> value.</returns>
    /// <exception cref="FormatException"><paramref name="value"/> is not a valid <see cref="ToolbarAlignment"/> string.</exception>
    protected override ToolbarAlignment ConvertFrom(CultureInfo culture, string value)
    {
        if (!Enum.TryParse(value, out ToolbarAlignment alignment))
        {
            throw new FormatException
            (
                string.Format($"'{value}' is not a valid Alignment value")
            );
        }
        return alignment;
    }

    /// <summary>
    /// Converts an <see cref="ToolbarAlignment"/> value to its string representation.
    /// </summary>
    /// <param name="culture">Not used.</param>
    /// <param name="value">The <see cref="ToolbarAlignment"/> to convert.</param>
    /// <returns>A string representation of the <paramref name="value"/>.</returns>
    protected override string ConvertTo(CultureInfo culture, ToolbarAlignment value)
    {
        return Enum.GetName<ToolbarAlignment>(value);
    }
}