namespace GlyphViewer.Views.Renderers;

/// <summary>
/// Provides a structure for describing the spacing around a rectangular area.
/// </summary>
internal readonly struct SkSpacing : IEquatable<SkSpacing>
{
    public static readonly SkSpacing Instance = new SkSpacing();

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="thickness">The <see cref="Thickness"/> to use to initialize this instance.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// One or more edges in <paramref name="thickness"/> is less than zero or a NaN value.
    /// </exception>
    public SkSpacing(Thickness thickness)
    {
        if (double.IsNaN(thickness.Left) || double.IsNaN(thickness.Top) || double.IsNaN(thickness.Right) || double.IsNaN(thickness.Bottom))
        {
            throw new ArgumentOutOfRangeException(nameof(thickness));
        }
        if (thickness.Left < 0 || thickness.Right < 0 || thickness.Bottom < 0 || thickness.Top < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(thickness));
        }

        Horizontal = (float)thickness.HorizontalThickness;
        Vertical = (float)thickness.VerticalThickness;
    }

    /// <summary>
    /// Gets the horizontal spacing.
    /// </summary>
    public readonly float Horizontal;

    /// <summary>
    /// Gets the vertical spacing.
    /// </summary>
    public readonly float Vertical;

    #region Equality

    /// <summary>
    /// Determines if this instance is equal to another <see cref="SkSpacing"/>.
    /// </summary>
    /// <param name="other">The <see cref="SkSpacing"/> to compare.</param>
    /// <returns>true if this structure equals <paramref name="other"/>; otherwise, false.</returns>
    public bool Equals(SkSpacing other)
    {
        return Horizontal == other.Horizontal
            && Vertical == other.Vertical;
    }

    /// <summary>
    /// Determines if this instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>true if this structure equals <paramref name="obj"/>; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not null && obj is SkSpacing)
        {
            return Equals((SkSpacing)obj);
        }
        return false;
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Horizontal, Vertical);
    }

    #endregion Equality

    #region Operator overloads

    /// <summary>
    /// Tests whether two <see cref="SkSpacing"/> structures are equal.
    /// </summary>
    /// <param name="left">The <see cref="SkSpacing"/> structure that is to the left of the equality operator.</param>
    /// <param name="right">The <see cref="SkSpacing"/> structure that is to the right of the equality operator.</param>
    /// <returns>true if the two <see cref="SkSpacing"/> structures are equal; otherwise, false.</returns>
    public static bool operator ==(SkSpacing left, SkSpacing right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Tests whether two <see cref="SkSpacing"/> structures are not equal.
    /// </summary>
    /// <param name="left">The <see cref="SkSpacing"/> structure that is to the left of the equality operator.</param>
    /// <param name="right">The <see cref="SkSpacing"/> structure that is to the right of the equality operator.</param>
    /// <returns>true if the two <see cref="SkSpacing"/> structures are not equal; otherwise, false.</returns>
    public static bool operator !=(SkSpacing left, SkSpacing right)
    {
        return !left.Equals(right);
    }

    #endregion Operator overloads
}
