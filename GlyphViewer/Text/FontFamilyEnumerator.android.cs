namespace GlyphViewer.Text;

using System.Collections.Generic;

internal partial class FontFamilyEnumerator
{
    static readonly string[] _directories =
    {
        "/system/fonts",
        "/product/fonts",
        "/vendor/fonts",
        "/data/fonts"
    };
    public IEnumerable<FontFamily> GetFamilies()
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
}
