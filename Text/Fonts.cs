namespace GlyphViewer.Text;

using SkiaSharp;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides font extensions
/// </summary>
public static class Fonts
{
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
    /// Converts a <see cref="TextAlignment"/> to an <see cref="SKTextAlign"/>.
    /// </summary>
    /// <param name="alignment">The <see cref="FontAttributes"/> to convert.</param>
    /// <returns>The <see cref="SKFontStyle"/> for the <paramref name="alignment"/>.</returns>
    public static SKTextAlign ToTextAlign(this TextAlignment alignment)
    {
        return alignment switch
        {
            TextAlignment.Start => SKTextAlign.Left,
            TextAlignment.Center => SKTextAlign.Center,
            TextAlignment.End => SKTextAlign.Right,
            _ => SKTextAlign.Left
        };
    }

    /// <summary>
    /// Gets the available font families.
    /// </summary>
    /// <returns>A <see cref="List{FontFamily}"/> of the available font families.</returns>
    public static List<FontFamily> GetFontFamilies()
    {
        List<FontFamily> families = [];
        foreach (string familyName in SKFontManager.Default.GetFontFamilies())
        {
            families.Add(new(familyName));
        }
        return families;
    }

    /// <summary>
    /// Converts a text point size to a pixels.
    /// </summary>
    /// <param name="emSize">The font size in points.</param>
    /// <returns>The font size in pixels.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float ToPixels(this float emSize)
    {
        return emSize * 96 / 72;
    }

    /// <summary>
    /// Experimental: Scales font points to pixels at the current display density.
    /// </summary>
    /// <param name="emSize">The font size in points.</param>
    /// <remarks>
    /// The goal is to scale the font size to the current display density such that 
    /// a character displayed via SkiaSharp is the same size as the same character displayed
    /// in Word using the same font and font size.
    /// </remarks>
    public static float ScalePoints(this float emSize)
    {
        float scale = (float)DeviceDisplay.Current.MainDisplayInfo.Density;
        return emSize.ToPixels() * scale;
    }

    /// <summary>
    /// Experimental: Scales font points to pixels at the current display density.
    /// </summary>
    /// <param name="emSize">The font size in points.</param>
    /// <remarks>
    /// The goal is to scale the font size to the current display density such that 
    /// a character displayed via SkiaSharp is the same size as the same character displayed
    /// in Word using the same font and font size.
    /// </remarks>
    public static float ScalePoints(this double emSize)
    {
        float scale = (float)DeviceDisplay.Current.MainDisplayInfo.Density;
        return ((float)emSize).ToPixels() * scale;
    }


    /// <summary>
    /// Experimental: Measures the text using the current display density.
    /// </summary>
    /// <param name="font">The <see cref="SKFont"/> to use to measure the text.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="bounds">The <see cref="SKRect"/> containing the bounds for the measured text.</param>
    /// <param name="paint">The optional <see cref="SKPaint"/> to use to measure.</param>
    /// <returns>The text width of the measured text.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="font"/> or <paramref name="text"/> is a null reference.
    /// </exception>
    public static float Measure(this SKFont font, string text, out SKRect bounds, SKPaint paint = null)
    {
        ArgumentNullException.ThrowIfNull(font);
        ArgumentNullException.ThrowIfNull(text);
        float size = font.Size;
        font.Size = font.Size.ScalePoints();
        float width = font.MeasureText(text, out bounds, paint);
        font.Size = size;
        return width;
    }

    /// <summary>
    /// Experimental: Draw text on a canvas.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw on.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    /// <param name="font">The <see cref="SKFont"/> to use to draw the text.</param>
    /// <param name="text">The string text to draw.</param>
    /// <param name="left">The left position to draw the text.</param>
    /// <param name="baseline">The text baseline.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="font"/> or <paramref name="text"/> or <paramref name="paint"/> is a null reference.
    /// </exception>
    public static void DrawText
    (
        this SKCanvas canvas,
        SKFont font,
        SKPaint paint,
        string text,
        float left, float baseline,
        SKTextAlign align = SKTextAlign.Left
    )
    {
        ArgumentNullException.ThrowIfNull(font, nameof(font));
        ArgumentNullException.ThrowIfNull(canvas, nameof(canvas));
        ArgumentNullException.ThrowIfNull(paint, nameof(paint));
        ArgumentNullException.ThrowIfNull(text, nameof(text));
        float size = font.Size;
        font.Size = font.Size.ScalePoints();
        canvas.DrawText(text, left, baseline, align, font, paint);
        font.Size = size;
    }
}
