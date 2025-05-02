namespace GlyphViewer.Views;

using GlyphViewer.Settings;
using GlyphViewer.Text;
using GlyphViewer.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.ComponentModel;

/// <summary>
/// Provides a view of a <see cref="Glyph"/>.
/// </summary>
public class GlyphView : SKCanvasView
{
    #region Fields

    SKFont _font;
    GlyphMetrics _glyphMetrics = GlyphMetrics.Empty;

    #endregion Fields

    public GlyphView()
    {
        MinimumHeightRequest = GlyphSetting.MinimumWidth;
        MinimumWidthRequest = GlyphSetting.MinimumWidth;
    }

    #region GlyphWidth

    /// <summary>
    /// Gets or sets the font size to use to draw the <see cref="Glyph"/>.
    /// </summary>
    public double GlyphWidth
    {
        get => (double)GetValue(GlyphWidthProperty);
        set => SetValue(GlyphWidthProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="GlyphWidth"/>.
    /// </summary>
    public static readonly BindableProperty GlyphWidthProperty = BindableProperty.Create
    (
        nameof(GlyphWidth),
        typeof(double),
        typeof(GlyphView),
        GlyphSetting.DefaultWidth,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is double width)
            {
                return Math.Clamp(width, GlyphSetting.MinimumWidth, GlyphSetting.MaximumWidth);
            }
            return GlyphSetting.DefaultWidth;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphView view)
            {
                view.OnGlyphWidthChanged();
            }
        }
    );

    void OnGlyphWidthChanged()
    {
        // Address GlyphView does not size correctly when glyph's height exceeds the height of the glyph view.
        HeightRequest = GlyphWidth;
        InvalidateMeasure();
        InvalidateSurface();
    }

    #endregion GlyphWidth

    #region FontSize

    /// <summary>
    /// Gets or sets the font size to use to draw the <see cref="Glyph"/>.
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
                view.OnMetricsPropertyChanged(view, MetricsModel.FontChangedEventArgs);
            }
        }
    );

    #endregion FontSize

    #region Metrics

    /// <summary>
    /// Gets or sets the Metrics to draw.
    /// </summary>
    internal MetricsModel Metrics
    {
        get => GetValue(MetricsProperty) as MetricsModel;
        set => SetValue(MetricsProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Metrics"/>.
    /// </summary>
    public static readonly BindableProperty MetricsProperty = BindableProperty.Create
    (
        nameof(Metrics),
        typeof(MetricsModel),
        typeof(MetricsView),
        null,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is GlyphView view)
            {
                view.OnMetricsChanged(oldValue as MetricsModel, newValue as MetricsModel);
            }
        }
    );

    /// <summary>
    /// Handles changes to the <see cref="Metrics"/> property.
    /// </summary>
    /// <param name="previous">The previous <see cref="MetricsModel"/>.</param>
    /// <param name="metrics">The new <see cref="MetricsModel"/>.</param>
    void OnMetricsChanged(MetricsModel previous, MetricsModel metrics)
    {
        if (previous is not null)
        {
            previous.PropertyChanged -= OnMetricsPropertyChanged;
        }

        if (metrics is not null)
        {
            metrics.PropertyChanged += OnMetricsPropertyChanged;
        }
        OnGlyphChanged();
    }

    /// <summary>
    /// Handles PropertyChanged notifications from the <see cref="MetricsModel"/> instance.
    /// </summary>
    /// <param name="sender">The object that raised the event (not used).</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> identifying the property that changed.</param>
    private void OnMetricsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if
        (
            // indicates either this.FontSize or this.Metrics.Font changed
            ReferenceEquals(e, MetricsModel.FontChangedEventArgs)
            ||
            ReferenceEquals(e, MetricsModel.GlyphChangedEventArgs)
        )
        {
            OnGlyphChanged();
        }
    }

    #endregion Metrics

    #region TextColor

    /// <summary>
    /// Gets or sets the color to use to draw text.
    /// </summary>
    /// <remarks>The default value is <see cref="GlyphSetting.DefaultTextColor"/>.</remarks>
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
        GlyphSetting.DefaultTextColor,
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
    /// <remarks>The default value is <see cref="GlyphSetting.DefaultLineColor"/>.</remarks>
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
        GlyphSetting.DefaultLineColor,
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
    /// <remarks>The default value is <see cref="GlyphSetting.DefaultBaselineColor"/>.</remarks>
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
        GlyphSetting.DefaultBaselineColor,
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

    #region Measure

    /// <summary>
    /// Updates the font and glyph metrics when the glyph or a dependenty font property changes.
    /// </summary>
    void OnGlyphChanged()
    {
        double heightRequest = GlyphWidth;

        Glyph glyph = Metrics is not null ? Metrics.Glyph : Glyph.Empty;
        UpdateGlyph(glyph);

        if (!glyph.IsEmpty)
        {
            if (_glyphMetrics.Size.Height > heightRequest)
            {
                heightRequest = _glyphMetrics.Size.Height;
            }
        }

        // Intent: Allow the height to increase to accomodate taller glyphs but return
        // to the desired height when the glyph is empty or the glyph height < desired height
        if (heightRequest != HeightRequest)
        {
            // Address GlyphView does not size correctly when glyph's height exceeds the height of the glyph view.
            // https://github.com/DanTravison/GlyphViewer/issues/23
            // See https://github.com/mono/SkiaSharp/issues/3239
            HeightRequest = heightRequest;
            InvalidateMeasure();
        }
        InvalidateSurface();
    }

    void UpdateGlyph(Glyph glyph)
    {
        if (!glyph.IsEmpty)
        {
            if (_font is null || _font.Typeface.FamilyName != glyph.FontFamily)
            {
                _font?.Dispose();
                _font = glyph.FontFamily.CreateFont((float)FontSize);
            }
            _glyphMetrics = GlyphMetrics.CreateInstance(glyph, _font);
        }
        else
        {
            _font?.Dispose();
            _font = null;
            _glyphMetrics = GlyphMetrics.Empty;
        }
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
        if (!_glyphMetrics.Glyph.IsEmpty)
        {
            double height = Math.Max(_glyphMetrics.Size.Height, GlyphWidth);
            size = new Size(GlyphWidth, height);
        }
        else
        {
            // Set a minimum size to ensure the glyph pane is visible. 
            size = new(GlyphWidth, GlyphWidth);
        }
        return size;
    }

    #endregion Measure

    #region Draw

    /// <summary>
    /// Paints the glyph on the canvas.
    /// </summary>
    /// <param name="e">The <see cref="SKPaintSurfaceEventArgs"/> contianing the details.</param>
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
        if (!_glyphMetrics.Glyph.IsEmpty)
        {
            using (SKPaint paint = new() { IsAntialias = true, Style = SKPaintStyle.Fill })
            {
                // Draw the glyph
                Draw(_glyphMetrics, canvas, _font, paint);
            }
        }
    }

    /// <summary>
    /// Draws a <see cref="Glyph"/> on the canvas.
    /// </summary>
    /// <param name="metrics">The <see cref="GlyphMetrics"/> for the <see cref="Glyph"/> to draw.</param>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="font">The <see cref="SKFont"/> to use to draw.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    void Draw(GlyphMetrics metrics, SKCanvas canvas, SKFont font, SKPaint paint)
    {
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

        canvas.DrawText(font, paint, metrics.Glyph.Text, start, baseline, SKTextAlign.Left);
    }

    #endregion Draw
}
