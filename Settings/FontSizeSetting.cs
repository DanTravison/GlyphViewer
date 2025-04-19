namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using System.ComponentModel;

/// <summary>
/// Provides a <see cref="DoubleSetting"/> for a font size.
/// </summary>
internal class FontSizeSetting : DoubleSetting
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="propertyChanged">The <see cref="ObservableProperty.NotifyPropertyChangedDelegate"/>  delegate to invoke to raised the property chagned event.</param>
    /// <param name="eventArgs">The optional <see cref="PropertyChangedEventArgs"/> to use when the value changes.</param>
    /// <param name="defaultValue">The default <see cref="Setting{T}.DefaultValue"/> of the setting.</param>
    /// <param name="displayName">The <see cref="Setting{T}.DisplayName"/> of the setting..</param>
    /// <param name="description">The <see cref="Setting{T}.Description"/> of the setting.</param>
    /// <param name="comparer">
    /// The optional <see cref="IEqualityComparer{T}"/> to use to compare the <see cref="ObservableProperty{T}.Value"/>.
    /// <para>
    /// The default value is <see cref="EqualityComparer{T}.Default"/>.
    /// </para>
    /// </param>
    public FontSizeSetting
    (
        NotifyPropertyChangedDelegate propertyChanged, 
        PropertyChangedEventArgs eventArgs,
        double defaultValue, 
        string displayName, 
        string description, 
        IEqualityComparer<double> comparer = null
    ) 
        : base(propertyChanged, eventArgs, defaultValue, displayName, description, comparer)
    {
    }

    /// <summary>
    /// Gets the font family to use to display the sample text.
    /// </summary>
    public string FontFamily
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the sample text to display.
    /// </summary>
    public string Text
    {
        get;
        init;
    }
}
