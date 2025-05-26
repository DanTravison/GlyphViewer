namespace GlyphViewer.Text;

using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;

/// <summary>
/// Provides a collection of <see cref="FontFamilyGroup"/> items.
/// </summary>
public sealed class FontFamilyGroupCollection : ReadOnlyOrderedList<FontFamilyGroup>
{
    #region Fields

    readonly Dictionary<string, FontFamilyGroup> _groupTable;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="groupTable">The <see cref="Dictionary{String, FontFamilyGroup}"/>.</param>
    /// <param name="groups">The sorted <see cref="List{FontFamilyGroup}"/> of groups.</param>
    /// <param name="bookmarks">The <see cref="Bookmarks"/> in the collection.</param>
    FontFamilyGroupCollection
    (
        Dictionary<string, FontFamilyGroup> groupTable,
        List<FontFamilyGroup> groups,
        Bookmarks bookmarks
    )
        : base(FontFamilyGroup.Comparer, groups, true)
    {
        Bookmarks = new List<IFontFamilyGroup>([bookmarks]);
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

    #region CreateInstance

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <returns>A new instance of a <see cref="FontFamilyGroupCollection"/>.</returns>
    internal static FontFamilyGroupCollection CreateInstance(Bookmarks bookmarks)
    {
        Dictionary<string, FontFamilyGroup> groupTable = new(StringComparer.OrdinalIgnoreCase);
        List<FontFamilyGroup> groups = [];

        List<FontFamily> families = Fonts.GetFontFamilies();
        // the bookmarked font families that are actually available.
        List<FontFamily> availableBookmarks = [];

        // Ensure group names and font family names are sorted.
        families.Sort(FontFamily.Comparer);

        foreach (FontFamily fontFamily in families)
        {
            if (bookmarks.Contains(fontFamily))
            {
                availableBookmarks.Add(fontFamily);
            }
            string groupName = fontFamily.Name[0].ToString();
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
