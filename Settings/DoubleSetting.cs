namespace GlyphViewer.Settings;
using GlyphViewer.ObjectModel;

using System.ComponentModel;

/// <summary>
/// Provides a <see cref="Setting{Double}"/>.
/// </summary>
internal class DoubleSetting : Setting<double>
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
    public DoubleSetting
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

    #region Properties

    /// <summary>
    /// Gets the minimum value.
    /// </summary>
    public double MininumValue
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the maximum value.
    /// </summary>
    public double MaximumValue
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the value increment
    /// </summary>
    public double Increment
    {
        get;
        init;
    }

    #endregion Properties

    #region Overrides

    /// <summary>
    /// Sets the value of the setting in <see cref="Preferences"/>.
    /// </summary>
    /// <param name="value">The value to set.</param>
    protected override void WriteValue(double value)
    {
        Preferences.Set(Name, value);
    }

    /// <summary>
    /// Gets the value from <see cref="Preferences"/>.
    /// </summary>
    /// <returns>The value from <see cref="Preferences"/>; otherwise, the <see cref="Setting{T}.DefaultValue"/>.</returns>
    protected override double ReadValue()
    {
        return Preferences.Get(Name, DefaultValue);
    }

    #endregion Overrides
}