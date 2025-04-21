namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using System.ComponentModel;

internal class GlyphWidthSetting : DoubleSetting
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="propertyChanged">The <see cref="ObservableProperty.NotifyPropertyChangedDelegate"/>  delegate to invoke to raised the property chagned event.</param>
    /// <param name="eventArgs">The optional <see cref="PropertyChangedEventArgs"/> to use when the value changes.</param>
    /// <param name="defaultValue">The default <see cref="Setting{T}.DefaultValue"/> of the setting.</param>
    /// <param name="displayName">The <see cref="Setting{T}.DisplayName"/> of the setting..</param>
    /// <param name="description">The <see cref="Setting{T}.Description"/> of the setting.</param>
    public GlyphWidthSetting
    (
        NotifyPropertyChangedDelegate propertyChanged,
        PropertyChangedEventArgs eventArgs,
        double defaultValue,
        string displayName,
        string description
    )
        : base(propertyChanged, eventArgs, defaultValue, displayName, description)
    {
    }
}
