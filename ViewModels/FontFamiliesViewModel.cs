namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Text;
using System.ComponentModel;
using System.Windows.Input;

/// <summary>
/// Privides a view model for the FontFamiliesView.
/// </summary>
internal class FontFamiliesViewModel : ObservableObject
{
    #region Fields

    FontFamilyGroupCollection _fontFamiliesGroups;
    FontFamilyGroup _selectedGroup;
    string _selectedBookmark;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="metrics">The <see cref="MetricsViewModel"/>.</param>
    public FontFamiliesViewModel(MetricsViewModel metrics)
    {
        ArgumentNullException.ThrowIfNull(metrics, nameof(metrics));
        Metrics = metrics;
    }

    /// <summary>
    /// Gets the <see cref="MetricsViewModel"/>.
    /// </summary>
    public MetricsViewModel Metrics
    {
        get;
    }

    #region Font Families

    /// <summary>
    /// Gets the <see cref="FontFamilyGroupCollection"/> for the font families in the system.
    /// </summary>
    /// <remarks>
    /// This property is updated by LoadFonts and consumed by FontFamiliesView.
    /// </remarks>
    public FontFamilyGroupCollection FontFamilyGroups
    {
        get => _fontFamiliesGroups;
        set
        {
            if (SetProperty(ref _fontFamiliesGroups, value, FontFamiliesChangedEventArgs))
            {
                Metrics.FontFamily = null;
            }
        }
    }

    #endregion Font Families

#region Family Group

#if (false)
    /// <summary>
    /// Gets or sets the command for the jump list to pick a font family group.
    /// </summary>
    /// <remarks>
    /// Not currently used pending figuring out the correct XAML binding.
    /// Currently, invoking the FamilyGroup picker is implemented in FontfamiliesView.OnPickGroup.
    /// </remarks>
    public ICommand PickFamilyGroupCommand
    {
        get;
        set;
    }
#endif

    /// <summary>
    /// Gets or sets the selected font family.
    /// </summary>
    /// <remarks>This property is bound to the FontFamiliesView</remarks>
    public FontFamilyGroup SelectedFamilyGroup
    {
        get => _selectedGroup;
        set
        {
            if (!ReferenceEquals(_selectedGroup, value))
            {
                _selectedGroup = value;
                OnPropertyChanged(SelectedFamilyGroupChangedEventArgs);
            }
        }
    }

    #endregion Family Group

    /// <summary>
    /// Gets or sets the selected bookmark.
    /// </summary>
    public string SelectedBookmark
    {
        get => _selectedBookmark;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Metrics.FontFamily = value;
            }
            else
            {
                // This path occurs after Glyphs are updated
                // to clear the selected bookmark in FontFamiliesView.
                _selectedBookmark = null;
                OnPropertyChanged(SelectedBookmarkChangedEventArgs);
            }
        }
    }

    #region PropertyChangedEventArgs

    static readonly PropertyChangedEventArgs FontFamiliesChangedEventArgs = new(nameof(FontFamilyGroups));
    static readonly PropertyChangedEventArgs SelectedBookmarkChangedEventArgs = new(nameof(SelectedBookmark));
    public static readonly PropertyChangedEventArgs SelectedFamilyGroupChangedEventArgs = new(nameof(SelectedFamilyGroup));

    #endregion PropertyChangedEventArgs
}
