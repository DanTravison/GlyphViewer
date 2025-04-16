namespace GlyphViewer.Views;

using GlyphViewer.Text;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

/// <summary>
/// Provides a view of a <see cref="Glyph"/>.
/// </summary>
public class GlyphView : SKCanvasView
{
    #region Constants

    /// <summary>
    /// Defines the default text color to use to draw the glyph.
    /// </summary>
    public static readonly Color DefaultTextColor = Colors.Black;
    /// <summary>
    /// Defines the default color to use to draw the <see cref="GlyphMetrics"/> indicator lines.
    /// </summary>
    public static readonly Color DefaultLineColor = Colors.Red;
    /// <summary>
    /// Defines the default color to use to draw the baseline.
    /// </summary>
    public static readonly Color DefaultBaselineColor = Colors.Blue;

    #endregion Constants

    #region Fields

    SKTypeface _typeface;
    SKFont _font;
    GlyphMetrics _metrics;

    #endregion Fields

    #region FontSize

    /// <summary>
    /// Gets or sets the starting pitch of the piano.
    /// </summary>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="FontSize"/>.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create
    (
        nameof(FontSize),
        typeof(double),
        typeof(GlyphView),
        128.0,
        BindingMode.OneWay,
        propertyChanged: (b, o, n) =>
        {
            if (b is GlyphView view)
            {
                view.OnFontSizeChanged();
            }
        }
    );

    void OnFontSizeChanged()
    {
        _typeface?.Dispose();
        _typeface = null;
        InvalidateSurface();
    }

    #endregion FontSize

    #region Glyph

    /// <summary>
    /// Gets or sets the glyph to draw.
    /// </summary>
    public Glyph Glyph
    {
        get => (Glyph)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Glyph"/>.
    /// </summary>
    public static readonly BindableProperty GlyphProperty = BindableProperty.Create
    (
        nameof(Glyph),
        typeof(Glyph),
        typeof(GlyphView),
        Glyph.Empty,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Glyph glyph)
            {
                return glyph;
            }
            return Glyph.Empty;
        },
        propertyChanged: (b, o, n) =>
        {
            if (b is GlyphView view)
            {
                view.OnGlyphChanged();
            }
        }
    );

    #endregion Glyph

    #region TextColor

    /// <summary>
    /// Gets or sets the color to use to draw text.
    /// </summary>
    /// <remarks>The default value is <see cref="DefaultTextColor"/>.</remarks>
    public Color TextColor
    {
        get => GetValue(TextColorProperty) as Color;
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="TextColor"/>.
    /// </summary>
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create
    (
        nameof(TextColor),
        typeof(Color),
        typeof(GlyphView),
        DefaultTextColor,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphView view)
            {
                view.InvalidateSurface();
            }
        }
     );

    #endregion TextColor

    #region LineColor

    /// <summary>
    /// Gets or sets the color to use to draw the indicator lines.
    /// </summary>
    /// <remarks>The default value is <see cref="DefaultLineColor"/>.</remarks>
    public Color LineColor
    {
        get => GetValue(LineColorProperty) as Color;
        set => SetValue(LineColorProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="LineColor"/>.
    /// </summary>
    public static readonly BindableProperty LineColorProperty = BindableProperty.Create
    (
        nameof(LineColor),
        typeof(Color),
        typeof(GlyphView),
        DefaultLineColor,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphView view)
            {
                view.InvalidateSurface();
            }
        }
     );

    #endregion LineColor

    #region BaselineColor

    /// <summary>
    /// Gets or sets the color to use to draw the indicator lines.
    /// </summary>
    /// <remarks>The default value is <see cref="DefaultBaselineColor"/>.</remarks>
    public Color BaselineColor
    {
        get => GetValue(BaselineColorProperty) as Color;
        set => SetValue(BaselineColorProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="BaselineColor"/>.
    /// </summary>
    public static readonly BindableProperty BaselineColorProperty = BindableProperty.Create
    (
        nameof(BaselineColor),
        typeof(Color),
        typeof(GlyphView),
        DefaultBaselineColor,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphView view)
            {
                view.InvalidateSurface();
            }
        }
     );

    #endregion BaselineColor

    #region Draw

    void OnGlyphChanged()
    {
        if (_typeface is null || Glyph.FontFamily != _typeface.FamilyName)
        {
            _typeface?.Dispose();
            _typeface = null;
        }
        if (!Glyph.IsEmpty)
        {
            _typeface = SKTypeface.FromFamilyName(Glyph.FontFamily, SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            _font = _typeface.ToFont();
            _font.Size = (float)FontSize;
            _font.Subpixel = true;
            _metrics = GlyphMetrics.CreateInstance(Glyph, _font);
            if (_metrics.Size.Height > MinimumWidthRequest)
            {
                InvalidateMeasure();
            }
        }

        InvalidateSurface();
    }

    /// <summary>
    /// Determines the size needed to draw the <see cref="Glyph"/>.
    /// </summary>
    /// <param name="widthConstraint">The width constraint to request.</param>
    /// <param name="heightConstraint">The height constraint to request.</param>
    /// <returns>The size needed to draw the <see cref="Glyph"/>.</returns>
    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        Size size = base.MeasureOverride(widthConstraint, heightConstraint);
        if (!Glyph.IsEmpty)
        {
            double height = Math.Max(_metrics.Size.Height, MinimumWidthRequest);
            size = new Size(MinimumWidthRequest, height);
        }
        else
        {
            // Set a minimum size to ensure the glyph pane is visible. 
            size = new(MinimumWidthRequest, MinimumWidthRequest);
        }
        return size;
    }

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
        if (!Glyph.IsEmpty)
        {
            using (SKPaint paint = new() { IsAntialias = true, Style = SKPaintStyle.Fill })
            {
                // Draw the glyph
                Draw(canvas, _font, paint);
            }
        }
    }

    void Draw(SKCanvas canvas, SKFont font, SKPaint paint)
    {
        GlyphMetrics metrics = _metrics;
        float width = CanvasSize.Width;
        float height = CanvasSize.Height;

        float top = (height - metrics.Size.Height) / 2;
        float left = (width - metrics.Size.Width) / 2;
        float start = left - metrics.Left;
        float baseline = top - metrics.Ascent;

        // Draw the bounding lines of the glyph metrics. 
        using (SKPathEffect effect = SKPathEffect.CreateDash([10, 10], 0))
        {
            float right = left + metrics.Size.Width;
            float ascent = baseline + metrics.Ascent;
            float descent = baseline + metrics.Descent;

            paint.StrokeWidth = 1;
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = LineColor.ToSKColor();
            paint.PathEffect = effect;
            // Draw ascent
            canvas.DrawLine(0, ascent, width, ascent, paint);
            // draw descent
            canvas.DrawLine(0, descent, width, descent, paint);
            // draw right edge of the glyph
            canvas.DrawLine(right, 0, right, height, paint);
            // draw left edge of the glyph
            canvas.DrawLine(left, 0, left, height, paint);

            // Draw baseline
            paint.Color = BaselineColor.ToSKColor();
            canvas.DrawLine(0, baseline, width, baseline, paint);
            // Draw the left
            left += metrics.Left;
            canvas.DrawLine(left, 0, left, height, paint);
        }

        // Draw the glyph
        paint.Color = TextColor.ToSKColor();
        paint.PathEffect = null;
        paint.Style = SKPaintStyle.Fill;

        canvas.DrawText(font, paint, Glyph.Text, start, baseline, SKTextAlign.Left);
    }

    #endregion Draw
}
