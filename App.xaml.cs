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

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new NavigationPage(new MainPage(_model)));
    }
}