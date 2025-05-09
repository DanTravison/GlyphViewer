using GlyphViewer.Text;
using GlyphViewer.ViewModels;

namespace GlyphViewer.Views;

public partial class FontFamiliesView : ContentView
{
    MainViewModel _model;

    public FontFamiliesView()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(string propertyName)
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
        if
        (
            _model is not null
            &&
            ReferenceEquals(e, MainViewModel.SelectedFamilyGroupChangedEventArgs)
            &&
            _model.SelectedFamilyGroup is not null
        )
        {
            string fontFamily = _model.SelectedFamilyGroup[0];
            Families.ScrollTo(fontFamily, _model.SelectedFamilyGroup, ScrollToPosition.Start, false);
        }
    }

    private void OnPickGroup(object sender, TappedEventArgs e)
    {
        GroupPicker.IsOpen = true;
    }
#if (false)
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if 
        (
            e.CurrentSelection.Count > 0 
            &&
            e.CurrentSelection[0] is string familyName
            &&
            familyName.Length > 0
        )
        {
            FontFamilyGroup group = _model.FontFamilyGroups.FromFamilyName(familyName);
            if (group is not null)
            {
                // ISSUE: This is not working.
                // Scrolling does not occur and the item is not visibly selected.
                Families.ScrollTo(familyName, group, ScrollToPosition.Center, false);
            }
        }
    }
#endif
}