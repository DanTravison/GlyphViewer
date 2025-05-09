using GlyphViewer.Settings;

namespace GlyphViewer.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void OnBack(object sender, EventArgs e)
    {
        BindingContext = null;
        App.Navigation.PopModalAsync();
    }
}