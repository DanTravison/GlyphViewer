namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Text;
using SkiaSharp;

/// <summary>
/// Provides a base class for a row in the <see cref="GlyphsView"/>.
/// </summary>
abstract class GlyphRowBase : IGlyphRow
{
    SKSize _size;

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
        private set;
    }

    /// <summary>
    /// Gets or sets the size of the row.
    /// </summary>
    public SKSize Size
    {
        get => _size;
        protected set
        {
            _size = value;
        }
    }

    /// <summary>
    /// Gets or sets the row index.
    /// </summary>
    internal int Row
    {
        get;
        set;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Layouts the contents of the glyph row.
    /// </summary>
    /// <param name="left">The X coordinate of the location to draw.</param>
    /// <param name="top">The Y coordinate of the location to draw.</param>
    public void Arrange(float left, float top)
    {
        Size = OnArrange(left, top);
        Bounds = new(left, top, left + Size.Width, top + Size.Height);
    }

    /// <summary>
    /// Implemented in the derived class to arrange the content.
    /// </summary>
    /// <param name="left">The X coordinate of the location to draw.</param>
    /// <param name="top">The Y coordinate of the location to draw.</param>
    /// <returns>The updated <see cref="Size"/> needed to draw the row.</returns>
    protected abstract SKSize OnArrange(float left, float top);

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
