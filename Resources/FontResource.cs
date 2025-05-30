namespace GlyphViewer.Resources;

using GlyphViewer.Diagnostics;
using SkiaSharp;
using System.Reflection;

/// <summary>
/// Provides a font description for loading a font stored as an embedded resource.
/// </summary>
public sealed class FontResource
{
    #region Constants

    /// <summary>
    /// The namespace where the embedded font resources are located.
    /// </summary>
    public const string FontNamespace = "GlyphViewer.Resources.Fonts";

    /// <summary>
    /// Gets the Font Family name for the default font.
    /// </summary>
    /// <remarks>
    /// This value mirrors the DefaultFontFamily StaticResource in the XAML resources.
    /// </remarks>
    public const string DefaultFamily = "OpenSansRegular";

    /// <summary>
    /// Gets the resource name for the <see cref="DefaultFamily"/>.
    /// </summary>
    public const string DefaultFamilyResourceName = "opensans-regular.ttf";

    /// <summary>
    /// Gets the Font Family name for the default semi-bold font.
    /// </summary>
    public const string DefaultSemiBoldFamily = "OpenSansSemibold";

    /// <summary>
    /// Gets the resource name for the <see cref="DefaultSemiBoldFamily"/>.
    /// </summary>
    public const string DefaultSemiBoldResourceName = "opensans-semibold.ttf";

    /// <summary>
    /// Gets the Font Family name for the default <see cref="FluentUI"/> font.
    /// </summary>   
    public const string FluentUIFamily = nameof(FluentUI);

    /// <summary>
    /// Gets the Font Family name for the default symbol font.
    /// </summary>
    /// <remarks>
    /// This value mirrors the DefaultSymbolFontFamily StaticResource in the XAML resources.
    /// </remarks>
    public const string DefaultSymbolFamily = nameof(FluentUI);

    /// <summary>
    /// Gets the resource name for the <see cref="DefaultSymbolFamily"/>.
    /// </summary>
    public const string DefaultSymbolResourceName = "fluentsystemicons-resizable.ttf";

    #endregion Constants

    #region Fields

    /// <summary>
    /// Gets the default fonts to load.
    /// </summary>
    readonly object _lock = new();
    SKTypeface _typeface;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> containing the embedded font resource.</param>
    /// <param name="fileName">The embedded resource's file name.</param>
    /// <param name="alias">The alias to use for the font. </param>
    /// <param name="namespaceName">The namespace where the font is declared.
    /// <para>
    /// The default value is <see cref="FontNamespace"/>.
    /// </para>
    /// </param>
    /// <remarks>This constructor is for fonts marked as EmbeddedResource.</remarks>
    public FontResource(Assembly assembly, string fileName, string alias, string namespaceName = FontNamespace)
    {
        Assembly = assembly;
        ResourceName = fileName;
        Alias = alias;
        string[] parts = fileName.Split('.');
        Name = parts.Length > 0 ? parts[0] : fileName;
        ManifestName = $"{namespaceName}.{fileName}";
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the resource name of the embedded resource font.
    /// </summary>
    public string ResourceName
    {
        get;
    }

    /// <summary>
    /// Gets the alias for the font.
    /// </summary>
    public string Alias
    {
        get;
    }

    /// <summary>
    /// Gets the font name.
    /// </summary>
    public string Name
    {
        get;
    }

    /// <summary>
    /// Gets the Assembly containing the embedded font.
    /// </summary>
    public Assembly Assembly
    {
        get;
    }


    /// <summary>
    /// Gets the full name of the embedded resource.
    /// </summary>
    /// <value>
    /// The full name of the embedded resource.
    /// </value> 
    public string ManifestName
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="SKTypeface"/> for the font.
    /// </summary>
    public SKTypeface GetTypeface()
    {
        lock (_lock)
        {
            if (_typeface is null)
            {
                try
                {
                    Trace.Line(TraceFlag.Font, this, nameof(GetTypeface), $"Loading font '{Name}' from embedded resource '{ManifestName}'");
                    _typeface = SKTypeface.FromStream(this.Assembly.GetManifestResourceStream(ManifestName));
                }
                catch (System.Exception ex)
                {
                    Trace.Exception(this, nameof(GetTypeface), ex, $"Failed to load font '{0}'. Using SKTypeface.Default", ManifestName);
                    _typeface = SKTypeface.Default;
                }
            }
            return _typeface;
        }
    }

    /// <summary>
    /// Gets the <see cref="FontResource"/> for the <see cref="DefaultFamily"/>.
    /// </summary>
    public static FontResource Default
    {
        get
        {
            return FontLoader.DefaultFont;
        }
    }

    #endregion Properties
}
