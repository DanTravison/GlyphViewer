namespace GlyphViewer.Settings;

using GlyphViewer.Resources;
using GlyphViewer.Settings.Properties;
using GlyphViewer.Text;

/// <summary>
/// Provides a <see cref="FontFamiliesSetting"/> for <see cref="FontFile"/> instances.
/// </summary>
internal class FileFonts : FontFamiliesSetting
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">The <see cref="ISetting"/> parent.</param>
    /// <exception cref="ArgumentNullException"><paramref name="parent"/> is a null reference.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="parent"/> does not implement <see cref="ISettingPropertyCollection"/>.
    /// </exception>
    public FileFonts(ISetting parent)
        : base(parent, nameof(Fonts), Strings.FontFilesName, Strings.FontFilesDescription, false)
    {
    }

    /// <summary>
    /// Adds a <see cref="FileFontFamily"/> to the collection.
    /// </summary>
    /// <param name="fontFamily">The <see cref="FileFontFamily"/> to add.</param>
    /// <returns>true if the <paramref name="fontFamily"/> was added; otherwise, 
    /// false if the <paramref name="fontFamily"/> already exists in the collection.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="fontFamily"/> must be a <see cref="FileFontFamily"/>.</exception>
    public override bool Add(FontFamily fontFamily)
    {
        if (fontFamily is not FileFontFamily)
        {
            throw new ArgumentException("Only FileFont instances can be added to FileFonts.", nameof(fontFamily));
        }
        return base.Add(fontFamily);
    }
}
