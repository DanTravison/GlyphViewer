namespace CodeCadence.Typography.Unicode;

using System;
using System.Collections.Generic;
using System.Text;

internal static class UnicodeUtility
{
    public static bool IsValidCodePoint(uint codepoint)
    {
        // Must be within Unicode scalar range
        if (codepoint > 0x10FFFF)
            return false;

        // Must not be a surrogate
        return codepoint < 0xD800 || codepoint > 0xDFFF;
    }
}
