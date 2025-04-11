namespace GlyphViewer.Views.Renderers;

using SkiaSharp;

/// <summary>
/// Provides a base class for a row in the <see cref="GlyphsView"/>.
/// </summary>
abstract class GlyphRowBase
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="context">The <see cref="DrawContext"/> to use to draw the row.</param>
    protected GlyphRowBase(DrawContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Gets the <see cref="DrawContext"/> to use to draw the row.
    /// </summary>
    protected DrawContext Context
    {
        get;
    }

    /// <summary>
    /// Gets the bounds for the row.
    /// </summary>
    public SKRect Bounds
    {
        get;
        protected set;
    }
}
