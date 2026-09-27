namespace GlyphViewer.Controls;
/// <summary>
/// Defines the positioning of an item in a <see cref="Toolbar"/>.
/// </summary>
public sealed class ToolbarLayoutInfo : LayoutInfo
{
    ToolbarAlignment _alignment;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="view">The <see cref="IView"/> to arrange.</param>
    /// <param name="alignment">The <see cref="Alignment"/> of the toolbar item.</param>
    public ToolbarLayoutInfo(IView view, ToolbarAlignment alignment) 
        : base(view)
    {
        _alignment = alignment;
    }

    /// <summary>
    /// Determines the alignment of the toolbar item in the toolbar.
    /// </summary>
    public ToolbarAlignment Alignment
    {
        get => _alignment;
        set
        {
            if (_alignment != value)
            {
                _alignment = value;
                NeedsMeasure = true;
            }
        }
    }
}
