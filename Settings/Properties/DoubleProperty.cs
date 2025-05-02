namespace GlyphViewer.Settings.Properties;

using GlyphViewer.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

/// <summary>
/// Provides a <see cref="SettingProperty{Double}"/>.
/// </summary>
public class DoubleProperty : SettingProperty<double>
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="name">The property <see cref="SettingProperty{Double}.Name"/>.</param>
    /// <param name="defaultValue">The default <see cref="ObservableProperty{Double}.Value"/>.</param>
    /// <param name="displayName">The <see cref="SettingProperty{Double}.DisplayName"/>.</param>
    /// <param name="description">The <see cref="SettingProperty{Double}.Description"/>.</param>
    /// <param name="comparer">
    /// The optional <see cref="IEqualityComparer{T}"/> to use to compare the <see cref="ObservableProperty{T}.Value"/>.
    /// <para>
    /// The default value is <see cref="EqualityComparer{T}.Default"/>.
    /// </para>
    /// </param>
    public DoubleProperty
    (
        string name,
        double defaultValue,
        string displayName,
        string description,
        IEqualityComparer<double> comparer = null
    )
        : base(name, defaultValue, displayName, description, comparer)
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
    /// Writes the <paramref name="value"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="value">The <see cref="double"/> value to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    protected override void WriteValue(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }

    /// <summary>
    /// Reads the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    /// <returns>The double value.</returns>
    protected override double ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        double value;
        try
        {
            value = reader.GetDouble();
            value = Math.Clamp(value, MininumValue, MaximumValue);
        }
        catch (JsonException ex)
        {
            Trace.WriteLine($"Invalid value for double", ex.Message);
            value = DefaultValue;
        }
        return value;
    }

    #endregion Overrides
}