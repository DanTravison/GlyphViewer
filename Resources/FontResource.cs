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

    #region Default Font

    /// <summary>
    /// The namespace where the embedded font resources are located.
    /// </summary>
    public const string FontNamespace = "GlyphViewer.Resources.Fonts";

    /// <summary>
    /// Gets the Font Family name for the default font.
    /// </summary>
    public const string DefaultFamilyName = "Open Sans";

    /// <summary>
    /// Gets the resource name for the <see cref="DefaultFamilyName"/>.
    /// </summary>
    public const string DefaultFamilyResourceName = "opensans-regular.ttf";

    /// <summary>
    /// Defines the <see cref="FontResource"/> for the default font.
    /// </summary>
    public static readonly FontResource DefaultFont = new
    (
        typeof(FontResource).Assembly, 
        resourceName:DefaultFamilyResourceName, 
        familyName:DefaultFamilyName, 
        alias:"OpenSansRegular", 
        namespaceName:FontNamespace
    );

    #endregion Default Font

    #region Symbol Font

    public const string FluentUIResoureName = "fluentsystemicons-resizable.ttf";
    /// <summary>
    /// Gets the Font Family name for the default <see cref="FluentUI"/> font.
    /// </summary>   
    public const string FluentUIFamilyName = "FluentSystemIcons-Resizable";

    /// <summary>
    /// Defines the <see cref="FontResource"/> for the default font.
    /// </summary>
    public static readonly FontResource FluentUIFont = new
    (
        typeof(FontResource).Assembly, 
        resourceName:FluentUIResoureName, 
        familyName:FluentUIFamilyName, 
        alias:nameof(FluentUI), 
        namespaceName:FontNamespace
    );

    #endregion Symbol Font

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
    /// <param name="resourceName">The embedded resource's file name.</param>
    /// <param name="familyName">The <see cref="FamilyName"/> of the font.</param>
    /// <param name="alias">The optional alias to use for the font.
    /// <para>
    /// The default value is <see cref="FamilyName"/>.
    /// </para></param>
    /// <param name="namespaceName">The namespace where the font is declared.
    /// <para>
    /// The default value is <see cref="FontNamespace"/>.
    /// </para>
    /// </param>
    /// <remarks>This constructor is for fonts marked as EmbeddedResource.</remarks>
    public FontResource(Assembly assembly, string resourceName, string familyName, string alias = null, string namespaceName = FontNamespace)
    {
        Assembly = assembly;
        ResourceName = resourceName;
        FamilyName = familyName;
        Alias = alias ?? FamilyName;
        string[] parts = resourceName.Split('.');
        FamilyName = parts.Length > 0 ? parts[0] : resourceName;
        ManifestName = $"{namespaceName}.{resourceName}";
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
    public string FamilyName
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
                    Trace.Line(TraceFlag.Font, this, nameof(GetTypeface), $"Loading font '{FamilyName}' from embedded resource '{ManifestName}'");
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

     #endregion Properties
}
