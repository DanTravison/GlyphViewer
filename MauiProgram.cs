namespace GlyphViewer;

using CommunityToolkit.Maui;
using GlyphViewer.Resources;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Trace.TraceFlags = TraceFlag.Application | TraceFlag.Navigation | TraceFlag.Layout;

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                FontLoader.Load(fonts, FontLoader.Defaults);
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
