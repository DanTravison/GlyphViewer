using GlyphViewer.Text;
using GlyphViewer.ViewModels;

namespace GlyphViewer.Views;

public partial class FontFamiliesView : ContentView
{
    FontFamiliesViewModel _model;

    public FontFamiliesView()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        if (_model is not null)
        {
            _model.PropertyChanged -= OnModelPropertyChanged;
        }
        _model = BindingContext as FontFamiliesViewModel;
        if (_model is not null)
        {
            _model.PropertyChanged += OnModelPropertyChanged;
        }
        base.OnBindingContextChanged();
    }

    private void OnPickGroup(object sender, TappedEventArgs e)
    {
        GroupPicker.IsOpen = true;
    }

    private void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if
        (
            _model is not null
            &&
            ReferenceEquals(e, FontFamiliesViewModel.SelectedFamilyGroupChangedEventArgs)
            &&
            _model.SelectedFamilyGroup is not null
        )
        {
            FontFamily fontFamily = _model.SelectedFamilyGroup[0];
            Families.ScrollTo(fontFamily, _model.SelectedFamilyGroup, ScrollToPosition.Start, false);
        }
    }
}