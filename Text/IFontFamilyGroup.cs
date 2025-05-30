namespace GlyphViewer.Text;

/// <summary>
/// Provides a collection of font family names.
/// </summary>
public interface IFontFamilyGroup : IReadOnlyList<FontFamily>
{
    /// <summary>
    /// Gets the name of the group.
    /// </summary>
    string Name { get; init; }

    /// <summary>
    /// adds a font family name to the group.
    /// </summary>
    /// <param name="fontFamily">The <see cref="FontFamily"/> to add.</param>
    /// <returns>true if the <paramref name="fontFamily"/> was added; otherwise, 
    /// false if the <paramref name="fontFamily"/> is already present.</returns>
    bool Add(FontFamily fontFamily);
}
