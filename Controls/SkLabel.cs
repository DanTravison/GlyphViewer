namespace GlyphViewer.Controls;

using GlyphViewer.Text;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

internal class SkLabel : SKCanvasView
{
    #region Constants

    static readonly Color DefaultTextColor = Colors.Black;
    const double DefaultFontSize = 12;
    const double MinimumFontSize = 6;
    static readonly FontFamily DefaultFontFamily = FontFamily.DefaultFontFamily;

    const FontAttributes DefaultFontAttributes = FontAttributes.None;

    #endregion Constants

    #region Fields

    SKTextMetrics _metrics;
    bool _needsMetrics = true;

    #endregion Fields

    #region Text

    /// <summary>
    /// Gets or sets the color to use to draw the glyph.
    /// </summary>
    public string Text
    {
        get => GetValue(TextProperty) as string;
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="Text"/> property.
    /// </summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create
    (
        nameof(Text),
        typeof(string),
        typeof(SkLabel),
        string.Empty,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is string)
            {
                return value;
            }
            return string.Empty;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateText();
        }
    );

    void InvalidateText()
    {
        // Workaround for SkLabel does not resize after the first call to InvalidateMeasure
        // https://github.com/DanTravison/GlyphViewer/issues/25
        // See https://github.com/mono/SkiaSharp/issues/3239
        // NOTE: Unless HeightRequest is explicitly cleared, MeasureOverride does not get called
        // after the first successful attempt to resize the control.
        // Additionally, MeasureOverride must explicitly set HeightRequest.
        HeightRequest = -1;

        _needsMetrics = true;
        InvalidateMeasure();
        InvalidateSurface();
    }

    #endregion ItemColor

    #region TextColor

    /// <summary>
    /// Gets or sets the color to use to draw the glyph.
    /// </summary>
    public Color TextColor
    {
        get => GetValue(TextColorProperty) as Color;
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="TextColor"/> property.
    /// </summary>
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create
    (
        nameof(TextColor),
        typeof(Color),
        typeof(SkLabel),
        DefaultTextColor,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return DefaultTextColor;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateSurface();
        }
    );

    #endregion ItemColor

    #region FontFamily

    /// <summary>
    /// Gets or sets the <see cref="Text"/> font size.
    /// </summary>
    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty) as FontFamily;
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="FontFamily"/> property.
    /// </summary>
    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create
    (
        nameof(FontFamily),
        typeof(FontFamily),
        typeof(SkLabel),
        FontFamily.DefaultFontFamily,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is not FontFamily fontFamily)
            {
                value = DefaultFontFamily;
            }
            return value;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateText();
        }
    );

    #endregion FontFamily

    #region FontAttributes

    /// <summary>
    /// Gets or sets the <see cref="Text"/> font size.
    /// </summary>
    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="FontAttributes"/> property.
    /// </summary>
    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create
    (
        nameof(FontAttributes),
        typeof(FontAttributes),
        typeof(SkLabel),
        DefaultFontAttributes,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateText();
        }
    );

    #endregion FontAttributes

    #region FontSize

    /// <summary>
    /// Gets or sets the <see cref="Text"/> font size.
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
        nameof(FontSize),
        typeof(double),
        typeof(SkLabel),
        DefaultFontSize,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is double size)
            {
                if (size >= MinimumFontSize)
                {
                    return size;
                }
            }
            return MinimumFontSize;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateText();
        }
    );

    #endregion FontSize

    #region HorizontalTextAlignment

    /// <summary>
    /// Gets or sets the horizontal <see cref="Text"/> alignment.
    /// </summary>
    public TextAlignment HorizontalTextAlignment
    {
        get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
        set => SetValue(HorizontalTextAlignmentProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HorizontalTextAlignment"/> property.
    /// </summary>
    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create
    (
        nameof(HorizontalTextAlignment),
        typeof(TextAlignment),
        typeof(SkLabel),
        TextAlignment.Start,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateMeasure();
        }
    );

    #endregion HorizontalTextAlignment

    #region VerticalTextAlignment

    /// <summary>
    /// Gets or sets the vertical <see cref="Text"/> alignment.
    /// </summary>
    public TextAlignment VerticalTextAlignment
    {
        get => (TextAlignment)GetValue(VerticalTextAlignmentProperty);
        set => SetValue(VerticalTextAlignmentProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="VerticalTextAlignment"/> property.
    /// </summary>
    public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create
    (
        nameof(VerticalTextAlignment),
        typeof(TextAlignment),
        typeof(SkLabel),
        TextAlignment.Start,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateMeasure();
        }
    );

    #endregion VerticalTextAlignment

    #region Padding

    /// <summary>
    /// Gets or sets the <see cref="Text"/> font size.
    /// </summary>
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="Padding"/> property.
    /// </summary>
    public static readonly BindableProperty PaddingProperty = BindableProperty.Create
    (
        nameof(Padding),
        typeof(Thickness),
        typeof(SkLabel),
        Thickness.Zero,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateText();
        }
    );

    #endregion Padding

    #region Measure

    SKFont GetFont()
    {
        return FontFamily.CreateFont((float)FontSize, FontAttributes);
    }

    /// <summary>
    /// Determines the size needed to draw the <see cref="SkLabel"/>.
    /// </summary>
    /// <param name="widthConstraint">The width constraint.</param>
    /// <param name="heightConstraint">The height constraint.</param>
    /// <returns>The size needed to draw the <see cref="SkLabel"/>.</returns>
    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        if (_needsMetrics)
        {
            if (string.IsNullOrEmpty(Text))
            {
                _metrics = SKTextMetrics.EmptyInstance;
            }
            else
            {
                using (SKFont font = GetFont())
                {
                    if (!string.IsNullOrEmpty(Text))
                    {
                        _metrics = new SKTextMetrics(Text, font);
                    }
                }
            }
            _needsMetrics = false;
        }

        float width = (float)Padding.HorizontalThickness + _metrics.TextWidth;
        float height = (float)Padding.VerticalThickness + _metrics.Size.Height;

        // Workaround for SkLabel does not resize after the first call to InvalidateMeasure
        // https://github.com/DanTravison/GlyphViewer/issues/25
        // See https://github.com/mono/SkiaSharp/issues/3239
        // NOTE: Must explicitly set HeightRequest here as well as in InvalidateText
        HeightRequest = height;

        return new Size(width, height);
    }

    #endregion Measure

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
            canvas.Clear();
        }

        if (!string.IsNullOrEmpty(Text))
        {
            DrawText(canvas);
        }
    }

    void DrawText(SKCanvas canvas)
    {
        Thickness padding = Padding;

        float y = (float)padding.Top;
        float x = (float)padding.Left;
        float width = (float)(CanvasSize.Width - padding.HorizontalThickness);
        float height = (float)(CanvasSize.Height - padding.VerticalThickness);

        using (SKPaint paint = new() { IsAntialias = true, Color = TextColor.ToSKColor() })
        {
            using (SKFont font = GetFont())
            {
                if (_metrics is null)
                {
                    _metrics = new SKTextMetrics(Text, font);
                }
                switch (VerticalTextAlignment)
                {
                    case TextAlignment.Center:
                        y += _metrics.Ascent + (height - _metrics.Size.Height) / 2;
                        break;
                    case TextAlignment.End:
                        y += height - _metrics.Descent;
                        break;
                    // TextAlignment.Start and Justify
                    default:
                        y -= _metrics.Ascent;
                        break;
                }
                switch (HorizontalTextAlignment)
                {
                    case TextAlignment.Center:
                        x += (width - _metrics.TextWidth) / 2;
                        break;
                    case TextAlignment.End:
                        x += width - _metrics.TextWidth;
                        break;
                    default:
                        break;
                }
                canvas.DrawText(font, paint, Text, x, y, SKTextAlign.Left);
            }
        }
    }

    #endregion Draw
}
