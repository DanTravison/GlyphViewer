namespace GlyphViewer.Views;

/// <summary>
/// Defines the layout style for rows and cells in the <see cref="GlyphsView"/>.
/// </summary>
[Flags]
public enum GlyphLayoutStyle
{
    /// <summary>
    /// The height of the row is based on the tallest glyph in the font.
    /// The cell width is based on the widest glyph in the font.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The height of the row is based on the tallest glyph in the row.
    /// </summary>
    /// <remarks>
    /// If a width flag is not specified, the cell width is based on the widest glyph in the font. 
    /// </remarks>
    Height = 1,

    /// <summary>
    /// The width of a cell is based on the widest glyph in the row.
    /// </summary>
    /// <remarks>
    /// If <see cref="Width"/> and <see cref="GlyphWidth"/> are both specified,
    /// <see cref="GlyphWidth"/>is ignored.
    /// </remarks>
    Width = 1 << 2,

    /// <summary>
    /// The width of a cell is based on the glyph's width.
    /// </summary>
    /// <remarks>
    /// If <see cref="Width"/> and <see cref="GlyphWidth"/> are both specified,
    /// <see cref="GlyphWidth"/>is ignored.
    /// </remarks>
    GlyphWidth = 1 << 3,
}
