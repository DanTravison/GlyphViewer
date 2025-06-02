namespace GlyphViewer;

using CommunityToolkit.Maui.Storage;
using GlyphViewer.Diagnostics;
using GlyphViewer.Resources;
using GlyphViewer.Text;
using GlyphViewer.ViewModels;
using GlyphViewer.Views;
using System.Runtime.Versioning;

public partial class App : Application
{
    readonly MainViewModel _model;

    public App()
    {
        InitializeComponent();
        _model = new();
        BindingContext = _model;
    }

    /// <summary>
    /// Gets the <see cref="INavigation"/> for the window page.
    /// </summary>
    public static INavigation Navigation
    {
        get => Current.Windows[0].Page.Navigation;
    }

    #region Window Management

    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = new Window(new NavigationPage(new MainPage(_model)));
        Subscribe(window);
        return window;
    }

    void Subscribe(Window window)
    {
        window.Deactivated += OnWindowDeactivated;
        window.Backgrounding += OnWindowBackgrounding;
        window.Destroying += OnWindowDestroying;
    }

    void Unsubscribe(Window window)
    {
        window.Deactivated -= OnWindowDeactivated;
        window.Backgrounding -= OnWindowBackgrounding;
        window.Destroying -= OnWindowDestroying;
    }

    private void OnWindowDestroying(object sender, EventArgs e)
    {
        _model.Settings.Save();
        Unsubscribe(sender as Window);
    }

    private void OnWindowBackgrounding(object sender, BackgroundingEventArgs e)
    {
        _model.Settings.Save();
    }

    private void OnWindowDeactivated(object sender, EventArgs e)
    {
        _model.Settings.Save();
    }

    #endregion Window Management

    /// <summary>
    /// Presents a dialog to select a file.
    /// </summary>
    /// <param name="fileTypes">An optional <see cref="FilePickerFileType"/> to filter the open file list.</param>
    /// <returns>The <see cref="FileInfo"/> on success; otherwise, a null reference.</returns>
    public static async Task<FileInfo> PickFile(string title, FilePickerFileType fileTypes)
    {
        PickOptions options = new()
        {
            PickerTitle = title,
        };
        if (fileTypes is not null)
        {
            options.FileTypes = fileTypes;
        }
        FileResult result;
        try
        {
            result = await FilePicker.Default.PickAsync(options);
        }
        catch (Exception ex)
        {
            Trace.Exception(typeof(App), nameof(PickFile), ex);
            return null;
        }
        if (result is not null && result.FullPath is not null)
        {
            return new FileInfo(result.FullPath);
        }
        return null;
    }

    /// <summary>
    /// Prompts the user to save a file with the specified <paramref name="fileName"/> and <paramref name="stream"/>.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="stream">The <see cref="Stream"/> to write to the file.</param>
    /// <returns></returns>
    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.2")]
    [SupportedOSPlatform("MacCatalyst14.2")]
    [SupportedOSPlatform("Windows")]
    public static async Task<FileInfo> SaveAs(string fileName, Stream stream)
    {
        FileSaverResult result = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);
        if (result.IsSuccessful && result.FilePath is not null)
        {
            return new FileInfo(result.FilePath);
        }
        return null;
    }

}