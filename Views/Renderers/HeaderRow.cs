namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Text;
using SkiaSharp;
using System.Diagnostics;

/// <summary>
/// Provides a renderer for drawing a Glyph group header.
/// </summary>
[DebuggerDisplay("{Name, nq}")]
class HeaderRow : GlyphRowBase, IGlyphRow
{
    SKTextMetrics _metrics = null;
    float _baseLine;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="context">The <see cref="DrawContext"/> to use to draw the row.</param>
    /// <param name="range">The <see cref="Text.Unicode.Range"/> of the <see cref="HeaderRow"/>.</param>
    public HeaderRow(DrawContext context, Text.Unicode.Range range)
        : base(context)
    {
        Name = range.Name;
        Id = range.Id;
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="Text.Unicode.Range.Id"/> for the associated Glyphs.
    /// </summary>
    public uint Id
    {
        get;
    }

    /// <summary>
    /// Gets the name of the <see cref="Glyph"/> group.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the next <see cref="HeaderRow"/>
    /// </summary>
    public HeaderRow Next
    {
        get;
        private set;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Layouts the contents of the <see cref="HeaderRow"/>.
    /// </summary>
    /// <param name="width">The width of the drawing area.</param>
    /// <returns>The <see cref="SKSize"/> needed to draw the <see cref="HeaderRow"/>.</returns>
    public void Arrange(SKPoint location, float width)
    {
        _metrics = new SKTextMetrics(Name, Context.HeaderFont);
        float height = _metrics.Size.Height + Context.VerticalSpacing * 2;
        _baseLine = location.Y + Context.VerticalSpacing - _metrics.Ascent;
        Bounds = new SKRect(location.X, location.Y, location.X + width, location.Y + height);
    }

    /// <summary>
    /// Draws the <see cref="HeaderRow"/> on the specified canvas.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    public void Draw(SKCanvas canvas, SKPaint paint)
    {
        paint.Color = Context.HeaderColor;
        canvas.DrawText
        (
            Name, 
            Bounds.Left, 
            _baseLine, 
            Context.HeaderFont, 
            paint
        );
    }

    /// <summary>
    /// Determines if the specified point is within the bounds of the row.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> to test.</param>
    /// <param name="metrics">The parameter is always set to a null reference.</param>
    /// <returns>true if the specified point is within the bounds of the row; otherwise, false.</returns>
    public bool HitTest(SKPoint point, out GlyphMetrics metrics)
    {
        metrics = GlyphMetrics.Empty;
        return Bounds.Contains(point);
    }

    #endregion Methods
}
