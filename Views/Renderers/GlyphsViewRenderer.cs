namespace GlyphViewer.Views.Renderers;

using GlyphViewer.ObjectModel;
using GlyphViewer.Text;
using SkiaSharp;
using System.ComponentModel;
using Diag = System.Diagnostics;
using UnicodeRange = GlyphViewer.Text.Unicode.Range;

/// <summary>
/// Provides a layout and rendering class for a <see cref="GlyphsView"/>.
/// </summary>
internal class GlyphsViewRenderer : ObservableObject
{
    #region Fields

    // The parent <see cref="GlyphsView"/>.
    readonly GlyphsView _view;

    // The <see cref="DrawContext"/> to use to measure and draw the glyphs.
    readonly DrawContext _drawContext;

    // The UnicodeRange to GlyphRange table.
    readonly GlyphRanges _glyphRanges = new();

    /// <summary>
    /// The Glyph to GlyphRenderer table.
    /// </summary>
    readonly Dictionary<Glyph, GlyphRenderer> _glyphs = [];

    // The glyphs for the currently selected font family.
    IReadOnlyList<Glyph> _content;

    // The glyph rows.
    readonly List<IGlyphRow> _rows = [];

    // The row displayed at the top of the list.
    int _firstRow;

    // The table of header rows
    readonly Dictionary<UnicodeRange, HeaderRow> _headers = [];

    // The current header row
    HeaderRow _currentHeader;

    /// <summary>
    /// Indicates the current invalidation state.
    /// </summary>
    readonly RenderingState _state;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="view">The parent <see cref="GlyphsView"/>.</param>
    public GlyphsViewRenderer(GlyphsView view)
    {
        _view = view;
        _state = new RenderingState(view);
        _drawContext = new(this);
    }

    #region Properties

    /// <summary>
    /// Gets the parent <see cref="GlyphsView"/>.
    /// </summary>
    internal GlyphsView View
    {
        get => _view;
    }

    /// <summary>
    /// Gets the <see cref="DrawContext"/>.
    /// </summary>
    internal DrawContext DrawContext
    {
        get => _drawContext;
    }

    /// <summary>
    /// Gets the <see cref="IGlyphRow"/> rows.
    /// </summary>
    /// <value>
    /// An <see cref="IReadOnlyList{IGlyphRow}"/> containing zero or more elements.
    /// </value>
    public IReadOnlyList<IGlyphRow> Rows
    {
        get => _rows;
    }

    /// <summary>
    /// Gets the number of rows.
    /// </summary>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="CountChangedEventArgs"/>
    /// </remarks>
    public int Count
    {
        get => _rows.Count;
    }

    /// <summary>
    /// Gets the current row index.
    /// </summary>
    /// <value>
    /// The zero-based index of the first row to display.
    /// </value>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="FirstRowChangedEventArgs"/>
    /// </remarks>
    public int FirstRow
    {
        get => _firstRow;
        set
        {
            value = Math.Clamp(value, 0, _rows.Count);
            if (SetProperty(ref _firstRow, value, FirstRowChangedEventArgs))
            {
                Invalidate(RenderState.Arrange);
            }
        }
    }

    /// <summary>
    /// Gets the last visible row.
    /// </summary>
    /// <value>
    /// The zero-based index of the last row to display; otherwise, 0 if the <see cref="Content"/> is not set
    /// and arranged.
    /// </value>
    public int LastRow
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the <see cref="IReadOnlyList{Glyph}"/> to render.
    /// </summary>
    /// <value>
    /// An <see cref="IReadOnlyList{IGlyphRow}"/> containing one or more elements; otherwise,
    /// a null reference if the content is not set.
    /// </value>
    public IReadOnlyList<Glyph> Content
    {
        get => _content;
        set
        {
            if (!ReferenceEquals(value, _content))
            {
                _content = value;
                LastRow = 0;
                Invalidate(RenderState.Measure);
            }
        }
    }

    /// <summary>
    /// Gets teh value indicating if the <see cref="Glyph"/> is in the <see cref="Content"/>.
    /// </summary>
    /// <param name="glyph">The <see cref="Glyph"/> to query.</param>
    /// <returns>true if the <paramref name="glyph"/> is in the content; otherwise, false.</returns>
    public bool Contains(Glyph glyph)
    {
        return glyph is not null && _glyphs.ContainsKey(glyph);
    }

    /// <summary>
    /// Gets the <see cref="HeaderRow"/> for the specified <paramref name="unicodeRange"/>.
    /// </summary>
    /// <param name="unicodeRange">The <see cref="UnicodeRange"/> to query.</param>
    /// <returns>
    /// The <see cref="HeaderRow"/> for the specified <paramref name="unicodeRange"/>; otherwise, 
    /// a null reference if the <see cref="Content"/> does not contain glyphs in the specified <paramref name="unicodeRange"/>.
    /// </returns>
    public HeaderRow this[UnicodeRange unicodeRange]
    {
        get
        {
            if (!unicodeRange.IsEmpty && _headers.TryGetValue(unicodeRange, out HeaderRow headerRow))
            {
                return headerRow;
            }
            return null;
        }
    }


    #endregion Properties

    #region Invalidate

    /// <summary>
    /// Invalidates the renderer.
    /// </summary>
    /// <param name="type">The <see cref="RenderState"/> to invalidate.</param>
    public void Invalidate(RenderState type)
    {
        _state.Invalidate(type);
    }

    #endregion Invalidate

    #region Measure

    void ResetContent()
    {
        _rows.Clear();
        _headers.Clear();
        _glyphs.Clear();
        _glyphRanges.Clear();
    }

    /// <summary>
    /// Generates GlyphMetrics for the <see cref="Content"/> and populates the <see cref="GlyphRenderers"/>.
    /// </summary>
    /// <remarks>
    /// This method popupates _codepoints, _glyphRanges, UnicodeRanges and _drawContext.GlyphSize.
    /// </remarks>
    void Measure()
    {
        if (!_state.ShouldMeasure)
        {
            return;
        }
        ResetContent();
        bool hasContent = _content is not null && _content.Count > 0;

        float width = 0;
        float height = 0;

        if (hasContent)
        {
            UnicodeRange unicodeRange = UnicodeRange.Empty;
            GlyphRenderers renderers = null;

            for (int i = 0; i < _content.Count; i++)
            {
                Glyph glyph = _content[i];
                if (_glyphs.ContainsKey(glyph))
                {
                    continue;
                }
                GlyphRenderer renderer = new(glyph, _drawContext);
                if (renderer.PreferredSize == SKSize.Empty)
                {
                    continue;
                }
                _glyphs.Add(glyph, renderer);

                if (glyph.Range != unicodeRange)
                {
                    renderers = _glyphRanges.Add(glyph.Range);
                    unicodeRange = glyph.Range;
                }

                width = Math.Max(renderer.PreferredSize.Width, width);
                height = Math.Max(renderer.PreferredSize.Height, height);

                renderers.Add(renderer);
            }
            Invalidate(RenderState.Layout);
        }

        _drawContext.MaximumGlyphSize = new(width, height);
        Invalidate(RenderState.Draw);
    }

    #endregion Measure

    #region Layout

    /// <summary>
    /// Populates _rows, _headers and FirstRow.
    /// </summary>
    /// <param name="canvasSize">The size of the drawing area.</param>
    void Layout(SKSize canvasSize)
    {
        Measure();

        if (!_state.ShouldLayout)
        {
            return;
        }

        bool hasContent = _glyphRanges.Count > 0;
        int firstRow = 0;

#if (false)

        if (hasContent && _rows.Count > 0)
        {
            // Identify the previous current row and, optionally, the first glyph in the previous row.
            // Intent: If the previous row is a header row, make that header row the 
            // current row after arrange; otherwise, if the previous row is a GlyphRow
            // set the current row to the row containing the first glyph of the previous row.
            // NOTE: If the glyphs changed, skip this logic.
            GlyphMetrics previousGlyph = null;
            IGlyphRow previousRow = _rows.Count > 0
                ? _rows[0] : null;

            if (previousRow is GlyphRow glyphRow)
            {
                previousGlyph = glyphRow[0];
            }
        }

#endif
        _rows.Clear();
        _headers.Clear();

        if (hasContent)
        {
            using SKPaint paint = new() { IsAntialias = true };

            HeaderRow previousHeader = null;

            for (int i = 0; i < _glyphRanges.Count; i++)
            {
                UnicodeRange unicodeRange = _glyphRanges[i];
                // get the glyph range for the UnicodeRange.
                GlyphRenderers renderers = _glyphRanges[unicodeRange];

                HeaderRow header = new(_drawContext, unicodeRange, previousHeader, _rows.Count);
                _headers.Add(unicodeRange, header);

                // NOTE: The header row for the first row is not added to the _rows list
                // to avoid the header being drawn twice when at the top of the list.
                if (_rows.Count > 0)
                {
                    _rows.Add(header);
                }
                previousHeader = header;

                GlyphRow row = new(_drawContext, _rows.Count);
                _rows.Add(row);

                for (int r = 0; r < renderers.Count; r++)
                {
                    GlyphRenderer renderer = renderers[r];
                    if (!row.Add(renderer))
                    {
                        row.SizeItems();
                        row = new(_drawContext, _rows.Count);
                        _rows.Add(row);
                        row.Add(renderer);
                    }
                }
                row.SizeItems();
            }

#if (false)
            if (previousRow is not null)
            {
                // After arrange, set the current row to reflect
                // the previous row.
                firstRow = GetFirstRow(previousRow, previousGlyph);
            }
#endif
            _state.Invalidate(RenderState.Arrange);
            OnPropertyChanged(CountChangedEventArgs);
        }
        FirstRow = firstRow;
    }

    #endregion Layout

    #region HitTest

    /// <summary>
    /// Determines the <paramref name="renderer"/> and associated <paramref name="row"/> at a specified <paramref name="point"/>.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> to query.</param>
    /// <param name="row">The <see cref="IGlyphRow"/> containing the <paramref name="point"/>; 
    /// otherwise, a null reference.</param>
    /// <param name="renderer">The <see cref="GlyphRenderer"/> containing the <paramref name="point"/>;
    /// otherwise, a null reference.
    /// </param>
    /// <returns>
    /// true if the <paramref name="point"/> row or glyph contains the <paramref name="point"/>; otherwise, false.
    /// <list type="table">
    ///     <listheader>
    ///         <term>Returns</term>
    ///         <description><paramref name="row"/></description>
    ///         <description><paramref name="renderer"/></description>
    ///     </listheader>
    ///     <item>
    ///         <term>true (Header Row)</term>
    ///         <description>The header row containing the point.</description>
    ///         <description>A null reference.</description>
    ///     </item>
    ///     <item>
    ///         <term>true (Glyph)</term>
    ///         <description>The row containing the glyph.</description>
    ///         <description>The <see cref="GlyphRenderer"/> containing the point.</description>
    ///     </item>
    ///     <item>
    ///         <term>false</term>
    ///         <description>A null reference.</description>
    ///         <description>A null reference.</description>
    ///     </item>
    /// </list>
    /// </returns>
    public bool HitTest(SKPoint point, out IGlyphRow row, out GlyphRenderer renderer)
    {
        if (_currentHeader is not null && _currentHeader.Bounds.Contains(point))
        {
            row = _currentHeader;
            renderer = null;
            return true;
        }
        for (int i = FirstRow; i < _rows.Count; i++)
        {
            row = _rows[i];
            if (row.HitTest(point, out renderer))
            {
                return true;
            }
        }
        renderer = null;
        row = null;
        return false;
    }

    #endregion HitTest

    #region Misc

    /// <summary>
    /// Gets the row containing the specified <see cref="Glyph"/>.
    /// </summary>
    /// <param name="glyph">The <see cref="Glyph"/> to query.</param>
    /// <returns>The zero-based index of the row containing the <paramref name="glyph"/>; 
    /// otherwise, -1 if the glyph is not present.
    /// </returns>
    public int GetRow(Glyph glyph)
    {
        if (glyph is not null && !glyph.IsEmpty)
        {
            for (int i = 0; i < _rows.Count; i++)
            {
                if (_rows[i] is GlyphRow glyphRow && glyphRow.Contains(glyph))
                {
                    return i;
                }
            }
        }
        return -1;
    }

    /// <summary>
    /// Determines if the specified <see cref="Glyph"/> is visible in the list.
    /// </summary>
    /// <param name="glyph">The <see cref="Glyph"/> to query.</param>
    /// <param name="row">
    /// The zero-based row containing the <paramref name="glyph"/>; 
    /// otherwise, -1 if the <paramref name="glyph"/> is not present.
    /// </param>
    /// <returns>
    /// true if the glyph is visible; otherwise, false if the glyph is not visible or is not present.
    /// </returns>
    /// <remarks>
    /// When false is returned and the returned <paramref name="row"/> is greater than or equal to zero,
    /// setting <see cref="FirstRow"/> to the <paramref name="row"/> will ensure the <paramref name="glyph"/> 
    /// is visible.
    /// </remarks>
    public bool IsVisible(Glyph glyph, out int row)
    {
        int glyphRow = GetRow(glyph);
        bool result = glyphRow >= FirstRow && glyphRow <= LastRow;

        row = glyphRow;
        return result;
    }

    #endregion Misc

    #region Draw

    /// <summary>
    /// Draws the <see cref="Content"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="canvasSize">The size of the <paramref name="canvas"/>.</param>
    public void Draw(SKCanvas canvas, SKSize canvasSize)
    {
        if (DrawContext.CanvasSize != canvasSize)
        {
            DrawContext.CanvasSize = canvasSize;
            Invalidate(RenderState.Layout);
        }
        Layout(canvasSize);

        // NOTE: arrange is performed while drawing to avoid walking the rows twice.
        bool needsArrange = _state.ShouldArrange;

        // NOTE: For now, the draw state is bookkeeping to ensure one and only one InvalidateSurface call
        // is made until the draw is complete versus using it to decide to draw or not.
        // However, we do need to clear it to enable future InvalidateSurface calls.

        // If the rendering model used an offscreen bitmap, the state 'could' be used to 
        // determine if the bitmap needs to be updated.
        _ = _state.ShouldDraw;

        if (_rows.Count == 0)
        {
            return;
        }

        using (SKPaint paint = new()
        {
            IsAntialias = true,
            Color = _drawContext.ItemColor,
            Style = SKPaintStyle.Fill
        })
        {
            float width = canvasSize.Width;
            float height = canvasSize.Height;
            float x = 0;
            float y = 0;

            _currentHeader = GetHeaderRow();
            if (_currentHeader is not null)
            {
                // Draw the header row in the header area
                // above the list.
                if (needsArrange)
                {
                    _currentHeader.Arrange(x, y);
                }
                _currentHeader.Draw(canvas, paint);
                y += _currentHeader.Bounds.Height;
            }
            int lastRow = FirstRow;
            for (int i = FirstRow; i < _rows.Count; i++)
            {
                IGlyphRow row = _rows[i];
                if (y + row.Size.Height > height)
                {
                    break;
                }
                lastRow = i;
                if (needsArrange)
                {
                    row.Arrange(x, y);
                }
                row.Draw(canvas, paint);
                y += row.Bounds.Height;
            }
            LastRow = lastRow;
        }
    }

    HeaderRow GetHeaderRow()
    {
        HeaderRow result = null;
        do
        {
            if (_rows.Count == 0)
            {
                break;
            }

            IGlyphRow row = _rows[FirstRow];
            if (row is GlyphRow glyphRow)
            {
                UnicodeRange unicodeRange = glyphRow[0].Glyph.Range;
                result = _headers[unicodeRange];
                break;
            }

            if (row is HeaderRow headerRow)
            {
                // NOTE: We don't update the header area until the first row is a GlyphRow.
                // otherwise, the top of the list will be a  duplicate of the header area.
                // Also, if there is no previous row, we are at the first group in the list.
                result = headerRow.Previous ?? headerRow;
            }

        } while (false);

        return result;
    }

    #endregion Draw

    #region Private Classes

    /// <summary>
    /// Provides the set of <see cref="GlyphRenderer"/> instances in a <see cref="UnicodeRange"/>.
    /// </summary>
    [Diag.DebuggerDisplay("{UnicodeRange.Name, nq}[{Count,nq}]")]
    class GlyphRenderers
    {
        List<GlyphRenderer> _renderers = [];
        SKSize _size;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="unicodeRange">The <see cref="UnicodeRange"/>.</param>
        public GlyphRenderers(UnicodeRange unicodeRange)
        {
            UnicodeRange = unicodeRange;
        }

        /// <summary>
        /// Gets the <see cref="UnicodeRange.Id"/>
        /// </summary>
        public UnicodeRange UnicodeRange
        {
            get;
        }

        /// <summary>
        /// Gets the maximum size of glyphs in the <see cref="Renderers"/>.
        /// </summary>
        public SKSize Size
        {
            get;
        }

        /// <summary>
        /// Gets the number of <see cref="GlyphRenderer"/> in the collection.
        /// </summary>
        public int Count
        {
            get => _renderers.Count;
        }

        /// <summary>
        /// Gets the <see cref="GlyphRenderer"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="GlyphRenderer"/> to get.</param>
        /// <returns>The <see cref="GlyphRenderer"/> at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero
        /// or greater than or equal to <see cref="Count"/>.</exception>
        public GlyphRenderer this[int index]
        {
            get => _renderers[index];
        }

        /// <summary>
        /// Adds a <see cref="GlyphRenderer"/>.
        /// </summary>
        /// <param name="renderer">The <see cref="GlyphRenderer"/> to add.</param>
        public void Add(GlyphRenderer renderer)
        {
            float width = Math.Max(_size.Width, renderer.Metrics.Size.Width);
            float height = Math.Max(_size.Height, renderer.Metrics.Size.Height);
            _size = new(width, height);
            _renderers.Add(renderer);
        }
    }

    /// <summary>
    /// Provides the list of <see cref="UnicodeRange"/> and the associated <see cref="GlyphRenderers"/>.
    /// </summary>
    class GlyphRanges
    {
        #region Fields

        readonly Dictionary<UnicodeRange, GlyphRenderers> _glyphRenderers = [];
        readonly List<UnicodeRange> _unicodeRanges = [];

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the number <see cref="UnicodeRange"/>s in the list.
        /// </summary>
        public int Count
        {
            get => _unicodeRanges.Count;
        }

        /// <summary>
        /// Gets the <see cref="UnicodeRange"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="UnicodeRange"/> to get.</param>
        /// <returns>The <see cref="UnicodeRange"/> at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or 
        /// greater than or equal to <see cref="Count"/>.
        /// </exception>
        public UnicodeRange this[int index]
        {
            get => _unicodeRanges[index];
        }

        /// <summary>
        /// Gets the <see cref="GlyphRenderers"/> for the specified <paramref name="unicodeRange"/>.
        /// </summary>
        /// <param name="unicodeRange">The <see cref="UnicodeRange"/> for the <see cref="GlyphRenderers"/> to get.</param>
        /// <returns>
        /// The <see cref="GlyphRenderers"/> for the specified <paramref name="unicodeRange"/>; otherwise, 
        /// a null reference.
        /// </returns>
        public GlyphRenderers this[UnicodeRange unicodeRange]
        {
            get
            {
                _glyphRenderers.TryGetValue(unicodeRange, out GlyphRenderers glyphRange);
                return glyphRange;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        public void Clear()
        {
            _unicodeRanges.Clear();
            _glyphRenderers.Clear();
        }

        /// <summary>
        /// Adds a <see cref="UnicodeRange"/> to the list.
        /// </summary>
        /// <param name="unicodeRange">The <see cref="UnicodeRange"/> to add.</param>
        /// <returns>
        /// A new instance of a <see cref="GlyphRenderers"/> if <paramref name="unicodeRange"/>
        /// is not in the list; otherwise, the existing <see cref="GlyphRenderers"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="unicodeRange"/> equals <see cref="UnicodeRange.Empty"/>.</exception>
        public GlyphRenderers Add(UnicodeRange unicodeRange)
        {
            if (unicodeRange.IsEmpty)
            {
                throw new ArgumentOutOfRangeException(nameof(unicodeRange));
            }
            if (_glyphRenderers.TryGetValue(unicodeRange, out GlyphRenderers glyphRange))
            {
                return glyphRange;
            }
            glyphRange = new(unicodeRange);
            _glyphRenderers.Add(unicodeRange, glyphRange);
            _unicodeRanges.Add(unicodeRange);
            return glyphRange;
        }

        #endregion Methods
    }

    #endregion Private Classes

    #region PropertyChangedEventArgs

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="FirstRow"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs FirstRowChangedEventArgs = new(nameof(FirstRow));

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="Count"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs CountChangedEventArgs = new(nameof(Count));

    #endregion PropertyChangedEventArgs
}
