namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using System.ComponentModel;

/// <summary>
/// Provides a <see cref="DoubleSetting"/> for a font size.
/// </summary>
public abstract class FontSetting : DoubleSetting
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="settings">The containing <see cref="SettingCollection"/>.</param>
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
    protected FontSetting
    (
        SettingCollection settings,
        PropertyChangedEventArgs eventArgs,
        double defaultValue,
        string displayName,
        string description,
        IEqualityComparer<double> comparer = null
    )
        : base(settings, eventArgs, defaultValue, displayName, description, comparer)
    {
        Increment = 1;
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
    /// Gets the font size to use to display the sample text.
    /// </summary>
    public double FontSize
    {
        get => Value;
    }

    /// <summary>
    /// Gets the <see cref="FontAttributes"/> to use to display the sample text.
    /// </summary>
    public FontAttributes FontAttributes
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
