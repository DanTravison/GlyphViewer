namespace GlyphViewer.Text;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

/// <summary>
/// Provides a group of bookmarked font families.
/// </summary>
public sealed class FontFamilyBookmarks : ReadOnlyCollection<string>, IFontFamilyGroup, INotifyCollectionChanged, INotifyPropertyChanged
{
    // ISSUE: CollectionView will not subscribe to CollectionChanged events
    // for a class that implements IReadOnlyList<T> and INotifyCollectionChanged.
    // However, it will subscribe to a class derived from ReadOnlyCollection<T>.
    // See https://github.com/dotnet/maui/discussions/29221
    // Once this is resolved, revert ReadOnlyCollection to IReadOnlyCollection<T>

    #region Fields

    readonly OrderedList<string> _families;
    readonly IComparer<string> _comparer = StringComparer.CurrentCulture;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public FontFamilyBookmarks()
        : base(new OrderedList<string>(StringComparer.CurrentCulture))
    {
        _families = base.Items as OrderedList<string>;
        Name = Strings.BookmarkGroupName;
    }

    #region Properties

    /// <summary>
    /// Gets the name of the group.
    /// </summary>
    public string Name
    {
        get;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Adds a new font family to the collection.
    /// </summary>
    /// <param name="familyName">The name of the font family to add.</param>
    /// <returns>true if the <paramref name="familyName"/> was added; otherwise, 
    /// false if the <paramref name="familyName"/> already exists in the collection.
    /// </returns>
    public bool Add(string familyName)
    {
        int index = _families.AddItem(familyName);
        if (index >= 0)
        {
            OnItemAdded(index, familyName);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Replaces the collection contents with the specified <paramref name="families"/>.
    /// </summary>
    /// <param name="families">The <see cref="IEnumerable{T}"/> for the items to use to populate the collection.</param>
    public void Update(IEnumerable<string> families)
    {
        _families.Clear();
        foreach (string familyName in families)
        {
            _families.AddItem(familyName);
        }
        NotifyCollectionChangedEventHandler handler = CollectionChanged;
        if (handler is not null && _families.Count > 0)
        {
            handler?.Invoke(this, new(NotifyCollectionChangedAction.Add, _families, 0));
        }
    }

    /// <summary>
    /// Removes a font family from the collection.
    /// </summary>
    /// <param name="familyName">The name of the font family to remove.</param>
    /// <returns>true if the <paramref name="familyName"/> was removed; otherwise,
    /// false if the <paramref name="familyName"/> does not exist in the collection.
    /// </returns>
    public bool Remove(string familyName)
    {
        int index = _families.RemoveItem(familyName);
        if (index >= 0)
        {
            OnItemRemoved(index, familyName);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear()
    {
        OnClear();
    }

    #endregion Methods

    #region INotifyPropertyChanged

    static readonly PropertyChangedEventArgs CountChangedEventArgs = new(nameof(Count));

    /// <summary>
    /// Occurs when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion INotifyPropertyChanged

    #region INotifyCollectionChanged

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    static readonly NotifyCollectionChangedEventArgs CollectionResetEventArgs = new(NotifyCollectionChangedAction.Reset);

    #endregion INotifyCollectionChanged

    #region Collection Updates

    void OnItemAdded(int index, string item)
    {
        NotifyCollectionChangedEventHandler handler = CollectionChanged;
        if (handler is not null)
        {
            handler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        PropertyChanged?.Invoke(this, CountChangedEventArgs);
    }

    void OnItemRemoved(int index, string item)
    {
        NotifyCollectionChangedEventHandler handler = CollectionChanged;
        if (handler is not null)
        {
            handler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }
        PropertyChanged?.Invoke(this, CountChangedEventArgs);
    }

    void OnClear()
    {
        NotifyCollectionChangedEventHandler handler = CollectionChanged;
        List<string> removed = null;
        if (handler is not null)
        {
            removed = new(_families);
        }
        _families.Clear();
        if (handler is not null)
        {
            handler.Invoke(this, new(NotifyCollectionChangedAction.Remove, removed, 0));
            handler.Invoke(this, CollectionResetEventArgs);
        }
        PropertyChanged?.Invoke(this, CountChangedEventArgs);
    }

    #endregion Collection Updates
}
