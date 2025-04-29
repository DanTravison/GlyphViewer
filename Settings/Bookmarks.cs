using GlyphViewer.Text;

namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;

/// <summary>
/// Provides a group of bookmarked font families.
/// </summary>
public sealed class Bookmarks : ReadOnlyCollection<string>, IFontFamilyGroup, INotifyCollectionChanged, INotifyPropertyChanged, ISetting
{
    // ISSUE: CollectionView will not subscribe to CollectionChanged events
    // for a class that implements IReadOnlyList<T> and INotifyCollectionChanged.
    // However, it will subscribe to a class derived from ReadOnlyCollection<T>.
    // See https://github.com/dotnet/maui/discussions/29221
    // Once this is resolved, revert ReadOnlyCollection to IReadOnlyCollection<T>

    #region Fields

    readonly OrderedList<string> _families;
    readonly SettingCollection _settings;


    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public Bookmarks(SettingCollection settings)
        : base(new OrderedList<string>(StringComparer.CurrentCulture))
    {
        _settings = settings;
        _families = Items as OrderedList<string>;
        DisplayName = Name = Strings.BookmarkGroupName;
        Description = string.Empty;
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
    /// Gets the name to display in the UI.
    /// </summary>
    public string DisplayName
    {
        get;
    }

    /// <summary>
    /// Gets the description of the setting.
    /// </summary>
    public string Description
    {
        get;
    }

    /// <summary>
    /// Gets the value indicating if the setting has the default value.
    /// </summary>
    public bool IsDefault
    {
        get => Count == 0;
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

    /// <summary>
    /// Resets the collection to the default values.
    /// </summary>
    public void Reset()
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

    void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        PropertyChanged?.Invoke(this, e);
        // NOTE: The bookmarks instance does not change but the contents do,
        // so notify the parent collection that the bookmarks have changed.
        _settings.HasChanges = true;
    }

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
        OnPropertyChanged(CountChangedEventArgs);
    }

    void OnItemRemoved(int index, string item)
    {
        NotifyCollectionChangedEventHandler handler = CollectionChanged;
        if (handler is not null)
        {
            handler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }
        OnPropertyChanged(CountChangedEventArgs);
    }

    void OnClear()
    {
        if (Count > 0)
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
            OnPropertyChanged(CountChangedEventArgs);
        }
    }

    #endregion Collection Updates

    #region Serialization

    public void ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        List<string> families = JsonSerializer.Deserialize<List<string>>(ref reader, options);
        Update(families);
    }

    public void WriteValue(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        List<string> families = new(this);
        JsonSerializer.Serialize(writer, families, options);
    }

    #endregion Serialization
}
