namespace GlyphViewer.ObjectModel;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// This is a placeholder class used by the bookmarks class to workaround https://github.com/dotnet/maui/discussions/29221
/// </summary>
/// <typeparam name="T">The type of item in the list.</typeparam>
public sealed class OrderedList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<T>
{
    // Once https://github.com/dotnet/maui/discussions/29221 is resolved...
    //  Change OrderedList<T> to ReadOnlyOrderedList<T>
    //  - implement IReadOnlyList<T>
    //  - add protected Add/Remove methods.

    private readonly IComparer<T> _comparer;
    private readonly List<T> _items = [];

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="comparer">The <see cref="IComparer{T}"/> to use to order the items in the list.</param>
    /// <param name="items">The optional <see cref="IEnumerable{T}"/> to use to populate the list.</param>
    /// <param name="isSorted">true if <paramref name="items"/> are sorted; otherwise, false.</param>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is a null reference.</exception>
    public OrderedList(IComparer<T> comparer, IEnumerable<T> items = null, bool isSorted = false)
    {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        if (items is not null)
        {
            SetItems(items, isSorted);
        }
    }

    #region Properties

    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    public int Count
    {
        get => _items.Count;
    }

    /// <summary>
    /// Gets the value indicating whether the list is read-only.
    /// </summary>
    /// <value>
    /// true if the list is read-only; otherwise, false.
    /// </value>
    public bool IsReadOnly
    {
        get;
    }

    /// <summary>
    /// Gets the item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to get.</param>
    /// <returns>The <typeparamref name="T"/> item at the specified <paramref name="index"/>.</returns>
    /// <exception cref="NotSupportedException">The list does not support setting an item at an arbitrary index.</exception>
    public T this[int index]
    {
        get => _items[index];
        set => throw new NotSupportedException();
    }

    /// <summary>
    /// Gets the index of the specified item in the list.
    /// </summary>
    /// <param name="item">The <typeparamref name="T"/> to query.</param>
    /// <returns>
    /// The zero-based index of the item in the list; otherwise, 
    /// -1 if the item is not present.</returns>
    public int IndexOf(T item)
    {
        int index = ItemIndex(item);
        if (index < 0)
        {
            index = -1;
        }
        return index;
    }

    /// <summary>
    /// Gets the value indicating if the list contains the specified item.
    /// </summary>
    /// <param name="item">the <typeparamref name="T"/> to query.</param>
    /// <returns>true if the list contains the <paramref name="item"/>; otherwise, false.</returns>
    public bool Contains(T item)
    {
        return ItemIndex(item) >= 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int ItemIndex(T item)
    {
        return _items.BinarySearch(item, _comparer);
    }

    #endregion Properties

    #region Add

    /// <summary>
    /// Inserts an item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index to insert.</param>
    /// <param name="item">The <typeparamref name="T"/> to insert.</param>
    /// <exception cref="NotSupportedException">Items cannot be set at arbitrary index. Use Add instead.</exception>
    public void Insert(int index, T item)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Adds an item to the list.
    /// </summary>
    /// <param name="item">The <typeparamref name="T"/> item to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is a null reference.</exception>
    public void Add(T item)
    {
        AddItem(item);
    }

    /// <summary>
    /// Adds an item to the list and returns the index at which it was added.
    /// </summary>
    /// <param name="item">The <typeparamref name="T"/> item to add.</param>
    /// <returns>The zero-based index where the item was inserted.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is a null reference.</exception>
    public int AddItem(T item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        int index = ItemIndex(item);
        if (index < 0)
        {
            index = ~index;
        }
        _items.Insert(index, item);

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        PropertyChanged?.Invoke(this, CountChangedEventArgs);
        return index;
    }

    /// <summary>
    /// Adds a range of items to the list.
    /// </summary>
    /// <param name="items">The <see cref="IEnumerable{T}"/> for the items to add.</param>
    /// <exception cref="ArgumentNullException">one or more items in <paramref name="items"/> is a null reference.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        if (items is not null)
        {
            foreach (T item in items)
            {
                AddItem(item);
            }
        }
    }

    void SetItems(IEnumerable<T> items, bool isSorted)
    {
        _items.Clear();
        if (items is not null)
        {
            _items.AddRange(items);
            if (!isSorted)
            {
                _items.Sort(_comparer);
            }
        }
    }

    /// <summary>
    /// Updates the contents of the list with the specified items.
    /// </summary>
    /// <param name="items">The <typeparamref name="T"/> to use to populate the list.</param>
    /// <param name="isSorted">true if <paramref name="items"/> are sorted, otherwise, false.</param>
    internal void Update(IEnumerable<T> items, bool isSorted)
    {
        if (items is not null)
        {
            SetItems(items, isSorted);
            if (_items.Count > 0)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, 0));
                PropertyChanged?.Invoke(this, CountChangedEventArgs);
            }
        }
        else
        {
            Clear();
        }
    }

    #endregion Add

    #region Remove

    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        T item = _items[index];
        _items.RemoveAt(index);
        OnItemRemoved(item, index);
    }

    /// <summary>
    /// Removes the first occurrence of a specific item from the list.
    /// </summary>
    /// <param name="item">The <typeparamref name="T"/> item to remove.</param>
    /// <returns>true if the <paramref name="item"/> was removed; otherwise, false.</returns>
    public bool Remove(T item)
    {
        int index = ItemIndex(item);
        if (index >= 0)
        {
            _items.RemoveAt(index);
            OnItemRemoved(item, index);
            return true;
        }
        return false;
    }

    void OnItemRemoved(T item, int index)
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        PropertyChanged?.Invoke(this, CountChangedEventArgs);
    }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
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

    #region CopyTo

    /// <summary>
    /// Copies the entire list to a compatible one-dimensional array
    /// </summary>
    /// <param name="array">The array to copy to.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is a null reference.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
    /// <exception cref="ArgumentException">
    /// The number of elements in the list is greater than the available space from arrayIndex to the end of the destination array.
    /// </exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Copies a range of elements from the list to a compatible one-dimensional array.
    /// </summary>
    /// <param name="index">The zero-based index in the list at which copying begins.</param>
    /// <param name="array">The one-dimensional Array to copy to. 
    /// <para>
    /// The Array must have zero-based indexing.
    /// </para>
    /// </param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <param name="count">The number of elements to copy.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is a null reference.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero, 
    /// -or- 
    /// <paramref name="arrayIndex"/> is less than zero,
    /// -or-
    /// <paramref name="count"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="index"/> is equal to or greater than the <see cref="Count"/>
    /// -or-
    /// The number of elements from <paramref name="index"/> to the end of the list 
    /// is greater than the available space from <paramref name="arrayIndex"/> to the end of the <paramref name="array"/>.
    /// </exception>
    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        _items.CopyTo(index, array, arrayIndex, count);
    }

    #endregion CopyTo

    #region Properties

    static readonly NotifyCollectionChangedEventArgs CollectionResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

    /// <summary>
    /// Occcurs when the contents of the list change.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    static readonly PropertyChangedEventArgs CountChangedEventArgs = new(nameof(Count));

    /// <summary>
    /// Occurs when a property of the list changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Properties

    #region IEnumerable

    /// <summary>
    /// Gets an  <see cref="IEnumerator{T}"/> to enumerate all <typeparamref name="T"/> items in the list.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/>.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    /// <summary>
    /// Gets an  <see cref="IEnumerator"/> to enumerate all <typeparamref name="T"/> items in the list.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/>.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    #endregion IEnumerable
}

