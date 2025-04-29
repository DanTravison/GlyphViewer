namespace GlyphViewer;

using GlyphViewer.ViewModels;
using GlyphViewer.Views;

public partial class App : Application
{
    public const string DefaultFontFamily = "OpenSansRegular";

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
}