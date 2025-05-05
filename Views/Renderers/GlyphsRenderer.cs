using GlyphViewer.ObjectModel;
using GlyphViewer.Text;
using SkiaSharp;
using System.ComponentModel;
using UnicodeRange = GlyphViewer.Text.Unicode.Range;

namespace GlyphViewer.Views.Renderers;

/// <summary>
/// Provides a layout and rendering class for a <see cref="GlyphsView"/>.
/// </summary>
internal class GlyphsRenderer : ObservableObject
{
    /// <summary>
    /// Provides the set of <see cref="GlyphRenderer"/> instances in a <see cref="UnicodeRange"/>.
    /// </summary>
    class GlyphRange
    {
        List<GlyphRenderer> _renderers = [];
        SKSize _size;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="unicodeRange">The <see cref="UnicodeRange"/>.</param>
        public GlyphRange(UnicodeRange unicodeRange)
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
        /// Gets the <see cref="IReadOnlyList{GlyphRenderer>"/> for glyphs in the <see cref="UnicodeRange"/>.
        /// </summary>
        public IReadOnlyList<GlyphRenderer> Renderers
        {
            get => _renderers;
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

    #region Fields

    // The parent <see cref="GlyphsView"/>.
    readonly GlyphsView _view;

    // The <see cref="DrawContext"/> to use to measure and draw the glyphs.
    readonly DrawContext _drawContext;

    // The measured <see cref="Glyphs"/>.
    readonly List<GlyphRenderer> _items = [];

    // The CodePoint to GlyphRenderer table.
    readonly Dictionary<ushort, GlyphRenderer> _glyphTable = [];

    // The list of unicode ranges
    readonly List<UnicodeRange> _unicodeRanges = [];

    // The table of unicode range to GlyphRenderes.
    readonly Dictionary<uint, GlyphRange> _glyphRanges = [];

    IReadOnlyList<Glyph> _content;

    // The glyph rows.
    readonly List<IGlyphRow> _rows = [];
    int _firstRow;

    // The table of header rows
    readonly Dictionary<uint, HeaderRow> _headers = [];

    // The current header row
    HeaderRow _currentHeader;

    // Indicates the <see cref="Glyphs"/> need to be arranged.
    bool _needsArrange;

    /// Gets the <see cref="SKSize"/> from the last call to <see cref="Arrange"/>
    SKSize _size;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="view">The parent <see cref="GlyphsView"/>.</param>
    public GlyphsRenderer(GlyphsView view)
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
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="RowsChangedEventArgs"/>
    /// </remarks>
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
        get => _items.Count;
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
                _view.InvalidateSurface();
            }
        }
    }

    /// <summary>
    /// Gets the index of the specified <paramref name="row"/>.
    /// </summary>
    /// <param name="row">The <see cref="IGlyphRow"/> to query.</param>
    /// <returns>
    /// The zero-based index of the <paramref name="row"/>; otherwise, 
    /// -1 if the row is not found.
    /// </returns>
    public int this[IGlyphRow row]
    {
        get => _rows.IndexOf(row);
    }

    /// <summary>
    /// Gets or sets the <see cref="IReadOnlyList{Glyph}"/> to render.
    /// </summary>
    /// <value>
    /// An <see cref="IReadOnlyList{IGlyphRow}"/> containing one or more elements; otherwise,
    /// a null reference if the content is not set.
    /// </value>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="ContentChangedEventArgs"/>
    /// </remarks>
    public IReadOnlyList<Glyph> Content
    {
        get => _content;
        set
        {
            if (!ReferenceEquals(value, _content))
            {
                _content = value;
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
            if (_glyphTable.TryGetValue(codePoint, out GlyphRenderer renderer))
            {
                return renderer.Metrics.Glyph;
            }
            return null;
        }
    }

    /// <summary>
    /// Gets the <see cref="HeaderRow"/> for the specified <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The <see cref="UnicodeRange"/> to query.</param>
    /// <returns>
    /// The <see cref="HeaderRow"/> for the specified <paramref name="range"/>; otherwise, 
    /// a null reference if the <see cref="Content"/> does not contain glyphs in the specified <paramref name="range"/>.
    /// </returns>
    public HeaderRow this[UnicodeRange range]
    {
        get
        {
            if (range is not null && _headers.TryGetValue(range.Id, out HeaderRow headerRow))
            {
                return headerRow;
            }
            return null;
        }
    }

    /// <summary>
    /// Gets list of <see cref="UnicodeRange"/> elements in the <see cref="Content"/>.
    /// </summary>
    /// <value>
    /// An <see cref="IReadOnlyList{UnicodeRange}"/> containing zero or more elements.
    /// </value>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="UnicodeRangesChangedEventArgs"/>.
    /// </remarks>
    public IReadOnlyList<UnicodeRange> UnicodeRanges
    {
        get => _unicodeRanges;
    }

    #endregion Properties

    #region ContentChanged

    void OnContentChanged(IReadOnlyList<Glyph> content)
    {
        bool needsArrange = _items.Count > 0;
        _items.Clear();
        _glyphTable.Clear();
        _unicodeRanges.Clear();

        List<UnicodeRange> unicodeRanges = [];
        float width = 0;
        float height = 0;

        if (content is not null && content.Count > 0)
        {
            UnicodeRange unicodeRange = UnicodeRange.Empty;
            GlyphRange glyphRange = null;

            using (SKPaint paint = new SKPaint() { IsAntialias = true })
            {
                foreach (Glyph glyph in content)
                {
                    if (_glyphTable.ContainsKey(glyph.CodePoint))
                    {
                        continue;
                    }
                    char ch = (char)glyph.CodePoint;
                    GlyphRenderer renderer = new(glyph, _drawContext, paint);
                    if (renderer.PreferredSize.Height == 0)
                    {
                        continue;
                    }

                    if (glyph.Range != unicodeRange)
                    {
                        glyphRange = new(unicodeRange);
                        _glyphRanges.Add(glyphRange.UnicodeRange.Id,  glyphRange);

                        unicodeRange = glyph.Range;
                        _unicodeRanges.Add(unicodeRange);
                    }

                    // calculate maximum size of all glyphs.
                    height = Math.Max(renderer.Metrics.Size.Height, height);
                    width = Math.Max(renderer.Metrics.Size.Width, width);

                    glyphRange.Add(renderer);
                    _items.Add(renderer);
                    _glyphTable.Add(glyph.CodePoint, renderer);
                }
            }
        }

        _drawContext.GlyphSize = new(width, height);

        _unicodeRanges.AddRange(unicodeRanges);
        if (needsArrange)
        {
            OnPropertyChanged(UnicodeRangesChangedEventArgs);
            InvalidateArrange();
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
            _view.InvalidateSurface();
        }
    }

    void Arrange(SKSize size)
    {
        if (!_needsArrange && size == DrawContext.CanvasSize)
        {
            return;
        }
        DrawContext.CanvasSize = size;
        bool hasContent = _rows.Count > 0;

        if (hasContent)
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

        _rows.Clear();
        _headers.Clear();

        if (hasContent)
        {
            using SKPaint paint = new() { IsAntialias = true };

            GlyphRow row = null;
            UnicodeRange currentRange = UnicodeRange.Empty;
            HeaderRow header = null;

            for (int i = 0; i < _items.Count; i++)
            {
                GlyphRenderer renderer = new(_content[i], DrawContext, paint);
                UnicodeRange unicodeRange = renderer.Metrics.Glyph.Range;

                // TODO: Handle GlyphLayoutStyle

                if (unicodeRange != currentRange)
                {
                    header = new(_drawContext, unicodeRange, header);
                    _headers.Add(header.UnicodeRange.Id, header);
                    // NOTE: The header row for the first row is not added to the _rows list
                    // to avoid the header being drawn twice for the first glyph group.
                    if (_rows.Count > 0)
                    {
                        _rows.Add(header);
                    }
                    currentRange = unicodeRange;
                    row = null;
                }
                else if (!row.Add(renderer))
                {
                    row = null;
                }
                if (row is null)
                {
                    row = new(_drawContext);
                    // assuming the first call to add always succeeds.
                    _ = row.Add(renderer);
                }
            }
        }

        if (hasContent)
        {
            OnPropertyChanged(ContentChangedEventArgs);
            OnPropertyChanged(CountChangedEventArgs);
#if (false)
            if (previousRow is not null)
            {
                // After arrange, set the current row to reflect
                // the previous row.
                SetFirstRow(previousRow, previousGlyph);
            }
#else
            FirstRow = 0;
#endif
        }
        _size = size;
        _needsArrange = false;
    }

    void SizeRenderer(GlyphRenderer renderer)
    {
        switch (_drawContext.LayoutStyle)
        {
            case GlyphLayoutStyle.Default:
                break;
        }


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

    #region Draw

    /// <summary>
    /// Indicates the view needs to be redrawn.
    /// </summary>
    public void InvalidateDraw()
    {
        _view?.InvalidateSurface();
    }

    /// <summary>
    /// Draws the <see cref="Content"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="canvasSize">The size of the <paramref name="canvas"/>.</param>
    public void Draw(SKCanvas canvas, SKSize canvasSize)
    {
        // NOTE: Calling arrange unconditionally
        Arrange(canvasSize);

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
                _currentHeader.Arrange(new SKPoint(x, y), new(width, _currentHeader.Bounds.Height));
                _currentHeader.Draw(canvas, paint);
                y += _currentHeader.Bounds.Height;
            }
            for (int i = FirstRow; i < _rows.Count; i++)
            {
                IGlyphRow row = _rows[i];
                // TODO: Set size based on GlyphLayoutStyle
                row.Arrange(new SKPoint(x, y), row.Bounds.Size);
                row.Draw(canvas, paint);
                y += row.Bounds.Height;
                if (y > height)
                {
                    break;
                }
            }
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
                uint id = glyphRow[0].Glyph.Range.Id;
                result = _headers[id];
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
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="Content"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs ContentChangedEventArgs = new(nameof(Content));

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="FirstRow"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs FirstRowChangedEventArgs = new(nameof(FirstRow));

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="Rows"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs RowsChangedEventArgs = new(nameof(Rows));

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="Count"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs CountChangedEventArgs = new(nameof(Count));

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="UnicodeRanges"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs UnicodeRangesChangedEventArgs = new(nameof(UnicodeRanges));

    #endregion PropertyChangedEventArgs
}
