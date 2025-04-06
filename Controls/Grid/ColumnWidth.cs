namespace GlyphViewer.Controls;

using GlyphViewer.Text;
using System.Globalization;
using System.Text;

/// <summary>
/// Provides a width definition for a <see cref="ColumnDefinition"/>.
/// </summary>
public readonly struct ColumnWidth
{
    #region Fields

    internal const char LiteralChar = 'w';
    internal const string Fill = "*";
    internal const string CharacterWidth = "#";
    internal const string LiteralWidth = "@";

    /// <summary>
    /// Defines a <see cref="ColumnUnitType.Auto"/> <see cref="ColumnWidth"/> instance.
    /// </summary>
    public static readonly ColumnWidth Auto = new ColumnWidth();

    /// <summary>
    /// Defines a <see cref="ColumnUnitType.Star"/> <see cref="ColumnWidth"/> instance
    /// with a zero <see cref="ColumnWidth.Width"/>.
    /// </summary>
    public static readonly ColumnWidth Star = new ColumnWidth(0, ColumnUnitType.Star);

    /// <summary>
    /// Defines the default <see cref="FontAttributes"/> for <see cref="ColumnWidth.CharacterWidth"/>
    /// and <see cref="ColumnWidth.Literal"/> columns.
    /// </summary>
    public const FontAttributes DefaultFontAttributes = FontAttributes.None;

    /// <summary>
    /// Defines the default font size, in points, for <see cref="ColumnWidth.CharacterWidth"/>
    /// and <see cref="ColumnWidth.Literal"/> columns.
    /// </summary>
    public const double DefaultFontSize = 11.0;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="ColumnUnitType.Auto"/> instance of this class.
    /// </summary>
    public ColumnWidth()
    {
        UnitType = ColumnUnitType.Auto;
    }

    /// <summary>
    /// Initializes a new <see cref="ColumnUnitType.Absolute"/> instance of this class.
    /// </summary>
    /// <param name="width">The width in device-specific units</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> is less than or equal to zero.</exception>
    public ColumnWidth(double width)
        : this(width, ColumnUnitType.Absolute)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="ColumnUnitType.Absolute"/> or <see cref="ColumnUnitType.Star"/> instance of this class.
    /// </summary>
    /// <param name="width">The width in device-specific or proportional units.</param>
    /// <param name="unitType">The <see cref="ColumnUnitType"/> of this column.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="unitType"/> must be <see cref="ColumnUnitType.Absolute"/> or <see cref="ColumnUnitType.Star"/>.</exception>
    public ColumnWidth(double width, ColumnUnitType unitType)
    {
        if (width < 0 || double.IsNaN(width))
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }
        else if (unitType == ColumnUnitType.Absolute && width == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }
        else if
        (
            unitType != ColumnUnitType.Absolute
            &&
            unitType != ColumnUnitType.Star
        )
        {
            throw new ArgumentOutOfRangeException(nameof(unitType));
        }
        Width = width;
        UnitType = unitType;
    }

    /// <summary>
    /// Initializes a new <see cref="ColumnUnitType.Character"/> instance of this class.
    /// </summary>
    /// <param name="width">The width in characters.</param>
    /// <param name="fontFamily">The font family to use to measure the characters.</param>
    /// <param name="fontSize">The font size, in points, to use to measure the characters.</param>
    /// <param name="attributes">The <see cref="FontAttributes"/> to use to measure the characters.</param>
    public ColumnWidth(double width, string fontFamily, double fontSize, FontAttributes attributes)
        : this(new string(LiteralChar, (int)width), fontFamily, fontSize, attributes)
    {
        UnitType = ColumnUnitType.Character;
    }

    /// <summary>
    /// Initializes a new <see cref="ColumnUnitType.Literal"/> instance of this class.
    /// </summary>
    /// <param name="literal">The literal string to use to calculate the <see cref="Value"/>.</param>
    /// <param name="fontFamily">The font family to use to measure the <paramref name="literal"/>.</param>
    /// <param name="fontSize">The font size, in points, to use to measure the <paramref name="literal"/>.</param>
    /// <param name="attributes">The <see cref="FontAttributes"/> to use to measure the <paramref name="literal"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="literal"/> is a null reference or empty string.</exception>
    public ColumnWidth(string literal, string fontFamily, double fontSize, FontAttributes attributes)
    {
        UnitType = ColumnUnitType.Literal;
        FontFamily = string.IsNullOrEmpty(fontFamily) ? App.DefaultFontFamily : fontFamily;
        FontSize = fontSize == 0.0 ? DefaultFontSize : fontSize;
        FontAttributes = attributes;
        Literal = literal;
        Value = TextUtilities.Measure(literal, fontFamily, attributes, fontSize).Width;

        // TODO: Determine why Measure and Label.Measure are not producing the same width.
        // The width returned from Measure doesn't currently match the width returned
        // by Label.Measure; it is too small.
        // For example, the SampleApp defines a column width of @255 for the displayed
        // slider value. If the width returned by TextUtilities.Measure is used exactly,
        // Label wraps the text or, if LineBreakMode is set to NoWrap, the text is clipped.
        // For now, adjust the width until a better solution is found by adding '.' to the 
        // literal string.

        // Note that using Character Count does not illustrate this problem because Character Count
        // use the letter 'W' for the repeating character which is generally sufficient
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the logical width of the column.
    /// </summary>
    public readonly double Width
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="ColumnUnitType"/>.
    /// </summary>
    public readonly ColumnUnitType UnitType
    {
        get;
    }

    /// <summary>
    /// Gets the optional <see cref="FontFamily"/>.
    /// </summary>
    /// <remarks>
    /// This property is used by <see cref="ColumnUnitType.Literal"/> and <see cref="ColumnUnitType.Character"/> instances.
    /// </remarks>
    public readonly string FontFamily
    {
        get;
    } = string.Empty;

    /// <summary>
    /// Gets the optional font family.
    /// </summary>
    /// <remarks>
    /// This property is used by <see cref="ColumnUnitType.Literal"/> and <see cref="ColumnUnitType.Character"/> instances.
    /// </remarks>
    public readonly FontAttributes FontAttributes
    {
        get;
    } = FontAttributes.None;

    /// <summary>
    /// Gets the optional font size.
    /// </summary>
    /// <remarks>
    /// This property is used by <see cref="ColumnUnitType.Literal"/> and <see cref="ColumnUnitType.Character"/> instances.
    /// </remarks>
    public readonly double FontSize
    {
        get;
    } = 12;

    /// <summary>
    /// Gets the string literal used to calculate the width.
    /// </summary>
    public readonly string Literal
    {
        get;
    } = string.Empty;

    /// <summary>
    /// Gets the <see cref="GridLength.Value"/>
    /// </summary>
    public readonly double Value
    {
        get;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Converts the current instance to its string representation.
    /// </summary>
    /// <returns>A string representation of the current instance.</returns>
    public readonly override string ToString()
    {
        StringBuilder builder;
        string result = string.Empty;

        switch (UnitType)
        {
            case ColumnUnitType.Auto:
                result = nameof(ColumnUnitType.Auto);
                break;

            case ColumnUnitType.Star:
                if (Width > 0)
                {
                    result = string.Format(CultureInfo.InvariantCulture, "{0}*", Width);
                }
                else
                {
                    result = Fill;
                }
                break;

            case ColumnUnitType.Absolute:
                result = string.Format(CultureInfo.InvariantCulture, "{0}", Width);
                break;

            case ColumnUnitType.Literal:
                builder = new StringBuilder();
                builder.AppendFormat("{0:N0}{1}", LiteralWidth, Literal);
                AppendFontInfo(builder);
                result = builder.ToString();
                break;

            case ColumnUnitType.Character:
                builder = new StringBuilder();
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0:N0}{1}", CharacterWidth, Width);
                AppendFontInfo(builder);
                result = builder.ToString();
                break;
        }
        return result;
    }

    const string FontSeparator = ", ";
    void AppendFontInfo(StringBuilder builder)
    {
        if (string.IsNullOrEmpty(FontFamily) || object.ReferenceEquals(FontFamily, App.DefaultFontFamily))
        {
            return;
        }
        builder.AppendFormat(FontSeparator);
        builder.Append(FontFamily);

        if (FontSize == 0)
        {
            return;
        }
        builder.Append(FontSeparator);
        builder.AppendFormat(CultureInfo.InvariantCulture, "{0:N0}", FontSize);

        if (FontAttributes != DefaultFontAttributes)
        {
            builder.Append(FontSeparator);
            builder.Append(FontAttributes.ToString());
        }
    }

    /// <summary>
    /// Gets this instance as a <see cref="GridLength"/>
    /// </summary>
    /// <returns>A <see cref="GridLength"/> for the current instance.</returns>
    public readonly GridLength ToGridLength()
    {

        GridLength result = GridLength.Auto;
        switch (UnitType)
        {
            case ColumnUnitType.Auto:
                break;

            case ColumnUnitType.Star:
                if (Width == 0)
                {
                    result = GridLength.Star;
                }
                else
                {
                    result = new GridLength(Width, GridUnitType.Star);
                }
                break;

            case ColumnUnitType.Absolute:
                result = new GridLength(Width);
                break;

            case ColumnUnitType.Literal:
            case ColumnUnitType.Character:
                result = new GridLength(Value, GridUnitType.Absolute);
                break;
        }

        return result;
    }

    #endregion Methods
}
