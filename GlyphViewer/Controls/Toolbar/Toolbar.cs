namespace GlyphViewer.Controls;

using GlyphViewer.Controls.Layouts;
using Microsoft.Maui;
using Microsoft.Maui.Layouts;
using System.ComponentModel;

/// <summary>
/// Provides a layout manager for a vertical or horizontal tool bar.
/// </summary>
public sealed class Toolbar : Layout<ToolbarLayoutInfo>
{
    /// <summary>
    /// Creates an instance of <see cref="ToolbarLayoutInfo"/>.
    /// </summary>
    /// <param name="view">The <see cref="IView"/> to arrange.</param>
    /// <returns>A new instance of a <see cref="ToolbarLayoutInfo"/>.</returns>
    protected override ToolbarLayoutInfo CreateItemInfo(IView view)
    {
        if (view is BindableObject bindable)
        {
            ToolbarAlignment alignment = GetAlignment(bindable);
            return new ToolbarLayoutInfo(view, alignment);
        }
        return null;
    }

    /// <summary>
    /// Creates a layout manager for arranging the toolbar items.
    /// </summary>
    /// <returns>A new instance of a <see cref="ToolbarLayoutManager"/>.</returns>
    protected override ILayoutManager CreateLayoutManager()
    {
        return new ToolbarLayoutManager(this);
    }

    #region Alignment Attached Property

    /// <summary>
    /// Defines an attached property for arranging a child <see cref="IView"/> in a <see cref="Toolbar"/>.
    /// </summary>
    [TypeConverter(typeof(ToolbarAlignmentTypeConverter))]
    public static readonly BindableProperty AlignmentProperty = BindableProperty.CreateAttached
    (
        nameof(ToolbarAlignment),
        typeof(ToolbarAlignment),
        typeof(Toolbar),
        ToolbarAlignment.Start,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is View view && view.Parent is Toolbar toolbar)
            {
                toolbar.OnAlignmentChanged(view, (ToolbarAlignment)newValue);
            }
        }
    );

    /// <summary>
    /// Gets the <see cref="ToolbarAlignment"/> o use to arrange the item in a <see cref="Toolbar"/>.
    /// </summary>
    /// <param name="bindable">A visual element.</param>
    /// <returns>The <see cref="ToolbarAlignment"/>.</returns>
    [TypeConverter(typeof(ToolbarAlignmentTypeConverter))]
    public static ToolbarAlignment GetAlignment(BindableObject bindable)
    {
        return (ToolbarAlignment)bindable.GetValue(AlignmentProperty);
    }

    /// <summary>
    /// Sets the layout bounds of a child element that will be arrange it..
    /// </summary>
    /// <param name="bindable">The view to delimit by bounds.</param>
    /// <param name="alignment">An <see cref="ToolbarAlignment"/> to use to arrange the item in a <see cref="Toolbar"/>.</param>
    public static void SetAlignment(BindableObject bindable, ToolbarAlignment alignment)
    {
        bindable.SetValue(AlignmentProperty, alignment);
    }

    void OnAlignmentChanged(View child, ToolbarAlignment value)
    {
        if (this[child] is ToolbarLayoutInfo info)
        {
            info.Alignment = value;
            if (info.NeedsMeasure)
            {
                InvalidateMeasure();
            }
        }
    }

    #endregion Alignment Attached Property

    #region ItemSpacing

    /// <summary>
    /// Defines the default <see cref="ItemSpacing"/>.
    /// </summary>
    public const double DefaultItemSpacing = 1.0;

    /// <summary>
    /// Gets or sets the item spacing for toolbar items.
    /// </summary>
    /// <value>
    /// The zero-based item spacing.
    /// <para>
    /// The default value is <see cref="DefaultItemSpacing"/>.
    /// </para>
    /// </value>
    public double ItemSpacing
    {
        get => (double)GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="ItemSpacing"/>.
    /// </summary>
    public static readonly BindableProperty ItemSpacingProperty = BindableProperty.Create
    (
        nameof(ItemSpacing),
        typeof(double),
        typeof(Toolbar),
        DefaultItemSpacing,
        validateValue: (bindableObject, newValue) =>
        {
            return ((double)newValue >= 0);
        },
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Toolbar toolbar)
            {
                toolbar.InvalidateMeasure();
            }
        }
    );

    #endregion ItemSpacing

    #region Orientation

    /// <summary>
    /// Gets or sets the <see cref="StackOrientation"/> of the toolbar.
    /// </summary>
    public StackOrientation Orientation
    {
        get => (StackOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Orientation"/>.
    /// </summary>
    public static readonly BindableProperty OrientationProperty = BindableProperty.Create
    (
        nameof(Orientation),
        typeof(StackOrientation),
        typeof(Toolbar),
        StackOrientation.Horizontal,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Toolbar toolbar)
            {
                toolbar.InvalidateMeasure();
            }
        }
    );

    #endregion Orientation
}
