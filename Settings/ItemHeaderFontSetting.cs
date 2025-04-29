namespace GlyphViewer.Settings;

using GlyphViewer.Views;
using GlyphViewer.Resources;
using System.ComponentModel;

/// <summary>
/// Provides an <see cref="ISetting"/> for the <see cref="GlyphsView.HeaderFontSize"/>.
/// </summary>
public sealed class ItemHeaderFontSetting : FontSetting
{
    /// <summary>
    /// Defines the minimum <see cref="GlyphsView.HeaderFontSize"/>.
    /// </summary>
    public const double Minimum = 8;

    /// <summary>
    /// Defines the maximum <see cref="GlyphsView.HeaderFontSize"/>.
    /// </summary>
    public const double Maximum = 40;

    /// <summary>
    /// Define the default font size.
    /// </summary>
    public const double Default = 20;

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
    public ItemHeaderFontSetting(SettingCollection settings, PropertyChangedEventArgs eventArgs)
        : base(settings, eventArgs, Default, Strings.ItemHeaderFontSizeLabel, Strings.ItemHeaderFontSizeDescription)
    {
        MininumValue = Minimum;
        MaximumValue = Maximum;
        Text = GlyphViewer.Text.Unicode.Ranges.Latin1Supplement.Name;
        FontFamily = Strings.DefaultFontFamily;
        FontAttributes = DefaultFontAttributes;
    }
}
