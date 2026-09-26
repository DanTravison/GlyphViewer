namespace GlyphViewer.Text;

using System.Globalization;
using System.Text;

/// <summary>
/// Defines a font family and text for a glyph.
/// </summary>
[System.Diagnostics.DebuggerDisplay("({GlyphId,nq} {Code,nq}) {Text}")]
public sealed class Glyph : IEquatable<Glyph>
{
    #region Fields

    /// <summary>
    /// Provides an empty <see cref="Glyph"/>.
    /// </summary>
    public static readonly Glyph Empty = new();

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes an <see cref="Empty"/> instance of this class.
    /// </summary>
    Glyph()
    {
        Text = string.Empty;
        Code = string.Empty;
        IsEmpty = true;
        Range = Unicode.Range.None;
    }

    internal Glyph(FontFamily fontFamily, OpenType.GlyphInfo info)
    {
        ArgumentNullException.ThrowIfNull(fontFamily, nameof(fontFamily));
        ArgumentNullException.ThrowIfNull(info, nameof(info));

        FontFamily = fontFamily;
        Range = info.Range;
        Category = info.Category;
        Codepoint = info.CodePoint;
        HasCodePoint = info.HasCodePoint;
        Name = info.Name;
        GlyphId = info.Id;

        Code = HasCodePoint ? $"U+{Codepoint:X4}" : string.Empty;
        if (HasCodePoint)
        {
            Rune rune = new(Codepoint);
            Text = rune.ToString();
        }
        else
        {
            Text = string.Empty;
        }
        IsEmpty = false;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the value indicating if this <see cref="Glyph"/> is empty.
    /// </summary>
    public bool IsEmpty
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="FontFamily"/> that contains the <see cref="Glyph"/>.
    /// </summary>
    public FontFamily FontFamily
    {
        get;
    }

    /// <summary>
    /// Gets the value indicating if the glyph has an associated <see cref="Codepoint"/>
    /// </summary>
    /// <value>true if the glyph has a <see cref="Codepoint"/>; otherwise, false.</value>
    public bool HasCodePoint
    {
        get;
    }
    /// <summary>
    /// Gets the codepoint for the glyph.
    /// </summary>
    /// <value>
    /// The codepoint for the glyph, if <see cref="HasCodePoint"/> is true; otherwise, a non-deterministic value.
    /// </value>
    public uint Codepoint
    {
        get;
    }

    /// <summary>
    /// Gets the glyph id for the glyph.
    /// </summary>
    public uint GlyphId
    {
        get;
    }

    /// <summary>
    /// Gets the text for the glyph.
    /// </summary>
    public string Text
    {
        get;
    }

    /// <summary>
    /// Gets the name of the glyph.
    /// </summary>
    /// <value>
    /// The name of the <see cref="Glyph"/>, if present
    /// in the font; otherwise, <see cref="string.Empty"/>.
    /// </value>
    public string Name
    {
        get;
    }

    /// <summary>
    /// Gets the string hex code for the glyph.
    /// </summary>
    public string Code
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="UnicodeCategory"/>.
    /// </summary>
    public UnicodeCategory Category
    {
        get;
    }

    /// <summary>
    /// Gets the unicode range for the <see cref="Char"/>.
    /// </summary>
    public Unicode.Range Range
    {
        get;
    }

    #endregion Properties

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
        if (obj is Glyph glyph)
        {
            return Equals(glyph);
        }
        return false;
    }

    /// <summary>
    ///  Determines whether the specified <paramref name="glyph"/> is equal to the current instance.
    /// </summary>
    /// <param name="glyph">The <see cref="Glyph"/> to compare with the current instance.</param>
    /// <returns>true if the specified <paramref name="glyph"/> is equal to the current instance; otherwise, false.</returns>
    public bool Equals(Glyph glyph)
    {
        return
        (
            glyph is not null
            &&
            GlyphId == glyph.GlyphId
            &&
            Codepoint == glyph.Codepoint
            &&
            FontFamily == glyph.FontFamily
        );
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(GlyphId, Codepoint, FontFamily.Name);
    }

    #endregion Equality

    #region Operator overloads

    /// <summary>
    /// Tests whether two <see cref="Glyph"/> structures are equal.
    /// </summary>
    /// <param name="left">The <see cref="Glyph"/> structure that is to the left of the equality operator.</param>
    /// <param name="right">The <see cref="Glyph"/> structure that is to the right of the equality operator.</param>
    /// <returns>true if the two <see cref="Glyph"/> structures are equal; otherwise, false.</returns>
    public static bool operator ==(Glyph left, Glyph right)
    {
        return left is not null && left.Equals(right);
    }

    /// <summary>
    /// Tests whether two <see cref="Glyph"/> structures are not equal.
    /// </summary>
    /// <param name="left">The <see cref="Glyph"/> structure that is to the left of the equality operator.</param>
    /// <param name="right">The <see cref="Glyph"/> structure that is to the right of the equality operator.</param>
    /// <returns>true if the two <see cref="Glyph"/> structures are not equal; otherwise, false.</returns>
    public static bool operator !=(Glyph left, Glyph right)
    {
        return !(left is not null && left.Equals(right));
    }

    #endregion Operator overloads
}
