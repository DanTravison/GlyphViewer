namespace GlyphViewer.Controls;

using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui.Handlers;

/// <summary>
/// Provides a custom <see cref="SKCanvasViewHandler"/> for handing issue 18700 https://github.com/mono/SkiaSharp/issues/3239
/// </summary>
internal class CanvasViewHandler : SKCanvasViewHandler
{
    public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
    {
        Size size = Size.Zero;
        if (this.VirtualView is SKCanvasView view)
        {
            size = view.DesiredSize;
        }
        if (size == Size.Zero)
        {
            size = base.GetDesiredSize(widthConstraint, heightConstraint);
        }
        return size;
    }
}

/// <summary>
/// Provides a <see cref="MauiAppBuilder"/> for configuring <see cref="CanvasViewHandler"/>.
/// </summary>
public static class CanvasViewExtensions
{
    public static MauiAppBuilder UseCanvasView(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<SKCanvasView, CanvasViewHandler>();
        });
        return builder;
    }
}
