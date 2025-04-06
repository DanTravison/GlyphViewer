namespace GlyphViewer.Controls;

using SkiaSharp;

readonly struct DrawRegion
{
    public static readonly DrawRegion Empty = new DrawRegion();

    public DrawRegion() { }

    public DrawRegion(StackOrientation orientation, ThumbStyle style, Size size, float trackSize, float radius)
    {
        Orientation = orientation;
        TrackSize = trackSize;
        Style = style;
        Radius = radius;


        switch (style)
        {
            case ThumbStyle.Rectangle:
                Radius = (float)radius * 3;
                break;
            case ThumbStyle.Circle:
            default:
                Radius = (float)radius;
                break;
        }

        HitRect = new(size, (float)Radius);

        if (orientation == StackOrientation.Horizontal)
        {
            MidPoint = (float)size.Height / 2;
            Start = HitRect.Left + Radius;
            End = HitRect.Right - Radius;
            TrackStart = new SKPoint(Start, MidPoint - TrackSize / 2);
            TrackEnd = new SKPoint(End, MidPoint - TrackSize / 2);
        }
        else
        {
            MidPoint = (float)size.Width / 2;
            Start = HitRect.Top + Radius;
            End = HitRect.Bottom - Radius;
            TrackStart = new SKPoint(MidPoint - TrackSize / 2, Start);
            TrackEnd = new SKPoint(MidPoint - TrackSize / 2, End);
        }
        Range = End - Start;
    }

    /// <summary>
    /// Constrains the thumb position to <see cref="Start"/> and <see cref="End"/>
    /// </summary>
    /// <param name="thumb">The thumb position on the slider.</param>
    public readonly void Constrain(ref float thumb)
    {
        float min = Math.Min(Start, End);
        float max = Math.Max(Start, End);
        thumb = float.Clamp(thumb, min, max);
    }

    /// <summary>
    /// Gets the value indicating if the region is empty.
    /// </summary>
    public readonly bool IsEmpty
    {
        get => Range == 0;
    }

    /// <summary>
    /// Gets the <see cref="StackOrientation"/> of the slider.
    /// </summary>
    public readonly StackOrientation Orientation
    {
        get;
    }

    /// <summary>
    /// Gets the midpoint of the track.
    /// </summary>
    public readonly float MidPoint
    {
        get;
    }

    /// <summary>
    /// Gets the start of the track.
    /// </summary>
    public readonly float Start
    {
        get;
    }

    /// <summary>
    /// Gets the end of the track.
    /// </summary>
    public readonly float End
    {
        get;
    }

    /// <summary>
    /// Gets the range of the slider thumb positions.
    /// </summary>
    public readonly float Range
    {
        get;
    }

    /// <summary>
    /// Gets the location of the start of slider track.
    /// </summary>
    public readonly SKPoint TrackStart
    {
        get;
    }

    /// <summary>
    /// Gets the location of the end of the slider track.
    /// </summary>
    public readonly SKPoint TrackEnd
    {
        get;
    }

    /// <summary>
    /// Gets the track's stroke size.
    /// </summary>
    public readonly float TrackSize
    {
        get;
    }

    /// <summary>
    /// Gets the radius of the thumb.
    /// </summary>
    public readonly float Radius
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="SKBoundingRect"/> for the slider's drawing area.
    /// </summary>
    public readonly SKBoundingRect HitRect
    {
        get;
    }

    /// <summary>
    /// Gets the style of thumb to draw.
    /// </summary>
    public readonly ThumbStyle Style
    {
        get;
    }
}
