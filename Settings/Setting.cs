namespace GlyphViewer.Settings;

using GlyphViewer.Settings.Properties;

/// <summary>
/// Defines an abstract base class for an <see cref="ISetting"/>.
/// </summary>
public abstract class Setting : SettingPropertyCollection, ISetting
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">The parent <see cref="ISetting"/>.</param>
    /// <param name="name">The <see cref="ISettingProperty.Name"/> of the setting.</param>
    /// <param name="displayName">The <see cref="DisplayName"/> to display in the UI.</param>
    /// <param name="description">The <see cref="Description"/> of the setting.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="parent"/> is a null reference,
    /// -or-
    /// <paramref name="name"/> is a null reference, empty string, or only contains white space.
    /// -or-
    /// <paramref name="displayName"/> is a null reference, empty string, or only contains white space.
    /// -or-
    /// <paramref name="description"/> is a null reference, empty string, or only contains white space.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="parent"/> does not implement <see cref="ISettingPropertyCollection"/>.
    /// </exception>
    protected Setting
    (
        ISetting parent,
        string name,
        string displayName,
        string description
    )
        : base(parent, name)
    {
        ArgumentNullException.ThrowIfNull(parent, nameof(parent));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(displayName, nameof(displayName));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(description, nameof(description));

        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    #region Properties

    /// <summary>
    /// Gets the parent <see cref="ISetting"/>.
    /// </summary>
    /// <value>
    /// The parent <see cref="ISetting"/>.
    /// </value>
    public ISetting Parent
    {
        get;
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
    /// Gets the value indicating if the instance is user editable.
    /// </summary>
    public bool CanEdit
    {
        get;
        init;
    } = true;

    #endregion Properties
}
