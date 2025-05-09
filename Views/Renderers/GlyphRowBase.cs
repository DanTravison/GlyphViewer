namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Text;
using SkiaSharp;

/// <summary>
/// Provides a base class for a row in the <see cref="GlyphsView"/>.
/// </summary>
abstract class GlyphRowBase : IGlyphRow
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="context">The <see cref="Renderers.DrawContext"/> to use to draw the row.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is a null reference.</exception>
    protected GlyphRowBase(DrawContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        DrawContext = context;
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="Renderers.DrawContext"/> to use to draw the row.
    /// </summary>
    protected DrawContext DrawContext
    {
        get;
    }

    /// <summary>
    /// Gets <see cref="SKRect"/> defining the area to draw the row.
    /// </summary>
    public SKRect Bounds
    {
        get;
        protected set;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Layouts the contents of the glyph row.
    /// </summary>
    /// <param name="location">The location of the <see cref="IGlyphRow"/></param>
    /// <param name="size">The suggested size of the drawing area.</param>
    public void Arrange(SKPoint location, SKSize size)
    {
        size = OnArrange(location, size);
        Bounds = new(location.X, location.Y, location.X + size.Width, location.Y + size.Height);
    }

    /// <summary>
    /// Implemented in the derived class to arrange the content.
    /// </summary>
    /// <param name="location">The <see cref="SKPoint"/> identifying the upper left coordinate.</param>
    /// <param name="size">The suggested size of the drawing area.</param>
    /// <returns>The <see cref="SKSize"/> needed to draw the content..</returns>
    protected abstract SKSize OnArrange(SKPoint location, SKSize size);

    /// <summary>
    /// Implement in the derived class to draw the row.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    public abstract void Draw(SKCanvas canvas, SKPaint paint);

    /// <summary>
    /// Determines if the specified point is within the bounds of the row.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> to test.</param>
    /// <param name="renderer">
    /// The <see cref="GlyphRenderer"/> for the associated <see cref="Glyph"/>; 
    /// otherwise, a null reference if the row is a header row.
    /// </param>
    /// <returns>true if the specified point is within the <see cref="Bounds"/> of the row; otherwise, false.</returns>
    public virtual bool HitTest(SKPoint point, out GlyphRenderer renderer)
    {
        renderer = null;
        return Bounds.Contains(point);
    }

    #endregion Methods
}
