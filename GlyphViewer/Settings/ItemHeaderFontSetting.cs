namespace GlyphViewer.Settings;

using GlyphViewer.Resources;
using GlyphViewer.Text;
using GlyphViewer.Views;
using FontAttr = Microsoft.Maui.Controls.FontAttributes;

/// <summary>
/// Provides an <see cref="ISetting"/> for the <see cref="GlyphsView.HeaderFontSize"/>.
/// </summary>
public sealed class ItemHeaderFontSetting : FontSetting
{
    #region Constants

    /// <summary>
    /// Defines the minimum <see cref="GlyphsView.HeaderFontSize"/>.
    /// </summary>
    public const double MinimumFontSize = 8;

    /// <summary>
    /// Defines the maximum <see cref="GlyphsView.HeaderFontSize"/>.
    /// </summary>
    public const double MaximumFontSize = 40;

    /// <summary>
    /// Define the default font size.
    /// </summary>
    public const double DefaultFontSize = 20;

    /// <summary>
    /// Define the default font family name.
    /// </summary>
    public static readonly FontFamily DefaultFontFamily = Text.FontFamily.Default;

    /// <summary>
    /// Define the default <see cref="Microsoft.Maui.Controls.FontAttributes"/>.
    /// </summary>
    public const FontAttr DefaultFontAttributes = FontAttr.Bold | FontAttr.Italic;

    /// <summary>
    /// Defines the default color for the <see cref="GlyphsView.HeaderColor"/>.
    /// </summary>
    public static readonly Color DefaultTextColor = Colors.Black;

    #endregion Constants

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">The parent <see cref="ISetting"/>.</param>
    public ItemHeaderFontSetting(ISetting parent)
        : base
    (
        parent,
        nameof(UserSettings.ItemHeaderFont),
        Strings.ItemHeaderFontLabel,
        Strings.ItemHeaderFontDescription,
        DefaultFontFamily,
        DefaultFontSize,
        MinimumFontSize,
        MaximumFontSize,
        DefaultFontAttributes
    )
    {
    }
}
