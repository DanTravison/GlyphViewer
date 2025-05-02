namespace GlyphViewer.Settings;

using GlyphViewer.Resources;
using FontAttr = Microsoft.Maui.Controls.FontAttributes;

/// <summary>
/// Provides a <see cref="FontSetting"/> for the main page title.
/// </summary>
public class TitleFontSetting : FontSetting
{
    /// <summary>
    /// Defines the minimum font size of the main page title.
    /// </summary>
    public const double MinimumFontSize = 20;

    /// <summary>
    /// Defines the minimum font size of the main page title.
    /// </summary>
    public const double MaximumFontSize = 50;

    /// <summary>
    /// Define the default font size of the main page title.
    /// </summary>
    public const double DefaultFontSize = 32;

    /// <summary>
    /// Define the default font family name.
    /// </summary>
    public const string DefaultFontFamily = App.DefaultFontFamily;

    /// <summary>
    /// Define the default <see cref="Microsoft.Maui.Controls.FontAttributes"/>.
    /// </summary>
    public const FontAttr DefaultFontAttributes = FontAttr.Bold | FontAttr.Italic;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">The parent <see cref="ISetting"/>.</param>
    public TitleFontSetting(ISetting parent)
        : base
        (
            parent,
            nameof(UserSettings.TitleFont),
            Strings.TitleFontLabel,
            Strings.TitleFontDescription,
            DefaultFontFamily,
            DefaultFontSize,
            MinimumFontSize,
            MaximumFontSize,
            DefaultFontAttributes
        )
    {
    }
}
