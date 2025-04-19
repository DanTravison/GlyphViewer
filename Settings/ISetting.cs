namespace GlyphViewer.Settings;

/// <summary>
/// Provides an interface for a setting property.
/// </summary>
public interface ISetting
{
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
    /// <summary>
    /// Resets the setting to its default state.
    /// </summary>
    void Reset();
}
