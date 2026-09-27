namespace GlyphViewer.Controls;

/// <summary>
/// Defines the layout region for a toolbar item.
/// </summary>
public enum ToolbarAlignment
{
    /// <summary>
    /// The item is aligned at the start of the <see cref="Toolbar"/>.
    /// </summary>
    Start = 1,

    /// <summary>
    /// The item is aligned at the center of the <see cref="Toolbar"/>.
    /// </summary>
    Center = 2,

    /// <summary>
    /// The item is aligned at the center of the <see cref="Toolbar"/> and fills the available space.
    /// </summary>
    CenterFill = 3,

    /// <summary>
    /// The item is aligned at the end of the <see cref="Toolbar"/>.
    /// </summary>
    End = 4
}
