﻿namespace GlyphViewer.Views.Renderers;

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
    /// <param name="row">The zero-based index of the <see cref="GlyphRow"/> in the containing collection..</param>
    public GlyphRow(DrawContext context, int row)
        : base(context, row)
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

    /// <summary>
    /// Determines if the row contains a specific <see cref="Glyph"/>.
    /// </summary>
    /// <param name="glyph">The <see cref="Glyph"/> to query.</param>
    /// <returns>true if the row contains the specified <paramref name="glyph"/>; otherwise, false.</returns>
    public bool Contains(Glyph glyph)
    {
        if (glyph is null || glyph.IsEmpty || _items.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Metrics.Glyph == glyph)
            {
                return true;
            }
        }
        return false;
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
    /// Sets the final size for the items in the row.
    /// </summary>
    public void SizeItems()
    {
        base.Size = _items.SizeItems();
    }

    /// <summary>
    /// Layouts the contents of the glyph group.
    /// </summary>
    /// <param name="left">The X coordinate of the location to draw.</param>
    /// <param name="top">The Y coordinate of the location to draw.</param>
    protected override SKSize OnArrange(float left, float top)
    {
        return _items.Arrange(left, top);
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
        if (base.HitTest(point, out renderer))
        {
            return _items.HitTest(point, out renderer);
        }
        return false;
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