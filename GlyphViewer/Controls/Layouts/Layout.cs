namespace GlyphViewer.Controls.Layouts;

/// <summary>
/// Provides an abstract base <see cref="Layout"/> class.
/// </summary>
/// <typeparam name="I">The type of <see cref="LayoutInfo"/> to use to layout child items.</typeparam>
public abstract class Layout<I> : Layout
    where I : LayoutInfo
{
    #region Fields

    readonly Dictionary<IView, I> _layoutInfo = new(ReferenceEqualityComparer.Instance);
    Size _measuredSize = Size.Zero;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    protected Layout()
    { }

    #region Properties

    /// <summary>
    /// Gets the <typeparamref name="I"/> <see cref="Info"/> for the specified <paramref name="view"/> view.
    /// </summary>
    /// <param name="view">The <see cref="IView"/> of information to retrieve.</param>
    /// <returns>The <typeparamref name="I"/> <see cref="Info"/>, if found; otherwise, a null reference.</returns>
    public I this[IView view]
    {
        get
        {
            _layoutInfo.TryGetValue(view, out I info);
            return info;
        }
    }

    /// <summary>
    /// Enumerates the <typeparamref name="I"/> <see cref="LayoutInfo"/> for each view.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/>.</returns>
    protected IEnumerable<I> Info
    {
        get => _layoutInfo.Values;
    }

    #endregion Properties

    #region Add

    void AddView(IView view)
    {
        I info = CreateItemInfo(view);
        if (info is not null)
        {
            _layoutInfo.Add(view, info);
            OnItemAdded(info);
        }
    }

    /// <summary>
    /// Creates an instance of <typeparamref name="I"/> <see cref="LayoutInfo"/>.
    /// </summary>
    /// <param name="view">The <see cref="IView"/> to arrange.</param>
    /// <returns>A new instance of a <typeparamref name="I"/> <see cref="LayoutInfo"/>.</returns>
    protected abstract I CreateItemInfo(IView view);

    /// <summary>
    /// Overridden in the derived class when a <see cref="IView"/> is added.
    /// </summary>
    /// <param name="info">The <typeparamref name="I"/> for the added view.</param>
    protected virtual void OnItemAdded(I info)
    {
    }

    #endregion Add

    #region Remove

    void RemoveView(IView view)
    {
        if (_layoutInfo.TryGetValue(view, out I info))
        {
            _layoutInfo.Remove(view);
            OnItemRemoved(info);
            info.Removed();
        }
    }

    /// <summary>
    /// Overridden in the derived class when a <see cref="IView"/> is removed.
    /// </summary>
    /// <param name="info">The <typeparamref name="I"/> for the removed view.</param>
    protected virtual void OnItemRemoved(I info)
    {
    }

    #endregion Remove

    #region Find

    /// <summary>
    /// Find the <typeparamref name="I"/> <see cref="LayoutInfo"/> for the item 
    /// at the specified <paramref name="point"/>.
    /// </summary>
    /// <param name="point">The <see cref="Point"/> to query.</param>
    /// <returns>
    /// The <typeparamref name="I"/> <see cref="LayoutInfo"/> for the item 
    /// at the specified <paramref name="point"/>; otherwise, a null reference.
    /// </returns>
    public I Find(Point point)
    {
        foreach (I info in _layoutInfo.Values)
        {
            if (info.Bounds.Contains(point))
            {
                return info;
            }
        }
        return null;
    }

    /// <summary>
    /// Find the child <see cref="View"/> at the specified <paramref name="point"/>.
    /// </summary>
    /// <param name="point">The <see cref="Point"/> to query.</param>
    /// <returns>
    /// The <see cref="View"/> for the child at the specified <paramref name="point"/>;
    /// otherwise, a null reference.
    /// </returns>
    public View FindChild(Point point)
    {
        if (Find(point) is I info)
        {
            return info.View as View;
        }
        return null;
    }

    #endregion Find

    #region Overrides

    /// <summary>
    /// Updates the size of an View.
    /// </summary>
    /// <param name="widthConstraint">The width that a parent element can allocate a child element.</param>
    /// <param name="heightConstraint">The height that a parent element can allocate a child element.</param>
    /// <returns>The desired Size for this element.</returns>
    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        bool needsMeasure = _measuredSize.Width != widthConstraint || _measuredSize.Height != heightConstraint;
        if (needsMeasure)
        {
            foreach (LayoutInfo info in _layoutInfo.Values)
            {
                info.View.InvalidateMeasure();
            }
        }
        else
        {
            foreach (LayoutInfo info in _layoutInfo.Values)
            {
                if (info.NeedsMeasure)
                {
                    needsMeasure = true;
                    break;
                }
            }
        }
        if (needsMeasure)
        {
            _measuredSize = base.MeasureOverride(widthConstraint, heightConstraint);
        }
        return _measuredSize;
    }

    /// <summary>
    /// Adds an <see cref="IView"/> to the <see cref="Layout.Children"/> collection.
    /// </summary>
    /// <param name="index">The zero-based index to add.</param>
    /// <param name="view">The <see cref="IView"/> to add.</param>
    protected override sealed void OnAdd(int index, IView view)
    {
        base.OnAdd(index, view);
        AddView(view);
        InvalidateMeasure();
    }

    /// <summary>
    /// Inserts an <see cref="IView"/> into the <see cref="Layout.Children"/> collection.
    /// </summary>
    /// <param name="index">The zero-based index to insert at.</param>
    /// <param name="view">The <see cref="IView"/> to add.</param>
    protected override sealed void OnInsert(int index, IView view)
    {
        base.OnInsert(index, view);
        AddView(view);
        InvalidateMeasure();
    }

    /// <summary>
    /// Removes an <see cref="IView"/> from the <see cref="Layout.Children"/> collection.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="IView"/> to remove.</param>
    /// <param name="view">The <see cref="IView"/> to remove.</param>
    protected override sealed void OnRemove(int index, IView view)
    {
        base.OnRemove(index, view);
        RemoveView(view);
        InvalidateMeasure();
    }

    /// <summary>
    /// Replaces an <see cref="IView"/> in the <see cref="Layout.Children"/> collection.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="IView"/> to update.</param>
    /// <param name="view">The <see cref="IView"/> to set.</param>
    /// <param name="oldView">The <see cref="IView"/> to replace.</param>
    protected override sealed void OnUpdate(int index, IView view, IView oldView)
    {
        base.OnUpdate(index, view, oldView);
        RemoveView(oldView);
        AddView(view);
        InvalidateMeasure();
    }

    /// <summary>
    /// Removes all items from the <see cref="Layout.Children"/> collection.
    /// </summary>
    protected override sealed void OnClear()
    {
        base.OnClear();
        foreach (I info in _layoutInfo.Values)
        {
            RemoveView(info.View);
        }
        InvalidateMeasure();
    }

    #endregion Overrides
}
