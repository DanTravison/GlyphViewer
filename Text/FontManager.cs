namespace GlyphViewer.Text;

using GlyphViewer.Resources;
using SkiaSharp;
using System.Runtime.CompilerServices;

public static class FontManager
{
    private readonly static FontFamilyEnumerator Current = new();

    public static IEnumerable<FontFamily> GetFontFamilies()
    {
        Dictionary<string, FontFamily> table = new(StringComparer.InvariantCultureIgnoreCase);
        List<FontFamily> families = [];

        foreach (FontFamily family in Current.GetFamilies())
        {
            table.Add(family.Name, family);
            yield return family;
        }

        foreach (FontResource resource in FontLoader.EmbeddedFonts)
        {
            if (!table.ContainsKey(resource.FamilyName))
            {
                FontFamily family = FontFamily.CreateInstance(resource.FamilyName);
                yield return family;
            }
        }
    }
}
