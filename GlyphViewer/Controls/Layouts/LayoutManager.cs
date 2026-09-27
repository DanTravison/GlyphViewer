namespace GlyphViewer.Controls.Layouts;

using Microsoft.Maui.Layouts;

/// <summary>
/// Provides an abstract <see cref="LayoutManager"/> base class.
/// </summary>
/// <typeparam name="C">The layout <see cref="Container"/>.</typeparam>
/// <typeparam name="I">The <see cref="LayoutInfo"/> used to layout items.</typeparam>
public abstract class LayoutManager<C, I> : LayoutManager
    where C : Layout<I>
    where I : LayoutInfo
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="layout">The <see cref="Container"/> <see cref="Layout{I}"/></param>
    protected LayoutManager(C layout)
        : base(layout)
    {
        Container = layout;
    }

    /// <summary>
    /// Gets the containing <see cref="Layout{I}"/>.
    /// </summary>
    protected C Container
    {
        get;
    }

    #region Overrides

    /// <summary>
    /// Measures the child items.
    /// </summary>
    /// <param name="widthConstraint">The width constraint.</param>
    /// <param name="heightConstraint">The height constraint.</param>
    /// <returns>The measured <see cref="Size"/> for the content.</returns>
    public override Size Measure(double widthConstraint, double heightConstraint)
    {
        Size size;
        if (Container.Children.Count > 0)
        {
            size = MeasureContent(widthConstraint, heightConstraint);
        }
        else
        {
            size = Size.Zero;
        }
        return size;
    }

    /// <summary>
    /// Arranges content in the specified <see cref="Rect"/>.
    /// </summary>
    /// <param name="bounds">The <see cref="Rect"/> to use to arrange the content.</param>
    /// <returns>The consumed <see cref="Size"/>.</returns>
    public override Size ArrangeChildren(Rect bounds)
    {
        return ArrangeContent(bounds);
    }

    #endregion Overrides

    #region Abstract Methods

    /// <summary>
    /// Implemented in the derived class to measure child items.
    /// </summary>
    /// <param name="widthConstraint">The width constraint.</param>
    /// <param name="heightConstraint">The height constraint.</param>
    /// <returns>The required <see cref="Size"/> for the content.</returns>
    protected abstract Size MeasureContent(double widthConstraint, double heightConstraint);

    /// <summary>
    /// Implemented in the derived class to arrange content.
    /// </summary>
    /// <param name="bounds">The <see cref="Rect"/> to use to arrange the content.</param>
    /// <returns>The consumed <see cref="Size"/>.</returns>
    protected abstract Size ArrangeContent(Rect bounds);

    #endregion Abstract Methods
}
