using GlyphViewer.ViewModels;

namespace GlyphViewer.Views;

public partial class MainPage : ContentPage
{
    readonly MainViewModel _model;
    internal MainPage(MainViewModel model)
    {
        _model = model;
        BindingContext = _model;
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (_model.Glyphs is null)
        {
            Task.Run(() => { _model.LoadFonts(Dispatcher); });
        }
    }
}
