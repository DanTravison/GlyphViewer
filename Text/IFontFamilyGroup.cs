namespace GlyphViewer.Text;

/// <summary>
/// Provides a collection of font family names.
/// </summary>
public interface IFontFamilyGroup : IReadOnlyList<string>
{
    /// <summary>
    /// Gets the name of the group.
    /// </summary>
    string Name { get; init; }

    /// <summary>
    /// adds a font family name to the group.
    /// </summary>
    /// <param name="familyName">The name of the font family.</param>
    /// <returns>true if the <paramref name="familyName"/> was added; otherwise, 
    /// false if the <paramref name="familyName"/> is already present.</returns>
    bool Add(string familyName);
}
