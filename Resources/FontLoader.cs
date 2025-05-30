#if (false)
[assembly: ExportFont(FontResource.DefaultFamilyResourceName, Alias = FontResource.DefaultFamily)]
[assembly: ExportFont(FontResource.DefaultSemiBoldResourceName, Alias = FontResource.DefaultSemiBoldFamily)]
[assembly: ExportFont(FontResource.DefaultSymbolResourceName, Alias = FontResource.DefaultSymbolFamily)]
#endif

namespace GlyphViewer.Resources;

using System.Reflection;

/// <summary>
/// Manages loading fonts.
/// </summary>
public static class FontLoader
{
    #region Fields

    static readonly object _lock = new();
    static readonly List<FontResource> _fonts = [];

    #endregion Fields

    #region Static Constructor

    static FontLoader()
    {
        Assembly assembly = typeof(FontResource).Assembly;
        List<FontResource> defaults =
        [
            new (assembly, FontResource.DefaultFamilyResourceName,    FontResource.DefaultFamily),
            new (assembly, FontResource.DefaultSemiBoldResourceName,  FontResource.DefaultSemiBoldFamily),
            new (assembly, FontResource.DefaultSymbolResourceName,    FontResource.FluentUIFamily),
        ];
        Defaults = defaults;
        DefaultFont = defaults[0];
    }

    #endregion Static Constructor

    #region Properties

    /// <summary>
    /// Gets the <see cref="FontResource"/> for the default font.
    /// </summary>
    public static FontResource DefaultFont
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="IReadOnlyList{T}"/> of the default fonts to load.
    /// </summary>
    public static IReadOnlyList<FontResource> Defaults
    {
        get;
    }

    /// <summary>
    /// Gets <see cref="IReadOnlyList{FontResource}"/> for fonts embedded in the application.
    /// </summary>
    public static IReadOnlyList<FontResource> EmbeddedFonts
    {
        get => _fonts;
    }

    #endregion Properties

    #region Load

    /// <summary>
    /// Loads the <see cref="FontResource"/> instances into a <see cref="IFontCollection"/>.
    /// </summary>
    /// <param name="fonts">The <see cref="IFontCollection"/> to populate.</param>
    /// <param name="fontResource">An <see cref="FontResource"/> to load.</param>
    public static void Load(IFontCollection fonts, FontResource fontResource)
    {
        lock (_lock)
        {
            fonts = fonts.AddEmbeddedResourceFont(fontResource.Assembly, fontResource.ResourceName, fontResource.Alias);
            _fonts.Add(fontResource);
        }
    }

    /// <summary>
    /// Loads the <see cref="FontResource"/> instances into a <see cref="IFontCollection"/>.
    /// </summary>
    /// <param name="fonts">The <see cref="IFontCollection"/> to populate.</param>
    /// <param name="fontResources">An <see cref="IEnumerable{FontResource}"/> for iterating the fonts to load.</param>
    public static void Load(IFontCollection fonts, IEnumerable<FontResource> fontResources)
    {
        lock (_lock)
        {
            foreach (FontResource desc in fontResources)
            {
                Load(fonts, desc);
            }
        }
    }

    #endregion Load

    #region Resolve

    /// <summary>
    /// Resolves a <see cref="FontResource"/> by its alias.
    /// </summary>
    /// <param name="alias">The alias to resolve.</param>
    /// <returns>The <see cref="FontResource"/>for the <paramref name="alias"/>; otherwise, a null reference.</returns>
    public static FontResource Resolve(string alias)
    {
        lock (_lock)
        {
            for (int i = 0; i < _fonts.Count; i++)
            {
                FontResource resource = _fonts[i];
                if
                (
                    StringComparer.Ordinal.Compare(alias, resource.Alias) == 0
                    ||
                    StringComparer.Ordinal.Compare(alias, resource.Name) == 0
                      ||
                    StringComparer.Ordinal.Compare(alias, resource.ResourceName) == 0
              )
                {
                    return resource;
                }
            }
            return null;
        }
    }

    #endregion Resolve
}
