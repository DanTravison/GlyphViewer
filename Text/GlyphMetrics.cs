namespace GlyphViewer.Text;

using SkiaSharp;

/// <summary>
/// Provides metrics for a measured <see cref="Glyph"/>.
/// </summary>
public class GlyphMetrics : SKTextMetrics, IEquatable<GlyphMetrics>
{
    /// <summary>
    /// Gets an empty instance of this class.
    /// </summary>
    public static readonly GlyphMetrics Empty = new();

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this struct.
    /// </summary>
    GlyphMetrics()
        : base()
    {
        Glyph = Glyph.Empty;
    }

    /// <summary>
    /// Initializes a new instance of this struct.
    /// </summary>
    /// <param name="glyph">The measured <see cref="Glyph"/>.</param>
    /// <param name="font">The <see cref="SKFont"/> containing the <paramref name="glyph"/>.</param>
    /// <param name="paint">The optional <see cref="SKPaint"/> used to measure the <paramref name="glyph"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="font"/> is a null reference.</exception>
    /// <exception cref="ArgumentException"><paramref name="glyph"/> is a null reference or<see cref="Glyph.Empty"/> instance.</exception>
    GlyphMetrics(Glyph glyph, SKFont font, SKPaint paint)
        : base(glyph.Text, font, paint)
    {
        Glyph = glyph;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the associated <see cref="Glyph"/>.
    /// </summary>
    public readonly Glyph Glyph;

    #endregion Properties

    #region Equality

    /// <summary>
    ///  Determines whether the specified <paramref name="other"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="GlyphMetrics"/> to compare with the current instance.</param>
    /// <returns>true if the specified <paramref name="other"/> is equal to the current instance; otherwise, false.</returns>
    public bool Equals(GlyphMetrics other)
    {
        return
        (
            other is not null
            &&
            Glyph == other.Glyph
            &&
            FontSize == other.FontSize
            &&
            FamilyName == other.FamilyName
        );
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Glyph.Code, FamilyName, FontSize);
    }

    #endregion Equality

    #region Public Methods

    /// <summary>
    /// Creates a new instance of <see cref="GlyphMetrics"/> based on the specified <paramref name="glyph"/>,
    /// </summary>
    /// <param name="glyph">The associated <see cref="Glyph"/> to measure.</param>
    /// <param name="font">The <see cref="SKFont"/> to use to measure the <paramref name="glyph"/>.</param>
    /// <param name="paint">The optional <see cref="SKPaint"/> to use to measure the <paramref name="glyph"/>.</param>
    /// <returns>The new instance of a <see cref="GlyphMetrics"/> for the <paramref name="glyph"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="font"/> is a null reference.</exception>
    /// <exception cref="ArgumentException"><paramref name="glyph"/> is a null reference or<see cref="Glyph.Empty"/> instance.</exception>
    public static GlyphMetrics CreateInstance(Glyph glyph, SKFont font, SKPaint paint = null)
    {
        if (glyph is null || glyph.IsEmpty)
        {
            throw new ArgumentException(nameof(glyph));
        }
        return new GlyphMetrics(glyph, font, paint);
    }

    #endregion Public Methods
}
