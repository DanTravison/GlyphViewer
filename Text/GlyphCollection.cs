namespace GlyphViewer.Text;

using GlyphViewer.Text.Unicode;
using SkiaSharp;
using System.Collections;
using System.Globalization;

/// <summary>
/// Provides a <see cref="Glyph"/> collection for the glyphs in a <see cref="SKTypeface"/>.
/// </summary>
class GlyphCollection : IReadOnlyList<Glyph>
{
    #region Fields

    readonly List<Glyph> _glyphs;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="typeface">The <see cref="SKTypeface"/> defining the glyph.</param>
    /// <param name="glyphs">The list of glyphs in the <paramref name="typeface"/>.</param>
    private GlyphCollection(SKTypeface typeface, List<Glyph> glyphs)
    {
        _glyphs = glyphs;

        FamilyName = typeface.FamilyName;
        FontWeight = typeface.FontWeight;
        FontWidth = typeface.FontWidth;
    }

    #region Properties

    /// <summary>
    /// Gets the font family name of the glyphs in the collection.
    /// </summary>
    public string FamilyName { get; }

    /// <summary>
    /// Gets the font weight of the glyphs in the collection.
    /// </summary>
    public int FontWeight { get; }

    /// <summary>
    /// Gets the font width of the glyphs in the collection.
    /// </summary>
    public int FontWidth { get; }

    /// <summary>
    /// Gets the number of glyphs in the collection.
    /// </summary>
    public int Count
    {
        get => _glyphs.Count;
    }

    /// <summary>
    /// Gets the <see cref="Glyph"/> at the specified index.
    /// </summary>
    /// <param name="index">the zero-based index of the <see cref="Glyph"/> to get.</param>
    /// <returns>The <see cref="Glyph"/> at the specified <paramref name="index"/></returns>
    public Glyph this[int index]
    {
        get => _glyphs[index];
    }

    #endregion Properties

    #region IEnumerable

    /// <summary>
    /// Gets an <see cref="IEnumerable{Glyph}"/> for enumerating the items in the collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{Glyph}"/> for enumerating the collection</returns>
    public IEnumerator<Glyph> GetEnumerator()
    {
        return _glyphs.GetEnumerator();
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable"/> for enumerating the items in the collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerable"/> for enumerating the collection</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_glyphs).GetEnumerator();
    }

    #endregion IEnumerable

    #region CreateInstance

    /// <summary>
    /// Creates a new instance of a <see cref="GlyphCollection"/>.
    /// </summary>
    /// <param name="typeface">The <see cref="SKTypeface"/> to use to populate the collection.</param>
    /// <returns>A new instance of a <see cref="GlyphCollection"/>.</returns>
    public static GlyphCollection CreateInstance(SKTypeface typeface, params UnicodeCategory[] filter)
    {
        List<Glyph> glyphs = new();
        for (ushort id = 0; id < 0xFFFF; id++)
        {
            char ch = (char)id;
            ushort codepoint = typeface.GetGlyph(ch);
            if (codepoint == 0)
            {
                continue;
            }
            Range range = Ranges.Find((ushort)ch);
            if (range is null)
            {
                continue;
            }
            UnicodeCategory category = char.GetUnicodeCategory(ch);
            if (filter != null && filter.Length > 0)
            {
                if (filter.Contains(category))
                {
                    continue;
                }
            }
            Glyph glyph = new(typeface.FamilyName, ch, codepoint, category, range);
            glyphs.Add(glyph);
        }
        return new GlyphCollection(typeface, glyphs);

    }

#endregion CreateInstance
}
