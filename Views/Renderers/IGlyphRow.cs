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
    /// Gets the <see cref="SKRect"/> identifying the bounds to render.
    /// </summary>
    /// <remarks>
    /// This property is set by <see cref="Arrange"/>.
    /// </remarks>
    SKRect Bounds
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="SKSize"/> identying the size of the row.
    /// </summary>
    SKSize Size
    {
        get;
    }

    /// <summary>
    /// Arranges <see cref="IGlyphRow"/>.
    /// </summary>
    /// <param name="left">The X coordinate of the location to draw.</param>
    /// <param name="top">The Y coordinate of the location to draw.</param>
    void Arrange(float left, float top);

    /// <summary>
    /// Draws the renderer on the <paramref name="canvas"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw on.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    void Draw(SKCanvas canvas, SKPaint paint);

    /// <summary>
    /// Determines if the specified point is within the bounds of the row.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> to test.</param>
    /// <param name="renderer">
    /// The <see cref="GlyphRenderer"/> for the associated <see cref="Glyph"/>; 
    /// otherwise, a null reference if the row is a header row.
    /// </param>
    /// <returns>true if the specified point is within the <see cref="Bounds"/> of the row; otherwise, false.</returns>
    bool HitTest(SKPoint point, out GlyphRenderer renderer);
}
