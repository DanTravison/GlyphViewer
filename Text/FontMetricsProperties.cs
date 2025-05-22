namespace GlyphViewer.Text;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using SkiaSharp;
using System.Collections;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides a collection of <see cref="SKFontMetrics"/> properties.
/// </summary>
public sealed class FontMetricsProperties : IReadOnlyCollection<NamedValue>
{
    #region Fields

    readonly NamedValue _glyphCount = new(Strings.GlyphCountName, 0);
    readonly List<NamedValue> _values = [];

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="font">The <see cref="SKFont"/> to query.</param>
    /// <exception cref="ArgumentNullException"><paramref name="font"/> is a null reference.</exception>
    public FontMetricsProperties(SKFont font)
    {
        ArgumentNullException.ThrowIfNull(font, nameof(font));

        font.GetFontMetrics(out SKFontMetrics metrics);
        Metrics = metrics;

        Add(_values, nameof(SKTextMetrics.FamilyName), font.Typeface.FamilyName);
        _values.Add(_glyphCount);
        Add(_values, nameof(SKTextMetrics.FontSize), font.Size);
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

    #region Add

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Add(List<NamedValue> values, string name, float value)
    {
        values.Add(new NamedValue(name, Math.Round(value, 2)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Add(List<NamedValue> values, string name, string value)
    {
        values.Add(new NamedValue(name, value));
    }

    #endregion Add

    #region Properties

    /// <summary>
    /// Gets or sets the number of glyphs in the font.
    /// </summary>
    public int GlyphCount
    {
        get
        {
            if (_glyphCount.Value is null)
            {
                return 0;
            }
            return (int)_glyphCount.Value;
        }
        set
        {
            _glyphCount.Value = value;
        }
    }

    /// <summary>
    /// Gets the number of <see cref="NamedValue"/> properties in the collection.
    /// </summary>
    public int Count
    {
        get => _values.Count;
    }

    /// <summary>
    /// Gets the <see cref="SKFontMetrics"/>.
    /// </summary>
    public SKFontMetrics Metrics
    {
        get;
    }

    #endregion Properties

    #region IEnumerable

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

    #endregion IEnumerable
}
