namespace GlyphViewer.Controls;

using Microsoft.Maui.Layouts;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides layout info for a <see cref="Layout"/> and <see cref="LayoutManager"/>class.
/// </summary>
public class LayoutInfo
{
    #region Fields

    IView _view;
    Rect _bounds;
    bool _needsMeasure = true;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="view">The <see cref="IView"/> for the item.</param>
    public LayoutInfo(IView view)
    {
        View = view;
    }

    /// <summary>
    /// Releases the associated <see cref="View"/> and any event handlers.
    /// </summary>
    internal void Removed()
    {
        if (View is VisualElement visual)
        {
            visual.MeasureInvalidated -= OnMeasureInvalidated;
            visual.BindingContext = null;
        }
        _view = null;
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="IView"/> for this item.
    /// </summary>
    public IView View
    {
        get => _view;
        protected set
        {
            if (_view is not null)
            {
                if (_view is VisualElement visual)
                {
                    visual.MeasureInvalidated -= OnMeasureInvalidated;
                }
            }
            _view = value;
            if (_view is not null)
            {
                if (_view is VisualElement visual)
                {
                    visual.MeasureInvalidated += OnMeasureInvalidated;
                }
                _needsMeasure = true;
            }
        }
    }

    /// <summary>
    /// Gets or sets the desired <see cref="Rect"/> for the item.
    /// </summary>
    public Rect Bounds
    {
        get => _bounds;
        set
        {
            if (_bounds != value)
            {
                _bounds = value;
                NeedsArrange = true;
            }
            NeedsMeasure = false;
        }
    }

    /// <summary>
    /// Gets the size of the item.
    /// </summary>
    public Size Size
    {
        get => _bounds.Size;
        set
        {
            if (_bounds.Size != value)
            {
                _bounds.Width = value.Width;
                _bounds.Height = value.Height;
                _needsMeasure = true;
            }
            else
            {
                _needsMeasure = false;
            }
        }
    }

    /// <summary>
    /// Gets the width.
    /// </summary>
    public double Width
    {
        get => _bounds.Width;
    }

    /// <summary>
    /// Gets the height.
    /// </summary>
    public double Height
    {
        get => _bounds.Height;
    }

    /// <summary>
    /// Gets or sets the value indicating if the item needs to be measured.
    /// </summary>
    public virtual bool NeedsMeasure
    {
        get
        {
            if (!_needsMeasure)
            {
                _needsMeasure =
                (
                    double.IsNaN(View.Width)
                    ||
                    double.IsNaN(View.Height)
                    ||
                    double.IsFinite(Bounds.Width) || !double.IsFinite(Bounds.Height)
                 );
            }
            return _needsMeasure;
        }
        set => _needsMeasure = value;
    }

    /// <summary>
    /// Gets or sets the value indicating if the item needs to be arranged.
    /// </summary>
    public bool NeedsArrange
    {
        get;
        set;
    }

    #endregion Properties

    #region Measure

    private void OnMeasureInvalidated(object sender, EventArgs e)
    {
        NeedsMeasure = true;
    }

    /// <summary>
    /// Measures the <see cref="View"/>.
    /// </summary>
    /// <param name="widthConstraint">The width constraint.</param>
    /// <param name="heightConstraint">The height constraint.</param>
    /// <returns>The measured size of the <see cref="View"/>.</returns>
    public virtual Size Measure(double widthConstraint, double heightConstraint)
    {
        Size size = View.Measure(widthConstraint, heightConstraint);
        Size = size;
        _needsMeasure = false;
        return Bounds.Size;
    }

    #endregion Measure

    #region Arrange

    /// <summary>
    /// Arranges the <see cref="View"/> is the specified <see cref="Rect"/>.
    /// </summary>
    /// <param name="bounds">The <see cref="Rect"/> to use to arrange the <see cref="View"/>.</param>
    public void Arrange(Rect bounds)
    {
        Bounds = bounds;
        Arrange();
    }

    /// <summary>
    /// Arranges the <see cref="View"/> to the <see cref="Bounds"/>.
    /// </summary>
    public virtual void Arrange()
    {
        Rect bounds = Bounds;

        if (bounds.X != View.Frame.X
            || bounds.Y != View.Frame.Y
            || bounds.Width != View.Width
            || bounds.Height != View.Height
        )
        {
            View.Arrange(Bounds);
        }
        NeedsArrange = false;
    }

    #endregion Arrange

    /// <summary>
    /// Constrains a dimension based on the <see cref="IView"/> minimum/maximum width or height constraints
    /// </summary>
    /// <param name="dimension"></param>
    /// <param name="byWidth">
    /// true to constrain the value based on the <see cref="IView.MinimumWidth"/>
    /// and <see cref="IView.MaximumWidth"/>; 
    /// otherwise, false to constrain the value based on the <see cref="IView.MinimumHeight"/>
    /// and <see cref="IView.MaximumHeight"/>
    /// </param>
    /// <returns>
    /// The constrained value.
    /// </returns>
    public double Constrain(double dimension, bool byWidth)
    {
        double minimum = byWidth ? View.MinimumWidth : View.MinimumHeight;
        double maximum = byWidth ? View.MaximumWidth : View.MaximumHeight;

        if (IsDefined(minimum))
        {
            dimension = Math.Max(minimum, dimension);
        }
        if (IsDefined(maximum))
        {
            dimension = Math.Min(maximum, dimension);
        }
        return dimension;
    }

    /// <summary>
    /// Determines if a value is defined.
    /// </summary>
    /// <param name="value">the value to check.</param>
    /// <returns>true if the value greater than or equal to zero and is finite.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value) && value > 0;
    }

}
