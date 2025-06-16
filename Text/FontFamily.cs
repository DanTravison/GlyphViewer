namespace GlyphViewer.Text;

using GlyphViewer.Diagnostics;
using GlyphViewer.Resources;
using SkiaSharp;


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

    static readonly Dictionary<string, FontFamily> _fontFamilies = new(StringComparer.OrdinalIgnoreCase);
    static readonly Lock _lock = new();

    #endregion Fields

    #region Constants

    /// <summary>
    /// Gets the name for the default font.
    /// </summary>
    public const string DefaultFontName = FontResource.DefaultFontName;

    /// <summary>
    /// Gets the default <see cref="FontFamily"/>
    /// </summary>
    public static readonly FontFamily Default;

    /// <summary>
    /// Gets the <see cref="FontFamily"/> for the <see cref="FluentUI"/> font.
    /// </summary>
    public static readonly FontFamily FluentUI;

    #endregion Constants

    #region Constructors

    static FontFamily()
    {
        _fontFamilies = new(StringComparer.OrdinalIgnoreCase);

        Default = CreateInstance(FontResource.DefaultFontName);
        FluentUI = CreateInstance(FontResource.FluentUIName);
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="name">The font family name.</param>
    protected FontFamily(string name)
    {
        Name = name;
    }

    #endregion Constructors

    #region Properties

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

    #endregion Properties

    #region CreateFont

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
        Trace.Line(TraceFlag.Font, this, nameof(CreateFont), "Creating font '{0}'", Name);
        SKTypeface typeface = GetTypeface(style);
        if (typeface is not null)
        {
            return typeface.ToFont(fontSize);
        }
        Trace.Error(TraceFlag.Font, this, nameof(CreateFont), "Failed to create font for '{0}", Name);
        return null;
    }

    #endregion CreateFont

    #region GetTypeface

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
    /// NOTE: Fonts loaded from embedded resources or the local file system do not support <see cref="SKFontStyle"/>.
    /// </para>
    /// <para>
    /// NOTE: The returned SKTypeface should be considered a global resource and not disposed.
    /// </para>
    /// </remarks>
    public virtual SKTypeface GetTypeface(SKFontStyle style)
    {
        // Determine if the caller is requesting a a custom font (embedded resource or local file system font).
        FontResource resource = FontLoader.Resolve(Name);

        // First try to load the system installed font.  We prefer the system installed font since
        // it allows us to apply SKFontStyle.
        SKTypeface typeface = SKTypeface.FromFamilyName
        (
            resource is null ? Name : resource.FamilyName,
            style
        );
        // If the request is not for a custom font...
        if (resource is null)
        {
            if (typeface.FamilyName != Name)
            {
                // we got a fallback font.
                Trace.Warning(TraceFlag.Font, this, nameof(GetTypeface), "Font '{0}' not found. Using fallback font '{1}'", Name, typeface.FamilyName);
            }
        }
        // Determine if the returned font is the desired font.
        else if (typeface is null || resource.FamilyName != typeface.FamilyName)
        {
            // Either the desired font was not found or we got a fallback font.
            // Load the custom font.
            typeface = resource.GetTypeface();
        }

        // if all else, use the default typeface.
        typeface = typeface ?? SKTypeface.Default;

        Trace.Line(TraceFlag.Font, this, nameof(GetTypeface), "Resolved '{0}' to '{1}'", Name, typeface?.FamilyName);
        return typeface;
    }

    /// <summary>
    /// Gets the <see cref="SKTypeface"/> for the <see cref="Name"/>
    /// </summary>
    /// <param name="attributes">The <see cref="FontAttributes"/> to apply.
    /// <para>
    /// This parameter is ignored if the font is loaded from an embedded resource or the local file system.
    /// </para>
    /// </param>
    /// <returns>
    /// An <see cref="SKTypeface"/> for the font; otherwise,
    /// <see cref="SKTypeface.Default"/> if the font was not found.
    /// </returns>
    public SKTypeface GetTypeface(FontAttributes attributes)
    {
        return GetTypeface(attributes.ToFontStyle());
    }

    #endregion GetTypeface

    #region CreateInstance

    /// <summary>
    /// Creates a new instance of the <see cref="FontFamily"/>.
    /// </summary>
    /// <param name="name">The <see cref="FontFamily.Name"/>.</param>
    /// <returns>The <see cref="FontFamily"/> for the specified <paramref name="name"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference,
    /// -or-
    /// an empty string,
    /// -or-
    /// contains only whitespace characters.
    /// </exception>
    public static FontFamily CreateInstance(string name)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        lock (_lock)
        {
            if (!_fontFamilies.TryGetValue(name, out FontFamily fontFamily))
            {
                fontFamily = new FontFamily(name);
                _fontFamilies.Add(name, fontFamily);
            }
            return fontFamily;
        }
    }

    #endregion CreateInstance

    #region Equality

    /// <summary>
    /// Gets the string representation
    /// </summary>
    /// <returns>The <see cref="Name"/> property.</returns>
    public override string ToString()
    {
        return Name;
    }

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
