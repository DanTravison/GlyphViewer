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
        _metrics = GlyphMetrics.Empty;

        if (_typeface is null || Glyph.FontFamily != _typeface.FamilyName)
        {
            _typeface?.Dispose();
            _typeface = null;
            if (!Glyph.IsEmpty)
            {
                _typeface = SKTypeface.FromFamilyName(Glyph.FontFamily, SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            }
        }
        InvalidateSurface();
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
            using (SKFont font = _typeface.ToFont())
            {
                // ISSUE: Is this correct? (i.e., points versus pixels)
                font.Size = FontSize.ToPixels();
                using (SKPaint paint = new() { IsAntialias = true })
                {
                    // Draw the glyph
                    Draw(canvas, font, paint);
                }
            }
        }
    }

    void Draw(SKCanvas canvas, SKFont font, SKPaint paint)
    {
        if (_metrics.IsEmpty)
        {
            _metrics = GlyphMetrics.CreateInstance(paint, font, Glyph);
        }
        GlyphMetrics metrics = _metrics;
        float width = CanvasSize.Width;
        float height = CanvasSize.Height;

        float top = (height - metrics.Size.Height) / 2;
        float left = (width - metrics.Size.Width) / 2;
        float start = left - metrics.Left;
        float right = start + metrics.Size.Width;
        float baseline = top - metrics.Ascent;
        float ascent = baseline + metrics.Ascent;
        float descent = baseline + metrics.Descent;

        // Draw the bounding lines of the glyph metrics. 
        using (SKPathEffect effect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0))
        {
            paint.StrokeWidth = 1;
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = LineColor.ToSKColor();
            paint.PathEffect = effect;
            // Draw ascent
            DrawLine(canvas, paint, 0, ascent, width, ascent);
            // draw descent
            DrawLine(canvas, paint, 0, descent, width, descent);
            // draw right edge
            DrawLine(canvas, paint, right, 0, right, height);
            // draw left edge
            DrawLine(canvas, paint, left, 0, left, height);
            // Draw baseline
            paint.Color = BaselineColor.ToSKColor();
            DrawLine(canvas, paint, 0, baseline, width, baseline);
        }

        // Draw the glyph
        paint.Color = TextColor.ToSKColor();
        paint.PathEffect = null;
        paint.Style = SKPaintStyle.Fill;

        canvas.DrawText(Glyph.Text, start, baseline, SKTextAlign.Left, font, paint);
    }

    void DrawLine(SKCanvas canvas, SKPaint paint, float left, float top, float right, float bottom)
    {
        canvas.DrawLine(left, top, right, bottom, paint);
    }

    #endregion Draw
}
