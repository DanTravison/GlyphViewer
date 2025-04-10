namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Text;
using SkiaSharp;
using System.Diagnostics;

[DebuggerDisplay("{StartIndex, nq}->{EndIndex, nq} ({Count, nq})")]
class GlyphRow : GlyphRowBase, IGlyphRow
{
    #region Fields

    readonly List<GlyphMetrics> _items = [];

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="context">The <see cref="DrawContext"/> to use to draw the row.</param>
    public GlyphRow(DrawContext context)
        : base(context)
    {
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
        get => _items[column];
        set => _items[column] = value;
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
    /// <param name="metrics">The <see cref="GlyphMetrics"/> to add.</param>
    public void Add(GlyphMetrics metrics)
    {
        _items.Add(metrics);
        Bounds = new SKRect(Bounds.Left, Bounds.Top, Bounds.Right + Context.ColumnWidth, Bounds.Bottom);
    }

    /// <summary>
    /// Layouts the contents of the glyph group.
    /// </summary>
    /// <param name="location">The <see cref="SKPoint"/> identifying the location to draw.</param>
    /// <param name="width">The width of the drawing area.</param>
    /// <returns>The <see cref="SKSize"/> needed to draw the group.</returns>
    public void Arrange(SKPoint location, float width)
    {
        Bounds = new SKRect(location.X, location.Y, location.X + Context.ColumnWidth * _items.Count, location.Y + Context.RowHeight);
    }

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
    public bool HitTest(SKPoint point, out GlyphMetrics metrics)
    {
        if (Bounds.Contains(point))
        {
            int column = (int)((point.X - Bounds.Left) / Context.ColumnWidth);
            if (column >= 0 && column < _items.Count)
            {
                metrics = _items[column];
                return true;
            }
        }
        metrics = null;
        return false;
    }

    #region Draw

    /// <summary>
    /// Draws the glyph group on the specified canvas.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    public void Draw(SKCanvas canvas, SKPaint paint)
    {
        float left = Bounds.Left;
        float top = Bounds.Top;

        foreach (GlyphMetrics metrics in _items)
        {
            DrawGlyph(metrics, canvas, paint, left, top);
            left += Context.ColumnWidth;
        }
    }

    void DrawGlyph(GlyphMetrics metrics, SKCanvas canvas, SKPaint paint, float x, float y)
    {
        float columnWidth = Context.ColumnWidth;
        float rowHeight = Context.RowHeight;

        bool isSelected = ReferenceEquals(metrics.Glyph, Context.SelectedItem);
        float start = x + (columnWidth - metrics.Size.Width) / 2;
        float top = y + (rowHeight - metrics.Size.Height) / 2;
        float baseLine = top - metrics.Ascent;
        float strokeWidth = 2;

        paint.Style = SKPaintStyle.Fill;
        paint.Color = Context.ItemColor;
        canvas.DrawText(metrics.Glyph.Text, start, baseLine, SKTextAlign.Left, Context.ItemFont, paint);

        if (isSelected)
        {
            float horizontalSpacing = Context.HorizontalSpacing;
            float verticalSpacing = Context.VerticalSpacing;
            SKRect bounds = new SKRect
            (
                x + strokeWidth / 2,
                y + strokeWidth / 2,
                x + columnWidth - strokeWidth / 2,
                y + rowHeight - strokeWidth / 2
            );
            paint.Color = Context.SelectedItemColor;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;

            canvas.DrawRect(bounds, paint);
        }
    }

    #endregion Draw

    #endregion Methods
}