namespace GlyphViewer.Text;

/// <summary>
/// Provides a collection of font family names.
/// </summary>
public interface IFontFamilyGroup : IReadOnlyList<string>
{
    string Name { get; }
}
