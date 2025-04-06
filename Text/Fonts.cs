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
}
