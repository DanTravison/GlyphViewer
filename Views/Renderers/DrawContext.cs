namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Text;
using SkiaSharp;
using SkiaSharp.Views.Maui;

sealed class DrawContext : IDisposable
{
    #region Fields

    SKFont _headerFont;
    SKFont _itemFont;

    delegate void PropertyChangedHandler(GlyphsView view, DrawContext context);
    static readonly Dictionary<string, PropertyChangedHandler> _handlers = new(StringComparer.Ordinal);

    static DrawContext()
    {
        _handlers.Add(GlyphsView.HeaderColorProperty.PropertyName, OnHeaderColorChanged);
        _handlers.Add(GlyphsView.HeaderBackgroundColorProperty.PropertyName, OnHeaderBackgroundColorChanged);
        _handlers.Add(GlyphsView.HeaderFontSizeProperty.PropertyName, OnHeaderFontSizeChanged);
        _handlers.Add(GlyphsView.HeaderFontFamilyProperty.PropertyName, OnHeaderFontFamilyChanged);
        _handlers.Add(GlyphsView.HeaderFontAttributesProperty.PropertyName, OnHeaderFontAttributesChanged);

        _handlers.Add(GlyphsView.ItemsProperty.PropertyName, OnItemsChanged);
        _handlers.Add(GlyphsView.ItemFontSizeProperty.PropertyName, OnItemFontSizeChanged);
        _handlers.Add(GlyphsView.ItemColorProperty.PropertyName, OnItemColorChanged);
        _handlers.Add(GlyphsView.SelectedItemColorProperty.PropertyName, OnSelectedItemColorChanged);
        _handlers.Add(GlyphsView.SelectedItemProperty.PropertyName, OnSelectedItemChanged);

        _handlers.Add(GlyphsView.HorizontalSpacingProperty.PropertyName, OnHorizontalSpacingChanged);
        _handlers.Add(GlyphsView.VerticalSpacingProperty.PropertyName, OnVerticalSpacingChanged);
    }

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="view">The parent <see cref="GlyphsView"/>.</param>
    public DrawContext(GlyphsView view)
    {
        ArgumentNullException.ThrowIfNull(view, nameof(view));
        View = view;

        // Initialize the properties.
        foreach (PropertyChangedHandler handler in _handlers.Values)
        {
            handler(view, this);
        }
    }

    /// <summary>
    /// Notifies the <see cref="DrawContext"/> that a property has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    public void OnViewPropertyChanged(string propertyName)
    {
        if (_handlers.TryGetValue(propertyName, out PropertyChangedHandler handler))
        {
            handler(View, this);
            View.InvalidateSurface();
        }
    }

    /// <summary>
    /// Gets the containing <see cref="GlyphsView"/>
    /// </summary>
    public GlyphsView View
    {
        get;
        private set;
    }

    #region Header Properties

    public SKFont HeaderFont
    {
        get
        {
            if (_headerFont is null)
            {
                _headerFont = GetFont(HeaderFontFamily, HeaderFontSize, HeaderFontStyle);
            }
            return _headerFont;
        }
    }

    public string HeaderFontFamily
    {
        get;
        private set;
    }
    static void OnHeaderFontFamilyChanged(GlyphsView view, DrawContext context)
    {
        context.HeaderFontFamily = view.HeaderFontFamily;
        context._headerFont?.Dispose();
        context._headerFont = null;
    }

    public float HeaderFontSize
    {
        get;
        private set;
    }
    static void OnHeaderFontSizeChanged(GlyphsView view, DrawContext context)
    {
        context.HeaderFontSize = (float)view.HeaderFontSize;
        context._headerFont?.Dispose();
        context._headerFont = null;
    }

    public SKFontStyle HeaderFontStyle
    {
        get;
        private set;
    }
    static void OnHeaderFontAttributesChanged(GlyphsView view, DrawContext context)
    {
        context.HeaderFontStyle = view.HeaderFontAttributes.ToFontStyle();
        context._headerFont?.Dispose();
        context._headerFont = null;
    }

    public SKColor HeaderColor
    {
        get;
        private set;
    }
    static void OnHeaderColorChanged(GlyphsView view, DrawContext context)
    {
        context.HeaderColor = view.HeaderColor.ToSKColor();
    }

    public SKColor HeaderBackgroundColor
    {
        get;
        private set;
    }
    static void OnHeaderBackgroundColorChanged(GlyphsView view, DrawContext context)
    {
        context.HeaderBackgroundColor = view.HeaderBackgroundColor.ToSKColor();
    }

    #endregion Header Properties

    #region Item Properties

    public float ItemWidth
    {
        get;
        set;
    }

    public int ItemHeight
    {
        get;
        set;
    }

    public string ItemFontFamily
    {
        get;
        private set;
    }
    static void OnItemsChanged(GlyphsView view, DrawContext context)
    {
        GlyphCollection items = view.Items;
        if (items is not null && items.Count > 0)
        {
            context.ItemFontFamily = items[0].FontFamily;
            context._itemFont?.Dispose();
            context._itemFont = null;
        }
    }

    public float ItemFontSize
    {
        get;
        private set;
    }
    static void OnItemFontSizeChanged(GlyphsView view, DrawContext context)
    {
        context.ItemFontSize = (float)view.ItemFontSize;
        context._itemFont?.Dispose();
        context._itemFont = null;
    }

    public SKFont ItemFont
    {
        get
        {
            if (_itemFont is null)
            {
                _itemFont = GetFont(ItemFontFamily, ItemFontSize, SKFontStyle.Normal);
            }
            return _itemFont;
        }
    }

    public SKColor ItemColor
    {
        get;
        private set;
    }
    static void OnItemColorChanged(GlyphsView view, DrawContext context)
    {
        context.ItemColor = view.ItemColor.ToSKColor();
    }

    public Glyph SelectedItem
    {
        get;
        private set;
    }
    static void OnSelectedItemChanged(GlyphsView view, DrawContext context)
    {
        context.SelectedItem = view.SelectedItem;
    }

    public SKColor SelectedItemColor
    {
        get;
        private set;
    }
    static void OnSelectedItemColorChanged(GlyphsView view, DrawContext context)
    {
        context.SelectedItemColor = view.SelectedItemColor.ToSKColor();
    }

    #endregion Item Properties

    /// <summary>
    /// Gets the width of a Glyph column.
    /// </summary>
    /// <remarks>
    /// This property is set directly by the <see cref="GlyphsView"/>.
    /// </remarks>
    public float ColumnWidth
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the height of a Glyph row.
    /// </summary>
    /// <remarks>
    /// This property is set directly by the <see cref="GlyphsView"/>.
    /// </remarks>
    public float RowHeight
    {
        get;
        set;
    }

    public float HorizontalSpacing
    {
        get;
        private set;
    }
    static void OnHorizontalSpacingChanged(GlyphsView view, DrawContext context)
    {
        context.HorizontalSpacing = (float)view.HorizontalSpacing;
    }

    public float VerticalSpacing
    {
        get;
        private set;
    }
    static void OnVerticalSpacingChanged(GlyphsView view, DrawContext context)
    {
        context.VerticalSpacing = (float)view.VerticalSpacing;
    }



    static SKFont GetFont(string familyName, float fontSize, SKFontStyle fontStyle)
    {
        using (SKTypeface typeface = SKTypeface.FromFamilyName(familyName, fontStyle))
        {
            SKFont font = typeface.ToFont();
            font.Size = fontSize.ToPixels();
            return font;
        }
    }

    /// <summary>
    /// Releases all resources and references.
    /// </summary>
    public void Dispose()
    {
        if (View is not null)
        {
            _headerFont?.Dispose();
            _itemFont?.Dispose();
            _headerFont = _itemFont = null;
            View = null;
            GC.SuppressFinalize(this);
        }
    }
}
