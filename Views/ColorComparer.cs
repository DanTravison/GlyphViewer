using SkiaSharp;
using System.Diagnostics.CodeAnalysis;

namespace GlyphViewer.Views;

/// <summary>
/// Provides a custom Color <see cref="IEqualityComparer{Color}"/>.
/// </summary>
public sealed class ColorComparer : EqualityComparer<Color>
{
    /// <summary>
    /// Gets a singleton instance of this class.
    /// </summary>
    public static readonly ColorComparer Comparer = new();

    private ColorComparer()
    { }

    /// <summary>
    /// Determines whether the <see cref="Color"/> objects are equal.
    /// </summary>
    /// <param name="x">The first <see cref="Color"/> to compare.</param>
    /// <param name="y">The second <see cref="Color"/> to compare.</param>
    /// <returns>true if the specified objects are equal; otherwise, false.</returns>
#pragma warning disable CA1822 // Mark members as static
    public bool Equals(SKColor x, SKColor y)
#pragma warning restore CA1822 // Mark members as static
    {
        return x.Equals(y);
    }

    /// <summary>
    /// Determines whether the <see cref="Color"/> objects are equal.
    /// </summary>
    /// <param name="x">The first <see cref="Color"/> to compare.</param>
    /// <param name="y">The second <see cref="Color"/> to compare.</param>
    /// <returns>true if the specified objects are equal; otherwise, false.</returns>
    public override bool Equals(Color x, Color y)
    {
        if (x is not null && y is not null)
        {
            return x.ToInt() == y.ToInt();
        }
        return false;
    }

    /// <summary>
    /// Returns a hash code for the specified object.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified object.</returns>
    public override int GetHashCode([DisallowNull] Color color)
    {
        return HashCode.Combine(color.Red, color.Green, color.Blue, color.Alpha);
    }

    /// <summary>
    /// Gets the value indicating if a <paramref name="color"/> is transparent.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to examine.</param>
    /// <returns>true if the color is transparent, otherwise, false.</returns>
    public static bool IsTransparent(Color color)
    {
        return color is null || color.ToInt() == Colors.Transparent.ToInt();
    }

}
