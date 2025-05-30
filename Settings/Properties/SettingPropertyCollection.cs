namespace GlyphViewer.Settings.Properties;

using GlyphViewer.Converter;
using GlyphViewer.Diagnostics;
using GlyphViewer.ObjectModel;
using System.Collections;
using System.ComponentModel;

using System.Text.Json;

/// <summary>
/// Provides an abstract base class for an see cref="ISetting"/> collection.
/// </summary>
public abstract class SettingPropertyCollection : ObservableObject, ISettingPropertyCollection
{
    #region Fields

    /// <summary>
    /// The parent ISettingPropertyCollection.
    /// </summary>
    readonly ISettingPropertyCollection _parentContainer;

    /// <summary>
    /// Contains the <see cref="ISetting"/> properties that are editable by the user.
    /// </summary>
    readonly List<ISettingProperty> _properties = [];

    /// <summary>
    /// The table of all <see cref="ISetting"/> instances.
    /// </summary>
    readonly Dictionary<string, ISettingProperty> _all = new(StringComparer.CurrentCulture);

    bool _hasChanges;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">
    /// The parent <see cref="ISetting"/>; otherwise, a null reference if the 
    /// containing <see cref="ISetting"/> is the root.
    /// </param>
    /// <param name="name">The name of the containing <see cref="ISetting"/>.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="parent"/> is not null and it does not implement <see cref="ISettingPropertyCollection"/>.
    /// </exception>
    protected SettingPropertyCollection(ISetting parent, string name)
    {
        if (parent is not null)
        {
            _parentContainer = parent as ISettingPropertyCollection;
            if (_parentContainer is null)
            {
                throw new ArgumentException($"{parent.GetType().Name} must implement ISettingPropertyCollection", nameof(parent));
            }
        }
        Name = name;
    }

    #region Properties

    /// <summary>
    /// Gets the name of the containing <see cref="ISetting"/>.
    /// </summary>
    /// <value>
    /// The name of the containing <see cref="ISetting"/>.
    /// </value>
    public string Name
    {
        get;
    }

    /// <summary>
    /// Gets the count of editable properties in the collection.
    /// </summary>
    public int Count
    {
        get => _properties.Count;
    }

    /// <summary>
    /// Gets the <see cref="ISettingProperty"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="ISettingProperty"/> to get.</param>
    /// <returns>
    /// The <see cref="ISettingProperty"/> with the specified <paramref name="name"/>;
    /// otherwise, a null reference.
    /// </returns>
    public ISettingProperty this[string name]
    {
        get
        {
            _all.TryGetValue(name, out ISettingProperty property);
            return property;
        }
    }

    /// <summary>
    /// Gets the value indicating the all <see cref="ISettingProperty"/> instances have the default value.
    /// </summary>
    public bool IsDefault
    {
        get
        {
            foreach (ISettingProperty property in _properties)
            {
                if (!property.IsDefault)
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Gets the value indicating if the collection needs to be serialized.
    /// </summary>
    public virtual bool HasChanges
    {
        get => _hasChanges;
        set
        {
            // TODO: Consider moving HasChanges ObservableObject 
            // with an overridable OnHasChangesChanged method?
            if (value != _hasChanges)
            {
                _hasChanges = value;
                Trace.Value(TraceFlag.Settings, this, nameof(HasChanges), _hasChanges);
                if (!_hasChanges)
                {
                    // Clear child containers to ensure future changes
                    // are propogated up.
                    foreach (ISettingProperty property in _properties)
                    {
                        if (property is ISettingPropertyCollection setting)
                        {
                            setting.HasChanges = false;
                        }
                    }
                }
                else if (_parentContainer is not null)
                {
                    _parentContainer.HasChanges = true;
                }
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="IReadOnlyList{ISetting}"/> of instances where <see cref="ISettingProperty.CanEdit"/> is true.
    /// </summary>
    /// <remarks>
    /// This property is used by the settings page to display user editable settings.
    /// The returned values may be a subset of all instances in the collection.
    /// </remarks>
    public IReadOnlyList<ISettingProperty> Properties
    {
        get => _properties;
    }

    #endregion Properties

    #region Add

    /// <summary>
    /// Adds an <see cref="ISetting"/> to the editable properties.
    /// </summary>
    /// <typeparam name="S">The type of <see cref="ISetting"/>.</typeparam>
    /// <param name="property">The <see cref="ISetting"/> to add.</param>
    /// <returns>The added <typeparamref name="S"/> <see cref="ISetting"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is a null reference.</exception>
    protected S AddItem<S>(S property)
        where S : ISettingProperty
    {
        ArgumentNullException.ThrowIfNull(property);
        if (property.CanEdit)
        {
            _properties.Add(property);
        }
        _all.Add(property.Name, property);
        property.PropertyChanged += OnChildPropertyChanged;
        return property;
    }

    private void OnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is ISettingProperty property)
        {
            HasChanges = true;
        }
    }

    /// <summary>
    /// Adds <see cref="ISetting"/> instances.
    /// </summary>
    /// <param name="properties">The <see cref="ISetting"/> instances to add.</param>
    /// <exception cref="ArgumentNullException">
    /// One or more of the specified <paramref name="properties"/> is a null reference.
    /// </exception>
    protected void AddRange(params ISettingProperty[] properties)
    {
        foreach (ISettingProperty property in properties)
        {
            AddItem(property);
        }
    }

    #endregion Add

    #region Reset

    /// <summary>
    /// Resets the all properties to the default state.
    /// </summary>
    public void Reset()
    {
        foreach (ISettingProperty property in _all.Values)
        {
            if (!property.IsDefault)
            {
                property.Reset();
            }
        }
    }

    /// <summary>
    /// Resets only the user editable properties.
    /// </summary>
    public void ResetEditable()
    {
        foreach (ISettingProperty property in _properties)
        {
            if (!property.IsDefault)
            {
                property.Reset();
            }
        }
    }

    #endregion Reset

    #region IEnumerable

    /// <summary>
    /// Gets an  <see cref="IEnumerator{ISettingProperty}"/> to enumerate all <see cref="ISetting"/> instances.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{ISettingProperty}"/>.</returns>
    public IEnumerator<ISettingProperty> GetEnumerator()
    {
        return _all.Values.GetEnumerator();
    }

    /// <summary>
    /// Gets an  <see cref="IEnumerator"/> to enumerate all <see cref="ISetting"/> instances.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/>.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_all.Values).GetEnumerator();
    }

    #endregion IEnumerable

    #region Serialization

    /// <summary>
    /// Reads the properties from the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to control serialization.</param>
    /// <returns>A new instance of a <see cref="UserSettings"/>.</returns>
    public void Read(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (_parentContainer is not null)
        {
            reader.ReadPropertyName();
        }
        reader.Read(JsonTokenType.StartObject);

        while (reader.TokenType == JsonTokenType.PropertyName)
        {
            string propertyName = reader.GetPropertyName();
            ISettingProperty property = this[propertyName];
            if (property is not null)
            {
                property.Read(ref reader, options);
                reader.Read();
            }
            else
            {
                // NOTE: Unknown property, skip it.
                Trace.Warning(TraceFlag.Storage, this, nameof(Read), $"Skipping unknown property '{propertyName}'");
                reader.Skip();
            }
        }
        reader.Verify(JsonTokenType.EndObject);
    }

    /// <summary>
    /// Writes the properties to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to control serialization.</param>
    public void Write(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        if (_parentContainer is not null)
        {
            writer.WriteStartObject(Name);
        }
        else
        {
            writer.WriteStartObject();
        }
        foreach (ISettingProperty property in this)
        {
            if (!property.IsDefault)
            {
                property.Write(writer, options);
            }
        }
        writer.WriteEndObject();
    }

    #endregion Serialization
}
