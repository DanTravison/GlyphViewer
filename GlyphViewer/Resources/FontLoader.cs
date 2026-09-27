namespace GlyphViewer.Resources;

using GlyphViewer.Diagnostics;
using System.Reflection;
using System.Threading;

/// <summary>
/// Manages loading fonts.
/// </summary>
public static class FontLoader
{
    #region Fields

    static readonly Lock _lock = new();
    static readonly List<FontResource> _fonts = [];

    #endregion Fields

    #region Static Constructor

    static FontLoader()
    {
        Assembly assembly = typeof(FontResource).Assembly;
        List<FontResource> defaults =
        [
            new
            (
                assembly,
                resourceName:FontResource.DefaultFamilyResourceName,
                alias:FontResource.DefaultFontName,
                familyName:FontResource.DefaultFamilyName
            ),
            new
            (
                assembly,
                resourceName:FontResource.FluentUIResourceName,
                alias:FontResource.FluentUIName,
                familyName:FontResource.FluentUIFamilyName
            ),
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
            Trace.Line(TraceFlag.Font, typeof(FontLoader), nameof(Load), "Adding '{0}' from '{1}' to Maui", fontResource.Alias, fontResource.ManifestName);
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
    /// <param name="name">The name to resolve.</param>
    /// <returns>The <see cref="FontResource"/>for the <paramref name="name"/>; otherwise, a null reference.</returns>
    public static FontResource Resolve(string name)
    {
        lock (_lock)
        {
            for (int i = 0; i < _fonts.Count; i++)
            {
                FontResource resource = _fonts[i];
                if
                (
                    StringComparer.Ordinal.Compare(name, resource.Alias) == 0
                    ||
                    StringComparer.Ordinal.Compare(name, resource.FamilyName) == 0
                    ||
                    StringComparer.Ordinal.Compare(name, resource.ResourceName) == 0
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
