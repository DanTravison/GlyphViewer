namespace GlyphViewer.Text.OpenType;

using GlyphViewer.Text.Unicode;
using System;
using System.Globalization;
using Range = GlyphViewer.Text.Unicode.Range;

/// <summary>
/// Provides inforamtion for a glyph id parsed from the <see cref="CMapTable"/>.
/// </summary>
internal sealed class CmapGlyphInfo : IEquatable<CmapGlyphInfo>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="glyphId">The font-specific glyph <see cref="Id"/>.</param>
    /// <param name="codepoint">The <see cref="CodePoint"/>.</param>
    public CmapGlyphInfo(uint glyphId, int codepoint)
    {
        Id = glyphId;
        CodePoint = codepoint;
        Range = Ranges.Find(codepoint);
        if (UnicodeUtility.IsValidCodePoint((uint)codepoint))
        {
            Category = CharUnicodeInfo.GetUnicodeCategory(codepoint);
        }
        else
        {
            Category = UnicodeCategory.OtherNotAssigned;
        }
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the font-specific glyph id.
    /// </summary>
    public uint Id { get; }

    /// <summary>
    /// Gets the code point.
    /// </summary>
    /// <value>
    /// The code point for the glyph.
    /// </value>
    public int CodePoint { get; }

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

    #endregion Properties

    /// <summary>
    /// etermines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as CmapGlyphInfo);
    }

    /// <summary>
    /// Determines if a <see cref="CmapGlyphInfo"/> equals the current instance.
    /// </summary>
    /// <param name="other">The <see cref="CmapGlyphInfo"/> to compare.</param>
    /// <returns>true if <paramref name="other"/> equals this instance; otherwise, false.</returns>
    public bool Equals(CmapGlyphInfo other)
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

