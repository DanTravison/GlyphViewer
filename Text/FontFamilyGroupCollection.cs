namespace GlyphViewer.Text;

using System.Collections;

/// <summary>
/// Provides a collection of <see cref="FontFamilyGroup"/> items.
/// </summary>
public sealed class FontFamilyGroupCollection : IReadOnlyCollection<FontFamilyGroup>
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
    FontFamilyGroupCollection(Dictionary<string, FontFamilyGroup> groupTable, List<FontFamilyGroup> groups)
    {
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

    #endregion Properties

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
    public static FontFamilyGroupCollection CreateInstance()
    {
        Dictionary<string, FontFamilyGroup> groupTable = new(StringComparer.OrdinalIgnoreCase);
        List<FontFamilyGroup> groups = [];

        List<string> families = Fonts.GetFontFamilies();
        // Ensure group names and font familyl names are sorted.
        families.Sort(StringComparer.CurrentCultureIgnoreCase);

        foreach (string fontFamily in families)
        {
            string groupName = fontFamily[0].ToString();
            if (!groupTable.TryGetValue(groupName, out FontFamilyGroup group))
            {
                group = new FontFamilyGroup(groupName);
                groupTable[groupName] = group;
                groups.Add(group);
            }
            group.Add(fontFamily);
        }
        return new(groupTable, groups);
    }

    #endregion CreateInstance
}
