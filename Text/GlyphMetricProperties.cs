using System.Collections;

namespace GlyphViewer.Text;

public sealed class GlyphMetricProperties
{
    public GlyphMetricProperties(GlyphMetrics metrics)
    {
        Properties = GlyphMetricProperty.CreateInstance(metrics);

        Glyph glyph = metrics.Glyph;
        List<GlyphMetricProperty> extended = [];
        extended.Add(new GlyphMetricProperty(nameof(Glyph.Category), glyph.Category.ToString()));
        extended.Add(new GlyphMetricProperty(nameof(Unicode.Range), glyph.Range.Name));
        extended.Add(new GlyphMetricProperty(nameof(Glyph.Name), glyph.Name));
        ExtendedProperties = extended;
    }

    public string FontFamily
    {
        get;
    }

    public IReadOnlyList<GlyphMetricProperty> Properties
    {
        get;
    }

    public IReadOnlyList<GlyphMetricProperty> ExtendedProperties
    {
        get;
    }
}