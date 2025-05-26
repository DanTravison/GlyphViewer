namespace GlyphViewer;

using GlyphViewer.Diagnostics;
using GlyphViewer.Resources;
using GlyphViewer.Text;
using GlyphViewer.ViewModels;
using GlyphViewer.Views;

public partial class App : Application
{
    /// <summary>
    /// Gets the family name for the default font.
    /// </summary>
    public const string DefaultFontFamilyName = "OpenSansRegular";

    /// <summary>
    /// Gets the default <see cref="FontFamily"/>
    /// </summary>
    public static readonly FontFamily DefaultFontFamily = new FontFamily(DefaultFontFamilyName);

    /// <summary>
    /// Gets the <see cref="FontFamily"/> for the <see cref="FluentUI"/> font.
    /// </summary>
    public static readonly FontFamily FluentUIFontFamily = new FontFamily(nameof(FluentUI));

    readonly MainViewModel _model;

    public App()
    {
        InitializeComponent();
        _model = new(Application.Current.Dispatcher);
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
}