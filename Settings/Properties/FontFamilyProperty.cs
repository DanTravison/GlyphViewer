namespace GlyphViewer.Settings.Properties;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;

/// <summary>
/// Provides a <see cref="StringProperty"/> for a font family name.
/// </summary>
public sealed class FontFamilyProperty : StringProperty
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="defaultValue">The <see cref="ObservableProperty{String}.DefaultValue"/>.</param>
    public FontFamilyProperty(string defaultValue = App.DefaultFontFamily)
        : base(nameof(FontSetting.FontFamily), defaultValue, Strings.FamilyNameLabel, Strings.FamilyNameDescription)
    {
    }
}
