namespace GlyphViewer.Text;

using System.Collections;

/// <summary>
/// Provides a group of font families.
/// </summary>
public sealed class FontFamilyGroup : IReadOnlyCollection<string>
{
    readonly List<string> _families = new();

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
    /// <returns>The font family at the specified <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero or greater than or equal to <see cref="Count"/>.
    /// </exception>
    public string this[int index]
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
    /// The specified <paramref name="family"/> is a
    /// null reference or empty string.
    /// </exception>
    public void Add(string family)
    {
        if (string.IsNullOrEmpty(family))
        {
            throw new ArgumentNullException(nameof(family));
        }
        _families.Add(family);
    }

    /// <summary>
    /// Sort the font families in the group.
    /// </summary>
    /// <param name="comparer">The <see cref="IComparer{T}"/> to use to sort the values.</param>
    public void Sort(IComparer<string> comparer)
    {
        _families.Sort(comparer);
    }

    #endregion Methods

    #region IEnumerable

    /// <summary>
    /// Gets an <see cref="IEnumerator{String}"/> for enumerating the font families in the group.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator{String}"/> for enumerating the font families in the group.
    /// </returns>
    public IEnumerator<string> GetEnumerator()
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