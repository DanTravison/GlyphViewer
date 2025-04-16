namespace GlyphViewer.Text;

using SkiaSharp;
using System.Collections;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides a collection of <see cref="SKFontMetrics"/> properties.
/// </summary>
public sealed class FontMetricsProperties : IReadOnlyCollection<NamedValue>
{
    #region Fields

    readonly List<NamedValue> _values = [];

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="typeface">The <see cref="SKTypeface"/> to query.</param>
    /// <param name="fontSize">The font size in points.</param>
    public FontMetricsProperties(SKTypeface typeface, float fontSize)
    {
        using (SKFont font = typeface.ToFont(fontSize))
        {

            SKFontMetrics metrics = font.Metrics;
            Metrics = metrics;

            _values.Add(new(nameof(SKTypeface.FamilyName), typeface.FamilyName));
            _values.Add(new("FontSize", fontSize));

            Add(_values, nameof(SKFontMetrics.Top), metrics.Top);
            Add(_values, nameof(SKFontMetrics.Ascent), metrics.Ascent);
            Add(_values, nameof(SKFontMetrics.Descent), metrics.Descent);
            Add(_values, nameof(SKFontMetrics.Bottom), metrics.Bottom);
            Add(_values, nameof(SKFontMetrics.Leading), metrics.Leading);
            Add(_values, nameof(SKFontMetrics.XHeight), metrics.XHeight);
            Add(_values, nameof(SKFontMetrics.CapHeight), metrics.CapHeight);
            Add(_values, nameof(SKFontMetrics.XMin), metrics.XMin);
            Add(_values, nameof(SKFontMetrics.XMax), metrics.XMax);
            Add(_values, "AvgWidth", metrics.AverageCharacterWidth);
            Add(_values, "MaxWidth", metrics.MaxCharacterWidth);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Add(List<NamedValue> values, string name, float value)
    {
        values.Add(new(name, Math.Round(value, 2)));
    }

    /// <summary>
    /// Gets the number of <see cref="NamedValue"/> properties in the collection.
    /// </summary>
    public int Count
    {
        get => _values.Count;
    }

    /// <summary>
    /// Gets the associated <see cref="SKFontMetrics"/>.
    /// </summary>
    public SKFontMetrics Metrics
    {
        get;
    }

    /// <summary>
    /// Gets an <see cref="IEnumerator{NamedValue}"/> for enumerating the <see cref="NamedValue"/> properties in the collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator{NamedValue}"/> for enumerating the <see cref="NamedValue"/> properties in the collection.
    /// </returns>
    public IEnumerator<NamedValue> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable"/> for enumerating the <see cref="NamedValue"/> properties in the collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable"/> for enumerating the<see cref="NamedValue"/> properties in the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_values).GetEnumerator();
    }

}
