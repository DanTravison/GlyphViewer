namespace GlyphViewer.Settings;

using GlyphViewer.Views;
using GlyphViewer.Resources;
using System.ComponentModel;

/// <summary>
/// Provides an <see cref="ISetting"/> for the <see cref="GlyphsView.ItemFontSize"/>.
/// </summary>
public sealed class ItemFontSetting : FontSetting
{
    /// <summary>
    /// Defines the minimum <see cref="FontSetting.FontSize"/>.
    /// </summary>
    public const double Minimum = 12;

    /// <summary>
    /// Defines the maximum <see cref="FontSetting.FontSize"/>.
    /// </summary>
    public const double Maximum = 40;

    /// <summary>
    /// Defines the default <see cref="FontSetting.FontSize"/>.
    /// </summary>
    public const double Default = 32;

    const string SampleItemFontText =
        FluentUI.ArrowExportRTL + " "
        + FluentUI.MusicNote1 + " "
        + FluentUI.MusicNote2 + " "
        + FluentUI.ArrowExportLTR;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="settings">The containing <see cref="SettingCollection"/>.</param>
    /// <param name="eventArgs">The <see cref="PropertyChangedEventArgs"/> for the associated property.</param>
    public ItemFontSetting(SettingCollection settings, PropertyChangedEventArgs eventArgs)
        : base(settings, eventArgs, Default, Strings.ItemFontSizeLabel, Strings.ItemFontSizeDescription)
    {
        MininumValue = Minimum;
        MaximumValue = Maximum;
        Text = SampleItemFontText;
        FontFamily = FluentUI.FamilyName;
        FontAttributes = FontAttributes.None;
    }
}
