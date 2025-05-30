namespace GlyphViewer.Settings;

using GlyphViewer.Resources;
using GlyphViewer.Settings.Properties;

/// <summary>
/// Provides a group of bookmarked font families.
/// </summary>
public sealed class Bookmarks : FontFamiliesSetting
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="parent"/> is a null reference.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="parent"/> does not implement <see cref="ISettingPropertyCollection"/>.
    /// </exception>
    public Bookmarks(ISetting parent)
        : base(parent, Strings.BookmarkGroupName, Strings.BookmarkGroupName)
    {
    }
}
