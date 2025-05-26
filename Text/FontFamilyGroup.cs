namespace GlyphViewer.Text;

using System.Collections;

/// <summary>
/// Provides a group of font families.
/// </summary>
public sealed class FontFamilyGroup : IReadOnlyCollection<FontFamily>, IFontFamilyGroup
{
    readonly List<FontFamily> _families = [];

    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="name">The name of the group.</param>
    public FontFamilyGroup(string name)
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

    /// <summary>
    /// Gets the number of font families in the group.
    /// </summary>
    public int Count
    {
        get => _families.Count;
    }

    /// <summary>
    /// Gets the font family at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the font family to get.</param>
    /// <returns>The <see cref="FontFamily"/> at the specified <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero or greater than or equal to <see cref="Count"/>.
    /// </exception>
    public FontFamily this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _families[index];
        }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Adds a font family to the group.
    /// </summary>
    /// <param name="family">The font family to add.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="family"/> is a null reference.
    /// </exception>
    public bool Add(FontFamily family)
    {
        ArgumentNullException.ThrowIfNull(family, nameof(family));
        _families.Add(family);
        return true;
    }

    #endregion Methods

    #region IEnumerable

    /// <summary>
    /// Gets an <see cref="IEnumerator{FontFamily}"/> for enumerating the font families in the group.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator{String}"/> for enumerating the font families in the group.
    /// </returns>
    public IEnumerator<FontFamily> GetEnumerator()
    {
        return _families.GetEnumerator();
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable"/> for enumerating the font families in the collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable"/> for enumerating the font families in the collection
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_families).GetEnumerator();
    }

    #endregion IEnumerable
}