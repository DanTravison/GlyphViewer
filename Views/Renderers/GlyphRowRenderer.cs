using SkiaSharp;

namespace GlyphViewer.Views.Renderers;

/// <summary>
/// Provides a layout and rendering class for a <see cref="GlyphRow"/>
/// </summary>
internal sealed class GlyphRowRenderer
{
    #region Fields

    readonly List<GlyphRenderer> _items = [];

    readonly GlyphLayoutStyle _style;

    // The width of the canvas.
    readonly float _canvasWidth;

    // The width of all items including cell spacing. 
    float _currentWidth;

    // The cell spacing.
    readonly SkSpacing _cellSpacing;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="drawContext">The <see cref="DrawContext"/> to use to arrange and draw cells.</param>
    public GlyphRowRenderer(DrawContext drawContext)
    {
        _style = drawContext.LayoutStyle;
        _canvasWidth = drawContext.CanvasSize.Width;
        _cellSpacing = drawContext.Spacing;
    }

    /// <summary>
    /// Gets the maximum glyph width
    /// </summary>
    public float GlyphWidth
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the maximum glyph height
    /// </summary>
    public float GlyphHeight
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the number of renderers
    /// </summary>
    public int Count
    {
        get => _items.Count;
    }

    /// <summary>
    /// Gets the <see cref="GlyphRenderer"/> at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The zero-based index to get.</param>
    /// <returns>The cref="GlyphRenderer"/> at the specified <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero 
    /// or greater than or equal to <see cref="Count"/>.</exception>
    public GlyphRenderer this[int index]
    {
        get => _items[index];
    }

    /// <summary>
    /// Adds a <see cref="GlyphRenderer"/>.
    /// </summary>
    /// <param name="renderer">The <see cref="GlyphRenderer"/> to add.</param>
    /// <returns>
    /// true if the <paramref name="renderer"/> was added; otherwise, 
    /// false if adding the <paramref name="renderer"/> would exceed the width of the row.
    /// </returns>
    public bool Add(GlyphRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer, nameof(renderer));
        
        float glyphWidth = Math.Max(GlyphWidth, renderer.PreferredSize.Width);
        float glyphHeight = Math.Max(GlyphHeight, renderer.PreferredSize.Height);

        if (_style.HasFlag(GlyphLayoutStyle.Width))
        {
            if (glyphWidth > GlyphWidth)
            {
                // recalculate the current width based on a larger cell width. 
                float currentWidth = _items.Count * (glyphWidth + _cellSpacing.Horizontal);
                if (currentWidth + glyphWidth > _canvasWidth)
                {
                    return false;
                }
                // save the accumulated width of the existing glyphs based on the 
                // increased cell width.
                _currentWidth = currentWidth;
            }
            else if (_currentWidth + GlyphWidth > _canvasWidth)
            {
                return false;
            }
        }
        else if (_style.HasFlag(GlyphLayoutStyle.GlyphWidth))
        {
            // size cell width to the glyph width.
        }
        else
        {
            // default: use cached DrawContext.GlyphSize.Width;
            glyphWidth = GlyphWidth;
        }

        if (_style.HasFlag(GlyphLayoutStyle.Height))
        {
            glyphHeight = Math.Max(glyphHeight, GlyphHeight);
        }

        float cellWidth = glyphWidth + _cellSpacing.Horizontal;
        if (_currentWidth + cellWidth <= _canvasWidth)
        {
            _items.Add(renderer);
            _currentWidth += cellWidth;

            if (glyphHeight > GlyphHeight)
            {
                GlyphHeight = glyphHeight;
            }
            if (glyphWidth > GlyphWidth)
            {
                GlyphWidth = glyphWidth;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Arranges the <see cref="GlyphRenderer"/> elements.
    /// </summary>
    /// <param name="location">The origin <see cref="SKPoint"/></param>
    public SKSize Arrange(SKPoint location)
    {
        float totalWidth = 0;
        float totalHeight = 0;
        float left = location.X;
        float top = location.Y;

        for (int i = 0; i < _items.Count; i++)
        {
            GlyphRenderer renderer = _items[i];
            float width = _cellSpacing.Horizontal;
            float height = GlyphHeight + _cellSpacing.Vertical;

            if (_style.HasFlag(GlyphLayoutStyle.GlyphWidth))
            {
                width += renderer.PreferredSize.Width;
            }
            else
            {
                width += GlyphWidth;
            }
            renderer.Arrange(new(left, top), new(width, height));

            left += renderer.Bounds.Width;
            totalWidth += renderer.Bounds.Width;
            totalHeight = Math.Max(totalHeight, renderer.Bounds.Height);
            
        }
        return new(totalWidth, totalHeight);
    }

    /// <summary>
    /// Determines if the specified point is within the bounds of the row.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> to test.</param>
    /// <param name="renderer">
    /// The <see cref="GlyphRenderer"/> at the specified <paramref name="point"/>; otherwise, 
    /// a null reference if no renderer is present at the specified <paramref name="point"/>.
    /// </param>
    /// <returns>true if the specified point is within the bounds of the glyph row; otherwise, false.</returns>
    public bool HitTest(SKPoint point, out GlyphRenderer renderer)
    {
        float left = point.X;
        float top = point.Y;

        if (left > _currentWidth)
        {
            // no match.
        }
        else if (_style.HasFlag(GlyphLayoutStyle.GlyphWidth))
        {
            // variable cell widths
            for (int x = 0; x < _items.Count; x++)
            {
                GlyphRenderer target = _items[x];
                if (target.Bounds.Contains(left, top))
                {
                    renderer = target;
                    return true;
                }
                left += target.Bounds.Width;
                if (left > target.Bounds.Right)
                {
                    break;
                }
                
            }
        }
        else
        {
            // fixed cell widths
            float cellWidth = GlyphWidth + _cellSpacing.Horizontal;
            
            int column = (int) (left / cellWidth);
            if (column <  _items.Count)
            {
                renderer = _items[column];
                return true;
            }
        }

        renderer = null;
        return false;
    }

    /// <summary>
    /// Draws the glyphs group on the specified canvas.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    /// <param name="drawContext">The <see cref="DrawContext"/> to use to draw.</param>
    public void Draw(SKCanvas canvas, SKPaint paint, DrawContext drawContext)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            GlyphRenderer renderer = _items[i];
            renderer.Draw(canvas, paint, drawContext);
        }
    }
}
