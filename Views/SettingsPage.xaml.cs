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

	public static void Show(UserSettings settings)
    {
        SettingsPage page = new()
        {
            BindingContext = settings
        };
        App.Navigation.PushModalAsync(page);
    }
}