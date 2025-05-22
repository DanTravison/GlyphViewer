namespace GlyphViewer.Views.Renderers;

using GlyphViewer.Diagnostics;
using SkiaSharp.Views.Maui.Controls;
using System.Runtime.CompilerServices;
using Diag = System.Diagnostics;

/// <summary>
/// Manages the invalidation state of a SKCanvasView.
/// </summary>
[Diag.DebuggerDisplay("{_state, nq}")]
class RenderingState
{
    #region Fields

    readonly SKCanvasView _view;
    RenderState _state;
    readonly string TraceSource;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="view">The associated <see cref="SKCanvasView"/>.</param>
    public RenderingState(SKCanvasView view)
    {
        _view = view;
        TraceSource = view.GetType().Name;
    }

    #region Properties

    /// <summary>
    /// Gets and clears the <see cref="RenderState.Measure"/> state.
    /// </summary>
    /// <value>
    /// true if the <see cref="RenderState.Measure"/> state was set; otherwise, false.
    /// </value>
    public bool ShouldMeasure
    {
        get => Clear(RenderState.Measure);
    }

    /// <summary>
    /// Gets and clears <see cref="RenderState.Layout"/> state.
    /// </summary>
    /// <value>
    /// true if the <see cref="RenderState.Layout"/> state was set; otherwise, false.
    /// </value>
    public bool ShouldLayout
    {
        get => Clear(RenderState.Layout);
    }

    /// <summary>
    /// Gets and clears the <see cref="RenderState.Arrange"/> state.
    /// </summary>
    /// <value>
    /// true if the <see cref="RenderState.Arrange"/> state was set; otherwise, false.
    /// </value>
    public bool ShouldArrange
    {
        get => Clear(RenderState.Arrange);
    }

    /// <summary>
    /// Gets and clears the <see cref="RenderState.Draw"/> state.
    /// </summary>
    /// <value>
    /// true if the <see cref="RenderState.Draw"/> state was set; otherwise, false.
    /// </value>
    public bool ShouldDraw
    {
        get => Clear(RenderState.Draw);
    }

    #endregion Properties

    #region Methods

    bool Clear(RenderState type, [CallerMemberName] string name = nameof(Clear))
    {
        bool result = _state.HasFlag(type);
        if (result)
        {
            _state &= ~type;
        }
        Trace.Line(TraceFlag.Layout, TraceSource, name, "[{0}] {1}:{2}", result, type, _state);
        return result;
    }

    /// <summary>
    /// Sets the current invalidate state to the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The <see cref="RenderState"/> to set.</param>
    public void Invalidate(RenderState type)
    {
        RenderState state = RenderState.None;
        switch (type)
        {
            case RenderState.Measure:
                state = RenderState.Measure | RenderState.Layout | RenderState.Arrange | RenderState.Draw;
                break;
            case RenderState.Layout:
                state = RenderState.Layout | RenderState.Arrange | RenderState.Draw;
                break;
            case RenderState.Arrange:
                state = RenderState.Arrange | RenderState.Draw;
                break;
            case RenderState.Draw:
                state = RenderState.Draw;
                break;
        }
        if ((state | _state) != _state)
        {
            if (!_state.HasFlag(RenderState.Draw))
            {
                Trace.Value(TraceFlag.Layout, TraceSource, nameof(GlyphsView.InvalidateSurface));
                _view.InvalidateSurface();
            }
            _state |= state;
            Trace.Line(TraceFlag.Layout, TraceSource, nameof(Invalidate), "{0}->{1}", type, _state);
        }
    }

    #endregion Methods
}

