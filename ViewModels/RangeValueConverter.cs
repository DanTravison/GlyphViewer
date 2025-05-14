namespace GlyphViewer.ViewModels;

using GlyphViewer.Text.Unicode;
using System.Globalization;

/// <summary>
/// Provide an <see cref="IValueConverter"/> for converting a <see cref="Range"/> to a string and vice versa.
/// </summary>
/// <remarks>
/// null object values are interpreted as <see cref="Range.Empty"/>.
/// <para>
/// This class is intended for use when binding a <see cref="Range"/> to a XAML property 
/// that is typed as object to avoid the binding warning when the default value is null.
/// Jumplist.SelectedItem is an example of such a property.
/// </para>
/// </remarks>
public sealed class RangeValueConverter : IValueConverter
{
    /// <summary>
    /// Provides a singleton instance of the <see cref="RangeValueConverter"/> class.
    /// </summary>
    public static readonly RangeValueConverter Instance = new();

    /// <summary>
    /// Converts a <see cref="Range"/> to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The type of the target.</param>
    /// <param name="parameter">Not used.</param>
    /// <param name="culture">Not used.</param>
    /// <returns>
    /// The <see cref="Range.Name"/> of the <paramref name="value"/>; otherwise, 
    /// <see cref="Range.Empty"/> if <paramref name="value"/> is a null reference or not a <see cref="Range"/>
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(string))
        {
            if (value is Range range)
            {
                return range.Name;
            }
        }
        return nameof(Range.Empty);
    }

    /// <summary>
    /// Converts a string to a <see cref="Range"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The target type to convert to.</param>
    /// <param name="parameter">Not used.</param>
    /// <param name="culture">Not used.</param>
    /// <returns>
    /// The <see cref="Range"/> for the specified value; otherwise,
    /// <see cref="Range.Empty"/>.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(Range))
        {
            if (value is string name)
            {
                return Ranges.Find(name ?? string.Empty);
            }
            else if (value is Range range)
            {
                return value;
            }
        }
        return Range.Empty;
    }
}
