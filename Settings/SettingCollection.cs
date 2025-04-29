namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using System.Collections;
using System.ComponentModel;

/// <summary>
/// Provides a serializable <see cref="ISetting"/> collection.
/// </summary>
public sealed class SettingCollection : ObservableObject, IReadOnlyCollection<ISetting>
{
    #region Fields

    /// <summary>
    /// Contains the <see cref="ISetting"/> properties that are editable by the user.
    /// </summary>
    readonly List<ISetting> _properties = [];
    readonly Dictionary<string, ISetting> _all = new(StringComparer.CurrentCulture);
    readonly NotifyPropertyChangedDelegate _propertyChanged;
    bool _hasChanges;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="propertyChanged">
    /// The <see cref="NotifyPropertyChangedDelegate"/> to invoke to notify the containing object a property has changed.
    /// </param>
    public SettingCollection(NotifyPropertyChangedDelegate propertyChanged)
    {
        _propertyChanged = propertyChanged;
    }

    #region Properties

    /// <summary>
    /// Gets the count of editable properties in the collection.
    /// </summary>
    public int Count
    {
        get => _properties.Count;
    }

    /// <summary>
    /// Gets the <see cref="IReadOnlyList{ISetting}"/> of settings that are user editable.
    /// </summary>
    public IReadOnlyList<ISetting> Properties
    {
        get => _properties;
    }

    #region HasChanges

    /// <summary>
    /// Gets the value indicating if the collection needs to be serialized.
    /// </summary>
    public bool HasChanges
    {
        get => _hasChanges;
        set => SetProperty(ref _hasChanges, value, HasChangesChangedeEventArgs);
    }

    static readonly PropertyChangedEventArgs HasChangesChangedeEventArgs = new(nameof(HasChanges));

    #endregion HasChanges

    /// <summary>
    /// Gets the <see cref="ISetting"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="ISetting"/> to get.</param>
    /// <returns>
    /// The <see cref="ISetting"/> with the specified <paramref name="name"/>;
    /// otherwise, a null reference.
    /// </returns>
    public ISetting this[string name]
    {
        get
        {
            _all.TryGetValue(name, out ISetting setting);
            return setting;
        }
    }

    #endregion Properties

    #region Public Methods

    /// <summary>
    /// Adds an <see cref="ISetting"/> to the editable properties.
    /// </summary>
    /// <typeparam name="S">The type of <see cref="ISetting"/>.</typeparam>
    /// <param name="setting">The <see cref="ISetting"/> to add.</param>
    /// <param name="canEdit">true if the user can edit the setting; otherwise, false.</param>
    /// <returns>The added <typeparamref name="S"/> <see cref="ISetting"/>.</returns>
    public S Add<S>(S setting, bool canEdit = true)
        where S : ISetting
    {
        ArgumentNullException.ThrowIfNull(setting);
        if (canEdit)
        {
            _properties.Add(setting);
        }
        _all.Add(setting.Name, setting);
        return setting;
    }

    /// <summary>
    /// Resets the properties to the default state.
    /// </summary>
    public void Reset()
    {
        foreach (ISetting setting in _all.Values)
        {
            if (!setting.IsDefault)
            {
                setting.Reset();
            }
        }
    }

    #endregion Public Methods

    #region NotifyPropertyChanged

    /// <summary>
    /// Notifies the containing object that the property value changed.
    /// </summary>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> identifying the property that changed.</param>
    /// <remarks>
    /// <see cref="UserSettings"/> should pass this method to the <see cref="ISetting"/> constructor.
    /// </remarks>
    public void NotifyPropertyChanged(PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        HasChanges = true;
        _propertyChanged?.Invoke(e);
    }

    #endregion NotifyPropertyChanged

    #region IEnumerable

    /// <summary>
    /// Gets an  <see cref="IEnumerator{ISetting}"/> to enumerate all <see cref="ISetting"/> instances.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{ISetting}"/>.</returns>
    public IEnumerator<ISetting> GetEnumerator()
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
}
