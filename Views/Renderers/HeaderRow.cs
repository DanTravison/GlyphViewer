namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Text;
using SkiaSharp;
using System.Diagnostics;

using UnicodeRange = Text.Unicode.Range;

/// <summary>
/// Provides a renderer for drawing a Glyph group header.
/// </summary>
[DebuggerDisplay("HeaderRow:{Name, nq}")]
class HeaderRow : GlyphRowBase
{
    #region Fields

    SKTextMetrics _metrics = null;
    float _baseLine;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="context">The <see cref="DrawContext"/> to use to draw the row.</param>
    /// <param name="unicodeRange">The <see cref="UnicodeRange"/> of the <see cref="HeaderRow"/>.</param>
    /// <param name="previous">The previous <see cref="HeaderRow"/>.</param>
    /// <param name="row">The zero-based index of the <see cref="HeaderRow"/> in the containing collection..</param>
    public HeaderRow(DrawContext context, UnicodeRange unicodeRange, HeaderRow previous, int row)
        : base(context, row)
    {
        Name = unicodeRange.Name;
        UnicodeRange = unicodeRange;
        Previous = previous;
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="Text.Unicode.Range.Id"/> for the associated Glyphs.
    /// </summary>
    public UnicodeRange UnicodeRange
    {
        get;
    }

    /// <summary>
    /// Gets the name of the <see cref="Glyph"/> group.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the previous <see cref="HeaderRow"/>
    /// </summary>
    public HeaderRow Previous
    {
        get;
        private set;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Layouts the contents of the <see cref="HeaderRow"/>.
    /// </summary>
    /// <param name="left">The X coordinate of the location to draw.</param>
    /// <param name="top">The Y coordinate of the location to draw.</param>
    /// <returns>The updated <see cref="Size"/> needed to draw the row.</returns>
    protected override SKSize OnArrange(float left, float top)
    {
        float verticalSpacing = DrawContext.Spacing.Vertical;

        _metrics = new SKTextMetrics(Name, DrawContext.HeaderFont);

        float height = _metrics.Size.Height + verticalSpacing * 2;
        _baseLine = verticalSpacing - _metrics.Ascent;

        return new(DrawContext.CanvasSize.Width, height);
    }

    /// <summary>
    /// Draws the <see cref="HeaderRow"/> on the specified canvas at the current <see cref="GlyphRowBase.Bounds"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    public override void Draw(SKCanvas canvas, SKPaint paint)
    {
        paint.Color = DrawContext.HeaderColor;
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawText
        (
            DrawContext.HeaderFont,
            paint,
            Name,
            Bounds.Left,
            Bounds.Top + _baseLine,
            SKTextAlign.Left
        );
    }

    #endregion Methods
}
