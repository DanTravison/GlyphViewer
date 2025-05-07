namespace GlyphViewer.Text;

using GlyphViewer.Settings;
using System.Collections;

/// <summary>
/// Provides a collection of <see cref="FontFamilyGroup"/> items.
/// </summary>
public sealed class FontFamilyGroupCollection : IReadOnlyList<FontFamilyGroup>
{
    #region Fields

    readonly Dictionary<string, FontFamilyGroup> _groupTable;
    readonly List<FontFamilyGroup> _groups;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="groupTable">The <see cref="Dictionary{String, FontFamilyGroup}"/>.</param>
    /// <param name="groups">The <see cref="List{FontFamilyGroup}"/> of groups.</param>
    FontFamilyGroupCollection
    (
        Dictionary<string, FontFamilyGroup> groupTable,
        List<FontFamilyGroup> groups,
        Bookmarks bookmarks
    )
    {
        Bookmarks = new List<IFontFamilyGroup>([bookmarks]);
        _groups = groups;
        _groupTable = groupTable;
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="FontFamilyGroup"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The <see cref="FontFamilyGroup.Name"/> to query.</param>
    /// <returns>
    /// The <see cref="FontFamilyGroup"/> with the specified <paramref name="name"/>; 
    /// otherwise, a null reference if the named <see cref="FontFamilyGroup"/> is not in the collection.</returns>
    public FontFamilyGroup this[string name]
    {
        get
        {
            if (_groupTable.TryGetValue(name, out FontFamilyGroup group))
            {
                return group;
            }
            return null;
        }
    }

    /// <summary>
    /// Gets the numnber of <see cref="FontFamilyGroup"/> items in the collection.
    /// </summary>
    public int Count
    {
        get => _groups.Count;
    }

    /// <summary>
    /// Gets the <see cref="FontFamilyGroup"/> at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="FontFamilyGroup"/> to get.</param>
    /// <returns>The <see cref="FontFamilyGroup"/> at the specified <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero or greater than or equal to <see cref="Count"/>.
    /// </exception>
    public FontFamilyGroup this[int index]
    {
        get => _groups[index];
    }

    /// <summary>
    /// Gets the bookmarks for the font families.
    /// </summary>
    public IReadOnlyList<IFontFamilyGroup> Bookmarks
    {
        get;
    }

    #endregion Properties

    /// <summary>
    /// Gets the <see cref="FontFamilyGroup"/> containing the specified <paramref name="familyName"/>.
    /// </summary>
    /// <param name="familyName">The family name to query.</param>
    /// <returns>
    /// The <see cref="FontFamilyGroup"/> containing the specified <paramref name="familyName"/>; otherwise, 
    /// a null reference.
    /// </returns>
    public FontFamilyGroup FromFamilyName(string familyName)
    {
        if (!string.IsNullOrEmpty(familyName))
        {
            string groupName = familyName[0].ToString();
            if (_groupTable.TryGetValue(groupName, out FontFamilyGroup group))
            {
                return group;
            }
        }
        return null;
    }

    #region IEnumerable

    /// <summary>
    /// Gets an <see cref="IEnumerator{String}"/> for enumerating the <see cref="FontFamilyGroup"/> items in the collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator{String}"/> for enumerating the <see cref="FontFamilyGroup"/> items in the collection.
    /// </returns>
    public IEnumerator<FontFamilyGroup> GetEnumerator()
    {
        return _groups.GetEnumerator();
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable"/> for enumerating the <see cref="FontFamilyGroup"/> items in the collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable"/> for enumerating the font families in the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_groups).GetEnumerator();
    }

    #endregion IEnumerable

    #region CreateInstance

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <returns>A new instance of a <see cref="FontFamilyGroupCollection"/>.</returns>
    internal static FontFamilyGroupCollection CreateInstance(Bookmarks bookmarks)
    {
        Dictionary<string, FontFamilyGroup> groupTable = new(StringComparer.OrdinalIgnoreCase);
        List<FontFamilyGroup> groups = [];

        List<string> families = Fonts.GetFontFamilies();
        // the bookmarked font families that are actually available.
        List<string> availableBookmarks = [];

        // Ensure group names and font family names are sorted.
        families.Sort(StringComparer.CurrentCultureIgnoreCase);

        foreach (string fontFamily in families)
        {
            if (bookmarks.Contains(fontFamily))
            {
                availableBookmarks.Add(fontFamily);
            }
            string groupName = fontFamily[0].ToString();
            if (!groupTable.TryGetValue(groupName, out FontFamilyGroup group))
            {
                group = new FontFamilyGroup(groupName);
                groupTable[groupName] = group;
                groups.Add(group);
            }
            group.Add(fontFamily);
        }

        bookmarks.Update(availableBookmarks);

        return new(groupTable, groups, bookmarks);
    }

    #endregion CreateInstance
}
