namespace GlyphViewer.Text.OpenType;

using System;

/// <summary>
/// Defines the flags for the <see cref="Os2Table"/> fsSelection field.
/// </summary>
[Flags]
internal enum SelectionFlags : ushort
{
    Italic = 1,
    Underscore = 1 << 1,
    Negative = 1 << 2,
    Outlined = 1 << 3,
    Strikeout = 1 << 4,
    Bold = 1 << 5,
    Regular = 1 << 6,
    UseTypoMetrics = 1 << 7,
    Names = 1 << 8,
    Oblique = 1 << 9,
    Reserved = 0x3F << 10
}
