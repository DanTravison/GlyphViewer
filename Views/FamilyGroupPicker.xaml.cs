namespace GlyphViewer.Views;

public partial class FamilyGroupPicker : ContentView
{
	public FamilyGroupPicker()
	{
		InitializeComponent();
	}

    protected override void OnPropertyChanged(string propertyName = null)
    {
        if (ZIndexProperty.PropertyName == propertyName)
        {
            IsVisible = ZIndex != -1;
        }
    }

    public void ShowPicker()
    {
        ZIndex = 1;
    }

    private void OnCancel(object sender, EventArgs e)
    {
		ZIndex = -1;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
         ZIndex = -1;
    }
}