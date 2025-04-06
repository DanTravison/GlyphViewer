namespace GlyphViewer.Controls;

using SkiaSharp;
using SkiaSharp.Views.Maui;


internal class SliderDrawable
{
    #region Fields

    /// <summary>
    /// Defines the region selectable region.
    /// </summary>
    DrawRegion _drawRegion = DrawRegion.Empty;

    readonly Slider _parent;

    float _thumbLocation;
    Size _size = Size.Zero;
    bool _isDragging;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">The parent <see cref="Slider"/>.</param>
    public SliderDrawable(Slider parent)
    {
        _parent = parent;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the thumb location.
    /// </summary>
    private float ThumbLocation
    {
        get => _thumbLocation;
        set
        {
            _drawRegion.Constrain(ref value);
            if (_thumbLocation != value)
            {
                _thumbLocation = value;
                _parent.InvalidateSurface();
            }
        }
    }

    /// <summary>
    /// Gets or sets the size of the drawable area.
    /// </summary>
    public Size Size
    {
        get => _size;
        set
        {
            if (value.Width != _size.Width || value.Height != Size.Height || _drawRegion.IsEmpty)
            {
                _size = value;
                _drawRegion = new DrawRegion(_parent.Orientation, _parent.ThumbStyle, value, (float)_parent.TrackSize, (float) _parent.Radius);
                Value = _parent.Value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the slider value.
    /// </summary>
    internal double Value
    {
        get
        {
            return _parent.Value;
        }
        set
        {
            value = Math.Max(0, value - _parent.Minimum);
            ThumbLocation = (float)Scale(value, _parent.Maximum - _parent.Minimum, _drawRegion.Range)
                + _drawRegion.Start;
        }
    }

    #endregion Properties

    #region Interaction

    public void OnTouch(SKTouchEventArgs e)
    {
        if (_drawRegion.IsEmpty)
        {
            return;
        }
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                OnPressedInteraction(e);
                break;
            case SKTouchAction.Moved:
                OnMovedInteraction(e);
                break;
            case SKTouchAction.Released:
                OnReleasedInteraction(e);
                break;
            case SKTouchAction.Exited:
                OnExitedInteraction(e);
                break;
            case SKTouchAction.Cancelled:
                OnCanceledInteraction(e);
                break;
        }
    }

    private void OnPressedInteraction(SKTouchEventArgs e)
    {
        if (_isDragging || !e.InContact)
        {
            OnCanceledInteraction(e);
            return;
        }
        Size = _parent.CanvasSize.ToMauiSize();

        SliderPart part = HitTest(e.Location, true, out double value);
        if (part != SliderPart.None)
        {
            if (part == SliderPart.Thumb)
            {
                _isDragging = true;
            }
            else
            {
                _parent.Value = AdjustForDirection(part, value);
            }
            e.Handled = true;
        }
    }

    private void OnMovedInteraction(SKTouchEventArgs e)
    {
        if (_isDragging)
        {
            SliderPart part = HitTest(e.Location, false, out double value);
            if (part == SliderPart.Start || part == SliderPart.End)
            {
                _parent.Value = value;
            }
            e.Handled = true;
        }
    }

    private void OnReleasedInteraction(SKTouchEventArgs e)
    {
        if (_isDragging)
        {
            SliderPart part = HitTest(e.Location, false, out double value);
            if (part == SliderPart.Start || part == SliderPart.End)
            {
                _parent.Value = AdjustForDirection(part, value);
            }
            OnCanceledInteraction(e);
        }
        else
        {
            _isDragging = false;
        }
    }

    void OnExitedInteraction(SKTouchEventArgs e)
    {
        if (e.InContact && _isDragging)
        {
            e.Handled = true;
        }
    }

    void OnCanceledInteraction(SKTouchEventArgs e)
    {
        _isDragging = false;
        _parent.InvalidateSurface();
        e.Handled = true;
    }

    private double AdjustForDirection(SliderPart part, double value)
    {
        value = Round(value, _parent.Interval);
        if (value == _parent.Value)
        {
            if (part == SliderPart.End)
            {
                value += _parent.Interval;
            }
            else if (part == SliderPart.Start)
            {
                value -= _parent.Interval;
            }
        }
        return value;
    }

    private SliderPart HitTest(SKPoint point, bool isPress, out double value)
    {
        SliderPart result = SliderPart.None;
        value = 0;

        if (!_parent.IsEnabled)
        {
            return result;
        }

        if (isPress && !_drawRegion.HitRect.HitTest(point))
        {
            value = 0;
            return result;
        }
        SKPoint pt = _drawRegion.HitRect.Constrain(point);

        double thumbLocation = _parent.Orientation == StackOrientation.Horizontal
            ? pt.X
            : pt.Y;
        double offset = thumbLocation - _drawRegion.Start;

        value = Scale(offset, _drawRegion.Range, _parent.Maximum - _parent.Minimum) + _parent.Minimum;

        if (thumbLocation == ThumbLocation)
        {
            result = SliderPart.Thumb;
        }
        else if (isPress && thumbLocation >= ThumbLocation - _drawRegion.Radius && thumbLocation <= ThumbLocation + _drawRegion.Radius)
        {
            result = SliderPart.Thumb;
        }
        else if (thumbLocation > ThumbLocation)
        {
            result = SliderPart.End;
        }
        else
        {
            result = SliderPart.Start;
        }
        return result;
    }

    #endregion Interaction

    /// <summary>
    /// Draws the slider.
    /// </summary>
    /// <param name="canvas">The <see cref="ICanvas"/> to draw upon.</param>
    public void Draw(SKCanvas canvas)
    {
        if (_parent.BackgroundColor is not null)
        {
            canvas.Clear(_parent.BackgroundColor.ToSKColor());
        }
        else
        {
            canvas.Clear();
        }

        SKPaint paint = new SKPaint()
        {
            Color =_parent.TrackColor.ToSKColor(),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = _drawRegion.TrackSize,
            StrokeCap = SKStrokeCap.Butt,
            IsAntialias = true
        };
        canvas.DrawLine(_drawRegion.TrackStart, _drawRegion.TrackEnd, paint);

        Color color = (_parent.IsEnabled ? _parent.ThumbColor : _parent.DisabledColor);
        if (color != Colors.Transparent)
        { 
            paint.Style = SKPaintStyle.Fill;
            paint.Color = color.ToSKColor();

            switch (_drawRegion.Style)
            {
                case ThumbStyle.Circle:
                    SKPoint circle = _parent.Orientation == StackOrientation.Horizontal
                       ? new SKPoint(ThumbLocation, _drawRegion.MidPoint)
                       : new SKPoint(_drawRegion.MidPoint, ThumbLocation);
                    canvas.DrawCircle(circle, (float)_drawRegion.Radius, paint);
                    break;

                case ThumbStyle.Rectangle:

                    SKPoint start;
                    SKPoint end;

                    if (_parent.Orientation == StackOrientation.Horizontal)
                    {
                        start = new SKPoint(ThumbLocation - _drawRegion.Radius, _drawRegion.TrackStart.Y);
                        end = new SKPoint(ThumbLocation + _drawRegion.Radius, _drawRegion.TrackEnd.Y);
                    }
                    else
                    {
                        start = new SKPoint(_drawRegion.TrackStart.X, ThumbLocation - _drawRegion.Radius);
                        end = new SKPoint(_drawRegion.TrackEnd.X, ThumbLocation + _drawRegion.Radius);
                    }
                    paint.StrokeCap = SKStrokeCap.Round;
                    canvas.DrawLine(start, end, paint);
                    break;
            }
        }
    }

    static internal double Scale(double value, double source, double target)
    {
        double scale = target / source;
        return value * scale;
    }

    static internal double Round(double value, double interval)
    {
        if (interval > 0)
        {
            double remainder = value % interval;
            double threshold = interval / 2;
            if (remainder == 0)
            {
            }
            else if (remainder >= threshold)
            {
                value = value - remainder + interval;
            }
            else
            {
                value -= remainder;
            }
        }
        return value;
    }

}
