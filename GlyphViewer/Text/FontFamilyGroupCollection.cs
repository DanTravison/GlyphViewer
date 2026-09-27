namespace GlyphViewer.Text;

using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;
using System.Collections.Specialized;

/// <summary>
/// Manages the collection of <see cref="FontFamily"/> items grouped by their first letter.
/// </summary>
/// <remarks>
/// The collection is populated with <see cref="FontFamily"/> instances installed on the system
/// and loaded from the local file system.
/// <para>
/// When fonts are loaded/unloaded from the file system, the collection and bookmarks are updated accordingly.
/// </para>
/// </remarks>
public sealed class FontFamilyGroupCollection : ReadOnlyOrderedList<FontFamilyGroup>
{
    #region Fields

    readonly Dictionary<string, FontFamilyGroup> _groupTable = new(StringComparer.OrdinalIgnoreCase);
    readonly FileFonts _fileFonts;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="bookmarks">The <see cref="Bookmarks"/> in the collection.</param>
    /// <param name="fileFonts">The <see cref="FontFamily"/> instances loaded from the local file system.</param>
    FontFamilyGroupCollection
    (
        Bookmarks bookmarks,
        FileFonts fileFonts
    )
        : base(FontFamilyGroup.Comparer, new List<FontFamilyGroup>(), true)
    {
        Bookmarks = bookmarks;
        _fileFonts = fileFonts;
        // NOTE: FileFontFamily instances were added during CreateInstance time.
        // Synchronize future changes to the collection.
        _fileFonts.CollectionChanged += OnFileFontsCollectionChanged;
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
    public Bookmarks Bookmarks
    {
        get;
    }

    /// <summary>
    /// Get the measured width, in pixels, of longest font family name.
    /// </summary>
    public double SuggestedWidth
    {
        get;
        private set;
    }

    #endregion Properties

    #region Add/Remove

    /// <summary>
    /// Adds a <see cref="FontFamily"/> to the collection.
    /// </summary>
    /// <param name="fontFamily">The <see cref="FontFamily"/> to add.</param>
    public void Add(FontFamily fontFamily)
    {
        string groupName = fontFamily.Name[0].ToString().ToUpper();
        if (!_groupTable.TryGetValue(groupName, out FontFamilyGroup group))
        {
            group = new FontFamilyGroup(groupName);
            _groupTable[groupName] = group;
            List.Add(group);
        }
        group.Add(fontFamily);
    }

    /// <summary>
    /// Removes a <see cref="FontFamily"/> from the collection.
    /// </summary>
    /// <param name="font">The <see cref="FontFamily"/> to remove.</param>
    /// <returns>true if the <paramref name="font"/> was found and removed; otherwise, false.</returns>
    public bool Remove(FontFamily font)
    {
        Bookmarks.Remove(font);

        string groupName = font.Name[0].ToString();

        if (_groupTable.TryGetValue(groupName, out FontFamilyGroup group))
        {
            group.Remove(font);
            // If the group is empty, remove it from the collection.
            if (group.Count == 0)
            {
                _groupTable.Remove(groupName);
            }
            return true;
        }
        return false;
    }

    private void OnFileFontsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (FileFontFamily file in e.NewItems)
            {
                Add(file);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (FileFontFamily file in e.OldItems)
            {
                Remove(file);
            }
        }
    }

    #endregion Add/Remove

    #region CreateInstance

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <returns>A new instance of a <see cref="FontFamilyGroupCollection"/>.</returns>
    internal static FontFamilyGroupCollection CreateInstance(UserSettings settings)
    {
        Bookmarks bookmarks = settings.Bookmarks;
        FileFonts files = settings.Fonts;
        FontFamilyGroupCollection groups = new FontFamilyGroupCollection(bookmarks, files);
        string longestName = string.Empty;

        List<FontFamily> families = Fonts.GetFontFamilies();

        // NOTE: Since the set of fonts (local and installed) may have changed since the last
        // session, build the list of bookmarks that are still valid.
        List<FontFamily> availableBookmarks = [];

        // Ensure group names and font family names are sorted.
        families.Sort(FontFamily.Comparer);

        foreach (FontFamily fontFamily in families)
        {
            if (bookmarks.Contains(fontFamily))
            {
                availableBookmarks.Add(fontFamily);
            }
            groups.Add(fontFamily);
            if (fontFamily.Name.Length >  longestName.Length)
            {
                longestName = fontFamily.Name;
            }
        }

        foreach (FileFontFamily file in files)
        {
            if (bookmarks.Contains(file))
            {
                availableBookmarks.Add(file);
            }
            groups.Add(file);
        }

        availableBookmarks.Sort(FontFamily.Comparer);

        // Update the bookmarks with the available font families.
        bookmarks.Update(availableBookmarks, true);

        // Workaround for https://github.com/dotnet/maui/issues/18700
        // [Windows] CollectionView GroupHeader doesn't fill space horizontally
        // SuggestedWidth is a workaround to ensure the header fills the space.
        groups.SuggestedWidth = (double)TextUtilities.TextWidth
        (
            longestName + "WWWWW",
            FontFamily.Default.Name,
            FontAttributes.Bold,
            14 // TODO: Read DefaultFontSize from Styles.xaml
        );
        return groups;
    }

    #endregion CreateInstance
}
