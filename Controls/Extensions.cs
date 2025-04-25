namespace GlyphViewer.Controls;

internal static class Extensions
{
    /// <summary>
    /// Gets the value indicating if a <paramref name="color"/> is transparent.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to examine.</param>
    /// <returns>true if the color is transparent, otherwise, false.</returns>
    public static bool IsTransparent(this Color color)
    {
        return color is null || color.ToInt() == Colors.Transparent.ToInt();
    }
}
