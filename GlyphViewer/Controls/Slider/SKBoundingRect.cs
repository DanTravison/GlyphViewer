namespace GlyphViewer.Controls;

using SkiaSharp;

/// <summary>
/// Provides a rect structure for constraining points and hit testing
/// using the <see cref="float"/> values.
/// </summary>
public readonly struct SKBoundingRect
{
    #region Fields

    // NOTE: Do not use RectF for storing the information. While RectF.Contains
    // works fine for HitTest, RectF cannot be used for Constrain since 
    // Rect.Right and Rect.Bottom are outside the rect so they cannot be
    // used to constrain a SKPoint.  As a result, we use
    // X,Y,MaxX, MaxY for HitTest and Left,Right,Top,Bottom for Constrain.

    /// <summary>
    /// Gets the left coordinate adjusted for the <see cref="Margin"/>.
    /// </summary>
    public readonly float Left;

    /// <summary>
    /// Gets the right coordinate adjusted for the <see cref="Margin"/>.
    /// </summary>
    public readonly float Right;

    /// <summary>
    /// Gets the top coordinate adjusted for the <see cref="Margin"/>.
    /// </summary>
    public readonly float Top;

    /// <summary>
    /// Gets the bottom coordinate adjusted for the <see cref="Margin"/>.
    /// </summary>
    public readonly float Bottom;

    /// <summary>
    /// Gets the margin around the edges of the <see cref="SKBoundingRect"/>.
    /// </summary>
    public readonly float Margin;

    /// <summary>
    /// Gets the X coordinate of the <see cref="SKBoundingRect"/>
    /// </summary>
    public readonly float X;

    /// <summary>
    /// Gets the maximum X coordinate of the <see cref="SKBoundingRect"/>
    /// </summary>
    private readonly float MaxX;

    /// <summary>
    /// Gets the Y coordinate of the <see cref="SKBoundingRect"/>
    /// </summary>
    public readonly float Y;

    /// <summary>
    /// Gets the maximum Y coordinate of the <see cref="SKBoundingRect"/>
    /// </summary>
    private readonly float MaxY;

    /// <summary>
    /// Gets the thickness of each stroke.
    /// </summary>
    /// <remarks>
    /// This value is used to determine the margin area within the <see cref="SKBoundingRect"/>.
    /// <para>
    /// The margin is equal to <see cref="Stroke"/> divided by 2.
    /// </para>>
    /// </remarks>
    public readonly float Stroke;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="size">The <see cref="Size"/> of the <see cref="SKBoundingRect"/>.</param>
    /// <param name="stroke">The stroke thickness to determine the inner margins of the <see cref="SKBoundingRect"/>.
    /// <para>
    /// The margin is used to adjust points when calling the constrain methods. A non-zero
    /// margin is used to account for the stroke thickness to ensure the point lies completely
    /// within the <see cref="SKBoundingRect"/>.
    /// </para>
    /// </param>
    public SKBoundingRect(Size size, float stroke)
    {
        Stroke = stroke;
        float margin = stroke / 2;
        Left = Top = Left = margin;
        Right = (float)size.Width - margin;
        Bottom = (float)size.Height - margin;
        MaxX = (float)size.Width;
        MaxY = (float)size.Height;
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="point">The center of the <see cref="SKBoundingRect"/>.</param>
    /// <param name="stroke">The width and height of the point.</param>
    /// <remarks>
    /// This constructor is used to define the rectangle created by a single
    /// point at the center and a stroke width and weight.
    /// </remarks>
    public SKBoundingRect(SKPoint point, float stroke)
    {
        Stroke = stroke;
        float margin = stroke / 2;
        X = Left = point.X - margin;
        Right = MaxX = point.X + margin;
        Y = Top = point.Y - margin;
        Bottom = MaxY = point.Y + margin;
    }

    #endregion Constructors

    #region HitTest

    /// <summary>
    /// Determines if the specified <paramref name="point"/> is within this <see cref="SKBoundingRect"/>.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> to test.</param>
    /// <returns>true if the specified <paramref name="point"/> coordinates is within this <see cref="SKBoundingRect"/>;
    /// otherwise, false.</returns>
    /// <remarks>
    /// This method includes the area defined by the margin.
    /// </remarks>
    public readonly bool HitTest(SKPoint point)
    {
        return
        (
            point.X >= X && point.X <= MaxX
            &&
            point.Y >= Y && point.Y <= MaxY
        );
    }

    /// <summary>
    /// Determine if a point is within a bounding rectangle.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <param name="tolerance">The tolerance to apply to the hit test.</param>
    /// <returns>true if the specified <paramref name="point"/> is within the 
    /// <see cref="SKBoundingRect"/>; otherwise, false.</returns>
    public bool HitTest(SKPoint point, float tolerance)
    {
        return point.X >= X - tolerance
               &&
               point.X < MaxX + tolerance
               &&
               point.Y >= Y - tolerance
               &&
               point.Y < MaxY + tolerance;
    }

    #endregion HitTest

    /// <summary>
    /// Constrains the specified <paramref name="point"/> to the <see cref="SKBoundingRect"/>.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> to constrain.</param>
    /// <returns>The <see cref="SKPoint"/> with the constrained coordinates.</returns>
    public readonly SKPoint Constrain(SKPoint point)
    {
        if (point.X < Left)
        {
            point.X = Left;
        }
        else if (point.X > Right)
        {
            point.X = Right;
        }
        if (point.Y < Top)
        {
            point.Y = Top;
        }
        else if (point.Y > Bottom)
        {
            point.Y = Bottom;
        }
        return new(point.X, point.Y);
    }
}
