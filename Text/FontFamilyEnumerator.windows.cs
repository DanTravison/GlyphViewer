namespace GlyphViewer.Text;

using SkiaSharp;
using System;
using System.Collections.Generic;

internal partial class FontFamilyEnumerator
{
    public IEnumerable<FontFamily> GetFamilies()
    {
        Dictionary<string, FontFamily> table = new(StringComparer.InvariantCultureIgnoreCase);
        List<FontFamily> families = [];
        foreach (string familyName in SKFontManager.Default.GetFontFamilies())
        {
            FontFamily family = FontFamily.CreateInstance(familyName);
            yield return family;
        }
    }
}
