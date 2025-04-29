namespace GlyphViewer.Settings;

using GlyphViewer.Resources;
using GlyphViewer.Views;
using System.ComponentModel;

/// <summary>
/// Provides an <see cref="ISetting"/> for the <see cref="GlyphView"/> width.
/// </summary>
public class GlyphWidthSetting : DoubleSetting
{
    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double Minimum = 200;

    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double Maximum = 500;

    /// <summary>
    /// Defines the default width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double Default = 300;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="settings">The containing <see cref="SettingCollection"/>.</param>
    /// <param name="eventArgs">The <see cref="PropertyChangedEventArgs"/> for the associated property.</param>
    public GlyphWidthSetting(SettingCollection settings, PropertyChangedEventArgs eventArgs)
        : base(settings, eventArgs, Default, Strings.GlyphWidthLabel, Strings.GlyphWidthDescription)
    {
        MininumValue = Minimum;
        MaximumValue = Maximum;
        Increment = 10;
    }
}
