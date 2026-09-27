using System.ComponentModel;

namespace GlyphViewer.Settings.Properties;

/// <summary>
/// Provides an interface for an <see cref="ISetting"/> property.
/// </summary>
public interface ISettingProperty : INotifyPropertyChanged, ISettingSerializer
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
    /// Gets the value indicating if the instance is user editable.
    /// </summary>
    /// <value>
    /// true if the instance is user editable; otherwise, false.
    /// <para>
    /// The default value is true.
    /// </para>
    /// </value>
    bool CanEdit
    {
        get;
    }
}
