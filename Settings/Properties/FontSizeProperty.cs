namespace GlyphViewer.Settings.Properties;

using GlyphViewer.Resources;

/// <summary>
/// Provides a <see cref="DoubleProperty"/> for a font size.
/// </summary>
public sealed class FontSizeProperty : DoubleProperty
{
    /// <summary>
    /// Defines the default value for the incrementing the font size.
    /// </summary>
    public const double DefaultSizeIncrement = 1;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="minimum">The minimum value for the font size.</param>
    /// <param name="maximum">The maximum value for the font size.</param>
    /// <param name="increment">The value to use to increment the font size.</param>
    public FontSizeProperty
    (
        double defaultValue,
        double minimum,
        double maximum,
        double increment = DefaultSizeIncrement
    )
        : base(nameof(FontSetting.FontSize), defaultValue, Strings.FontSizeLabel, Strings.FontSizeDescription)
    {
        base.MininumValue = minimum;
        base.MaximumValue = maximum;
        base.Increment = increment;
    }
}
