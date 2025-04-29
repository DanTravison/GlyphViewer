namespace GlyphViewer.Settings;

/// <summary>
/// Provides an interface for a setting property.
/// </summary>
public interface ISetting : ISettingSerializer
{
    #region Properties

    /// <summary>
    /// Gets the name of the setting.
    /// </summary>
    string Name
    {
        get;
    }

    /// <summary>
    /// Gets the name to display in the UI.
    /// </summary>
    string DisplayName
    {
        get;
    }

    /// <summary>
    /// Gets the description of the setting.
    /// </summary>
    string Description
    {
        get;
    }

    #endregion Properties

    /// <summary>
    /// Resets the setting to its default state.
    /// </summary>
    void Reset();
}
