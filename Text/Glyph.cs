namespace GlyphViewer.Text;

using System.Diagnostics;
using System.Globalization;
using System.Text;

/// <summary>
/// Defines a font family and text for a glyph.
/// </summary>
[DebuggerDisplay("({codepoint,nq}) {Text}")]
public class Glyph : IEquatable<Glyph>
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
    public Glyph()
    {
        FontFamily = string.Empty;
        Text = string.Empty;
        Code = string.Empty;
        Char = '\0';
        IsEmpty = true;
        CodePoint = 0;
        Range = Unicode.Range.Empty;
    }

    public Glyph(string fontFamily, char ch, ushort codepoint, UnicodeCategory category, Unicode.Range range)
    {
        FontFamily = fontFamily;
        Text = char.ConvertFromUtf32(ch);
        CodePoint = codepoint;
        Char = ch;
        Category = category;
        IsEmpty = false;
        StringBuilder sb = new();
        foreach (char c in Text)
        {
            if (sb.Length > 0)
            {
                sb.Append(',');
            }
            sb.AppendFormat("U+{0:X4}", (int)c);
        }
        Code = sb.ToString();
        Range = range;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the value indicating if this <see cref="Glyph"/> is empty.
    /// </summary>
    public readonly bool IsEmpty;

    /// <summary>
    /// Gets the font family to use to draw the <see cref="Text"/>.
    /// </summary>
    public readonly string FontFamily;

    /// <summary>
    /// Gets the text for the glyph.
    /// </summary>
    public readonly string Text;

    /// <summary>
    /// Gets the unicode character for the glyph.
    /// </summary>
    public readonly Char Char;

    /// <summary>
    /// Gets the glyph code point.
    /// </summary>
    public readonly ushort CodePoint;

    /// <summary>
    /// Gets the string hex code for the <see cref="CodePoint"/>.
    /// </summary>
    public readonly string Code;

    /// <summary>
    /// Gets the <see cref="UnicodeCategory"/>.
    /// </summary>
    public readonly UnicodeCategory Category;

    /// <summary>
    /// Gets the unicode range for the <see cref="Char"/>.
    /// </summary>
    public readonly Unicode.Range Range;

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
    ///  Determines whether the specified <paramref name="other"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="Glyph"/> to compare with the current instance.</param>
    /// <returns>true if the specified <paramref name="other"/> is equal to the current instance; otherwise, false.</returns>
    public bool Equals(Glyph other)
    {
        return
        (
            other is not null
            &&
            CodePoint == other.CodePoint
            &&
            FontFamily == other.FontFamily
        );
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Text, FontFamily);
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
