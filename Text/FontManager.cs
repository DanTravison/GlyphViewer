namespace GlyphViewer.Text;

using GlyphViewer.Resources;
using SkiaSharp;
using System.Runtime.CompilerServices;

public static class FontManager
{
    public static IEnumerable<FontFamily> GetFontFamilies()
    {
        foreach (FontFamily family in EnumerateFamilies())
        {
            yield return family;
        }
    }

#if !ANDROID

    private static IEnumerable<FontFamily> EnumerateFamilies()
    {
        Dictionary<string, FontFamily> table = new(StringComparer.InvariantCultureIgnoreCase);
        List<FontFamily> families = [];
        foreach (string familyName in SKFontManager.Default.GetFontFamilies())
        {
            FontFamily family = FontFamily.CreateInstance(familyName);
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

#else // ANDROID

    static readonly string[] _directories = 
    { 
        "/system/fonts", 
        "/product/fonts", 
        "/vendor/fonts", 
        "/data/fonts" 
    };
    private static IEnumerable<FontFamily> EnumerateFamilies()
    {
        foreach (string dir in _directories)
        {
            foreach (FileInfo file in GetFiles(dir))
            {
                FileFontFamily family = null;

                try
                {
                    family = new(file);
                }
                catch 
                {
                    family = null;
                }
                if (family is not null)
                {
                    yield return family;
                }
            }
        }
    }

    private static IEnumerable<FileInfo> GetFiles(string dir)
    {
        string[] files;
        try
        {
            files = Directory.GetFiles(dir);
        }
        catch
        {
            yield break;
        }
        foreach (string file in files)
        {
            if (file.EndsWith(".ttf") || file.EndsWith(".otf") || file.EndsWith(".ttc"))
            {
                yield return new FileInfo(file);
            }
        }
    }

#endif // ANDROID
}
