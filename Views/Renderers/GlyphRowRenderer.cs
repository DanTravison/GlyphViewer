namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Diagnostics;
using GlyphViewer.Settings;
using SkiaSharp;

/// <summary>
/// Provides a layout and rendering class for a <see cref="GlyphRow"/>
/// </summary>
internal sealed class GlyphRowRenderer
{
    #region Fields

    readonly List<GlyphRenderer> _items = [];
    readonly DrawContext _drawContext;

    // The current width of all items including cell spacing. 
    float _currentWidth;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="drawContext">The <see cref="DrawContext"/> to use to arrange and draw cells.</param>
    public GlyphRowRenderer(DrawContext drawContext)
    {
        _drawContext = drawContext;
    }

    /// <summary>
    /// Gets the maximum glyph width.
    /// </summary>
    /// <value>
    /// The value is based on the <see cref="CellWidthLayout"/> value in the <see cref="CellLayoutStyle"/>.
    /// <list type="table">
    ///     <listheader>
    ///         <term><see cref="CellWidthLayout"/></term>
    ///         <description>value</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="CellWidthLayout.Default"/>.</term>
    ///         <description>The maximum width of all glyphs in the font.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="CellWidthLayout.Width"/></term>
    ///         <description>The maximum width of all glyphs in the row.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="CellWidthLayout.Dynamic"/>.</term>
    ///         <description>Zero. The glyph width is used.</description>
    ///     </item>
    /// </list>    
    /// </value>
    public float GlyphWidth
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the maximum glyph height
    /// </summary>
    /// <value>
    /// When using <see cref="CellHeightLayout.Dynamic"/>, the maximum height of the glyphs in the row;
    /// otherwise, the maximimum height of all glyphs in the font.
    /// </value>
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

        CellLayoutStyle cellLayout = _drawContext.CellLayout;
        SkSpacing spacing = _drawContext.Spacing;
        float canvasWidth = _drawContext.CanvasSize.Width;
        float glyphWidth = Math.Max(renderer.PreferredSize.Width, _drawContext.MinimumGlyphSize.Width);
        float glyphHeight = Math.Max(renderer.PreferredSize.Height, _drawContext.MinimumGlyphSize.Height);
        float cellWidth;

        if (cellLayout.Width == CellWidthLayout.Width)
        {
            // if the current glyph width cause the cell width to increase,
            // adjust the _currentWidth to the new cell width.
            if (glyphWidth > GlyphWidth)
            {
                float currentWidth = _currentWidth;
                cellWidth = glyphWidth + spacing.Horizontal;

                // Adjust the current width based on the increased cell width.
                currentWidth = _items.Count * cellWidth;
                if (currentWidth > canvasWidth)
                {
                    // The adjusted current width would be wider than the canvas.
                    return false;
                }
                if (currentWidth + cellWidth > canvasWidth)
                {
                    // The new width would be wider than the canvas.
                    return false;
                }
                // Save the adjusted current width.
                _currentWidth = currentWidth;
            }
            else
            {
                // Size the current glyph width to the current max glyph width.
                glyphWidth = GlyphWidth;
            }
        }
        else if (cellLayout.Width == CellWidthLayout.Dynamic)
        {
            // size cell width to the glyph width.
        }
        else
        {
            // use the global width.
            glyphWidth = Math.Max(_drawContext.MaximumGlyphSize.Width, _drawContext.MinimumGlyphSize.Width);
        }

        if (cellLayout.Height == CellHeightLayout.Dynamic)
        {
            glyphHeight = Math.Max(glyphHeight, GlyphHeight);
        }
        else
        {
            // use the global height
            glyphHeight = _drawContext.MaximumGlyphSize.Height;
        }

        cellWidth = glyphWidth + spacing.Horizontal;
        if (_currentWidth + cellWidth <= canvasWidth)
        {
            _items.Add(renderer);
            _currentWidth += cellWidth;

            GlyphHeight = glyphHeight;
            GlyphWidth = Math.Max(GlyphWidth, glyphWidth);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets the sizes of the individual glyphs.
    /// </summary>
    /// <returns>The size required for the renderer.</returns>
    public SKSize SizeItems()
    {
        float totalWidth = 0;
        float totalHeight = 0;
        CellLayoutStyle cellLayout = _drawContext.CellLayout;
        SkSpacing spacing = _drawContext.Spacing;

        float minimumHeight = _drawContext.MinimumGlyphSize.Height;
        float minimumWidth = _drawContext.MinimumGlyphSize.Width;

        for (int i = 0; i < _items.Count; i++)
        {
            GlyphRenderer renderer = _items[i];
            float cellWidth = spacing.Horizontal;
            float cellHeight = spacing.Vertical + Math.Max(GlyphHeight, minimumHeight);

            if (cellLayout.Width == CellWidthLayout.Dynamic)
            {
                cellWidth += Math.Max(renderer.PreferredSize.Width, minimumWidth);
            }
            else
            {
                cellWidth += GlyphWidth;
            }

            renderer.Size = new(cellWidth, cellHeight);

            totalWidth += cellWidth;
            totalHeight = Math.Max(totalHeight, cellHeight);
        }
        if (totalWidth > _drawContext.CanvasSize.Width)
        {
            Trace.Error
            (
                TraceFlag.Drawing, this, nameof(SizeItems),
                $"Row width {0} > Canvas width {1}.",
                totalWidth, _drawContext.CanvasSize.Width
            );
        }
        return new(totalWidth, totalHeight);
    }

    /// <summary>
    /// Arranges the <see cref="GlyphRenderer"/> elements.
    /// </summary>
    /// <param name="left">The X coordinate of the location to draw.</param>
    /// <param name="top">The Y coordinate of the location to draw.</param>
    public SKSize Arrange(float left, float top)
    {
        float totalWidth = 0;
        float totalHeight = 0;

        for (int i = 0; i < _items.Count; i++)
        {
            GlyphRenderer renderer = _items[i];
            renderer.Arrange(left, top);

            left += renderer.Size.Width;
            totalWidth += renderer.Size.Width;
            totalHeight = Math.Max(totalHeight, renderer.Size.Height);
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
        else if (_drawContext.CellLayout.Width == CellWidthLayout.Dynamic)
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
                if (left < target.Bounds.Left)
                {
                    break;
                }
            }
        }
        else
        {
            // fixed cell widths
            float cellWidth = GlyphWidth + _drawContext.Spacing.Horizontal;

            int column = (int)(left / cellWidth);
            if (column < _items.Count)
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
