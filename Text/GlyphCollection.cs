namespace GlyphViewer.Text;

using GlyphViewer.Text.Unicode;
using HarfBuzzSharp;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using HarfBuzzFont = HarfBuzzSharp.Font;

/// <summary>
/// Provides a <see cref="Glyph"/> collection for the glyphs in a <see cref="SKTypeface"/>.
/// </summary>
public sealed class GlyphCollection : IReadOnlyList<Glyph>
{
    #region Fields

    readonly List<Glyph> _glyphs;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="typeface">The <see cref="SKTypeface"/> defining the glyph.</param>
    /// <param name="glyphs">The list of glyphs in the <paramref name="typeface"/>.</param>
    /// <param name="hasGlyphNames">true if some or all of the <paramref name="glyphs"/> has
    /// a <see cref="Glyph.Name"/>; otherwise, false.</param>
    private GlyphCollection(SKTypeface typeface, List<Glyph> glyphs, bool hasGlyphNames)
    {
        _glyphs = glyphs;

        FamilyName = typeface.FamilyName;
        FontWeight = typeface.FontWeight;
        FontWidth = typeface.FontWidth;
        HasGlyphNames = hasGlyphNames;
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

    /// <summary>
    /// Gets the value indicating if some or all of the glyphs in the collection.
    /// </summary>
    /// <value>
    /// true if some or all of the glyphs in the collection have a <see cref="Glyph.Name"/>; 
    /// otherwise, false.
    /// </value>
    public bool HasGlyphNames
    {
        get;
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
    /// <param name="filter">The optional <see cref="UnicodeCategory"/> values to exclude from the results.</param>
    /// <returns>A new instance of a <see cref="GlyphCollection"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="typeface"/> is a null reference.</exception>
    public static GlyphCollection CreateInstance(SKTypeface typeface, params UnicodeCategory[] filter)
    {
        if (typeface is null)
        {
            throw new ArgumentNullException(nameof(typeface));
        }
        HarfBuzzFont hbFont = OpenFont(typeface);
        bool hasGlyphNames = false;

        List<Glyph> glyphs = new();
        for (ushort unicode = 0; unicode < 0xFFFF; unicode++)
        {
            char ch = (char)unicode;
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
            string name = string.Empty;
            if (hbFont is not null)
            {
                if (GetGlyphName(hbFont, unicode, out name))
                {
                    hasGlyphNames = true;
                }
            }
            Glyph glyph = new(typeface.FamilyName, ch, codepoint, category, range, name);
            glyphs.Add(glyph);
        }
        hbFont?.Dispose();
        return new GlyphCollection(typeface, glyphs, hasGlyphNames);
    }

    #endregion CreateInstance

    #region HarfBuzzSharp

    static bool GetGlyphName(HarfBuzzFont font, ushort unicode, out string name)
    {
        if (font.TryGetGlyph(unicode, out uint glyph) && font.TryGetGlyphName(glyph, out name))
        {
            return true;
        }
        name = string.Empty;
        return false;
    }

    static HarfBuzzFont OpenFont(SKTypeface typeface)
    {
        try
        {
            using (Blob blob = typeface.OpenStream(out int ttcIndex).ToHarfBuzzBlob())
            {
                using (Face face = new Face(blob, ttcIndex))
                {
                    face.Index = ttcIndex;
                    face.UnitsPerEm = typeface.UnitsPerEm;
                    Font font = new Font(face);
                    // TODO: ???
                    font.SetScale(512, 512);
                    font.SetFunctionsOpenType();
                    return font;
                }
            }
        }
        catch (Exception)
        {
            Trace.TraceError($"Error opening font {typeface.FamilyName} to get Glyph names.");
        }
        return null;
    }

    #endregion HarfBuzzSharp

}
