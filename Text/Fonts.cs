namespace GlyphViewer.Text;

using SkiaSharp;
using System.Globalization;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides font extensions
/// </summary>
public static class Fonts
{
    /// <summary>
    /// Converts a text point size to a pixels.
    /// </summary>
    /// <param name="emSize">The font size in points.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToPixels(this float emSize)
    {
        return emSize * 96 / 72;
    }

    /// <summary>
    /// Converts a text point size to a pixels.
    /// </summary>
    /// <param name="emSize">The font size in points.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToPixels(this double emSize)
    {
        return (float)emSize * 96 / 72;
    }

    /// <summary>
    /// Gets the available font families.
    /// </summary>
    /// <returns>A <see cref="List{String}"/> of the available font families.</returns>
    public static List<string> GetFontFamilies()
    {
        return new(SKFontManager.Default.GetFontFamilies());
    }

    /// <summary>
    /// Gets the glyphs defined by a <paramref name="typeface"/>.
    /// </summary>
    /// <param name="typeface">The <see cref="SKTypeface"/> to query.</param>
    /// <param name="filter">The zero or more UnicodeCategory values to filter.</param>
    /// <returns>A <see cref="List{Glypy}"/> containing the glyphs defined by a <paramref name="typeface"/>.</returns>
    public static List<Glyph> Glyphs(this SKTypeface typeface, params UnicodeCategory[] filter)
    {
        List<Glyph> glyphs = [];
        int glyphCount = typeface.GlyphCount;

        for (ushort glyphIndex = 0; glyphIndex < glyphCount; glyphIndex++)
        {
            ushort codePoint = typeface.GetGlyph(glyphIndex);
            if (codePoint == 0)
            {
                continue;
            }
            if (filter != null && filter.Length > 0)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory((char)codePoint);
                if (filter.Contains(category))
                {
                    continue;
                }
            }

            glyphs.Add(new Glyph(typeface.FamilyName, codePoint));
        }
        return glyphs;
    }
}
