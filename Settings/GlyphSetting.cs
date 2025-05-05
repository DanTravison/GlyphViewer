namespace GlyphViewer.Settings;

using GlyphViewer.Resources;
using GlyphViewer.Settings.Properties;
using GlyphViewer.Text;
using GlyphViewer.Views;
using System.ComponentModel;

/// <summary>
/// Provides an <see cref="ISetting"/> for the glyph properties.
/// </summary>
public class GlyphSetting : Setting
{
    #region Constants and Fields

    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double MinimumWidth = 200;

    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double MaximumWidth = 500;

    /// <summary>
    /// Defines the default width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double DefaultWidth = 300;

    /// <summary>
    /// Defines the default size increment to use to resize the width.
    /// </summary>
    public const double DefaultSizeIncrment = 10;

    /// <summary>
    /// Defines the default color to use to draw the edges of the glyph.
    /// </summary>
    public static readonly Color DefaultLineColor = Colors.Red;

    /// <summary>
    /// Defines the default color to use to draw the baseline and <see cref="SKTextMetrics.Left"/> lines.
    /// </summary>
    public static readonly Color DefaultBaselineColor = Colors.Blue;

    /// <summary>
    /// Defines the default color to use to draw the baseline and <see cref="SKTextMetrics.Left"/> lines.
    /// </summary>
    public static readonly Color DefaultTextColor = Colors.Black;

    /// <summary>
    /// Defines the minimum spacing around a Glyph.
    /// </summary>
    public static readonly Thickness MinimumSpacing = new(2);

    /// <summary>
    /// Defines the default spacing around a Glyph.
    /// </summary>
    public static readonly Thickness DefaultSpacing = new(5);

    /// <summary>
    /// Defines the default layout style for glyphs in the <see cref="GlyphsView"/>.
    /// </summary>
    public const GlyphLayoutStyle DefaultLayoutStyle = GlyphLayoutStyle.Default;

    // FUTURE: Support various colors, such as Glyph Baseline/Left, and boundaries
    // in the GlyphView. These would need to be Theme aware.

    #endregion Constants and Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">The parent <see cref="ISetting"/>.</param>
    public GlyphSetting(ISetting parent)
        : base(parent, nameof(UserSettings.Glyph), Strings.GlyphWidthLabel, Strings.GlyphWidthDescription)
    {
        Width = new
        (
            nameof(Width),
            DefaultWidth,
            Strings.GlyphWidthLabel,
            Strings.GlyphWidthDescription
        )
        {
            MininumValue = MinimumWidth,
            MaximumValue = MaximumWidth,
            Increment = DefaultSizeIncrment
        };
        AddItem(Width);
    }

    /// <summary>
    /// Gets the width, in pixels, to display the glyph in the <see cref="GlyphView"/>.
    /// </summary>
    public DoubleProperty Width
    {
        get;
    }

    #region PropertyChangedEventArgs

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="Width"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs WidthChangedEventArgs = new(nameof(Width));

    #endregion PropertyChangedEventArgs
}
