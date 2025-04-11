namespace GlyphViewer.Controls;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides a slider with vertical and horizontal orientations.
/// </summary>
public sealed class Slider : SKCanvasView
{
    #region Defaults Constants

    /// <summary>
    /// Defines the default <see cref="Radius"/> for the thumb.
    /// </summary>
    public const double DefaultRadius = 10.0;

    /// <summary>
    /// Defines the maximum <see cref="Radius"/> for the thumb.
    /// </summary>
    public const double MaximumRadius = 20.0;

    /// <summary>
    /// Defines the default <see cref="TrackColor"/> <see cref="Color"/>.
    /// </summary>
    public static readonly Color DefaultTrackColor = Colors.White;

    /// <summary>
    /// Defines the default <see cref="ThumbColor"/> <see cref="Color"/>.
    /// </summary>
    public static readonly Color DefaultThumbColor = Colors.White;

    /// <summary>
    /// Defines the default <see cref="DisabledColor"/> <see cref="Color"/>. 
    /// </summary>
    public static readonly Color DefaultDisabledColor = Colors.Gray;

    /// <summary>
    /// Defines the default orientation.
    /// </summary>
    public const StackOrientation DefaultOrientation = StackOrientation.Horizontal;

    /// <summary>
    /// Defines the default track thickness.
    /// </summary>
    public const double DefaultTrackSize = 2.0;

    #endregion Defaults Constants

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public Slider()
    {
        Drawable = new(this)
        {
            Value = Value
        };
        EnableTouchEvents = true;
    }

    SliderDrawable Drawable
    {
        get;
    }

    #region Overrides

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        if (StringComparer.Ordinal.Compare(propertyName, BackgroundColorProperty.PropertyName) == 0)
        {
            InvalidateSurface();
        }
        else if (StringComparer.Ordinal.Compare(propertyName, IsEnabledProperty.PropertyName) == 0)
        {
            InvalidateSurface();
        }
        base.OnPropertyChanging(propertyName);
    }

    /// <summary>
    /// Updates the size of an View.
    /// </summary>
    /// <param name="widthConstraint">The width that a parent element can allocate a child element.</param>
    /// <param name="heightConstraint">The height that a parent element can allocate a child element.</param>
    /// <returns>The desired Size for this element.</returns>
    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        Size size = base.MeasureOverride(widthConstraint, heightConstraint);

        double minDimension;
        if (this.ThumbStyle == ThumbStyle.Circle)
        {
            minDimension = (Radius + TrackSize) * 2;
        }
        else
        {
            minDimension = TrackSize * 1.5;
        }

        if (Orientation == StackOrientation.Vertical)
        {
            if (size.Width < minDimension)
            {
                size.Width = minDimension + Margin.HorizontalThickness;
            }
        }
        else
        {
            if (size.Height < minDimension)
            {
                size.Height = minDimension + Margin.VerticalThickness;
            }
        }
        return size;
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        Drawable.Size = CanvasSize.ToMauiSize();
        Drawable.Draw(canvas);
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        Drawable.OnTouch(e);
    }

    #endregion Overrides

    #region Bindable Properties

    #region Value

    /// <summary>
    /// Gets or sets value.
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Value"/>.
    /// </summary>
    public static readonly BindableProperty ValueProperty = BindableProperty.Create
    (
        nameof(Value),
        typeof(double),
        typeof(Slider),
        0.0,
        coerceValue: (bindableObject, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                double value = SliderDrawable.Round((double)newValue, slider.Interval);

                if (value < slider.Minimum + slider.Interval)
                {
                    value = slider.Minimum;
                }
                else if (value > slider.Maximum - slider.Interval)
                {
                    value = slider.Maximum;
                }
                return value;
            }
            return newValue;
        },
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider && slider.Drawable is not null)
            {
                slider.Drawable.Value = (double)newValue;
            }
        }
    );

    #endregion Value

    #region Interval

    /// <summary>
    /// Gets or sets the value to use to increase or decrease the <see cref="Value"/>
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    /// <remarks>
    /// When this property is greater than zero, the <see cref="Value"/> is rounded
    /// to the nearest interval.
    /// </remarks>
    public double Interval
    {
        get => (double)GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Interval"/>.
    /// </summary>
    public static readonly BindableProperty IntervalProperty = BindableProperty.Create
    (
        nameof(Interval),
        typeof(double),
        typeof(Slider),
        1.0,
        validateValue: (bindableObject, newValue) =>
        {
            if ((double)newValue > 0)
            {
                return true;
            }
            return false;
        },
        propertyChanged: (bindableObject, oldInterval, newInterval) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.InvalidateSurface();
            }
        }
    );

    #endregion Interval

    #region Minimum

    /// <summary>
    /// Gets or sets the minimum <see cref="Value"/>.
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Minimum"/>.
    /// </summary>
    public static readonly BindableProperty MinimumProperty = BindableProperty.Create
    (
        nameof(Minimum),
        typeof(double),
        typeof(Slider),
        0.0,
        validateValue: (bindableObject, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                if ((double)newValue <= slider.Maximum)
                {
                    return true;
                }
            }
            return false;
        },
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.OnMinimumChanged((double)newValue);
            }
        }
    );

    void OnMinimumChanged(double minimum)
    {
        if (Value < minimum)
        {
            Value = minimum;
        }
    }

    #endregion Minimum

    #region Maximum

    /// <summary>
    /// Gets or sets the maximum <see cref="Value"/>.
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Maximum"/>.
    /// </summary>
    public static readonly BindableProperty MaximumProperty = BindableProperty.Create
    (
        nameof(Maximum),
        typeof(double),
        typeof(Slider),
        double.MaxValue,
        coerceValue: (bindableObject, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                double value = (double)newValue;
                if (value < slider.Minimum)
                {
                    value = slider.Minimum;
                }
                return value;
            }
            return newValue;
        },

        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.OnMaximumChanged((double)newValue);
            }
        }
    );

    void OnMaximumChanged(double maximum)
    {
        if (Value > maximum)
        {
            Value = maximum;
        }
    }

    #endregion Maximum

    #region ThumbStyle

    /// <summary>
    /// Gets or sets the maximum <see cref="Value"/>.
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    public ThumbStyle ThumbStyle
    {
        get => (ThumbStyle)GetValue(ThumbStyleProperty);
        set => SetValue(ThumbStyleProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="ThumbStyle"/>.
    /// </summary>
    public static readonly BindableProperty ThumbStyleProperty = BindableProperty.Create
    (
        nameof(ThumbStyle),
        typeof(ThumbStyle),
        typeof(Slider),
        ThumbStyle.Circle,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.OnThumbStyleChanged();
            }
        }
    );

    void OnThumbStyleChanged()
    {
        InvalidateMeasure();
    }

    #endregion ThumbStyle

    #region Radius

    /// <summary>
    /// Gets or sets the radius of the thumb.
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 10f.
    /// </value>
    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Radius"/>.
    /// </summary>
    public static readonly BindableProperty RadiusProperty = BindableProperty.Create
    (
        nameof(Radius),
        typeof(double),
        typeof(Slider),
        DefaultRadius,
        coerceValue: (bindableObject, newValue) =>
        {
            double radius = (double)newValue;
            if (bindableObject is Slider slider)
            {
                if (radius < 0)
                {
                    radius = 0;
                }
                else if (radius > MaximumRadius)
                {
                    radius = MaximumRadius;
                }
            }
            return radius;
        },
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.InvalidateMeasure();
            }
        }
    );

    #endregion Radius

    #region TrackSize

    /// <summary>
    /// Gets or sets the radius of the thumb.
    /// </summary>
    /// <value>
    /// A double value.
    /// </value>
    public double TrackSize
    {
        get => (double)GetValue(TrackSizeProperty);
        set => SetValue(TrackSizeProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="TrackSize"/>.
    /// </summary>
    public static readonly BindableProperty TrackSizeProperty = BindableProperty.Create
    (
        nameof(TrackSize),
        typeof(double),
        typeof(Slider),
        DefaultTrackSize,
        coerceValue: (bindableObject, newValue) =>
        {
            double trackSize = (double)newValue;
            if (bindableObject is Slider slider)
            {
                if (trackSize < 0)
                {
                    trackSize = 1;
                }
            }
            return trackSize;
        },
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.InvalidateMeasure();
            }
        }
    );

    #endregion TrackSize

    #region DisabledColor

    /// <summary>
    /// Gets or sets the color to use to draw the slider when it is disabled..
    /// </summary>
    public Color DisabledColor
    {
        get => (Color)GetValue(DisabledColorProperty);
        set => SetValue(DisabledColorProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="DisabledColor"/>.
    /// </summary>
    public static readonly BindableProperty DisabledColorProperty = BindableProperty.Create
    (
        nameof(DisabledColor),
        typeof(Color),
        typeof(Slider),
        DefaultDisabledColor,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.InvalidateSurface();
            }
        }
    );

    #endregion DisabledColor

    #region TrackColor

    /// <summary>
    /// Gets or sets the color to use to draw the slider's track.
    /// </summary>
    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="TrackColor"/>.
    /// </summary>
    public static readonly BindableProperty TrackColorProperty = BindableProperty.Create
    (
        nameof(TrackColor),
        typeof(Color),
        typeof(Slider),
        Colors.White,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.InvalidateSurface();
            }
        }
    );

    #endregion TrackColor

    #region ThumbColor

    /// <summary>
    /// Gets or sets the color to use to draw the slider's thumb.
    /// </summary>
    public Color ThumbColor
    {
        get => (Color)GetValue(ThumbColorProperty);
        set => SetValue(ThumbColorProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="ThumbColor"/>.
    /// </summary>
    public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create
    (
        nameof(ThumbColor),
        typeof(Color),
        typeof(Slider),
        Colors.White,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.InvalidateSurface();
            }
        }
    );

    #endregion ThumbColor

    #region Orientation

    /// <summary>
    /// Gets or sets the <see cref="StackOrientation"/> of the thumb.
    /// </summary>
    public StackOrientation Orientation
    {
        get => (StackOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Orientation"/>.
    /// </summary>
    public static readonly BindableProperty OrientationProperty = BindableProperty.Create
    (
        nameof(Orientation),
        typeof(StackOrientation),
        typeof(Slider),
        DefaultOrientation,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Slider slider)
            {
                slider.OnOrientationChanged();
            }
        }
    );

    /// <summary>
    /// Override in the derived class to handle change to <see cref="Orientation"/>.
    /// </summary>
    void OnOrientationChanged()
    {
        InvalidateMeasure();
        InvalidateSurface();
    }

    #endregion Orientation

    #endregion Bindable Properties
}
