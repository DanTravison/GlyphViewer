namespace GlyphViewer.Text;

/// <summary>
/// Provides linear search table for a <see cref="Glyph"/> collection.
/// </summary>
internal sealed class GlyphSearchTable
{
    #region Fields

    readonly struct SearchEntry
    {
        readonly string _text;
        public SearchEntry(Glyph glyph)
        {
            if (glyph.Name is not null)
            {
                string name = glyph.Name.Trim().Replace('_', ' ');
                if (!string.IsNullOrEmpty(name))
                {
                    _text = name;
                }
            }
            Glyph = glyph;
        }

        public readonly Glyph Glyph;

        public readonly bool Match(string searchText)
        {
            return
            (
                 Glyph.Code.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                 ||
                 !string.IsNullOrEmpty(_text) && _text.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            );
        }
    }

    readonly List<SearchEntry> _items = [];
    readonly IReadOnlyList<Glyph> _glyphs;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="glyphs">The <see cref="IReadOnlyList{Glyph}"/> to use to initialize the search table.</param>
    /// <exception cref="ArgumentNullException"><paramref name="glyphs"/> is a null reference.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="glyphs"/> contains zero entries.</exception>
    public GlyphSearchTable(IReadOnlyList<Glyph> glyphs)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        if (glyphs.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(glyphs));
        }

        _glyphs = glyphs;
        for (int i = 0; i < _glyphs.Count; i++)
        {
            _items.Add(new SearchEntry(_glyphs[i]));
        }
    }

    /// <summary>
    /// Gets the number of glyphs in the search table.
    /// </summary>
    public int Count
    {
        get => _items.Count;
    }

    /// <summary>
    /// Searches the glyphs for the specified <paramref name="searchText"/>.
    /// </summary>
    /// <param name="searchText">The string text to search.</param>
    /// <returns>An <see cref="IReadOnlyList{Glyph}"/> containing zero or more entries.</returns>
    public IReadOnlyList<Glyph> Search(string searchText)
    {
        List<Glyph> results = [];

        // TODO: Consider tokenizing the search text and searching for any token.
        // Currently, the search is a single token and is order dependent.
        if (!string.IsNullOrEmpty(searchText) && searchText.Length > 0)
        {
            searchText = searchText.Trim();
            if (searchText.StartsWith("U+", StringComparison.OrdinalIgnoreCase))
            {
                searchText = searchText[2..];
            }
        }
        if (!string.IsNullOrEmpty(searchText))
        {
            for (int i = 0; i < _items.Count; i++)
            {
                SearchEntry entry = _items[i];
                if (entry.Match(searchText))
                {
                    results.Add(entry.Glyph);
                }
            }
        }
        return results;
    }
}
