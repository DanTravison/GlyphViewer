namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Text;
using SkiaSharp;
using System.Diagnostics;

/// <summary>
/// Provides an <see cref="IGlyphRow"/> for a glyph row
/// </summary>
[DebuggerDisplay("GlyphRow [{Count, nq}]")]
class GlyphRow : GlyphRowBase
{
    readonly GlyphRowRenderer _items;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="context">The <see cref="DrawContext"/> to use to draw the row.</param>
    public GlyphRow(DrawContext context)
        : base(context)
    {
        _items = new GlyphRowRenderer(context);
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="GlyphMetrics"/> at the specified <paramref name="column"/>.
    /// </summary>
    /// <param name="column">The zero-based column number.</param>
    /// <returns>The <see cref="GlyphMetrics"/> at the specified <paramref name="column"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="column"/> is less than zero or greater than
    /// or equal to <paramref name="column"/>.</exception>
    public GlyphMetrics this[int column]
    {
        get => _items[column].Metrics;
    }

    /// <summary>
    /// Gets the number of glyphs in the row.
    /// </summary>
    public int Count
    {
        get => _items.Count;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Adds a <see cref="GlyphMetrics"/> to the row.
    /// </summary>
    /// <param name="renderer">The <see cref="GlyphRenderer"/> to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="renderer"/> is a null reference.</exception>
    public bool Add(GlyphRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer, nameof(renderer));
        return _items.Add(renderer);
    }

    #region Arrange

    /// <summary>
    /// Layouts the contents of the glyph group.
    /// </summary>
    /// <param name="location">The <see cref="SKPoint"/> identifying the upper left coordinate.</param>
    /// <param name="size">The suggested size of the drawing area.</param>
    /// <returns>The <see cref="SKSize"/> needed to draw the content.</returns>
    protected override SKSize OnArrange(SKPoint location, SKSize size)
    {
        return _items.Arrange(location);
    }

    #endregion Arrange

    /// <summary>
    /// Determines if the specified point is within the bounds of the row.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> to test.</param>
    /// <param name="renderer">
    /// The <see cref="GlyphRenderer"/> that was hit;
    /// otherwise, null reference if no renderer is present at the specified <paramref name="point"/>.
    /// </param>
    /// <returns>true if the specified point is within the bounds of the glyph row; otherwise, false.</returns>
    public override bool HitTest(SKPoint point, out GlyphRenderer renderer)
    {
        return _items.HitTest(point, out renderer);
    }

    /// <summary>
    /// Draws the glyphs group on the specified canvas.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    public override void Draw(SKCanvas canvas, SKPaint paint)
    {
        _items.Draw(canvas, paint, DrawContext);
    }

    #endregion Methods

}