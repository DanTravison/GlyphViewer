namespace GlyphViewer.Settings.Properties;

using GlyphViewer.Resources;

/// <summary>
/// Provides an <see cref="EnumProperty{FontAttributes}"/>.
/// </summary>
public sealed class FontAttributesProperty : EnumProperty<FontAttributes>
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="defaultValue">The default <see cref="FontAttributes"/> value.</param>
    public FontAttributesProperty(FontAttributes defaultValue = FontAttributes.None)
        : base(nameof(FontAttributes), defaultValue, Strings.FontAttributesLabel, Strings.FontAttributesDescription)
    {
    }
}
