
namespace GlyphViewer.Controls;

/// <summary>
/// Provides the results of a <see cref="SliderDrawable.HitTest"/>.
/// </summary>
internal enum SliderPart
{
    /// <summary>
    /// The point is not in a selectable area.
    /// </summary>
    None,
    /// <summary>
    /// The point is left/below than the current value.
    /// </summary>
    Start,
    /// <summary>
    ///  The point is right/above than the current value.
    /// </summary>
    End,
    /// <summary>
    ///  The point is on the thumb.
    /// </summary>
    Thumb
}
