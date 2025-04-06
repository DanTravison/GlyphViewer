namespace GlyphViewer.Text.Unicode;

/// <summary>
/// Defines a Unicode range.
/// </summary>
public sealed class Range : IEquatable<Range>
{
    #region Fields

    /// <summary>
    /// Gets the empty range.
    /// </summary>
    public static readonly Range Empty = new();

    /// <summary>
    /// Gets the first code point in the range.
    /// </summary>
    public readonly uint First;

    /// <summary>
    /// Gets the first code point in the range.
    /// </summary>
    public readonly uint Last;

    /// <summary>
    /// Gets the number of code points in the range.
    /// </summary>
    public readonly uint Length;

    /// <summary>
    /// Gets the name of the range.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Gets the identifier for the range.
    /// </summary>
    public uint Id
    {
        get => First;
    }

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes an <see cref="Empty"/> instance of this class.
    /// </summary>
    private Range()
    {
        First = Last = 0;
        Length = 0;
        Name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="name">The name of the range.</param>
    /// <param name="first">The first code point in the range.</param>
    /// <param name="last">The last code point in the range.</param>
    internal Range(string name, ushort first, ushort last)
        : this(first, last, name)
    {
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="first">The first code point in the range.</param>
    /// <param name="last">The last code point in the range.</param>
    /// <param name="name">The name of the range.</param>
    internal Range(uint first, uint last, string name)
    {
        // This construct is used for extended ranges.
        if (last < first)
        {
            throw new ArgumentOutOfRangeException(nameof(first));
        }
        First = first;
        Last = last;
        Length = ((uint)last - (uint)first) + 1;
        Name = name;
    }

    #endregion Constructors

    /// <summary>
    /// Gets the value indicating if the range contains a specified <paramref name="codePoint"/>.
    /// </summary>
    /// <param name="codePoint">The code point to query.</param>
    /// <returns>true if the range contains a specified <paramref name="codePoint"/>; otherwise, false.</returns>
    public bool Contains(ushort codePoint)
    {
        return codePoint >= First && codePoint <= Last;
    }

    #region Equality

    /// <summary>
    /// Determines if the specified object is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>
    /// <paramref name="obj"/> is a <see cref="Glyph"/> equal to this instance;
    /// otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj is Range range)
        {
            return Equals(range);
        }
        return false;
    }

    /// <summary>
    ///  Determines whether the specified <paramref name="other"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="Range"/> to compare with the current instance.</param>
    /// <returns>true if the specified <paramref name="other"/> is equal to the current instance; otherwise, false.</returns>
    public bool Equals(Range other)
    {
        return First == other.First && Length == other.Length;
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(First, Length);
    }

    #endregion Equality

    #region Operator overloads

    /// <summary>
    /// Tests whether two <see cref="Range"/> structures are equal.
    /// </summary>
    /// <param name="left">The <see cref="Range"/> structure that is to the left of the equality operator.</param>
    /// <param name="right">The <see cref="Range"/> structure that is to the right of the equality operator.</param>
    /// <returns>true if the two <see cref="Range"/> structures are equal; otherwise, false.</returns>
    public static bool operator ==(Range left, Range right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Tests whether two <see cref="Range"/> structures are not equal.
    /// </summary>
    /// <param name="left">The <see cref="Range"/> structure that is to the left of the equality operator.</param>
    /// <param name="right">The <see cref="Range"/> structure that is to the right of the equality operator.</param>
    /// <returns>true if the two <see cref="Range"/> structures are not equal; otherwise, false.</returns>
    public static bool operator !=(Range left, Range right)
    {
        return !(left.Equals(right));
    }

    #endregion Operator overloads
}
