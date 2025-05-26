namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Settings;
using GlyphViewer.Text;
using GlyphViewer.Views;
using SkiaSharp;
using SkiaSharp.Views.Maui;

sealed class DrawContext : IDisposable
{
    #region Fields

    SKFont _headerFont;
    SKFont _itemFont;
    GlyphsViewRenderer _renderer;

    /// <summary>
    /// Provides a delegate for handling <see cref="GlyphsView"/> property changes.
    /// </summary>
    /// <param name="renderer">The <see cref="GlyphsViewRenderer"/> for the <see cref="GlyphsView"/>.</param>
    /// <param name="context">The <see cref="DrawContext"/>.</param>
    /// <returns>true if arrange is needed; otherwise, false.</returns>
    delegate RenderState PropertyChangedHandler(GlyphsViewRenderer renderer, DrawContext context);

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
        Add(GlyphsView.SelectedItemBackgroundColorProperty, OnSelectedItemBackgroundColorChanged);
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
    /// <param name="renderer">The <see cref="GlyphsViewRenderer"/>.</param>
    public DrawContext(GlyphsViewRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer, nameof(renderer));
        _renderer = renderer;

        // Initialize the properties.
        foreach (PropertyChangedHandler handler in _handlers.Values)
        {
            handler(renderer, this);
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
            RenderState change = handler(_renderer, this);
            _renderer.Invalidate(change);
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
    public FontFamily HeaderFontFamily
    {
        get;
        private set;
    }
    static RenderState OnHeaderFontFamilyChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.HeaderFontFamily = renderer.View.HeaderFontFamily;
        context._headerFont?.Dispose();
        context._headerFont = null;
        return RenderState.Measure;
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
    static RenderState OnHeaderFontSizeChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.HeaderFontSize = (float)renderer.View.HeaderFontSize;
        context._headerFont?.Dispose();
        context._headerFont = null;
        return RenderState.Measure;
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
    static RenderState OnHeaderFontAttributesChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.HeaderFontStyle = renderer.View.HeaderFontAttributes.ToFontStyle();
        context._headerFont?.Dispose();
        context._headerFont = null;
        return RenderState.Draw;
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
    static RenderState OnHeaderColorChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.HeaderColor = renderer.View.HeaderColor.ToSKColor();
        return RenderState.Draw;
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
    static RenderState OnHeaderBackgroundColorChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.HeaderBackgroundColor = renderer.View.HeaderBackgroundColor.ToSKColor();
        return RenderState.Draw;
    }

    #endregion Header Properties

    #region Item Properties

    /// <summary>
    /// Gets the item font family name.
    /// </summary>
    /// <remarks>
    /// This property is set from the <see cref="Glyph.FontFamily"/> of the first item in <see cref="GlyphsView.Items"/>
    /// </remarks>
    public FontFamily ItemFontFamily
    {
        get;
        private set;
    }
    static RenderState OnItemsChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        GlyphCollection items = renderer.View.Items;
        if (items is not null && items.Count > 0)
        {
            context.ItemFontFamily = items.FontFamily;
            context._itemFont?.Dispose();
            context._itemFont = null;
        }
        return RenderState.Measure;
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
    static RenderState OnItemFontSizeChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.ItemFontSize = (float)renderer.View.ItemFontSize;
        context._itemFont?.Dispose();
        context._itemFont = null;
        return RenderState.Measure;
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
    static RenderState OnItemColorChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.ItemColor = renderer.View.ItemColor.ToSKColor();
        return RenderState.Draw;
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
    static RenderState OnSelectedItemChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.SelectedItem = renderer.View.SelectedItem;
        return RenderState.Draw;
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
    static RenderState OnSelectedItemColorChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.SelectedItemColor = renderer.View.SelectedItemColor.ToSKColor();
        return RenderState.Draw;
    }

    /// <summary>
    /// Gets the background color of the <see cref="SelectedItem"/>.
    /// </summary>
    /// <remarks>
    /// This property is set from <see cref="GlyphsView.SelectedItemBackgroundColor"/>.
    /// </remarks>
    public SKColor SelectedItemBackgroundColor
    {
        get;
        private set;
    }
    static RenderState OnSelectedItemBackgroundColorChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        context.SelectedItemBackgroundColor = renderer.View.SelectedItemBackgroundColor.ToSKColor();
        return RenderState.Draw;
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
    public SKSize MaximumGlyphSize
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
    static RenderState OnSpacingChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        SkSpacing spacing = new(renderer.View.Spacing);
        if (spacing != context.Spacing)
        {
            context.Spacing = spacing;
            return RenderState.Layout;
        }
        return RenderState.None;
    }

    public CellLayoutStyle CellLayout
    {
        get;
        private set;
    }
    static RenderState OnCellLayoutChanged(GlyphsViewRenderer renderer, DrawContext context)
    {
        CellLayoutStyle cellLayout = renderer.View.CellLayout;
        if (cellLayout != context.CellLayout)
        {
            context.CellLayout = cellLayout;
            return RenderState.Layout;
        }
        return RenderState.None;
    }

    #endregion Layout Properties

    /// <summary>
    /// Releases all resources and references.
    /// </summary>
    public void Dispose()
    {
        if (_renderer is not null)
        {
            _headerFont?.Dispose();
            _itemFont?.Dispose();
            _headerFont = _itemFont = null;
            _renderer = null;
            GC.SuppressFinalize(this);
        }
    }
}
