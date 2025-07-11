﻿namespace GlyphViewer.Views;

using GlyphViewer.Settings;
using GlyphViewer.Text;
using GlyphViewer.Views.Renderers;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.ComponentModel;
using System.Windows.Input;
using UnicodeRange = Text.Unicode.Range;

/// <summary>
/// Provides a view of the glyphs in a <see cref="SKTypeface"/>.
/// </summary>
public sealed class GlyphsView : SKCanvasView
{
    #region Fields

    readonly GlyphsViewRenderer _renderer;
    readonly DrawContext _context;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public GlyphsView()
    {
        base.EnableTouchEvents = true;
        _renderer = new(this);
        _context = _renderer.DrawContext;
        _renderer.PropertyChanged += OnRendererPropertyChanged;
    }

    #region Event Handlers

    private void OnRendererPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // NOTE: The following properties are not bound to a DrawContext property.
        // As such they must be handled manually.
        // The alternative would be to allow the GlyphsRenderer to update the 
        // properties directly in the property setter.
        if (ReferenceEquals(e, GlyphsViewRenderer.CountChangedEventArgs))
        {
            Rows = _renderer.Rows.Count;
        }
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        // NOTE: OnPropertyChanged can be called before constructor completes.
        _context?.OnViewPropertyChanged(propertyName);
    }

    #endregion Event Handlers

    #region Properties

    #region Spacing

    /// <summary>
    /// Gets or sets the spacing between the glyphs in the horizontal direction.
    /// </summary>
    public Thickness Spacing
    {
        get => (Thickness)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="Spacing"/> property.
    /// </summary>
    public static readonly BindableProperty SpacingProperty = BindableProperty.Create
    (
        nameof(Spacing),
        typeof(Thickness),
        typeof(GlyphsView),
        GlyphSetting.DefaultSpacing,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Thickness spacing)
            {
                Thickness minimum = GlyphSetting.MinimumSpacing;
                double horizontal = spacing.HorizontalThickness;
                double vertical = spacing.VerticalThickness;

                if (horizontal < minimum.HorizontalThickness)
                {
                    horizontal = minimum.HorizontalThickness;
                }
                if (vertical < minimum.VerticalThickness)
                {
                    vertical = minimum.VerticalThickness;
                }
                return new Thickness(horizontal, vertical);
            }
            return GlyphSetting.DefaultSpacing;
        }
    );

    #endregion Spacing

    #region CellLayout

    /// <summary>
    /// Gets or sets the glyph layout style.
    /// </summary>
    public CellLayoutStyle CellLayout
    {
        get => (CellLayoutStyle)GetValue(CellLayoutProperty);
        set => SetValue(CellLayoutProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="CellLayout"/> property.
    /// </summary>
    public static readonly BindableProperty CellLayoutProperty = BindableProperty.Create
    (
        nameof(CellLayout),
        typeof(CellLayoutStyle),
        typeof(GlyphsView),
        GlyphSetting.DefaultCellLayout,
        BindingMode.OneWay,
        coerceValue: (bindable, newValue) =>
        {
            if (newValue is not CellLayoutStyle)
            {
                return GlyphSetting.DefaultCellLayout;
            }
            return newValue;
        }
    );

    #endregion CellLayout

    #region Item Properties

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
                view._renderer.Content = newValue as GlyphCollection;
                view.InvalidateSurface();
            }
        }
    );

    #endregion Items

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
        coerceValue: (bindable, value) =>
        {
            if (bindable is GlyphsView view && value is Glyph selectedItem)
            {
                if (view._renderer.Contains(selectedItem))
                {
                    return selectedItem;
                }
            }
            return null;
        },
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
        EnsureVisible(glyph);
    }

    /// <summary>
    /// Ensures that the specified <see cref="Glyph"/> is visible in the view.
    /// </summary>
    /// <param name="glyph">The <see cref="Glyph"/> to ensure visibility.</param>
    /// <returns>
    /// true if the <see cref="Glyph"/> is visible; otherwise, 
    /// false if the <see cref="Glyph"/> is a null reference or not present.
    /// </returns>
    public bool EnsureVisible(Glyph glyph)
    {
        bool result = _renderer.IsVisible(glyph, out int row);
        if (result == false)
        {
            if (row >= 0)
            {
                Row = row;
                result = true;
            }
        }
        return result;
    }

    #endregion SelectedItem

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
        ItemFontSetting.DefaultItemColor,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return ItemFontSetting.DefaultItemColor;
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
        ItemFontSetting.DefaultSelectedItemColor,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return ItemFontSetting.DefaultSelectedItemColor;
        }
    );

    #endregion SelectedItemColor

    #region SelectedItemBackgroundColor

    /// <summary>
    /// Gets or sets the color to use to draw the selected glyph.
    /// </summary>
    public Color SelectedItemBackgroundColor
    {
        get => GetValue(SelectedItemBackgroundColorProperty) as Color;
        set => SetValue(SelectedItemBackgroundColorProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="SelectedItemBackgroundColor"/> property.
    /// </summary>
    public static readonly BindableProperty SelectedItemBackgroundColorProperty = BindableProperty.Create
    (
        nameof(SelectedItemBackgroundColor),
        typeof(Color),
        typeof(GlyphsView),
        ItemFontSetting.DefaultSelectedItemBackgroundColor,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return ItemFontSetting.DefaultSelectedItemBackgroundColor;
        }
    );

    #endregion SelectedItemBackgroundColor

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
        ItemFontSetting.DefaultFontSize,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is double fontSize)
            {
                return Math.Clamp(fontSize, ItemFontSetting.MinimumFontSize, ItemFontSetting.MaximumFontSize);
            }
            return ItemFontSetting.DefaultFontSize;
        }
    );

    #endregion ItemFontSize

    #endregion Item Properties

    #region Header Properties

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
        ItemHeaderFontSetting.DefaultTextColor,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return ItemHeaderFontSetting.DefaultTextColor;
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
        }
    );

    #endregion HeaderBackgroundColor

    #region HeaderFontFamily

    /// <summary>
    /// Gets or sets the font to use for header text.
    /// </summary>
    public FontFamily HeaderFontFamily
    {
        get => GetValue(HeaderFontFamilyProperty) as FontFamily;
        set => SetValue(HeaderFontFamilyProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HeaderFontFamily"/> property.
    /// </summary>
    public static readonly BindableProperty HeaderFontFamilyProperty = BindableProperty.Create
    (
        nameof(HeaderFontFamily),
        typeof(FontFamily),
        typeof(GlyphsView),
        ItemHeaderFontSetting.DefaultFontFamily,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {

            if (value is not FontFamily)
            {
                return ItemHeaderFontSetting.DefaultFontFamily;
            }
            return value;
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
        ItemHeaderFontSetting.DefaultFontSize,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is double fontSize)
            {
                return Math.Clamp(fontSize, ItemHeaderFontSetting.MinimumFontSize, ItemHeaderFontSetting.MaximumFontSize);
            }
            return ItemHeaderFontSetting.DefaultFontSize;
        }
    );

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

    #endregion Header Properties

    #region Row Properties

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
                if (row >= 0 && row < view._renderer.Count)
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
                view._renderer.FirstRow = (int)newValue;
            }
        }
    );

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

    #endregion Row Properties

    #region SelectedUnicodeRange

    /// <summary>
    /// Gets or sets the selected unicode range.
    /// </summary>
    public UnicodeRange SelectedUnicodeRange
    {
        get => (UnicodeRange)GetValue(SelectedUnicodeRangeProperty);
        private set => SetValue(SelectedUnicodeRangeProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="SelectedUnicodeRange"/> property.
    /// </summary>
    public static readonly BindableProperty SelectedUnicodeRangeProperty = BindableProperty.Create
    (
        nameof(SelectedUnicodeRange),
        typeof(UnicodeRange),
        typeof(GlyphsView),
        UnicodeRange.Empty,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is UnicodeRange range)
            {
                return range;
            }
            return UnicodeRange.Empty;
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
        UnicodeRange range = SelectedUnicodeRange;
        HeaderRow header = _renderer[range];
        if (header is not null)
        {
            int row;
            // Since the first unicode range header is not in the _rows list,
            // go to row 0
            if (header.UnicodeRange.Id == _renderer.Content[0].Range.Id)
            {
                Row = 0;
            }
            else
            {
                // go to the first row after the header row.
                row = header.Row;
                if (row > 0 && row < Rows - 1)
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
            if (_renderer.HitTest(e.Location, out IGlyphRow row, out GlyphRenderer renderer))
            {
                if (row is HeaderRow)
                {
                    HeaderClickedCommand?.Execute(row);
                }
                else if (renderer is not null)
                {
                    SelectedItem = renderer.Metrics.Glyph;
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
            if (row >= 0 && row < Rows)
            {
                Row = row;
            }
        }
        e.Handled = true;
    }

    #endregion Touch Interaction

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
        _renderer.Draw(canvas, CanvasSize);
    }

    #endregion Draw
}
