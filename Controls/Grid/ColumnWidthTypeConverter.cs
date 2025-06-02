namespace GlyphViewer.Controls;

using GlyphViewer.Text;
using System.ComponentModel;
using System.Globalization;

/// <summary>
/// Provides a <see cref="TypeConverter"/> a <see cref="ColumnWidth"/> to and from a <see cref="String"/>.
/// </summary>
public class ColumnWidthTypeConverter : TypeConverter
{
    #region ConvertFrom

    /// <summary>
    /// Returns whether this converter can convert an object of the given type to a <see cref="ColumnWidth"/>.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.
    /// <para>
    /// Not used.
    /// </para></param>
    /// <param name="sourceType">A <see cref="Type"/> that represents the type to convert from.</param>
    /// <returns>true if <paramref name="sourceType"/> is of type <see cref="String"/>; otherwise, false.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Converts the <see cref="string"/> <paramref name="value"/> to a <see cref="ColumnWidth"/>.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.
    /// <para>
    /// Not used.
    /// </para>
    /// </param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.
    /// <para>
    /// Not used.
    /// </para>
    /// </param>
    /// <param name="value">The string value to convert as an <see cref="Object"/>.</param>
    /// <returns>A <see cref="ColumnWidth"/> for the string; otherwise, <see cref="ColumnWidth.Auto"/>
    /// if the value is an empty string.</returns>
    /// <exception cref="FormatException">The specified <paramref name="value"/> is not a valid
    /// <see cref="ColumnWidth"/> string representation.</exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        string strValue = value?.ToString();

        if (strValue is null)
        {
            return ColumnWidth.Auto;
        }

        do // while (false)
        {
            if (StringComparer.OrdinalIgnoreCase.Compare(strValue, nameof(ColumnWidth.Auto)) == 0)
            {
                return ColumnWidth.Auto;
            }

            if (strValue == ColumnWidth.Fill)
            {
                return ColumnWidth.Star;
            }

            if (strValue.EndsWith(ColumnWidth.Fill, StringComparison.Ordinal))
            {
                string number = strValue.Substring(0, strValue.Length - 1);

                if (!Parse(number, out double length))
                {
                    break;
                }
                return new ColumnWidth(length, ColumnUnitType.Star);
            }

            if (strValue.StartsWith(ColumnWidth.CharacterWidth, StringComparison.Ordinal))
            {
                // Valid formats
                // 1: The character width 'N#'
                //    #3
                // 2: The character width and the font family
                //    #4, OpenSansRegular
                // 3: The character width, font family, and font size
                //    #5, OpenSansRegular, 24
                // 4: The character width, font family, font size, and font attributes
                //    #6, OpenSansRegular, 24, Bold
                if (!ParseWidthColumn(strValue, ColumnWidth.CharacterWidth, out ColumnWidth columnWidth))
                {
                    break;
                }
                return columnWidth;
            }
            if (strValue.StartsWith(ColumnWidth.LiteralWidth, StringComparison.Ordinal))
            {
                // Valid formats
                // 1: The character width 'N#'
                //    @000
                // 2: The character width and the font family
                //    @ABC, OpenSansRegular
                // 3: The character width, font family, and font size
                //    @0.0, OpenSansRegular, 24
                // 4: The character width, font family, font size, and font attributes
                //    @WWW, OpenSansRegular, 24, Bold
                if (!ParseWidthColumn(strValue, ColumnWidth.LiteralWidth, out ColumnWidth columnWidth))
                {
                    break;
                }
                return columnWidth;
            }
        } while (false);

        throw new FormatException(strValue);
    }

    static bool ParseWidthColumn(string value, string lengthChar, out ColumnWidth columnWidth)
    {
        bool result = false;
        columnWidth = ColumnWidth.Auto;
        do
        {
            double width = 0;

            string[] parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1 || parts.Length > 4)
            {
                break;
            }

            string pattern = parts[0].Trim();
            if (!pattern.StartsWith(lengthChar))
            {
                break;
            }

            //
            // parse character width/literal.
            //
            string literal = pattern.Substring(1);
            if (literal.Length == 0)
            {
                break;
            }

            if (lengthChar == ColumnWidth.CharacterWidth)
            {
                if (!Parse(literal, out width))
                {
                    break;
                }
            }

            //
            // Parse the font family, weight, and attributes
            //
            string fontFamily = FontFamily.DefaultFontName;
            double fontSize = ColumnWidth.DefaultFontSize;
            FontAttributes fontAttributes = ColumnWidth.DefaultFontAttributes;

            if (parts.Length > 1)
            {
                fontFamily = parts[1].Trim();
            }

            if (parts.Length > 2)
            {
                if (!Parse(parts[2], out fontSize))
                {
                    break;
                }
            }

            if (parts.Length > 3)
            {
                if (!Enum.TryParse<FontAttributes>(parts[3], out fontAttributes))
                {
                    break;
                }
            }

            if (lengthChar == ColumnWidth.CharacterWidth)
            {
                columnWidth = new(width, fontFamily, fontSize, fontAttributes);
            }
            else
            {
                columnWidth = new(literal, fontFamily, fontSize, fontAttributes);
            }
            result = true;
        } while (false);

        return result;
    }

    static bool Parse(string text, out double value)
    {
        return double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    #endregion ConvertFrom

    #region ConvertTo

    /// <summary>
    /// Returns whether this converter can convert the object to the specified type.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.
    /// <para>
    /// Not used.
    /// </para>
    /// </param>
    /// <param name="destinationType">A <see cref="Type"/> that represents the type to convert to.</param>
    /// <returns>true if <paramref name="destinationType"/> is of type <see cref="String"/>; otherwise, 
    /// false.</returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(string);
    }

    /// <summary>
    /// Converts a <see cref="ColumnWidth"/> to a string.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.
    /// <para>
    /// Not used.
    /// </para>
    /// </param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.
    /// <para>
    /// Not used.
    /// </para>
    /// </param>
    /// <param name="value">The <see cref="ColumnWidth"/> object to convert.</param>
    /// <param name="destinationType">The type to convert to.</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">
    /// <paramref name="value"/> is not a <see cref="ColumnWidth"/>
    /// -or-
    /// <paramref name="destinationType"/> is not <see cref="String"/>.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (value is not ColumnWidth columnWidth)
        {
            throw new NotSupportedException();
        }

        if (destinationType == typeof(string))
        {
            throw new NotSupportedException();
        }

        return columnWidth.ToString();
    }

    #endregion ConvertTo
}
