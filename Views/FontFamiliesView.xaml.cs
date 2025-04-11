using GlyphViewer.ViewModels;
using System.Runtime.CompilerServices;

namespace GlyphViewer.Views;

public partial class FontFamiliesView : ContentView
{
    MainViewModel _model;

    public FontFamiliesView()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        if (BindingContextProperty.PropertyName == propertyName)
        {
            if (_model is not null)
            {
                _model.PropertyChanged -= OnModelPropertyChanged;
            }
            _model = BindingContext as MainViewModel;
            if (_model is not null)
            {
                _model.PropertyChanged += OnModelPropertyChanged;
            }
        }
        base.OnPropertyChanged(propertyName);
    }

    private void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_model is not null && ReferenceEquals(e, MainViewModel.SelectedFamilyGroupChangedEventArgs))
        {
            if (_model.SelectedFamilyGroup is not null)
            {
                string fontFamily = _model.SelectedFamilyGroup[0];
                Families.ScrollTo(fontFamily, _model.SelectedFamilyGroup, ScrollToPosition.Start, false);
            }
        }
    }

    private void OnPickGroup(object sender, TappedEventArgs e)
    {
        GroupPicker.IsOpen = true;
    }
}