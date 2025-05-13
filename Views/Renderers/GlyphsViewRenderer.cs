using GlyphViewer.ObjectModel;
using GlyphViewer.Text;
using SkiaSharp;
using System.ComponentModel;
using System.Diagnostics;
using UnicodeRange = GlyphViewer.Text.Unicode.Range;

namespace GlyphViewer.Views.Renderers;

/// <summary>
/// Provides a layout and rendering class for a <see cref="GlyphsView"/>.
/// </summary>
internal class GlyphsViewRenderer : ObservableObject
{
    #region Private Classes

    /// <summary>
    /// Provides the set of <see cref="GlyphRenderer"/> instances in a <see cref="UnicodeRange"/>.
    /// </summary>
    [DebuggerDisplay("{UnicodeRange.Name, nq}[{Count,nq}]")]
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

    #region Fields

    // The parent <see cref="GlyphsView"/>.
    readonly GlyphsView _view;

    // The <see cref="DrawContext"/> to use to measure and draw the glyphs.
    readonly DrawContext _drawContext;

    // The UnicodeRange to GlyphRange table.
    readonly GlyphRanges _glyphRanges = new();

    // The code point to GlyphRenderer table.
    readonly Dictionary<ushort, GlyphRenderer> _codepoints = [];

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

    // Indicates the content need to be arranged.
    bool _needsArrange;

    // Indicates the content needs to be drawn.
    bool _needsDraw;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="view">The parent <see cref="GlyphsView"/>.</param>
    public GlyphsViewRenderer(GlyphsView view)
    {
        _view = view;
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
                InvalidateDraw();
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
                OnContentChanged(_content);
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="Glyph"/> in the <see cref="Content"/> with the specified <paramref name="codePoint"/>.
    /// </summary>
    /// <param name="codePoint">The <see cref="Glyph.CodePoint"/> to get.</param>
    /// <returns>The <see cref="Glyph"/> with the specified <paramref name="codePoint"/>; otherwise, 
    /// a null reference if the <see cref="Content"/> does not contain the <see cref="Glyph"/>.
    /// </returns>
    public Glyph this[ushort codePoint]
    {
        get
        {
            if (_codepoints.TryGetValue(codePoint, out GlyphRenderer renderer))
            {
                return renderer.Metrics.Glyph;
            }
            return null;
        }
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

    #region ContentChanged

    void ResetContent()
    {
        _rows.Clear();
        _headers.Clear();
        _codepoints.Clear();
        _glyphRanges.Clear();
    }

    /// <summary>
    /// Handles <see cref="Content"/> changes.
    /// </summary>
    /// <param name="content">IReadOnlyList{Glyph} content.</param>
    /// <remarks>
    /// This method popupates _codepoints, _glyphRanges, UnicodeRanges and _drawContext.GlyphSize.
    /// </remarks>
    void OnContentChanged(IReadOnlyList<Glyph> content)
    {
        ResetContent();
        bool hasContent = content is not null && content.Count > 0;

        float width = 0;
        float height = 0;

        if (hasContent)
        {
            UnicodeRange unicodeRange = UnicodeRange.Empty;
            GlyphRenderers renderers = null;

            using (SKPaint paint = new SKPaint() { IsAntialias = true })
            {
                for (int i = 0; i < content.Count; i++)
                {
                    Glyph glyph = _content[i];
                    if (_codepoints.ContainsKey(glyph.CodePoint))
                    {
                        continue;
                    }
                    GlyphRenderer renderer = new(glyph, _drawContext, paint);
                    if (renderer.PreferredSize == SKSize.Empty)
                    {
                        continue;
                    }
                    _codepoints.Add(glyph.CodePoint, renderer);

                    if (glyph.Range != unicodeRange)
                    {
                        renderers = _glyphRanges.Add(glyph.Range);
                        unicodeRange = glyph.Range;
                    }

                    width = Math.Max(renderer.PreferredSize.Width, width);
                    height = Math.Max(renderer.PreferredSize.Height, height);

                    renderers.Add(renderer);
                }
            }
        }

        _drawContext.GlyphSize = new(width, height);

        if (hasContent)
        {
            InvalidateArrange();
        }
        else
        {
            InvalidateDraw();
        }
    }

    #endregion ContentChanged

    #region Arrange

    /// <summary>
    /// Indicates the contents need to be arranged.
    /// </summary>
    public void InvalidateArrange()
    {
        if (!_needsArrange)
        {
            _needsArrange = true;
            InvalidateDraw();
        }
    }

    /// <summary>
    /// Populates _rows, _headers and FirstRow.
    /// </summary>
    /// <param name="size">The size of the drawing area.</param>
    void PopulateRows(SKSize size)
    {
        // TODO: Consider renaming this to something like PopulateRows.
        if (!_needsArrange && size == DrawContext.CanvasSize)
        {
            return;
        }
        DrawContext.CanvasSize = size;
        bool hasContent = _glyphRanges.Count > 0;

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
                SetFirstRow(previousRow, previousGlyph);
            }
            else
            {
                FirstRow = 0;
            }
#else
            FirstRow = 0;
#endif
            OnPropertyChanged(CountChangedEventArgs);
        }
        _needsArrange = false;
    }

    #endregion Arrange

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
    /// The zero-based row to set to ensure the <paramref name="glyph"/> is visible; 
    /// otherwise, -1 if the <paramref name="glyph"/> is not present.
    /// </param>
    /// <returns>
    /// true if the glyph is visible; otherwise, false.
    /// </returns>
    /// <remarks>
    /// When false is returned and the returned <paramref name="row"/> is greater than or equal to zero,
    /// setting <see cref="FirstRow"/> to the <paramref name="row"/> will ensure the <paramref name="glyph"/> 
    /// is visible.
    /// </remarks>
    public bool IsVisible(Glyph glyph, out int row)
    {
        int glyphRow = GetRow(glyph);
        if (glyphRow >= 0)
        {
            if (glyphRow < FirstRow)
            {
                row = glyphRow;
                return false;
            }
            if (glyphRow <= LastRow)
            {
                row = glyphRow;
                return true;
            }
            row = glyphRow;
            return false;
        }
        row = -1;
        return false;
    }

    #region Draw

    /// <summary>
    /// Indicates the view needs to be redrawn.
    /// </summary>
    public void InvalidateDraw()
    {
        if (!_needsDraw)
        {
            _view?.InvalidateSurface();
            _needsDraw = true;
        }
    }

    /// <summary>
    /// Draws the <see cref="Content"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="canvasSize">The size of the <paramref name="canvas"/>.</param>
    public void Draw(SKCanvas canvas, SKSize canvasSize)
    {
        // NOTE: Calling arrange unconditionally
        PopulateRows(canvasSize);
        _needsDraw = false;

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
                _currentHeader.Arrange(x, y);
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
                row.Arrange(x, y);
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
