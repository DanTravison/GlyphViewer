namespace GlyphViewer.Views;

using GlyphViewer.Text;
using GlyphViewer.Text.Unicode;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Globalization;

class GlyphsView : SKCanvasView
{
    class GlyphInfo
    {
        public GlyphInfo(GlyphMetrics metrics)
        {
            Metrics = metrics;
            Unicode = Ranges.Find(metrics.Glyph.CodePoint);
        }

        public Glyph Glyph
        {
            get => Metrics.Glyph;
        }

        public GlyphMetrics Metrics
        {
            get;
        }

        public Range Unicode
        {
            get;
        }
    }

    #region Constants

    const double MinimumSpacing = 2.0;
    const double DefaultSpacing = 5.0;
    const float MinimumItemSize = 12f;
    const double MinimumFontSize = 12f;
    const double DefaultFontSize = 64f;
    static readonly Color DefaultItemColor = Colors.Black;
    static readonly Color DefaultSelectedItemColor = Colors.Plum;

    #endregion Constants

    #region Fields

    // The number of rows based on the _columnCount
    // Updated by LayoutItems.
    int _rows;

    /// <summary>
    /// The row to display at the top of the canvas.
    /// </summary>
    int _firstRow;

    /// <summary>
    /// The height of a row in pixels
    /// </summary>
    /// <remarks>
    /// Maximum width of a glyph in the column + 2 * HorizontalSpacing
    /// </remarks>
    float _rowHeight;

    /// <summary>
    /// The width of a column in pixels
    /// </summary>  
    /// <remarks>
    /// Maximum width of a glyph in the column + 2 * HorizontalSpacing
    /// </remarks>
    float _columnWidth;

    // the number of columns based on the view width (versus CanvasSize.Width)
    // Updated by LayoutItems.
    int _columns;

    bool _needsLayout = false;

    /// <summary>
    /// The <see cref="SKFont"/> to use to draw the glyph.
    /// </summary>
    SKFont _glyphFont;

    GlyphInfo _selectedItem;

    // FUTURE: TextColor and TextFont
    // when the Glyph.Code is drawn.

    readonly List<GlyphInfo> _items = [];
    readonly Dictionary<ushort, GlyphInfo> _glyphs = new();

    #endregion Fields

    public GlyphsView()
    {
        base.EnableTouchEvents = true;
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
        nameof(HorizontalSpacingProperty),
        typeof(double),
        typeof(GlyphsView),
        DefaultSpacing,
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
        nameof(VerticalSpacingProperty),
        typeof(double),
        typeof(GlyphsView),
        DefaultSpacing,
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
        nameof(ItemColorProperty),
        typeof(Color),
        typeof(GlyphsView),
        DefaultItemColor,
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
        nameof(SelectedItemColorProperty),
        typeof(Color),
        typeof(GlyphsView),
        DefaultSelectedItemColor,
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

    #region FontSize

    /// <summary>
    /// Gets or sets the spacing between the glyphs in the vertical direction.
    /// </summary>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="FontSize"/> property.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create
    (
        nameof(FontSizeProperty),
        typeof(double),
        typeof(GlyphsView),
        DefaultFontSize,
        coerceValue: (bindable, value) =>
        {
            if (value is double spacing)
            {
                if (spacing >= MinimumFontSize)
                {
                    return spacing;
                }
            }
            return MinimumFontSize;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphsView view)
            {
                view.OnItemsChanged();
            }
        }
    );

    #endregion FontSize

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
        if (Items is not null && Items.Count > 0)
        {
            float height = MinimumItemSize;
            float width = MinimumItemSize;
            using (SKPaint paint = new SKPaint() { IsAntialias = true })
            {
                using (SKTypeface typeface = SKTypeface.FromFamilyName(Items.FamilyName, SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright))
                {
                    _glyphFont?.Dispose();
                    _glyphFont = typeface.ToFont();
                    _glyphFont.Size = (float)FontSize;
                    foreach (Glyph glyph in Items)
                    {
                        if (_glyphs.ContainsKey(glyph.CodePoint))
                        {
                            continue;
                        }
                        char ch = (char)glyph.CodePoint;
                        UnicodeCategory unicodeCategory = char.GetUnicodeCategory(ch);
                        /*
                        if (!CanDisplay(unicodeCategory))
                        {
                            continue;
                        }
                        */

                        GlyphMetrics metrics = GlyphMetrics.CreateInstance(paint, _glyphFont, glyph);
                        if (metrics.Size.Height == 0)
                        {
                            continue;
                        }
                        height = Math.Max(metrics.Size.Height, height);
                        width = Math.Max(metrics.TextWidth, width);
                        GlyphInfo info = new(metrics);
                        _items.Add(info);
                        _glyphs.Add(glyph.CodePoint, info);
                    }
                }
            }

            float dimension = Math.Max(width, height);
            _columnWidth = dimension + 2 * (float)HorizontalSpacing;
            _rowHeight = dimension + 2 * (float)VerticalSpacing;
        }
        else
        {
            _glyphFont?.Dispose();
            _glyphFont = null;
        }
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
            if (bindable is GlyphsView view)
            {
                List<GlyphInfo> items = view._items;
                if (items.Count > 0 && value is int row)
                {
                    if (row < 0)
                    {
                        row = 0;
                    }
                    else if (row >= items.Count)
                    {
                        row = items.Count - 1;
                    }
                    return row;
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

    #region SelectedItemMetrics

    /// <summary>
    /// Gets or sets the selected <see cref="Glyph"/>.
    /// </summary>
    public GlyphMetrics SelectedItemMetrics
    {
        get => (GlyphMetrics) GetValue(SelectedItemMetricsProperty);
        set => SetValue(SelectedItemMetricsProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="SelectedItemMetrics"/>  property.
    /// </summary>
    public static readonly BindableProperty SelectedItemMetricsProperty = BindableProperty.Create
    (
        nameof(SelectedItemMetrics),
        typeof(GlyphMetrics),
        typeof(GlyphsView),
        GlyphMetrics.Empty,
        BindingMode.OneWayToSource,
        coerceValue: (bindable, value) =>
        {
            if (value is GlyphMetrics metrics)
            {
                return metrics;
            }
            return GlyphMetrics.Empty;
        }
    );

    #endregion SelectedItemMetrics

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
        GlyphInfo info;

        Glyph glyph = SelectedItem;
        if (glyph is not null && !glyph.IsEmpty)
        {
            _glyphs.TryGetValue(SelectedItem.CodePoint, out info);
        }
        else
        {
            info = null;
        }
        SelectItem(info);
    }

    void SelectItem(GlyphInfo info)
    {
        if (!ReferenceEquals(info, _selectedItem))
        {
            _selectedItem = info;
            if (info is not null)
            {
                SelectedItemMetrics = info.Metrics;
            }
            else
            {
                SelectedItemMetrics = GlyphMetrics.Empty;
            }
            InvalidateSurface();
        }
    }

    #endregion SelectedItem

    #endregion Properties

    #region Touch Interaction

    protected override void OnTouch(SKTouchEventArgs e)
    {
        if (e.ActionType == SKTouchAction.Pressed)
        {
            if (_items.Count > 0)
            {
                SKPoint point = e.Location;
                GlyphInfo info = HitTest(point.X, point.Y);
                SelectedItem = info.Glyph;
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
            if (row >= 0 && row < _rows)
            {
                Row = row;
            }
        }
        e.Handled = true;
    }

    GlyphInfo HitTest(float x, float y)
    {
        if (_items.Count == 0)
        {
            return null;
        }
        int column = (int)(x / _columnWidth);
        int row = (int)(y / _rowHeight) + _firstRow;
        int index = row * _columns + column;
        if (index < 0 || index >= _items.Count)
        {
            return null;
        }
        return _items[index];
    }

    #endregion Touch Interaction

    #region Layout

    void LayoutItems(SKSize size)
    {
        int rows = 0;
        int columns = 0;

        if (_items.Count > 0)
        {
            columns = (int)(size.Width / _columnWidth);
            rows = _items.Count / columns;
            if (_items.Count % columns != 0)
            {
                rows++;
            }
            _needsLayout = false;
        }
        _columns = columns;
        if (rows > 0)
        {
            Row = Math.Min(_firstRow, rows - 1);
        }
        else
        {
            Row = 0;
        }
        _rows = rows;
        Rows = rows;
    }

    protected override Size ArrangeOverride(Rect bounds)
    {
        _needsLayout = true;
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

            int count = _items.Count;
            int columnCount = _columns;
            int row = _firstRow;
            int column = 0;

            float x = 0;
            float y = 0;

            using (SKPaint paint = new() 
            { 
                IsAntialias = true, 
                Color = ItemColor.ToSKColor(), 
                Style = SKPaintStyle.Fill
            })
            {
                int firstIndex = _firstRow * columnCount;
                // TODO: set index to the first item in the first displayed row.
                for (int index = firstIndex; index < _items.Count; index++)
                { 
                    GlyphInfo info = _items[index];

                    DrawGlyph(info, canvas, paint, x, y);

                    column++;
                    if (column >= columnCount)
                    {
                        column = 0;
                        row++;
                        x = 0;
                        y += _rowHeight;
                    }
                    else
                    {
                        x += _columnWidth;
                    }

                    if (y >= height)
                    {
                        break;
                    }
                }
            }
        }
    }

    void DrawGlyph(GlyphInfo info, SKCanvas canvas, SKPaint paint, float x, float y)
    {
        GlyphMetrics metrics = info.Metrics;
        float top = y + (_rowHeight - metrics.Size.Height) / 2;
        float start = x + (_columnWidth - metrics.Size.Width) / 2;
        float baseLine = top - metrics.Ascent;
        float strokeWidth = 2;

        canvas.DrawText(metrics.Glyph.Text, start, baseLine, SKTextAlign.Left, _glyphFont, paint);

        if (_selectedItem is not null && metrics.Glyph == _selectedItem.Glyph)
        {
            float horizontalSpacing = (float)HorizontalSpacing;
            float verticalSpacing = (float)VerticalSpacing;
            SKRect bounds = new SKRect
            (
                x + strokeWidth / 2,
                y + strokeWidth / 2, 
                x + _columnWidth - strokeWidth / 2, 
                y + _rowHeight - strokeWidth / 2
            );
            paint.Color = SelectedItemColor.ToSKColor();
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;

            canvas.DrawRect(bounds, paint);

            paint.Style = SKPaintStyle.Fill;
            paint.Color = ItemColor.ToSKColor();
        }
    }

    #endregion Draw
}
