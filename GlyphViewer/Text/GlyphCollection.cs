namespace GlyphViewer.Text;

using CodeCadence.Typography.OpenType;
using CodeCadence.Typography.Text;
using CodeCadence.Typography.Unicode;
using SkiaSharp;
using System.Collections;
using System.Globalization;
using UnicodeRange = CodeCadence.Typography.Unicode.Range;

/// <summary>
/// Provides a <see cref="Glyph"/> searchable collection for the glyphs in a <see cref="SKTypeface"/>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{FamilyName,nq}[{Count,nq}]")]
public sealed class GlyphCollection : IReadOnlyList<Glyph>
{
    #region Fields

    readonly List<Glyph> _glyphs;
    readonly GlyphSearchTable _searchTable;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="fontFamily">The <see cref="Text.FontFamily"/> defining the glyph.</param>
    /// <param name="glyphs">The list of glyphs in the <paramref name="fontFamily"/>.</param>
    private GlyphCollection(FontFamily fontFamily, List<Glyph> glyphs, HashSet<Range> ranges)
    {
        _glyphs = glyphs;
        FontFamily = fontFamily;
        UnicodeRanges = new List<Range>(ranges);
        _searchTable = new(glyphs);
    }

    #region Properties

    /// <summary>
    /// Gets the font family name.
    /// </summary>
    public FontFamily FontFamily
    {
        get;
    }

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

    /// <summary>
    /// Gets the <see cref="UnicodeRange"/>s for the glyphs in the collection.
    /// </summary>
    public IReadOnlyList<UnicodeRange> UnicodeRanges
    {
        get;
    }

    #endregion Properties

    #region Search

    /// <summary>
    /// Searches the Glyphs for the specified <paramref name="searchText"/>.
    /// </summary>
    /// <param name="searchText">The string search text to search for.</param>
    /// <returns>An <see cref="IReadOnlyList{Glyph}"/> containing zero or more entries.</returns>
    /// <remarks>
    /// This method performs a linear search of all glyphs in the collection.
    /// </remarks>
    public IReadOnlyList<Glyph> Search(string searchText)
    {
        return _searchTable.Search(searchText);
    }

    #endregion Search

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
    /// <param name="fontFamily">The <see cref="FontFamily"/> to use to populate the collection.</param>
    /// <param name="filter">The optional <see cref="UnicodeCategory"/> values to exclude from the results.</param>
    /// <returns>A new instance of a <see cref="GlyphCollection"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fontFamily"/> is a null reference.</exception>
    public static GlyphCollection CreateInstance(FontFamily fontFamily, params UnicodeCategory[] filter)
    {
        FontReader reader;
        if (fontFamily is FileFontFamily file)
        {
            reader = FontReader.CreateInstance(file.FilePath);
        }
        else
        {
            SKTypeface typeface = fontFamily.GetTypeface(SKFontStyle.Normal);
            if (typeface is null)
            {
                return null;
            }
            reader = FontReader.CreateInstance(typeface);
        }

        using (reader)
        {
            List<Glyph> glyphs = [];
            HashSet<Range> ranges = new(RangeComparer.Comparer);

            foreach (GlyphInfo info in reader.GetGlyphs())
            {
                if (!ranges.Contains(info.Range))
                {
                    ranges.Add(info.Range);
                }
                Glyph glyph = new(fontFamily.Name, info);

                glyphs.Add(glyph);
            }

            return new GlyphCollection(fontFamily, glyphs, ranges);
        }
    }

    #endregion CreateInstance
}
