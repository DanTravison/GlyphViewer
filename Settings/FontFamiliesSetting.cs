namespace GlyphViewer.Settings;

using GlyphViewer.Converter;
using GlyphViewer.ObjectModel;
using GlyphViewer.Settings.Properties;
using GlyphViewer.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;

/// <summary>
/// Provides a base class for a <see cref="FontFamily"/> collection setting.
/// </summary>
public abstract class FontFamiliesSetting : ReadOnlyOrderedList<FontFamily>, IFontFamilyGroup, ISetting
{
    #region Fields

    readonly ISettingPropertyCollection _container;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">The parent <see cref="ISetting"/>.</param>
    /// <param name="name">The name to use when serializing the setting.</param>
    /// <param name="displayName">The name to display in the settings editor.</param>
    /// <param name="description">The text to display to describe the setting.</param>
    /// <param name="canEdit">true if the setting can be edited in the setting editor; otherwise, false.
    /// <para>
    /// The default is false, which means the setting is read-only in the settings editor.
    /// </para>
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="parent"/> is a null reference.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="parent"/> does not implement <see cref="ISettingPropertyCollection"/>.
    /// </exception>
    protected FontFamiliesSetting(ISetting parent, string name, string displayName, string description = null, bool canEdit = false)
        : base(FontFamily.Comparer)
    {
        ArgumentNullException.ThrowIfNull(parent, nameof(parent));
        Parent = parent;
        _container = parent as ISettingPropertyCollection;
        if (_container is null)
        {
            throw new ArgumentException($"{parent.GetType().Name} does not implement ISettingPropertyCollection");
        }
        DisplayName = displayName;
        Name = name;
        Description = description ?? string.Empty;
        CanEdit = canEdit;
    }

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
        init;
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
    /// Gets the value indicating if the setting has can be edited in the settings editor.
    /// </summary>
    public bool CanEdit
    {
        get;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Adds a new font family to the collection.
    /// </summary>
    /// <param name="fontFamily">The name of the font family to add.</param>
    /// <returns>true if the <paramref name="fontFamily"/> was added; otherwise, 
    /// false if the <paramref name="fontFamily"/> already exists in the collection.
    /// </returns>
    public virtual bool Add(FontFamily fontFamily)
    {
        if (List.Contains(fontFamily))
        {
            return false; // Already exists, no need to add again.
        }
        return List.AddItem(fontFamily) >= 0;
    }

    /// <summary>
    /// Replaces the collection contents with the specified <paramref name="families"/>.
    /// </summary>
    /// <param name="families">The <see cref="IEnumerable{FontFamily}"/> for the items to use to populate the collection.</param>
    /// <returns>
    /// true if the bookmarks were updated; otherwise, 
    /// false if <paramref name="families"/> is the same as the current content.
    /// <para>
    /// This method assumes that <paramref name="families"/> is sorted.
    /// </para>
    /// </returns>
    public bool Update(IReadOnlyList<FontFamily> families)
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
            List.Clear();
            List.AddRange(families);
        }
        return needsUpdate;
    }

    /// <summary>
    /// Removes a font family from the collection.
    /// </summary>
    /// <param name="fontFamily">The name of the font family to remove.</param>
    /// <returns>true if the <paramref name="fontFamily"/> was removed; otherwise,
    /// false if the <paramref name="fontFamily"/> does not exist in the collection.
    /// </returns>
    public bool Remove(FontFamily fontFamily)
    {
        return List.Remove(fontFamily);
    }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear()
    {
        List.Clear();
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
        List.Clear();
    }

    #endregion Methods

    #region Overrides

    /// <summary>
    /// Handles the <see cref="ReadOnlyOrderedList{T}.CollectionChanged"/> event.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        _container.HasChanges = true;
    }

    /// <summary>
    /// Handles the <see cref="ReadOnlyOrderedList{T}.PropertyChanged"/> event.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        _container.HasChanges = true;
    }

    #endregion Event Handlers

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
        List<FontFamily> families = JsonSerializer.Deserialize<List<FontFamily>>(ref reader, options);
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
        if (Count > 0)
        {
            List<FontFamily> families = new(List);
            writer.WritePropertyName(Name);
            JsonSerializer.Serialize(writer, families, options);
            families.Clear();
        }
    }

    #endregion Serialization
}
