using GlyphViewer.Text;
using SkiaSharp;

namespace GlyphViewer.Views.Renderers;

/// <summary>
/// Provides a <see cref="Glyph"/> renderer.
/// </summary>
internal class GlyphRenderer
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="glyph">The <see cref="Glyph"/> to draw.</param>
    /// <param name="drawContext">The <see cref="DrawContext"/> to use to measure.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to measure.</param>
    public GlyphRenderer(Glyph glyph, DrawContext drawContext, SKPaint paint)
    {
        Metrics = GlyphMetrics.CreateInstance(glyph, drawContext.ItemFont, paint);
        float height = Metrics.Descent - Metrics.Ascent;
        if (height == 0)
        {
            // for characters such as space, height may be zero.
            // if so, use the text width.
            height = Metrics.TextWidth;
        }
        PreferredSize = new (Metrics.TextWidth, height);
    }

    /// <summary>
    /// Gets the <see cref="GlyphMetrics"/> to render.
    /// </summary>
    public readonly GlyphMetrics Metrics;

    /// <summary>
    /// Gets the initial preferred size needed to render the glyph.
    /// </summary>
    public SKSize PreferredSize
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the bounds to render the glyph.
    /// </summary>
    /// <remarks>
    /// This property is set by <see cref="Arrange"/>.
    /// </remarks>
    /// 
    public SKRect Bounds
    {
        get;
        private set;
    }

    /// <summary>
    /// Called by <see cref="GlyphRow"/> to set the final bounds.
    /// </summary>
    /// <param name="location">The <see cref="SKPoint"/> identifying the upper left coordinate.</param>
    /// <param name="size">The <see cref="SKSize"/> identifying the area available to render the glyph</param>
    public void Arrange(SKPoint location, SKSize size)
    {
        Bounds = new(location.X, location.Y, location.X + size.Width, location.Y + size.Height);
    }

    /// <summary>
    /// Draws the glyph on the <paramref name="canvas"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw on.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    /// <param name="drawContext">The <see cref="DrawContext"/> to use to draw.</param>
    public void Draw(SKCanvas canvas, SKPaint paint, DrawContext drawContext)
    {
        float width = Bounds.Width;
        float height = Bounds.Height;
        float x = Bounds.Left;
        float y = Bounds.Top;

        bool isSelected = ReferenceEquals(Metrics.Glyph, drawContext.SelectedItem);

        float start = x - Metrics.Left + (width - Metrics.Size.Width) / 2;
        float top = y + (height - Metrics.Size.Height) / 2;
        float baseLine = top - Metrics.Ascent;
        float strokeWidth = 2;

        paint.Style = SKPaintStyle.Fill;
        paint.Color = drawContext.ItemColor;
        canvas.DrawText(drawContext.ItemFont, paint, Metrics.Glyph.Text, start, baseLine, SKTextAlign.Left);

        if (isSelected)
        {
            SKRect bounds = new SKRect
            (
                x + strokeWidth / 2,
                y + strokeWidth / 2,
                x + width - strokeWidth / 2,
                y + height - strokeWidth / 2
            );
            paint.Color = drawContext.SelectedItemColor;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;

            canvas.DrawRect(bounds, paint);
        }
    }
}
