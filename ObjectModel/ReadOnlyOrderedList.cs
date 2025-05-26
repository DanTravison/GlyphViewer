namespace GlyphViewer.ObjectModel;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

/// <summary>
/// Encapsulates an <see cref="OrderedList{T}"/> as a <see cref="ReadOnlyCollection{T}"/> that supports change notifications.
/// </summary>
/// <typeparam name="T">The type of item in the list.</typeparam>
public class ReadOnlyOrderedList<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="comparer">The <see cref="IComparer{T}"/> to use to order the list.</param>
    /// <param name="items">The optional <see cref="IEnumerable{T}"/> to use to populate the list.</param>
    /// <param name="presorted">true if <paramref name="items"/> are presorted; otherwise, false.</param>
    /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is a null reference.</exception>
    protected ReadOnlyOrderedList(IComparer<T> comparer, IEnumerable<T> items = null, bool presorted = false) 
        : base(new OrderedList<T>(comparer, items, presorted))
    {
        List = (OrderedList<T>)Items;
        List.CollectionChanged += OnListCollectionChanged;
        List.PropertyChanged += OnListPropertyChanged;
    }

    /// <summary>
    /// Gets the encapsulated <see cref="OrderedList{T}"/>.
    /// </summary>
    protected OrderedList<T> List
    {
        get;
    }

    #region INotifyPropertyChanged

    /// <summary>
    /// Occurs when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Handles the <see cref="INotifyPropertyChanged.PropertyChanged"/> event from the <see cref="List"/>.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> containing the event details.</param>
    private void OnListPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Override in the derived class to handle property changes.
    /// </summary>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> containing the event details.</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
    }

    #endregion INotifyPropertyChanged

    #region INotifyCollectionChanged

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary>
    /// Handles the <see cref="INotifyCollectionChanged.CollectionChanged"/> event from the <see cref="List"/>.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> containing the event details.</param>
    private void OnListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
        OnCollectionChanged(e);
    }

    /// <summary>
    /// Override in the derived class to handle collection changes.
    /// </summary>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> containing the event details.</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
    }

    #endregion INotifyCollectionChanged
}
