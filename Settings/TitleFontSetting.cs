namespace GlyphViewer.Settings;

using GlyphViewer.Resources;
using GlyphViewer.Views;
using System.ComponentModel;

public class TitleFontSetting : FontSetting
{
    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double Minimum = 20;

    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double Maximum = 50;

    /// <summary>
    /// Define the default font size of the main page header text.
    /// </summary>
    public const double Default = 32;

    /// <summary>
    /// Define the default font family name.
    /// </summary>
    public const string DefaultFontFamily = App.DefaultFontFamily;

    /// <summary>
    /// Define the default <see cref="FontAttributes"/>.
    /// </summary>
    public const FontAttributes DefaultFontAttributes = FontAttributes.Bold | FontAttributes.Italic;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="settings">The containing <see cref="SettingCollection"/>.</param>
    /// <param name="eventArgs">The <see cref="PropertyChangedEventArgs"/> for the associated property.</param>
    public TitleFontSetting(SettingCollection settings, PropertyChangedEventArgs eventArgs)
        : base(settings, eventArgs, Default, Strings.TitleFontSizeLabel, Strings.TitleFontSizeDescription)
    {
        MininumValue = Minimum;
        MaximumValue = Maximum;
        Text = DefaultFontFamily;
        FontFamily = DefaultFontFamily;
        FontAttributes = DefaultFontAttributes;
    }
}
