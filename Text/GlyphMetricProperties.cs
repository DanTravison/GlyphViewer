using System.Collections;

namespace GlyphViewer.Text;

public sealed class GlyphMetricProperties : IEnumerable<GlyphMetricProperty>
{
    readonly IReadOnlyList<GlyphMetricProperty> _properties;

    public GlyphMetricProperties(GlyphMetrics metrics)
    {
        _properties = GlyphMetricProperty.CreateInstance(metrics);
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