namespace GlyphViewer.Text;

public sealed class GlyphMetricProperty
{
    public GlyphMetricProperty(string name, object value)
    {
        Name = name;
        Value = value;
    }

    public string Name
    {
        get;
    }

    public object Value
    {
        get;
    }

    public static IReadOnlyList<GlyphMetricProperty> CreateInstance(GlyphMetrics metric)
    {
        List<GlyphMetricProperty> properties = [];

        properties.Add(new GlyphMetricProperty(nameof(metric.FontSize), metric.FontSize));
        properties.Add(new GlyphMetricProperty(nameof(Glyph.Code), metric.Glyph.Code));
        properties.Add(new GlyphMetricProperty(nameof(metric.Size.Width), metric.Size.Width));
        properties.Add(new GlyphMetricProperty(nameof(metric.Size.Height), metric.Size.Height));
        properties.Add(new GlyphMetricProperty(nameof(GlyphMetrics.Ascent), metric.Ascent));
        properties.Add(new GlyphMetricProperty(nameof(GlyphMetrics.Descent), metric.Descent));
        properties.Add(new GlyphMetricProperty(nameof(GlyphMetrics.Left), metric.Left));
        properties.Add(new GlyphMetricProperty(nameof(GlyphMetrics.TextWidth), Math.Round(metric.TextWidth, 2)));

        return properties;
    }
}

