using GlyphViewer.Text;

namespace GlyphViewer.Settings;

using GlyphViewer.Converter;
using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using GlyphViewer.Settings.Properties;
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
    // Once this is resolved...
    //  Change OrderedList<T> to ReadOnlyOrderedList<T>
    //  - implement IReadOnlyList<T>
    //  - add protected Add/Remove methods.
    //  Derive from ReadOnlyOrderedList<T> instead of ReadOnlyCollection<T>
    //  Remove the INotifyCollectionChanged and INotifyPropertyChanged interfaces since ReadOnlyOrderList<T> will implement them.

    #region Fields

    readonly OrderedList<string> _families;
    readonly ISettingPropertyCollection _container;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="parent"/> is a null reference.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="parent"/> does not implement <see cref="ISettingPropertyCollection"/>.
    /// </exception>
    public Bookmarks(ISetting parent)
        : base(new OrderedList<string>(StringComparer.CurrentCulture))
    {
        ArgumentNullException.ThrowIfNull(parent, nameof(parent));
        Parent = parent;
        _container = parent as ISettingPropertyCollection;
        if (_container is null)
        {
            throw new ArgumentException($"{parent.GetType().Name} does not implement ISettingContainer");
        }

        _families = Items as OrderedList<string>;
        _families.CollectionChanged += OnFamiliesCollectionChanged;
        _families.PropertyChanged += OnFamiliesPropertyChanged;

        DisplayName = Name = Strings.BookmarkGroupName;
        Description = string.Empty;
    }

    #endregion Constructor

    #region Properties

    /// <summary>
    /// Gets the parent <see cref="ISetting"/>.
    /// </summary>
    public ISetting Parent
    {
        get;
    }

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

    /// <summary>
    /// Gets the value indicating if the setting has the default value.
    /// </summary>
    /// <value>
    /// this property always returns false.
    /// </value>
    public bool CanEdit
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
        return _families.AddItem(familyName) >= 0;
    }

    /// <summary>
    /// Replaces the collection contents with the specified <paramref name="families"/>.
    /// </summary>
    /// <param name="families">The <see cref="IEnumerable{T}"/> for the items to use to populate the collection.</param>
    /// <returns>
    /// true if the bookmarks were updated; otherwise, 
    /// false if <paramref name="families"/> is the same as the current content.
    /// <para>
    /// This method assumes that <paramref name="families"/> is sorted.
    /// </para>
    /// </returns>
    public bool Update(IReadOnlyList<string> families)
    {
        bool needsUpdate = Count != families.Count;
        if (!needsUpdate)
        {
            for (int x = 0; x < families.Count; x++)
            {
                if (families[x] != this[x])
                {
                    needsUpdate = true;
                    break;
                }
            }
        }
        if (needsUpdate)
        {
            _families.Clear();
            foreach (string familyName in families)
            {
                _families.AddItem(familyName);
            }
        }
        return needsUpdate;
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
        return _families.RemoveItem(familyName) >= 0;
    }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear()
    {
        _families.Clear();
    }

    /// <summary>
    /// Resets the collection to the default values.
    /// </summary>
    public void Reset()
    {
        // TODO: Consider requiring an explicit call to clear the
        // bookmarks and make this a NOP.
        // The SettingsPage's Reset action clears all settings
        // which may not be intended.
        _families.Clear();
    }

    #endregion Methods

    #region Event Handlers

    /// <summary>
    /// Handles <see cref="INotifyPropertyChanged.PropertyChanged"/> from the <see cref="OrderedList{T}"/>.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> identifying the property that changed.</param>
    private void OnFamiliesPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
        _container.HasChanges = true;
    }

    /// <summary>
    /// Handles <see cref="INotifyCollectionChanged.CollectionChanged"/> from the <see cref="OrderedList{T}"/>.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> containing the event details.</param>
    private void OnFamiliesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
        _container.HasChanges = true;
    }

    #endregion Event Handlers

    #region Events

    /// <summary>
    /// Occurs when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    #endregion Events

    #region Serialization

    /// <summary>
    /// Reads the bookmarks from the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to control serialization.</param>
    /// <returns>A new instance of a <see cref="UserSettings"/>.</returns>
    public void Read(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        reader.ReadPropertyName();
        List<string> families = JsonSerializer.Deserialize<List<string>>(ref reader, options);
        Update(families);
        families.Clear();
    }

    /// <summary>
    /// Writes the bookmarks to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to control serialization.</param>
    public void Write(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WritePropertyName(Name);
        List<string> families = new(this);
        JsonSerializer.Serialize(writer, families, options);
        families.Clear();
    }

    #endregion Serialization
}
