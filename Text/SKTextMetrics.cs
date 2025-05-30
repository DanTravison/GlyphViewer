namespace GlyphViewer.Text;

using SkiaSharp;

/// <summary>
/// Provides metrics for a measured string.
/// </summary>
public class SKTextMetrics : IEquatable<SKTextMetrics>
{
    /// <summary>
    /// Defines an empty instance of this class.
    /// </summary>
    public static readonly SKTextMetrics EmptyInstance = new();

    /// <summary>
    /// Initialies an empty instance of this class.
    /// </summary>
    protected SKTextMetrics()
    {
        IsEmpty = true;
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The <see cref="SKFont"/> to use to measure the text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="font"/> is a null reference.</exception>
    /// <exception cref="ArgumentException"><paramref name="text"/> is a null or empty string.</exception>
    public SKTextMetrics(string text, SKFont font)
    {
        ArgumentNullException.ThrowIfNull(font, nameof(font));
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException(nameof(text));
        }
        FamilyName = font.Typeface.FamilyName;
        FontSize = font.Size;
        Text = text;
        TextWidth = font.Measure(text, out SKRect bounds, null);
        Descent = bounds.Bottom;
        Ascent = bounds.Top;
        Size = bounds.Size;
        Left = bounds.Left;
        TextBounds = bounds;
    }

    #region Properties

    /// <summary>
    /// Gets the value indicating if this instance is empty.
    /// </summary>
    public readonly bool IsEmpty;

    /// <summary>
    /// Gets the text value.
    /// </summary>
    public readonly string Text;

    /// <summary>
    /// Gets the font family name used to measure the <see cref="Text"/>.
    /// </summary>
    public string FamilyName
    {
        get;
    }

    /// <summary>
    /// Gets the height of the font used to measure the text, in pixels.
    /// </summary>
    public readonly float FontSize;

    /// <summary>
    /// Gets the ascent offset from the baseline.
    /// </summary>
    /// <remarks>
    /// The value will be a negative number indicating the offset from the baseline
    /// to the top of the text.
    /// </remarks>
    public readonly float Ascent;

    /// <summary>
    /// Gets the offset from the baseline to the bottom of the text.
    /// </summary>
    public readonly float Descent;

    /// <summary>
    /// Gets the size required to render the <see cref="Text"/>.
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
    /// This value can be used to adjust the X-Coordinate of the character when drawing the text if the 
    /// goal is to ensure all text is left-aligned. 
    /// </para>
    /// </remarks>
    public readonly float Left;

    /// <summary>
    /// Gets the width returned by <see cref="SKFont.MeasureText(string, out SKRect, SKPaint)"/>
    /// </summary>
    public readonly float TextWidth;

    /// <summary>
    /// Gets the <see cref="SKRect"/> returned by <see cref="SKFont.MeasureText(string, out SKRect, SKPaint)"/>
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
    /// <paramref name="obj"/> is a <see cref="SKTextMetrics"/> equal to this instance;
    /// otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as SKTextMetrics);
    }

    /// <summary>
    ///  Determines whether the specified <paramref name="other"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="SKTextMetrics"/> to compare with the current instance.</param>
    /// <returns>
    /// true if the specified <paramref name="other"/> has the same
    /// <see cref="FamilyName"/>, <see cref="FontSize"/>, and <see cref="Text"/>; 
    /// otherwise, false.
    /// </returns>
    public bool Equals(SKTextMetrics other)
    {
        return
        (
            other is not null
            &&
            Text == other.Text
            &&
            FamilyName == other.FamilyName
            &&
            FontSize == other.FontSize
        );
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Text, FamilyName, FontSize);
    }

    #endregion Equality
}
