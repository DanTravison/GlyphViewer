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
    public GlyphRenderer(Glyph glyph, DrawContext drawContext)
    {
        Metrics = GlyphMetrics.CreateInstance(glyph, drawContext.ItemFont);
        float height = Metrics.Descent - Metrics.Ascent;
        if (height == 0)
        {
            // for characters such as space, height may be zero.
            // if so, use the text width.
            height = Metrics.TextWidth;
        }
        PreferredSize = new(Metrics.TextWidth, height);
    }

    #region Properties

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
    public SKRect Bounds
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the size of the renderer.
    /// </summary>
    public SKSize Size
    {
        get;
        set;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Called by <see cref="GlyphRow"/> to set the final bounds.
    /// </summary>
    /// <param name="left">The X coordinate.</param>
    /// <param name="top">The Y coordinate.</param>
    public void Arrange(float left, float top)
    {
        Bounds = new(left, top, left + Size.Width, top + Size.Height);
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
        float strokeWidth = 2;

        if (isSelected)
        {
            SKRect bounds = new SKRect
            (
                x + strokeWidth / 2,
                y + strokeWidth / 2,
                x + width - strokeWidth / 2,
                y + height - strokeWidth / 2
            );

            SKColor fillColor = drawContext.SelectedItemBackgroundColor;
            paint.Color = fillColor;
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawRect(bounds, paint);

            paint.Color = drawContext.SelectedItemColor;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;

            canvas.DrawRect(bounds, paint);
        }

        float start = x - Metrics.Left + (width - Metrics.Size.Width) / 2;
        float top = y + (height - Metrics.Size.Height) / 2;
        float baseLine = top - Metrics.Ascent;

        paint.Style = SKPaintStyle.Fill;
        paint.Color = drawContext.ItemColor;
        canvas.DrawText(drawContext.ItemFont, paint, Metrics.Glyph.Text, start, baseLine, SKTextAlign.Left);
    }

    #endregion Methods
}
