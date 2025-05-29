using SkiaSharp;

namespace GlyphViewer.Text;

/// <summary>
/// Provides an encapsulation of a font family.
/// </summary>
public class FontFamily : IEquatable<FontFamily>
{
    class FontFamilyComparer : IComparer<FontFamily>
    {
        public static readonly FontFamilyComparer Default = new FontFamilyComparer();

        public int Compare(FontFamily x, FontFamily y)
        {
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }
    }

    #region Fields

    /// <summary>
    /// Gets a <see cref="IComparer{FontFamily}"/> for comparing <see cref="FontFamily"/> instances.
    /// </summary>
    public static IComparer<FontFamily> Comparer => FontFamilyComparer.Default;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="name">The font family name.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference,
    /// empty string, or only contains whitespace.
    /// </exception>
    public FontFamily(string name)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        Name = name;
    }

    /// <summary>
    /// Gets the name of the font.
    /// </summary>
    /// <value>
    /// The font family name for installed fonts; otherwise, the 
    /// file name for a font loaded from the file system.
    /// </value>
    public string Name
    {
        get;
    }

    #region Methods

    /// <summary>
    /// Creates an <see cref="SKFont"/>. 
    /// </summary>
    /// <param name="fontSize">The font size in points.</param>
    /// <param name="attributes">The <see cref="FontAttributes"/>.
    /// <para>
    /// The default value is <see cref="FontAttributes.None"/>
    /// </para></param>
    /// <returns>A new instance of an <see cref="SKFont"/>.</returns>
    public SKFont CreateFont(float fontSize, FontAttributes attributes = FontAttributes.None)
    {
        return CreateFont(fontSize, attributes.ToFontStyle());
    }

    /// <summary>
    /// Creates an <see cref="SKFont"/> for the specified <paramref name="fontSize"/>
    /// </summary>
    /// <param name="fontSize">The the size of the font in points</param>
    /// <param name="style">The <see cref="SKFontStyle"/> to apply to the font.</param>
    /// <returns>The <see cref="SKFont"/>; otherwise, a null reference if the font could not be created.</returns>
    public SKFont CreateFont(float fontSize, SKFontStyle style)
    {
        SKTypeface typeface = GetTypeface(style);
        if (typeface is not null)
        {
            return typeface.ToFont(fontSize);
        }
        return null;
    }

    /// <summary>
    /// Gets the <see cref="SKTypeface"/> for the <see cref="Name"/>
    /// </summary>
    /// <param name="style">The optional <see cref="SKFontStyle"/> to apply.</param>
    /// <returns>An instance of a <see cref="SKTypeface"/>; otherwise, a null
    /// reference.
    /// </returns>
    /// <remarks>
    /// A null reference can be returned if the font family was not found 
    /// or could not be loaded.
    /// <para>
    /// NOTE: Fonts loaded from the local file system will ignore <see cref="SKFontStyle"/>.
    /// </para>
    /// <para>
    /// NOTE: The returned SKTypeface should be considered a global resource and not disposed.
    /// </para>
    /// </remarks>
    public virtual SKTypeface GetTypeface(SKFontStyle style)
    {
        return SKTypeface.FromFamilyName(Name, style);
    }

    /// <summary>
    /// Gets the string representation
    /// </summary>
    /// <returns>The <see cref="Name"/> property.</returns>
    public override string ToString()
    {
        return Name;
    }

    #endregion Methods

    #region Equality

    /// <summary>
    /// Determines if the specified <paramref name="obj"/> is equal to this struct.
    /// </summary>
    /// <param name="obj">The object to compare..</param>
    /// <returns>
    /// true <paramref name="obj"/> is a <see cref="FontFamily"/> and is equal to this struct;
    /// otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj is FontFamily fontFamily)
        {
            return Equals(fontFamily);
        }
        return false;
    }

    /// <summary>
    /// Determines if the specified <paramref name="other"/> is equal to this struct.
    /// </summary>
    /// <param name="other">The <see cref="FontFamily"/> to compare..</param>
    /// <returns>
    /// true if <paramref name="other"/> is equal to this struct; otherwise, false.
    /// </returns>
    public bool Equals(FontFamily other)
    {
        return other is not null && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    #endregion Equality
}
