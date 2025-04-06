namespace GlyphViewer.Text;

using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

/// <summary>
/// Provides metrics for a measured <see cref="Glyph"/>.
/// </summary>
/// <remarks>
/// See GlyphMetrics.md in the docs folder for more information about rendering the associated glyph.
/// </remarks>
public class GlyphMetrics : IEquatable<GlyphMetrics>
{
    /// <summary>
    /// Gets an empty instance of this class.
    /// </summary>
    public static readonly GlyphMetrics Empty = new();

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this struct.
    /// </summary>
    public GlyphMetrics()
    {
        IsEmpty = true;
    }

    /// <summary>
    /// Initializes a new instance of this struct.
    /// </summary>
    /// <param name="glyph">The measured <see cref="Glyph"/>.</param>
    /// <param name="font">The <see cref="SKFont"/> containing the <see cref="Glyph"/>.</param>
    /// <param name="paint">The <see cref="SKPaint"/> used to measure the <see cref="Glyph"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="font"/> is a null reference
    /// -or-
    /// <paramref name="paint"/> is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="glyph"/> is empty.</exception>
    public GlyphMetrics(Glyph glyph, SKFont font, SKPaint paint)
    {
        ArgumentNullException.ThrowIfNull(font, nameof(font));
        ArgumentNullException.ThrowIfNull(paint, nameof(paint));
        if (glyph.IsEmpty)
        {
            throw new ArgumentException(nameof(glyph));
        }
        TextWidth = font.MeasureText(glyph.Text, out SKRect bounds, paint);
        Glyph = glyph;
        Descent = bounds.Bottom;
        Ascent = bounds.Top;
        Size = bounds.Size;
        Left = bounds.Left;
        FontSize = font.Size;

        TextBounds = bounds;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the value indicating if this instance is empty.
    /// </summary>
    public readonly bool IsEmpty;

    /// <summary>
    /// Gets the associated <see cref="Glyph"/>.
    /// </summary>
    public readonly Glyph Glyph;

    /// <summary>
    /// Gets the height of the containing font, in pixels.
    /// </summary>
    public readonly float FontSize;

    /// <summary>
    /// Gets the ascent offset from the baseline.
    /// </summary>
    /// <remarks>
    /// The value will be a negative number indicating the distance from the baseline
    /// to the top of the character.
    /// </remarks>
    public readonly float Ascent;

    /// <summary>
    /// Gets the descent offset from the baseline.
    /// </summary>
    public readonly float Descent;

    /// <summary>
    /// Gets the size required to render the <see cref="Glyph"/>.
    /// </summary>
    public SKSize Size
    {
        get;
    }

    /// <summary>
    /// Gets the X offset from the left.
    /// </summary>
    /// <remarks>
    /// The value is the negative of the <see cref="SKRect.Left"/> of <see cref="TextBounds"/>.
    /// <para>
    /// This value can be used to adjust the X-Coordinate of the character when calling DrawText if the 
    /// goal is to ensure all Glyphs are left-aligned. 
    /// edge of all glyphs align correctly.
    /// </para>
    /// </remarks>
    public readonly float Left;

    /// <summary>
    /// Gets the width returned by <see cref="SKPaint.MeasureText(string, ref SKRect)"/>
    /// </summary>
    public readonly float TextWidth;

    /// <summary>
    /// Gets the <see cref="SKRect"/> returned by <see cref="SKPaint.MeasureText(string, ref SKRect)"/>
    /// </summary>
    public SKRect TextBounds
    {
        get;
    }

    #endregion Properties

    #region Equality

    /// <summary>
    /// Determines if the specified object is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>
    /// <paramref name="obj"/> is a <see cref="GlyphMetrics"/> equal to this instance;
    /// otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj is GlyphMetrics metrics)
        {
            return Equals(metrics);
        }
        return false;
    }

    /// <summary>
    ///  Determines whether the specified <paramref name="other"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="GlyphMetrics"/> to compare with the current instance.</param>
    /// <returns>true if the specified <paramref name="other"/> is equal to the current instance; otherwise, false.</returns>
    public bool Equals(GlyphMetrics other)
    {
        return (Glyph == other.Glyph && FontSize == other.FontSize);
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Glyph.CodePoint, FontSize);
    }

    #endregion Equality

    #region Public Methods

    /// <summary>
    /// Creates a new instance of <see cref="GlyphMetrics"/> based on the specified <paramref name="glyph"/>,
    /// </summary>
    /// <param name="paint">The <see cref="SKPaint"/> to use to measure the text.</param>
    /// <param name="font">The <see cref="SKFont"/> containing the <paramref name="glyph"/>.</param>
    /// <param name="glyph">The associated <see cref="Glyph"/> to measure.</param>
    /// <returns>The new instance of a <see cref="GlyphMetrics"/> for the <paramref name="glyph"/>.</returns>
    public static GlyphMetrics CreateInstance(SKPaint paint, SKFont font, Glyph glyph)
    {
        return new GlyphMetrics(glyph, font, paint);
    }

    #endregion Public Methods
}
