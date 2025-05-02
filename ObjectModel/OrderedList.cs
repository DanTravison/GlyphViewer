namespace GlyphViewer.ObjectModel;

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

/// <summary>
/// This is a placeholder class used by the bookmarks class to workaround https://github.com/dotnet/maui/discussions/29221
/// </summary>
/// <typeparam name="T">The type of item in the list.</typeparam>
internal class OrderedList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    // Once https://github.com/dotnet/maui/discussions/29221 is resolved...
    //  Change OrderedList<T> to ReadOnlyOrderedList<T>
    //  - implement IReadOnlyList<T>
    //  - add protected Add/Remove methods.

    private readonly IComparer<T> _comparer;
    private readonly List<T> _items = [];

    public OrderedList(IComparer<T> comparer, IEnumerable<T> items = null)
    {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        if (items is not null)
        {
            foreach (T item in items)
            {
                AddItem(item);
            }
        }
    }

    #region Properties

    public int Count
    {
        get => _items.Count;
    }

    public bool IsReadOnly
    {
        get => false;
    }

    public T this[int index]
    {
        get => _items[index];
        set => throw new NotSupportedException();
    }

    public int IndexOf(T item)
    {
        int index = _items.BinarySearch(item, _comparer);
        if (index < 0)
        {
            index = -1;
        }
        return index;
    }

    int ItemIndex(T item)
    {
        return _items.BinarySearch(item, _comparer);
    }

    #endregion Properties

    #region Add

    public void Insert(int index, T item)
    {
        throw new NotSupportedException();
    }

    public void Add(T item)
    {
        int index = ItemIndex(item);

        if (index < 0)
        {
            index = ~index;
            _items.Insert(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            PropertyChanged?.Invoke(this, CountChangedEventArgs);
        }
        else
        {
            throw new ArgumentException("Item already exists in the list.", nameof(item));
        }
    }

    public int AddItem(T item)
    {
        int index = ItemIndex(item);
        if (index < 0)
        {
            index = ~index;
            _items.Insert(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            PropertyChanged?.Invoke(this, CountChangedEventArgs);
            return index;
        }
        return -1;
    }

    #endregion Add

    #region Remove

    public void RemoveAt(int index)
    {
        throw new NotSupportedException();
    }

    public bool Remove(T item)
    {
        throw new NotSupportedException();
    }

    public int RemoveItem(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            _items.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            PropertyChanged?.Invoke(this, CountChangedEventArgs);
            return index;
        }
        return -1;
    }

    public void Clear()
    {
        if (_items.Count > 0)
        {
            List<T> removed = new(_items);
            _items.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, 0));
            CollectionChanged?.Invoke(this, CollectionResetEventArgs);
            PropertyChanged?.Invoke(this, CountChangedEventArgs);
        }
    }

    #endregion Remove

    #region Misc

    public bool Contains(T item)
    {
        return IndexOf(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    #endregion Misc

    #region Properties

    static readonly NotifyCollectionChangedEventArgs CollectionResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    static readonly PropertyChangedEventArgs CountChangedEventArgs = new(nameof(Count));
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Properties

    #region IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_items).GetEnumerator();
    }

    #endregion IEnumerable
}

