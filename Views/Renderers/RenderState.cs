namespace GlyphViewer.Views.Renderers;

/// <summary>
/// Specifies the type of update that should be performed by a renderer.
/// </summary>
[Flags]
enum RenderState
{
    None = 0,
    /// <summary>
    /// The renderer should be drawn.
    /// </summary>
    Draw = 1,
    /// <summary>
    /// The renderer should be laid out.
    /// </summary>
    Layout = 1 << 1,
    /// <summary>
    /// The renderer should be arranged.
    /// </summary>
    Arrange = 1 << 2,
    /// <summary>
    /// The renderer should be measured.
    /// </summary>
    Measure = 1 << 3,
}
