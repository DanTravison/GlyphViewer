namespace GlyphViewer.Controls;

using GlyphViewer.Controls.Layouts;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides a layout manager for arranging toolbar items in a <see cref="Toolbar"/> control.
/// </summary>
internal sealed class ToolbarLayoutManager : LayoutManager<Toolbar, ToolbarLayoutInfo>
{
    readonly Toolbar _layout;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolbarLayoutManager"/> class.
    /// </summary>
    /// <param name="layout">The containing <see cref="Toolbar"/>.</param>
    public ToolbarLayoutManager(Toolbar layout) 
        : base(layout)
    {
        _layout = layout;
    }

    /// <summary>
    /// Measures the size of the toolbar content based on the available constraints.
    /// </summary>
    /// <param name="widthConstraint">The width constriant.</param>
    /// <param name="heightConstraint">The height constraint.</param>
    /// <returns>The size needed to arrange the child items.</returns>
    protected override Size MeasureContent(double widthConstraint, double heightConstraint)
    {
        Thickness padding = Container.Padding;
        bool isHorizontal = Container.Orientation == StackOrientation.Horizontal;

        double availableWidth = widthConstraint;
        double availableHeight = heightConstraint;

        double measuredWidth = 0;
        double measuredHeight = 0;

        int count = 0;
        foreach (IView view in Container.Children)
        {
            if (view is null || view.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            ToolbarLayoutInfo info = Container[view];
            if (info is null)
            {
                continue;
            }
            if (info.NeedsMeasure)
            {
                double height = availableHeight;
                double width = availableWidth;
               info.Measure(width, height);
            }
            if (isHorizontal)
            {
                if (info.Height > measuredHeight)
                {
                    measuredHeight = info.Height;
                }
                measuredWidth += info.Width;
            }
            else
            {
                measuredWidth = Math.Max(measuredWidth, info.Width);
                measuredHeight += info.Height;
            }
            count++;
        }

        count--;
        double spacing = count * Container.ItemSpacing;
        if (isHorizontal)
        {
            measuredWidth += spacing;
        }
        else
        {
            measuredHeight += spacing;
        }

        return new Size
        (
            measuredWidth + padding.HorizontalThickness,
            measuredHeight + padding.VerticalThickness
        );
    }

    /// <summary>
    /// Arranges the toolbar content within the specified bounds.
    /// </summary>
    /// <param name="bounds">The <see cref="Rect"/> defining the bounds.</param>
    /// <returns>The size used to arrange the content.</returns>
    protected override Size ArrangeContent(Rect bounds)
    {
        Thickness padding = Container.Padding;
        bool isHorizontal = Container.Orientation == StackOrientation.Horizontal;

        Rect layoutRect = new Rect
        (
            padding.Left, padding.Top,
            bounds.Width - padding.HorizontalThickness,
            bounds.Height - padding.VerticalThickness
        );

        List<ToolbarLayoutInfo> start = [];
        List<ToolbarLayoutInfo> end = [];
        List<ToolbarLayoutInfo> center = [];

        double startDimension = 0.0;
        double centerDimension = 0.0;
        double endDimension = 0.0;
        double spacing = Container.ItemSpacing;
        int fillCount = 0;

        // Calculate the size of each layout region (Start, Center, End)
        foreach (IView view in Container.Children)
        {
            if (view is null || view.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            if (Container[view] is not ToolbarLayoutInfo info)
            {
                continue;
            }
            switch (info.Alignment)
            {
                case ToolbarAlignment.Start:
                    start.Add(info);
                    startDimension += ItemDimension(info, spacing, isHorizontal);
                    break;

                case ToolbarAlignment.Center:
                case ToolbarAlignment.CenterFill:
                    center.Add(info);
                    centerDimension += ItemDimension(info, spacing, isHorizontal);
                    if (info.Alignment == ToolbarAlignment.CenterFill)
                    {
                        fillCount++;
                    }
                    break;

                case ToolbarAlignment.End:
                    end.Add(info);
                    endDimension += ItemDimension(info, spacing, isHorizontal);
                    break;
            }
        }

        // Trim the spacing from last item
        // in each group.
        startDimension -= spacing;
        centerDimension -= spacing;
        endDimension -= spacing;

        //
        // Arrange the Start items.
        //
        double x = layoutRect.Left;
        double y = layoutRect.Top;
        double width = isHorizontal ? startDimension : layoutRect.Width;
        double height = isHorizontal ? layoutRect.Height : startDimension;

        ArrangeItems(start, isHorizontal, x, y, width, height, spacing, layoutRect);

        //
        // Arrange the End items.
        //
        if (isHorizontal)
        {
            x = layoutRect.Right - endDimension;
            width = endDimension;
        }
        else
        {
            y = layoutRect.Bottom - endDimension;
            height = endDimension;
        }
        ArrangeItems(end, isHorizontal, x, y, width, height, spacing, layoutRect);

        if (isHorizontal)
        {
            width = layoutRect.Width - (startDimension + endDimension);
        }
        else
        {
            height = layoutRect.Height - (startDimension - endDimension);
        }

        if (fillCount > 0)
        {
            if (isHorizontal)
            {
                width = layoutRect.Width - (startDimension + endDimension);
                x = layoutRect.Left + startDimension;
            }
            else
            {
                height = layoutRect.Height - (startDimension + endDimension);
                y = layoutRect.Top + startDimension;
            }

            // If there are fill items
            ArrangeCenterItems(center, isHorizontal, x, y, width, height, spacing, layoutRect);
            return bounds.Size;
        }
        else
        {
            if (isHorizontal)
            {
                x = (layoutRect.Width - centerDimension) / 2;
                width = centerDimension;
            }
            else
            {
                y = (layoutRect.Height - centerDimension) / 2;
                height = centerDimension;
            }
            ArrangeItems(center, isHorizontal, x, y, width, height, spacing, layoutRect);
        }
        return bounds.Size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static double ItemDimension(ToolbarLayoutInfo info, double spacing, bool isHorizontal)
    {
        return spacing + (isHorizontal ? info.Width : info.Height);
    }

    /// <summary>
    /// Arrange the toolbar items in the center when one or more items are marked<see cref="ToolbarAlignment.CenterFill"/>
    /// </summary>
    /// <param name="items">The <see cref="ToolbarLayoutInfo"/> for the region.</param>
    /// <param name="isHorizontal">true if the toolbar is horizontal; otherwise, false.</param>
    /// <param name="x">The X coordinate of the region.</param>
    /// <param name="y">The Y coordinate of the region.</param>
    /// <param name="width">The width of the region.</param>
    /// <param name="height">The height of the region.</param>
    /// <param name="spacing">The spacing between items.</param>
    /// <param name="bounds">The overall bounds of the toolbar.</param>
    static void ArrangeCenterItems
    (
        List<ToolbarLayoutInfo> items, bool isHorizontal,
        double x, double y,
        double width, double height,
        double spacing,
        Rect bounds
    )
    {
        int fillCount = 0;
        double fillDimension = isHorizontal ? width : height;

        // calculate how much space is available for fill items
        for (int i = 0; i < items.Count; i++)
        {
            ToolbarLayoutInfo info = items[i];
            if (info.Alignment == ToolbarAlignment.CenterFill)
            {
                fillCount++;
            }
            else
            { 
                fillDimension -= isHorizontal ? info.Width : info.Height;
                if (i < items.Count - 1)
                {
                    fillDimension -= spacing;
                }
            }   
        }

        // Distribute the remaining space evenly for fill items
        fillDimension = fillDimension / fillCount;

        // Arrange the items.
        for (int i = 0; i < items.Count; i++)
        {
            ToolbarLayoutInfo info = items[i];
            if (info.Alignment == ToolbarAlignment.CenterFill)
            {
                double itemWidth = isHorizontal ? fillDimension : width;
                double itemHeight = isHorizontal ? height : fillDimension;
                ArrangeItem(info, isHorizontal, x, y, itemWidth, itemHeight, bounds);
            }
            else
            {
                ArrangeItem(info, isHorizontal, x, y, width, height, bounds);
            }
            if (i == items.Count - 1)
            {
                // Don't add spacing after the last item
                break;
            }
            if (isHorizontal)
            {
                x = info.Bounds.Right + spacing;
                width -= info.Bounds.Width + spacing;
            }
            else
            {
                y = info.Bounds.Bottom + spacing;
                height -= info.Bounds.Height + spacing;
            }
        }
    }

    /// <summary>
    /// Arrange the toolbar items.
    /// </summary>
    /// <param name="items">The <see cref="ToolbarLayoutInfo"/> for the region.</param>
    /// <param name="isHorizontal">true if the toolbar is horizontal; otherwise, false.</param>
    /// <param name="x">The X coordinate of the region.</param>
    /// <param name="y">The Y coordinate of the region.</param>
    /// <param name="width">The width of the region.</param>
    /// <param name="height">The height of the region.</param>
    /// <param name="spacing">The spacing between items.</param>
    /// <param name="bounds">The overall bounds of the toolbar.</param>
    static void ArrangeItems
    (
        List<ToolbarLayoutInfo> items, bool isHorizontal,
        double x, double y,
        double width, double height,
        double spacing,
        Rect bounds
    )
    {
        for (int i = 0; i < items.Count; i++)
        {
            ToolbarLayoutInfo info = items[i];
            ArrangeItem(info, isHorizontal, x, y, width, height, bounds);

            if (i == items.Count - 1)
            {
                // Don't add spacing after the last item
                break;
            }
            if (isHorizontal)
            {
                x = info.Bounds.Right + spacing;
                width -= info.Bounds.Width + spacing;
            }
            else
            {
                y = info.Bounds.Bottom + spacing;
                height -= info.Bounds.Height + spacing;
            }
        }
    }

    /// <summary>
    /// Calculate the <see cref="LayoutInfo.Bounds"/> of a toolbar item.
    /// </summary>
    /// <param name="info">The <see cref="ToolbarLayoutInfo"/> to update.</param>
    /// <param name="isHorizontal">true if the toolbar is horizontal; otherwise, false.</param>
    /// <param name="x">The X coordinate of the item.</param>
    /// <param name="y">The Y coordinate of the item.</param>
    /// <param name="widthConstraint">The width of the layout region.</param>
    /// <param name="heightConstraint">The height of the layout region.</param>
    /// <returns>A <see cref="Rect"/> defining the bounds of the item.</returns>
    static void ArrangeItem
    (
        ToolbarLayoutInfo info,
        bool isHorizontal,
        double x, double y,
        double widthConstraint, double heightConstraint,
        Rect layoutBounds
    )
    {
        bool isFill = info.Alignment == ToolbarAlignment.CenterFill;

        double actualWidth;
        double actualHeight;

        if (isHorizontal)
        {
            actualHeight = Math.Min(info.Height, heightConstraint);
            y += (heightConstraint - actualHeight) / 2;
            actualWidth = isFill ? widthConstraint : info.Width; 
        }
        else
        {
            actualWidth = Math.Min(info.Width, widthConstraint);
            x += (widthConstraint - actualWidth) / 2;
            actualHeight = isFill ? heightConstraint : info.Height;
        }

        info.Arrange(new Rect(x, y, actualWidth, actualHeight));
    }
}
