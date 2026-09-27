namespace CodeCadence.Typography.Unicode;

using System.Diagnostics;

/// <summary>
/// Defines a Unicode range.
/// </summary>
[DebuggerDisplay("{Name, nq}[{Length,nq}]")]
public struct Range : IEquatable<Range>
{
    #region Fields

    /// <summary>
    /// Gets the empty range.
    /// </summary>
    public static readonly Range Empty = new(false);

    const string NoRange = "None";

    /// <summary>
    /// Gets the <see cref="Range"/> for a codepoint or glyph that is not in a range.
    /// </summary>
    public static readonly Range None = new(true);

    /// <summary>
    /// Gets the first codepoint in the range.
    /// </summary>
    public readonly uint First;

    /// <summary>
    /// Gets the first codepoint in the range.
    /// </summary>
    public readonly uint Last;

    /// <summary>
    /// Gets the number of codepoints in the range.
    /// </summary>
    public readonly int Length;

    /// <summary>
    /// Gets the name of the range.
    /// </summary>
    public string Name
    {
        get;
    }

    /// <summary>
    /// Gets the identifier for the range.
    /// </summary>
    public uint Id
    {
        get => First;
    }

    /// <summary>
    /// Gets the value indicating if this is an <see cref="Empty"/> instance.
    /// </summary>
    public bool IsEmpty
    {
        get;
    }

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes an <see cref="Empty"/> instance of this class.
    /// </summary>
    private Range(bool isNone)
    {
        First = Last = 0;
        Length = 0;

        if (isNone)
        {
            Name = NoRange;
            First = Last = uint.MaxValue;
        }
        else
        {
            IsEmpty = true;
            Name = string.Empty;
        }
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="name">The name of the range.</param>
    /// <param name="first">The first codepoint in the range.</param>
    /// <param name="last">The last codepoint in the range.</param>
    internal Range(string name, uint first, uint last)
        : this(first, last, name)
    {
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="first">The first codepoint in the range.</param>
    /// <param name="last">The last codepoint in the range.</param>
    /// <param name="name">The name of the range.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="last"/> is less than <paramref name="first"/>.</exception>
    internal Range(uint first, uint last, string name)
    {
        if (last < first)
        {
            throw new ArgumentOutOfRangeException(nameof(last));
        }
        First = first;
        Last = last;
        Length = (int)(last - first + 1);
        Name = name;
    }

    #endregion Constructors

    /// <summary>
    /// Gets the value indicating if the range contains a specified <paramref name="codepoint"/>.
    /// </summary>
    /// <param name="codepoint">The codepoint to query.</param>
    /// <returns>true if the range contains a specified <paramref name="codepoint"/>; otherwise, false.</returns>
    public readonly bool Contains(uint codepoint)
    {
        return codepoint >= First && codepoint <= Last;
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
    public override readonly bool Equals(object obj)
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
    public readonly bool Equals(Range other)
    {
        return
        (
            First == other.First && Length == other.Length
        );
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public readonly override int GetHashCode()
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
