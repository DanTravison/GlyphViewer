namespace GlyphViewer.Text;

using GlyphViewer.ObjectModel;

/// <summary>
/// Provides a group of font families.
/// </summary>
public sealed class FontFamilyGroup : ReadOnlyOrderedList<FontFamily>, IFontFamilyGroup
{
    class FontFamilyGroupComparer : IComparer<FontFamilyGroup>
    {
        public int Compare(FontFamilyGroup x, FontFamilyGroup y)
        {
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Provides an <see cref="IComparer{FontFamilyGroup}"/> for comparing <see cref="FontFamilyGroup"/> instances."/>
    /// </summary>
    public static readonly IComparer<FontFamilyGroup> Comparer = new FontFamilyGroupComparer();

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="name">The name of the group.</param>
    public FontFamilyGroup(string name)
        : base(FontFamily.Comparer)
    {
        Name = name;
    }

    #region Properties

    /// <summary>
    /// Gets the name of the group.
    /// </summary>
    public string Name
    {
        get;
        init;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Adds a font family to the group.
    /// </summary>
    /// <param name="family">The font family to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="family"/> is a null reference.</exception>
    public bool Add(FontFamily family)
    {
        ArgumentNullException.ThrowIfNull(family, nameof(family));
        List.Add(family);
        return true;
    }

    /// <summary>
    /// Removes a font family from the group.
    /// </summary>
    /// <param name="family">The font family to add.</param>
    /// <returns>true if the <paramref name="family"/> was found and removed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="family"/> is a null reference.</exception>
    public bool Remove(FontFamily family)
    {
        ArgumentNullException.ThrowIfNull(family, nameof(family));
        return List.Remove(family);
    }

    #endregion Methods
}