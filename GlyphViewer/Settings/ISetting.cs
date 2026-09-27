namespace GlyphViewer.Settings;

using GlyphViewer.Settings.Properties;

/// <summary>
/// Provides an interface for a setting property.
/// </summary>
public interface ISetting : ISettingProperty
{
    /// <summary>
    /// Gets the parent <see cref="ISetting"/> of the current instance.
    /// </summary>
    /// <value>
    /// The parent <see cref="ISetting"/> of the current instance; otherwise, 
    /// a null reference if the current instance is the root.
    /// </value>
    ISetting Parent
    {
        get;
    }
}
