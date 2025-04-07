namespace GlyphViewer.Text;

using System.Collections;

/// <summary>
/// Provides a grouped collection of font families
/// </summary>
public sealed class FontFamilyCollection : IReadOnlyCollection<string>
{
    class GroupComparer : IComparer<FontFamilyGroup>
    {
        public static readonly GroupComparer Comparer = new();

        private GroupComparer()
        { }

        public int Compare(FontFamilyGroup x, FontFamilyGroup y)
        {
            return string.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    #region Fields

    readonly Dictionary<string, FontFamilyGroup> _groupTable;
    readonly List<string> _families;
    readonly List<FontFamilyGroup> _groups;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="groupTable">The <see cref="Dictionary{String, FontFamilyGroup}"/>.</param>
    /// <param name="groups">The <see cref="List{FontFamilyGroup}"/> of groups.</param>
    /// <param name="families">The <see cref="List{String}"/> of font families.</param>
    FontFamilyCollection(Dictionary<string, FontFamilyGroup> groupTable, List<FontFamilyGroup> groups, List<string> families)
    {
        _families = families;
        _groups = groups;
        _groupTable = groupTable;
    }

    #region Properties

    /// <summary>
    /// Gets the number of font families in the collection.
    /// </summary>
    public int Count
    {
        get => _families.Count;
    }

    /// <summary>
    /// Gets the font family at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the font family to get.</param>
    /// <returns>The string font family at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The index is less than zero or greater than or equal to <see cref="Count"/>.
    /// </exception>"
    public string this[int index]
    {
        get
        {
            if (index < 0 || index >= _families.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _families[index];
        }
    }

    /// <summary>
    /// Gets the <see cref="FontFamilyGroup"/> items in the collection.
    /// </summary>
    public IReadOnlyList<FontFamilyGroup> Groups
    {
        get => _groups;
    }

    /// <summary>
    /// Gets the font family group by name.
    /// </summary>
    /// <param name="name">The name of the group to get.
    /// <remarks>
    /// A group name is the first character of the font family name.
    /// </remarks>
    /// </param>
    /// <returns>
    /// A <see cref="FontFamilyGroup"/> object if the group exists; otherwise, a null reference."/>
    /// </returns>
    public FontFamilyGroup this[string name]
    {
        get
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (_groupTable.TryGetValue(name, out FontFamilyGroup group))
                {
                    return group;
                }
            }
            return null;
        }
    }

    #endregion Properties

    #region IEnumerable

    /// <summary>
    /// Gets an <see cref="IEnumerator{String}"/> for enumerating the font families in the collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator{String}"/> for enumerating the font families in the collection
    /// </returns>
    public IEnumerator<string> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable"/> for enumerating the font families in the collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable"/> for enumerating the font families in the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion IEnumerable

    #region CreateInstance

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <returns>A new instance of a <see cref="FontFamilyCollection"/>.</returns>
    public static FontFamilyCollection CreateInstance()
    {
        Dictionary<string, FontFamilyGroup> groupTable = new(StringComparer.OrdinalIgnoreCase);
        List<string> families = [];
        List<FontFamilyGroup> groups = [];

        foreach (string fontFamily in Fonts.GetFontFamilies())
        {
            string groupName = fontFamily[0].ToString();
            if (!groupTable.TryGetValue(groupName, out FontFamilyGroup group))
            {
                group = new FontFamilyGroup(groupName);
                groupTable[groupName] = group;
                groups.Add(group);
            }
            group.Add(fontFamily);
            families.Add(fontFamily);
        }
        groups.Sort(GroupComparer.Comparer);
        families.Sort(StringComparer.CurrentCultureIgnoreCase);

        return new(groupTable, groups, families);
    }

    #endregion CreateInstance
}
