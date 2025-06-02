namespace GlyphViewer.Resources;

using GlyphViewer.Diagnostics;
using SkiaSharp;
using System.Reflection;
using System.Threading;

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
    /// Gets the name/alias for the default font.
    /// </summary>
    public const string DefaultFontName = "OpenSansRegular";

    /// <summary>
    /// Gets the family name for the default font.
    /// </summary>
    public const string DefaultFamilyName = "Open Sans";

    /// <summary>
    /// Gets the resource name for the default font.
    /// </summary>
    public const string DefaultFamilyResourceName = "opensans-regular.ttf";

    /// <summary>
    /// Gets the name/alias name for the fluent ui font.
    /// </summary>
    public const string FluentUIName = nameof(FluentUI);

    /// <summary>
    /// Gets the family name for the fluent ui font.
    /// </summary>
    public const string FluentUIFamilyName= "FluentSystemIcons-Resizable";

    /// <summary>
    /// Gets the resource name for the fluent ui font.
    /// </summary>
    public const string FluentUIResourceName = "fluentsystemicons-resizable.ttf";

    #endregion Constants

    #region Fields

    /// <summary>
    /// Gets the default fonts to load.
    /// </summary>
    readonly Lock _lock = new();
    SKTypeface _typeface;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> containing the embedded font resource.</param>
    /// <param name="resourceName">The embedded resource's file name.</param>
    /// <param name="alias">The <see cref="Alias"/> to use for the font. </param>
    /// <param name="familyName">The expected <see cref="FamilyName"/>.</param>
    /// <param name="namespaceName">The namespace where the font is declared.
    /// <para>
    /// The default value is <see cref="FontNamespace"/>.
    /// </para>
    /// </param>
    /// <remarks>This constructor is for fonts marked as EmbeddedResource.</remarks>
    public FontResource(Assembly assembly, string resourceName, string alias, string familyName, string namespaceName = FontNamespace)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(resourceName, nameof(resourceName));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(alias, nameof(alias));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(familyName, nameof(familyName));

        Assembly = assembly;
        ResourceName = resourceName;
        Alias = alias;
        FamilyName = familyName;
        ManifestName = $"{namespaceName}.{resourceName}";
    }

    #endregion Constructors

    #region Properties

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
    /// Gets the resource name of the embedded resource font.
    /// </summary>
    public string ResourceName
    {
        get;
    }

    /// <summary>
    /// Gets the namespace-qualified <see cref="ResourceName"/> of the embedded resource.
    /// </summary>
    /// <value>
    /// The  namespace-qualified <see cref="ResourceName"/> of the embedded resource.
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
            // NOTE: Since SKTypeface.FromStream() does not support SKFontStyle, 
            // we can cached the SKTypeface instance.
            return _typeface;
        }
    }

    #endregion Properties
}
