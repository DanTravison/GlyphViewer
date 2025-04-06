using System.Collections;

namespace GlyphViewer.Text;

public sealed class GlyphMetricProperties : IEnumerable<GlyphMetricProperty>
{
    readonly IReadOnlyList<GlyphMetricProperty> _properties;

    public GlyphMetricProperties(GlyphMetrics metrics)
    {
        _properties = GlyphMetricProperty.CreateInstance(metrics);
        Category = metrics.Glyph.Category.ToString();
        Range = metrics.Glyph.Range.Name;
        FontFamily = metrics.Glyph.FontFamily;
    }

    public string FontFamily
    {
        get;
    }

    public string Category
    {
        get;
    }

    public string Range
    {
        get;
    }

    public IReadOnlyList<GlyphMetricProperty> Properties
    {
        get;
    }

    public IEnumerator<GlyphMetricProperty> GetEnumerator()
    {
        return _properties.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_properties).GetEnumerator();
    }
}