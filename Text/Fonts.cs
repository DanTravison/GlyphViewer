namespace GlyphViewer.Text;

using SkiaSharp;
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
    /// <returns>The font size in pixels.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToPixels(this float emSize)
    {
        return emSize * 96 / 72;
    }

    /// <summary>
    /// Converts a text point size to a pixels.
    /// </summary>
    /// <param name="emSize">The font size in points.</param>
    /// <returns>The font sie in pixels.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToPixels(this double emSize)
    {
        return (float)emSize * 96 / 72;
    }

    /// <summary>
    /// Converts a <see cref="FontAttributes"/> to an <see cref="SKFontStyle"/>.
    /// </summary>
    /// <param name="attributes">The <see cref="FontAttributes"/> to convert.</param>
    /// <returns>The <see cref="SKFontStyle"/> for the <paramref name="attributes"/>.</returns>
    public static SKFontStyle ToFontStyle(this FontAttributes attributes)
    {
        return attributes switch
        {
            FontAttributes.Bold => SKFontStyle.Bold,
            FontAttributes.Italic => SKFontStyle.Italic,
            (FontAttributes.Bold | FontAttributes.Italic) => SKFontStyle.BoldItalic,
            _ => SKFontStyle.Normal
        };
    }

    /// <summary>
    /// Gets the available font families.
    /// </summary>
    /// <returns>A <see cref="List{String}"/> of the available font families.</returns>
    public static List<string> GetFontFamilies()
    {
        return new(SKFontManager.Default.GetFontFamilies());
    }
}
