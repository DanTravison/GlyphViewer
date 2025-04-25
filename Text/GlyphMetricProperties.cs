namespace GlyphViewer.Text;

public sealed class GlyphMetricProperties
{
    public GlyphMetricProperties(GlyphMetrics metrics)
    {
        Metrics = metrics;
        Properties = CreateInstance(metrics);

        Glyph glyph = metrics.Glyph;
        List<NamedValue> extended = [];
        extended.Add(new(nameof(Glyph.Category), glyph.Category.ToString()));
        extended.Add(new(nameof(Unicode.Range), glyph.Range.Name));
        extended.Add(new(nameof(Glyph.Name), glyph.Name));
        ExtendedProperties = extended;
    }

    #region Properties

    /// <summary>
    /// Gets the associated <see cref="GlyphMetrics"/>.
    /// </summary>
    public GlyphMetrics Metrics
    {
        get;
    }

    /// <summary>
    /// Gets the list of <see cref="GlyphMetrics"/> properties.
    /// </summary>
    public IReadOnlyList<NamedValue> Properties
    {
        get;
    }

    /// <summary>
    /// Gets the list of extended properties.
    /// </summary>
    public IReadOnlyList<NamedValue> ExtendedProperties
    {
        get;
    }

    #endregion Properties

    static IReadOnlyList<NamedValue> CreateInstance(GlyphMetrics metric)
    {
        List<NamedValue> properties = [];

        properties.Add(new(nameof(metric.FontSize), metric.FontSize));
        properties.Add(new(nameof(Glyph.Code), metric.Glyph.Code));
        properties.Add(new(nameof(metric.Size.Width), metric.Size.Width));
        properties.Add(new(nameof(metric.Size.Height), metric.Size.Height));
        properties.Add(new(nameof(GlyphMetrics.Ascent), metric.Ascent));
        properties.Add(new(nameof(GlyphMetrics.Descent), metric.Descent));
        properties.Add(new(nameof(GlyphMetrics.Left), metric.Left));
        properties.Add(new(nameof(GlyphMetrics.TextWidth), Math.Round(metric.TextWidth, 2)));

        return properties;
    }
}