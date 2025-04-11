namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Text;
using GlyphViewer.Views;
using SkiaSharp;

/// <summary>
/// Defines a row in the <see cref="GlyphsView"/>
/// </summary>
interface IGlyphRow
{
    /// <summary>
    /// Gets the bounds for the row.
    /// </summary>
    SKRect Bounds
    {
        get;
    }

    /// <summary>
    /// Layouts the contents of the glyph row.
    /// </summary>
    /// <param name="location">The location of the <see cref="IGlyphRow"/></param>
    /// <param name="width">The width of the drawing area.</param>
    void Arrange(SKPoint location, float width);

    /// <summary>
    /// Draws the glyph row on the specified canvas.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    void Draw(SKCanvas canvas, SKPaint paint);

    /// <summary>
    /// Determines if the specified point is within the bounds of the glyph row.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> to test.</param>
    /// <param name="metrics">
    /// The <see cref="GlyphMetrics"/> that was hit;
    /// otherwise, <see cref="GlyphMetrics.Empty"/> if no <see cref="GlyphMetrics"/> 
    /// is present at the specified <paramref name="point"/>.
    /// </param>
    /// <returns>true if the specified point is within the bounds of the glyph row; otherwise, false.</returns>
    bool HitTest(SKPoint point, out GlyphMetrics metrics);
}
