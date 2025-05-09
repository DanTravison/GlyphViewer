namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Views;
using GlyphViewer.Text;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using GlyphViewer.Settings;

sealed class DrawContext : IDisposable
{
    #region Fields

    SKFont _headerFont;
    SKFont _itemFont;
    GlyphsViewRenderer _layout;

    /// <summary>
    /// Provides a delegate for handling <see cref="GlyphsView"/> property changes.
    /// </summary>
    /// <param name="layout">The <see cref="GlyphsViewRenderer"/> for the <see cref="GlyphsView"/>.</param>
    /// <param name="context">The <see cref="DrawContext"/>.</param>
    /// <returns>true if arrange is needed; otherwise, false.</returns>
    delegate bool PropertyChangedHandler(GlyphsViewRenderer layout, DrawContext context);

    static readonly Dictionary<string, PropertyChangedHandler> _handlers = new(StringComparer.Ordinal);

    static DrawContext()
    {
        Add(GlyphsView.HeaderColorProperty, OnHeaderColorChanged);
        Add(GlyphsView.HeaderBackgroundColorProperty, OnHeaderBackgroundColorChanged);
        Add(GlyphsView.HeaderFontSizeProperty, OnHeaderFontSizeChanged);
        Add(GlyphsView.HeaderFontFamilyProperty, OnHeaderFontFamilyChanged);
        Add(GlyphsView.HeaderFontAttributesProperty, OnHeaderFontAttributesChanged);

        Add(GlyphsView.ItemsProperty, OnItemsChanged);
        Add(GlyphsView.ItemFontSizeProperty, OnItemFontSizeChanged);
        Add(GlyphsView.ItemColorProperty, OnItemColorChanged);
        Add(GlyphsView.SelectedItemColorProperty, OnSelectedItemColorChanged);
        Add(GlyphsView.SelectedItemProperty, OnSelectedItemChanged);
        Add(GlyphsView.SpacingProperty, OnSpacingChanged);
        Add(GlyphsView.CellLayoutProperty, OnCellLayoutChanged);
    }

    static void Add(BindableProperty property, PropertyChangedHandler handler)
    {
        _handlers.Add(property.PropertyName, handler);
    }

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="layout">The <see cref="GlyphsViewRenderer"/>.</param>
    public DrawContext(GlyphsViewRenderer layout)
    {
        ArgumentNullException.ThrowIfNull(layout, nameof(layout));
        _layout = layout;

        // Initialize the properties.
        foreach (PropertyChangedHandler handler in _handlers.Values)
        {
            handler(layout, this);
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
            bool needsArrange = handler(_layout, this);
            if (needsArrange)
            {
                _layout.InvalidateArrange();
            }
            else
            {
                _layout.InvalidateDraw();
            }
        }
    }

    #region Header Properties

    /// <summary>
    /// Get the font for drawing header text.
    /// </summary>
    public SKFont HeaderFont
    {
        get
        {
            if (_headerFont is null)
            {
                _headerFont = HeaderFontFamily.CreateFont(HeaderFontSize, HeaderFontStyle);
            }
            return _headerFont;
        }
    }

    /// <summary>
    /// Gets the font family name for the header rows.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.HeaderFontFamily"/>.
    /// </remarks>
    public string HeaderFontFamily
    {
        get;
        private set;
    }
    static bool OnHeaderFontFamilyChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        context.HeaderFontFamily = layout.View.HeaderFontFamily;
        context._headerFont?.Dispose();
        context._headerFont = null;
        return true;
    }

    /// <summary>
    /// Gets the font size for the header rows.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.HeaderFontSize"/>.
    /// </remarks>
    public float HeaderFontSize
    {
        get;
        private set;
    }
    static bool OnHeaderFontSizeChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        context.HeaderFontSize = (float)layout.View.HeaderFontSize;
        context._headerFont?.Dispose();
        context._headerFont = null;
        return true;
    }

    /// <summary>
    /// Gets the font style for the header rows.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.HeaderFontAttributes"/>.
    /// </remarks>
    public SKFontStyle HeaderFontStyle
    {
        get;
        private set;
    }
    static bool OnHeaderFontAttributesChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        context.HeaderFontStyle = layout.View.HeaderFontAttributes.ToFontStyle();
        context._headerFont?.Dispose();
        context._headerFont = null;
        return true;
    }

    /// <summary>
    /// Gets the text color for header rows.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.HeaderColor"/>.
    /// </remarks>
    public SKColor HeaderColor
    {
        get;
        private set;
    }
    static bool OnHeaderColorChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        context.HeaderColor = layout.View.HeaderColor.ToSKColor();
        return false;
    }

    /// <summary>
    /// Gets the background color for header rows.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.HeaderBackgroundColor"/>.
    /// </remarks>
    public SKColor HeaderBackgroundColor
    {
        get;
        private set;
    }
    static bool OnHeaderBackgroundColorChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        context.HeaderBackgroundColor = layout.View.HeaderBackgroundColor.ToSKColor();
        return false;
    }

    #endregion Header Properties

    #region Item Properties

    /// <summary>
    /// Gets the item font family name.
    /// </summary>
    /// <remarks>
    /// This property is set from the <see cref="Glyph.FontFamily"/> of the first item in <see cref="GlyphsView.Items"/>
    /// </remarks>
    public string ItemFontFamily
    {
        get;
        private set;
    }
    static bool OnItemsChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        GlyphCollection items = layout.View.Items;
        if (items is not null && items.Count > 0)
        {
            context.ItemFontFamily = items[0].FontFamily;
            context._itemFont?.Dispose();
            context._itemFont = null;
        }
        return true;
    }

    /// <summary>
    /// Gets the item font size.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.ItemFontSize"/>.
    /// </remarks>
    public float ItemFontSize
    {
        get;
        private set;
    }
    static bool OnItemFontSizeChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        context.ItemFontSize = (float)layout.View.ItemFontSize;
        context._itemFont?.Dispose();
        context._itemFont = null;
        return true;
    }

    /// <summary>
    /// Gets the item font.
    /// </summary>
    public SKFont ItemFont
    {
        get
        {
            if (_itemFont is null)
            {
                _itemFont = ItemFontFamily.CreateFont(ItemFontSize);

                // Calculate a minimum glyph size for the font.
                using (SKFont font = App.DefaultFontFamily.CreateFont(ItemFontSize))
                {
                    SKTextMetrics metrics = new("W", font);
                    float dimension = Math.Max(metrics.TextWidth, metrics.Size.Width);
                    MinimumGlyphSize = new SKSize(dimension, dimension);
                }
            }
            return _itemFont;
        }
    }

    /// <summary>
    /// Gets the item color.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.ItemColor"/>.
    /// </remarks>
    public SKColor ItemColor
    {
        get;
        private set;
    }
    static bool OnItemColorChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        context.ItemColor = layout.View.ItemColor.ToSKColor();
        return false;
    }

    /// <summary>
    /// Gets the currently selected glyph.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.SelectedItem"/>.
    /// </remarks>
    public Glyph SelectedItem
    {
        get;
        private set;
    }
    static bool OnSelectedItemChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        context.SelectedItem = layout.View.SelectedItem;
        return false;
    }

    /// <summary>
    /// Gets the color of the <see cref="SelectedItem"/>.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.SelectedItemColor"/>.
    /// </remarks>
    public SKColor SelectedItemColor
    {
        get;
        private set;
    }
    static bool OnSelectedItemColorChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        context.SelectedItemColor = layout.View.SelectedItemColor.ToSKColor();
        return false;
    }

    #endregion Item Properties

    #region Layout Properties

    /// <summary>
    /// Gets cached <see cref="CanvasSize"/>.
    /// </summary>
    /// <remarks>
    /// This property is only value during Arrange and Draw.
    /// </remarks>
    public SKSize CanvasSize
    {
        get;
        set;
    }

    /// <summary>
    /// Defines the maximum width and height of all glyphs
    /// </summary>
    public SKSize GlyphSize
    {
        get;
        set;
    }

    /// <summary>
    /// Defines the average width and height of all glyphs
    /// </summary>
    public SKSize MinimumGlyphSize
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the spacing around Glyphs.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.Spacing"/>.
    /// </remarks>
    public SkSpacing Spacing
    {
        get;
        private set;
    }
    static bool OnSpacingChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        SkSpacing spacing = new(layout.View.Spacing);
        if (spacing != context.Spacing)
        {
            context.Spacing = spacing;
            return true;
        }
        return false;
    }

    public CellLayoutStyle CellLayout
    {
        get;
        private set;
    }
    static bool OnCellLayoutChanged(GlyphsViewRenderer layout, DrawContext context)
    {
        CellLayoutStyle cellLayout = layout.View.CellLayout;
        if (cellLayout != context.CellLayout)
        {
            context.CellLayout = cellLayout;
            return true;
        }
        return false;
    }

    #endregion Layout Properties

    /// <summary>
    /// Releases all resources and references.
    /// </summary>
    public void Dispose()
    {
        if (_layout is not null)
        {
            _headerFont?.Dispose();
            _itemFont?.Dispose();
            _headerFont = _itemFont = null;
            _layout = null;
            GC.SuppressFinalize(this);
        }
    }
}
