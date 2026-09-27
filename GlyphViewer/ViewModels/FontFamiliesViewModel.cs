namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;
using GlyphViewer.Text;
using System.ComponentModel;

/// <summary>
/// Provides a view model for the FontFamiliesView.
/// </summary>
internal class FontFamiliesViewModel : ObservableObject
{
    #region Fields

    FontFamilyGroupCollection _fontFamiliesGroups;
    FontFamilyGroup _selectedGroup;
    FontFamily _selectedBookmark;
    readonly FileFonts _fileFonts;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="settings">The <see cref="UserSettings"/> to use for bookmarks.</param>
    /// <param name="metrics">The <see cref="MetricsViewModel"/> to use for the current font family.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="settings"/> or <paramref name="metrics"/> is a null reference.
    /// </exception>
    public FontFamiliesViewModel(UserSettings settings, MetricsViewModel metrics)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        ArgumentNullException.ThrowIfNull(metrics, nameof(metrics));

        Metrics = metrics;
        _fileFonts = settings.Fonts;
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="MetricsViewModel"/>.
    /// </summary>
    public MetricsViewModel Metrics
    {
        get;
    }

    /// <summary>
    /// Gets or sets the selected bookmark.
    /// </summary>
    public FontFamily SelectedBookmark
    {
        get => _selectedBookmark;
        set
        {
            if (value is not null)
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
            if (!ReferenceEquals(_fontFamiliesGroups, value))
            {
                _fontFamiliesGroups = value;
                Metrics.FontFamily = null;
                OnPropertyChanged(FontFamiliesChangedEventArgs);
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

    #endregion Properties

    #region Add/Remove Font Family

    /// <summary>
    /// Adds a <see cref="FileFontFamily"/>.
    /// </summary>
    /// <param name="font">The <see cref="FileFontFamily"/> to add.</param>
    public void Add(FileFontFamily font)
    {
        ArgumentNullException.ThrowIfNull(font, nameof(font));
        _fileFonts.Add(font);
    }

    /// <summary>
    /// Removes a <see cref="FileFontFamily"/> from the <see cref="FontFamilyGroups"/>.
    /// </summary>
    /// <param name="font">The <see cref="FileFontFamily"/> to remove.</param>
    /// <returns>true if the <paramref name="font"/> was found and removed; otherwise, false.</returns>
    public bool Remove(FileFontFamily font)
    {
        ArgumentNullException.ThrowIfNull(font, nameof(font));
        bool result = _fileFonts.Remove(font);
        if (result)
        {
            if (Metrics.FontFamily == font)
            {
                Metrics.FontFamily = null;
            }
            if (SelectedBookmark == font)
            {
                SelectedBookmark = null;
            }
        }
        return result;
    }

    #endregion Add/Remove Font Family

    #region PropertyChangedEventArgs

    static readonly PropertyChangedEventArgs FontFamiliesChangedEventArgs = new(nameof(FontFamilyGroups));
    static readonly PropertyChangedEventArgs SelectedBookmarkChangedEventArgs = new(nameof(SelectedBookmark));
    public static readonly PropertyChangedEventArgs SelectedFamilyGroupChangedEventArgs = new(nameof(SelectedFamilyGroup));

    #endregion PropertyChangedEventArgs
}
