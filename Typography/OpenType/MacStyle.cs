namespace CodeCadence.Typography.OpenType;

using System;
using System.Collections.Generic;
using System.Text;

[Flags]
internal enum MacStyle : ushort
{
    Bold = 1,
    Italic = 1 << 1,
    Underline = 1 << 2,
    Outline = 1 << 3,
    Shadow = 1 << 4,
    Condensed = 1 << 5,
    Extended = 1 << 6,
    // 7 – 15: Reserved (set to 0)
}
