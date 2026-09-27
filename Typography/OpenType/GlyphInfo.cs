namespace CodeCadence.Typography.OpenType;

using CodeCadence.Typography.Unicode;
using System;
using System.Globalization;
using Range = CodeCadence.Typography.Unicode.Range;

/// <summary>
/// Provides inforamtion for a glyph id parsed from the <see cref="CmapTable"/>.
/// </summary>
public sealed class GlyphInfo : IEquatable<GlyphInfo>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class for a glyph that doesn't have a <see cref="CodePoint"/>. 
    /// </summary>
    /// <param name="glyphId">The font-specific glyph <see cref="Id"/>.</param>
    internal GlyphInfo(uint glyphId)
    {
        Id = glyphId;
        Range = Range.None;
        Category = UnicodeCategory.OtherNotAssigned;
        HasCodePoint = false;
    }

    /// <summary>
    /// Initializes a new instance of this class for a glyph that has an associated <see cref="CodePoint"/>.
    /// </summary>
    /// <param name="glyphId">The font-specific glyph <see cref="Id"/>.</param>
    /// <param name="codepoint">The <see cref="CodePoint"/>.</param>
    internal GlyphInfo(uint glyphId, uint codepoint)
    {
        Id = glyphId;
        CodePoint = codepoint;
        Range = Ranges.Find(codepoint);
        if (UnicodeUtility.IsValidCodePoint((uint)codepoint))
        {
            Category = CharUnicodeInfo.GetUnicodeCategory((int)codepoint);
        }
        else
        {
            Category = UnicodeCategory.OtherNotAssigned;
        }
        HasCodePoint = true;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the font-specific glyph id.
    /// </summary>
    public uint Id { get; }

    /// <summary>
    /// Gets the value indicating if the glyph has an associated <see cref="CodePoint"/>
    /// </summary>
    /// <value>
    /// true if the glyph has a <see cref="CodePoint"/>; otherwise, false.
    /// </value>
    public bool HasCodePoint
    {
        get;
    }

    /// <summary>
    /// Gets the code point.
    /// </summary>
    /// <value>
    /// The code point for the glyph if <see cref="HasCodePoint"/> is true; otherwise, a non-determinstic value.
    /// </value>
    public uint CodePoint { get; }

    /// <summary>
    /// Gets the <see cref="UnicodeCategory"/>
    /// </summary>
    /// <value>The <see cref="UnicodeCategory"/> if the the <see cref="CodePoint"/> 
    /// has an assigned category; otherwise, 
    /// <see cref="UnicodeCategory.OtherNotAssigned"/>.
    /// </value>
    public UnicodeCategory Category { get; }

    /// <summary>
    /// Gets the <see cref="Range"/> containing the <see cref="CodePoint"/>.
    /// </summary>
    public Range Range { get; }

    /// <summary>
    /// Gets the name of the glyph
    /// </summary>
    /// <value>The name of the glyph; otherwise, an empty string.</value>
    public string Name { get; internal set; } = string.Empty;

    #endregion Properties

    /// <summary>
    /// etermines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as GlyphInfo);
    }

    /// <summary>
    /// Determines if a <see cref="GlyphInfo"/> equals the current instance.
    /// </summary>
    /// <param name="other">The <see cref="GlyphInfo"/> to compare.</param>
    /// <returns>true if <paramref name="other"/> equals this instance; otherwise, false.</returns>
    public bool Equals(GlyphInfo other)
    {
        // TODO: Can we qualify this with a font identifier
        // to ensure comparing GlypRecord across fonts 
        // returns false?
        return other is not null && Id == other.Id;
    }

    /// <summary>
    /// Serves 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
