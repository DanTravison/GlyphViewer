namespace GlyphViewer.Views;

using GlyphViewer.Settings;
using GlyphViewer.Text;
using GlyphViewer.Views.Renderers;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Range = Text.Unicode.Range;

/// <summary>
/// Provides a view of the glyphs in a <see cref="SKTypeface"/>.
/// </summary>
public sealed class GlyphsView : SKCanvasView
{
    #region Constants

    /// <summary>
    /// Defines the minimum <see cref="VerticalSpacing"/> and <see cref="HorizontalSpacing"/>.
    /// </summary>
    public const double MinimumSpacing = 2.0;

    /// <summary>
    /// Defines the default <see cref="VerticalSpacing"/> and <see cref="HorizontalSpacing"/>.
    /// </summary>
    public const double DefaultSpacing = 5.0;

    /// <summary>
    /// Defines the default <see cref="HeaderColor"/>.
    /// </summary>
    public static readonly Color DefaultHeaderColor = Colors.Black;

    /// <summary>
    /// Defines the default <see cref="ItemColor"/>.
    /// </summary>
    public static readonly Color DefaultItemColor = Colors.Black;

    /// <summary>
    /// Defines the default colors for the <see cref="SelectedItem"/> border.
    /// </summary>
    public static readonly Color DefaultSelectedItemColor = Colors.Plum;

    #endregion Constants

    #region Fields

    /// <summary>
    /// The row to display at the top of the canvas.
    /// </summary>
    int _firstRow;

    /// <summary>
    /// Gets the size of the view from the last call to ArrangeOverride.
    /// </summary>
    Size _layoutSize = Size.Zero;

    /// <summary>
    /// Determines if layout is needed.
    /// </summary>
    bool _needsLayout = false;

    /// <summary>
    /// The <see cref="SKFont"/> to use to draw the glyph.
    /// </summary>
    SKFont _glyphFont;

    /// <summary>
    /// The <see cref="SKFont"/> to use to draw a header row.
    /// </summary>
    SKFont _headerFont;

    /// <summary>
    /// The currently selected glyph.
    /// </summary>
    GlyphMetrics _selectedItem;

    readonly List<GlyphMetrics> _items = [];
    readonly Dictionary<ushort, GlyphMetrics> _glyphs = [];
    readonly List<IGlyphRow> _rows = [];
    readonly DrawContext _context;
    readonly Dictionary<uint, HeaderRow> _headers = [];
    HeaderRow _currentHeader;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public GlyphsView()
    {
        base.EnableTouchEvents = true;
        _context = new DrawContext(this);
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        // NOTE: OnPropertyChanged can be called before constructor completes.
        _context?.OnViewPropertyChanged(propertyName);
    }

    #region Properties

    #region HorizontalSpacing

    /// <summary>
    /// Gets or sets the spacing between the glyphs in the horizontal direction.
    /// </summary>
    public double HorizontalSpacing
    {
        get => (double)GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HorizontalSpacing"/> property.
    /// </summary>
    public static readonly BindableProperty HorizontalSpacingProperty = BindableProperty.Create
    (
        nameof(HorizontalSpacing),
        typeof(double),
        typeof(GlyphsView),
        DefaultSpacing,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is double spacing)
            {
                if (spacing >= MinimumSpacing)
                {
                    return spacing;
                }
            }
            return MinimumSpacing;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.InvalidateSurface();
            }
        }
    );

    #endregion HorizontalSpacing

    #region VerticalSpacing

    /// <summary>
    /// Gets or sets the spacing between the glyphs in the vertical direction.
    /// </summary>
    public double VerticalSpacing
    {
        get => (double)GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="VerticalSpacing"/> property.
    /// </summary>
    public static readonly BindableProperty VerticalSpacingProperty = BindableProperty.Create
    (
        nameof(VerticalSpacing),
        typeof(double),
        typeof(GlyphsView),
        DefaultSpacing,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is double spacing)
            {
                if (spacing >= MinimumSpacing)
                {
                    return spacing;
                }
            }
            return MinimumSpacing;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.InvalidateSurface();
            }
        }
    );

    #endregion VerticalSpacing

    #region ItemColor

    /// <summary>
    /// Gets or sets the color to use to draw the glyph.
    /// </summary>
    public Color ItemColor
    {
        get => GetValue(ItemColorProperty) as Color;
        set => SetValue(ItemColorProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="ItemColor"/> property.
    /// </summary>
    public static readonly BindableProperty ItemColorProperty = BindableProperty.Create
    (
        nameof(ItemColor),
        typeof(Color),
        typeof(GlyphsView),
        DefaultItemColor,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return DefaultItemColor;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.InvalidateSurface();
            }
        }
    );

    #endregion ItemColor

    #region SelectedItemColor

    /// <summary>
    /// Gets or sets the color to use to draw the glyph.
    /// </summary>
    public Color SelectedItemColor
    {
        get => GetValue(SelectedItemColorProperty) as Color;
        set => SetValue(SelectedItemColorProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="SelectedItemColor"/> property.
    /// </summary>
    public static readonly BindableProperty SelectedItemColorProperty = BindableProperty.Create
    (
        nameof(SelectedItemColor),
        typeof(Color),
        typeof(GlyphsView),
        DefaultSelectedItemColor,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return DefaultSelectedItemColor;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.InvalidateSurface();
            }
        }
    );

    #endregion SelectedItemColor

    #region ItemFontSize

    /// <summary>
    /// Gets or sets the font size for a <see cref="Glyph"/> item.
    /// </summary>
    public double ItemFontSize
    {
        get => (double)GetValue(ItemFontSizeProperty);
        set => SetValue(ItemFontSizeProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="ItemFontSize"/> property.
    /// </summary>
    public static readonly BindableProperty ItemFontSizeProperty = BindableProperty.Create
    (
        nameof(ItemFontSize),
        typeof(double),
        typeof(GlyphsView),
        ItemFontSetting.Default,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is double fontSize)
            {
                return Math.Clamp(fontSize, ItemFontSetting.Minimum, ItemFontSetting.Maximum);
            }
            return ItemFontSetting.Default;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.OnItemsChanged();
            }
        }
    );

    #endregion ItemFontSize

    #region HeaderColor

    /// <summary>
    /// Gets or sets the color to use to draw the glyph.
    /// </summary>
    public Color HeaderColor
    {
        get => GetValue(HeaderColorProperty) as Color;
        set => SetValue(HeaderColorProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HeaderColor"/> property.
    /// </summary>
    public static readonly BindableProperty HeaderColorProperty = BindableProperty.Create
    (
        nameof(HeaderColor),
        typeof(Color),
        typeof(GlyphsView),
        UserSettings.DefaultItemHeaderColor,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return DefaultHeaderColor;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.InvalidateSurface();
            }
        }
    );

    #endregion HeaderColor

    #region HeaderBackgroundColor

    /// <summary>
    /// Gets or sets the color to use to draw the glyph.
    /// </summary>
    public Color HeaderBackgroundColor
    {
        get => GetValue(HeaderBackgroundColorProperty) as Color;
        set => SetValue(HeaderBackgroundColorProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HeaderBackgroundColor"/> property.
    /// </summary>
    public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create
    (
        nameof(HeaderBackgroundColor),
        typeof(Color),
        typeof(GlyphsView),
        Colors.Transparent,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return Colors.Transparent;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.InvalidateSurface();
            }
        }
    );

    #endregion HeaderBackgroundColor

    #region HeaderFontFamily

    /// <summary>
    /// Gets or sets the font to use for header text.
    /// </summary>
    public string HeaderFontFamily
    {
        get => (string)GetValue(HeaderFontFamilyProperty);
        set => SetValue(HeaderFontFamilyProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HeaderFontFamily"/> property.
    /// </summary>
    public static readonly BindableProperty HeaderFontFamilyProperty = BindableProperty.Create
    (
        nameof(HeaderFontFamily),
        typeof(string),
        typeof(GlyphsView),
        ItemHeaderFontSetting.DefaultFontFamily,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is string family)
            {
                family = family.Trim();
                if (family.Length > 0)
                {
                    return value;
                }
            }
            return ItemHeaderFontSetting.DefaultFontFamily;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.OnHeaderFontChanged();
            }
        }
    );

    #endregion HeaderFontFamily

    #region HeaderFontSize

    /// <summary>
    /// Gets or sets the font size for the header text.
    /// </summary>
    public double HeaderFontSize
    {
        get => (double)GetValue(HeaderFontSizeProperty);
        set => SetValue(HeaderFontSizeProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HeaderFontSize"/> property.
    /// </summary>
    public static readonly BindableProperty HeaderFontSizeProperty = BindableProperty.Create
    (
        nameof(HeaderFontSize),
        typeof(double),
        typeof(GlyphsView),
        ItemHeaderFontSetting.Default,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is double fontSize)
            {
                return Math.Clamp(fontSize, ItemHeaderFontSetting.Minimum, ItemHeaderFontSetting.Maximum);
            }
            return ItemHeaderFontSetting.Default;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.OnHeaderFontChanged();
            }
        }
    );

    void OnHeaderFontChanged()
    {
        _headerFont?.Dispose();
        _headerFont = HeaderFontFamily.CreateFont((float)HeaderFontSize);
        _needsLayout = true;
        InvalidateSurface();
    }

    #endregion HeaderFontSize

    #region HeaderFontAttributes

    /// <summary>
    /// Gets or sets the <see cref="FontAttributes"/> for the header text.
    /// </summary>
    public FontAttributes HeaderFontAttributes
    {
        get => (FontAttributes)GetValue(HeaderFontAttributesProperty);
        set => SetValue(HeaderFontAttributesProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HeaderFontAttributes"/> property.
    /// </summary>
    public static readonly BindableProperty HeaderFontAttributesProperty = BindableProperty.Create
    (
        nameof(HeaderFontAttributes),
        typeof(FontAttributes),
        typeof(GlyphsView),
        ItemHeaderFontSetting.DefaultFontAttributes,
        BindingMode.OneWay
    );

    #endregion HeaderFontAttributes

    #region HeaderClickedCommand

    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> to invoke when a header is clicked.
    /// </summary>
    public ICommand HeaderClickedCommand
    {
        get => GetValue(HeaderClickedCommandProperty) as ICommand;
        set => SetValue(HeaderClickedCommandProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HeaderClickedCommand"/> property.
    /// </summary>
    public static readonly BindableProperty HeaderClickedCommandProperty = BindableProperty.Create
    (
        nameof(HeaderClickedCommand),
        typeof(ICommand),
        typeof(GlyphsView),
        null,
        BindingMode.OneWay
    );

    #endregion HeaderClickedCommand

    #region Items

    /// <summary>
    /// Gets or sets the <see cref="GlyphCollection"/> of items to display.
    /// </summary>
    public GlyphCollection Items
    {
        get => GetValue(ItemsProperty) as GlyphCollection;
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="Items"/> property.
    /// </summary>
    public static readonly BindableProperty ItemsProperty = BindableProperty.Create
    (
        nameof(Items),
        typeof(GlyphCollection),
        typeof(GlyphsView),
        null,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.OnItemsChanged();
            }
        }
    );

    void OnItemsChanged()
    {
        _items.Clear();
        _glyphs.Clear();
        _rows.Clear();
        List<Range> ranges = [];
        if (Items is not null && Items.Count > 0)
        {
            float height = 0;
            float width = 0;
            Range range = Range.Empty;
            using (SKPaint paint = new SKPaint() { IsAntialias = true })
            {
                foreach (Glyph glyph in Items)
                {
                    if (_glyphs.ContainsKey(glyph.CodePoint))
                    {
                        continue;
                    }
                    char ch = (char)glyph.CodePoint;
                    UnicodeCategory unicodeCategory = char.GetUnicodeCategory(ch);
                    GlyphMetrics metrics = GlyphMetrics.CreateInstance(glyph, _context.ItemFont, paint);
                    if (metrics.Size.Height == 0)
                    {
                        continue;
                    }
                    if (metrics.Glyph.Range != range)
                    {
                        range = metrics.Glyph.Range;
                        ranges.Add(range);
                    }
                    height = Math.Max(metrics.Size.Height, height);
                    width = Math.Max(metrics.TextWidth, width);
                    _items.Add(metrics);
                    _glyphs.Add(glyph.CodePoint, metrics);
                }
            }

            float dimension = Math.Max(width, height);
            _context.ColumnWidth = dimension + 2 * (float)HorizontalSpacing;
            _context.RowHeight = dimension + 2 * (float)VerticalSpacing;
        }
        else
        {
            _glyphFont?.Dispose();
            _glyphFont = null;
        }
        UnicodeRanges = ranges;
        Row = 0;
        _needsLayout = true;
        InvalidateSurface();
    }

    #endregion Items

    #region Row

    /// <summary>
    /// Gets or sets the first row to display.
    /// </summary>
    public int Row
    {
        get => (int)GetValue(RowProperty);
        set => SetValue(RowProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="Row"/> property.
    /// </summary>
    public static readonly BindableProperty RowProperty = BindableProperty.Create
    (
        nameof(RowProperty),
        typeof(int),
        typeof(GlyphsView),
        0,
        BindingMode.TwoWay,
        coerceValue: (bindable, value) =>
        {
            if (bindable is GlyphsView view && value is int row)
            {
                if (row >= 0 && row < view._rows.Count)
                {
                    return value;
                }
            }
            return 0;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.OnRowChanged();
            }
        }
    );

    void OnRowChanged()
    {
        if (_firstRow != Row)
        {
            _firstRow = Row;
            InvalidateSurface();
        }
    }

    #endregion Row

    #region Rows

    /// <summary>
    /// Gets or sets the the number of rows
    /// </summary>
    public int Rows
    {
        get => (int)GetValue(RowsProperty);
        private set => SetValue(RowsProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="Rows"/> property.
    /// </summary>
    public static readonly BindableProperty RowsProperty = BindableProperty.Create
    (
        nameof(Rows),
        typeof(int),
        typeof(GlyphsView),
        0,
        BindingMode.OneWayToSource
    );

    #endregion Rows

    #region SelectedItem

    /// <summary>
    /// Gets or sets the selected <see cref="Glyph"/>.
    /// </summary>
    public Glyph SelectedItem
    {
        get => (Glyph)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="SelectedItem"/>  property.
    /// </summary>
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create
    (
        nameof(SelectedItem),
        typeof(Glyph),
        typeof(GlyphsView),
        Glyph.Empty,
        BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.OnSelectedItemChanged();
            }
        }
    );

    void OnSelectedItemChanged()
    {
        Glyph glyph = SelectedItem;
        if (glyph is not null && !glyph.IsEmpty)
        {
            _glyphs.TryGetValue(SelectedItem.CodePoint, out _selectedItem);
        }
        else
        {
            _selectedItem = null;
        }
        InvalidateSurface();
    }

    #endregion SelectedItem

    #region UnicodeRanges

    /// <summary>
    /// Gets or sets the the number of rows
    /// </summary>
    public IReadOnlyList<Range> UnicodeRanges
    {
        get => GetValue(UnicodeRangesProperty) as IReadOnlyList<Range>;
        private set => SetValue(UnicodeRangesProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="UnicodeRanges"/> property.
    /// </summary>
    public static readonly BindableProperty UnicodeRangesProperty = BindableProperty.Create
    (
        nameof(UnicodeRanges),
        typeof(IReadOnlyList<Range>),
        typeof(GlyphsView),
        null,
        BindingMode.OneWayToSource
    );

    #endregion UnicodeRanges

    #region SelectedUnicodeRange

    /// <summary>
    /// Gets or sets the selected unicode range.
    /// </summary>
    public Range SelectedUnicodeRange
    {
        get => GetValue(SelectedUnicodeRangeProperty) as Range;
        private set => SetValue(SelectedUnicodeRangeProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="SelectedUnicodeRange"/> property.
    /// </summary>
    public static readonly BindableProperty SelectedUnicodeRangeProperty = BindableProperty.Create
    (
        nameof(SelectedUnicodeRange),
        typeof(Range),
        typeof(GlyphsView),
        Range.Empty,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Range range)
            {
                return range;
            }
            return Range.Empty;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.OnSelectedUnicodeRangeChanged();
            }
        }
    );

    void OnSelectedUnicodeRangeChanged()
    {
        Range range = SelectedUnicodeRange;
        if (!range.IsEmpty && _headers.TryGetValue(range.Id, out HeaderRow header))
        {
            int row;
            // Since the first unicode range header is not in the _rows list,
            // go to row 0
            if (header.Id == _items[0].Glyph.Range.Id)
            {
                Row = 0;
            }
            else
            {
                // go to the first row after the header row.
                row = _rows.IndexOf(header);
                if (row > 0 && row < _rows.Count - 1)
                {
                    Row = row + 1;
                }
            }
        }
    }

    #endregion SelectedUnicodeRange

    #endregion Properties

    #region Touch Interaction

    protected override void OnTouch(SKTouchEventArgs e)
    {
        if (e.ActionType == SKTouchAction.Pressed)
        {
            if (_items.Count > 0)
            {
                SKPoint point = e.Location;
                if (HitTest(point.X, point.Y, out IGlyphRow row, out GlyphMetrics metrics))
                {
                    if (row is HeaderRow)
                    {
                        HeaderClickedCommand?.Execute(row);
                    }
                    else if (!metrics.IsEmpty)
                    {
                        SelectedItem = metrics.Glyph;
                    }
                }
            }
        }
        else if (e.ActionType == SKTouchAction.WheelChanged)
        {
            int row = Row;
            if (e.WheelDelta > 0)
            {
                row--;
            }
            else
            {
                row++;
            }
            if (row >= 0 && row < _rows.Count)
            {
                Row = row;
            }
        }
        e.Handled = true;
    }

    bool HitTest(float x, float y, out IGlyphRow row, out GlyphMetrics metrics)
    {
        if (_currentHeader is not null && _currentHeader.HitTest(new SKPoint(x, y), out metrics))
        {
            row = _currentHeader;
            metrics = null;
            return true;
        }
        for (int i = _firstRow; i < _rows.Count; i++)
        {
            row = _rows[i];
            if (row.HitTest(new SKPoint(x, y), out metrics))
            {
                return true;
            }
        }
        metrics = null;
        row = null;
        return false;
    }

    #endregion Touch Interaction

    #region Layout

    void LayoutItems(SKSize size)
    {
        _rows.Clear();
        _headers.Clear();

        if (_items.Count > 0)
        {
            GlyphRow row = null;
            float columnWidth = _context.ColumnWidth;
            Range currentRange = Range.Empty;
            HeaderRow header = null;

            for (int i = 0; i < _items.Count; i++)
            {
                GlyphMetrics metrics = _items[i];
                Range range = metrics.Glyph.Range;
                if (range != currentRange)
                {
                    header = new(_context, range, header);
                    _headers.Add(header.Id, header);
                    // NOTE: The header row for the first row is not added to the _rows list.
                    // to avoid the header being drawn twice for the first glyph group.
                    if (_rows.Count > 0)
                    {
                        _rows.Add(header);
                    }
                    currentRange = range;
                    row = null;
                }
                else if (row.Bounds.Width + columnWidth > size.Width)
                {
                    row = null;
                }
                if (row is null)
                {
                    row = new GlyphRow(_context);
                    _rows.Add(row);
                }
                row.Add(metrics);
            }
            _needsLayout = false;
        }
        Rows = _rows.Count;
    }

    protected override Size ArrangeOverride(Rect bounds)
    {
        if (bounds.Size != _layoutSize)
        {
            _layoutSize = bounds.Size;
            _needsLayout = true;
        }
        return base.ArrangeOverride(bounds);
    }

    #endregion Layout

    #region Draw

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        if (BackgroundColor is not null && BackgroundColor != Colors.Transparent)
        {
            canvas.Clear(BackgroundColor.ToSKColor());
        }
        else
        {
            // clear the canvas with a transparent color
            canvas.Clear();
        }

        if (_needsLayout)
        {
            LayoutItems(CanvasSize);
        }

        if (_items.Count > 0)
        {
            float width = CanvasSize.Width;
            float height = CanvasSize.Height;
            float x = 0;
            float y = 0;

            using (SKPaint paint = new()
            {
                IsAntialias = true,
                Color = ItemColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            })
            {
                _currentHeader = GetHeaderRow();
                if (_currentHeader is not null)
                {
                    // Draw the header row in the header area
                    // above the list.
                    _currentHeader.Arrange(new SKPoint(x, y), width);
                    _currentHeader.Draw(canvas, paint);
                    y += _currentHeader.Bounds.Height;
                }
                for (int i = _firstRow; i < _rows.Count; i++)
                {
                    IGlyphRow row = _rows[i];
                    row.Arrange(new SKPoint(x, y), width);
                    row.Draw(canvas, paint);
                    y += row.Bounds.Height;
                    if (y > height)
                    {
                        break;
                    }
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

            IGlyphRow row = _rows[_firstRow];
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
}
