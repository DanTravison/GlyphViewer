namespace CodeCadence.Typography.OpenType;

/// <summary>
/// Provides glyph bounding box details scaled to a given font size.
/// </summary>
public readonly struct GlyphBounds
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="fontSize">The font size to use to calculate the bounds.</param>
    /// <param name="unitsPerEm">The font's units per em.</param>
    /// <param name="xMin">The fonts design time minimum X coordinate for all glyph bounding boxes.</param>
    /// <param name="xMax">The fonts design time maximum X coordinate for all glyph bounding boxes.</param>
    /// <param name="yMin">The fonts design time minimum Y coordinate for all glyph bounding boxes.</param>
    /// <param name="yMax">The fonts design time maximum Y coordinate for all glyph bounding boxes.</param>
    internal GlyphBounds(double fontSize, ushort unitsPerEm, short xMin, short xMax, short yMin, short yMax)
    {
        FontSize = fontSize;
        double scale = fontSize / unitsPerEm;
        Scale = scale;
        MinX = xMin * scale;
        MaxX = xMax * scale;
        MinY = yMin * scale;
        MaxY = yMax * scale;
        Width = (MaxX - MinX);
        Height = (MaxY - MinY);
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="fontSize">The font size to use to calculate the bounds.</param>
    /// <param name="table">The <see cref="HeadTable"/> containing the units to scale.</param>
    internal GlyphBounds(double fontSize, HeadTable table)
        : this(fontSize, table.UnitsPerEm, table.MinX, table.MaxX, table.MinY, table.MaxY)
    {
    }

    /// <summary>
    /// Gets the font size for the bounds.
    /// </summary>
    public double FontSize { get; }
    /// <summary>
    /// Gets the scaling factor used to convert the font design units.
    /// </summary>
    public double Scale { get; }
    /// <summary>
    /// Gets the minimium X coordinate for all glyph bounding boxes.
    /// </summary>
    public double MinX { get; }
    /// <summary>
    /// Gets the maximum X coordinate for all glyph bounding boxes.
    /// </summary>
    public double MaxX { get; }
    /// <summary>
    /// Gets the minimium Y coordinate for all glyph bounding boxes.
    /// </summary>
    public double MinY { get; }
    /// <summary>
    /// Gets the maximum Y coordinate for all glyph bounding boxes.
    /// </summary>
    public double MaxY { get; }
    /// <summary>
    /// Gets the maximum width of all glyph bounding boxes
    /// </summary>
    public double Height { get; }
    /// <summary>
    /// Gets the maximum height of all glyph bounding boxes
    /// </summary>
    public double Width { get; }
}
