namespace GlyphViewer.ViewModels;

using GlyphViewer.Diagnostics;
using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;
using GlyphViewer.Text;
using GlyphViewer.Views;
using System.ComponentModel;

internal sealed class MainViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public MainViewModel()
    {
        UserSettings = UserSettings.Load();
        Metrics = new MetricsViewModel(UserSettings);
        Metrics.PropertyChanged += OnMetricsPropertyChanged;

        BookmarkCommand = new(UserSettings, Metrics);
        FontGlyphs = new(UserSettings, Metrics);
        FontFamilies = new(UserSettings, Metrics);

        Settings = new SettingsViewModel(UserSettings, FontFamilies);
        Busy = new();
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="SettingsViewModel"/>
    /// </summary>
    public SettingsViewModel Settings
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="Settings.UserSettings"/>.
    /// </summary>
    public UserSettings UserSettings
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="MetricsViewModel"/>.
    /// </summary>
    public MetricsViewModel Metrics
    {
        get;
    }

    /// <summary>
    /// Gets the command for updating the currently selected font family in bookmarks.
    /// </summary>
    public BookmarkCommand BookmarkCommand
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="FontGlyphsViewModel"/> for the <see cref="FontGlyphsView"/> and font family.
    /// </summary>
    public FontGlyphsViewModel FontGlyphs
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="FontFamiliesViewModel"/>
    /// </summary>
    public FontFamiliesViewModel FontFamilies
    {
        get;
    }

    /// <summary>
    /// Gets a busy scope for entering and leaving a CPU bound operation.
    /// </summary>
    public BusyScope Busy
    {
        get;
    }

    #endregion Properties

    #region Font Info Loading

    public async Task LoadFonts()
    {
        using (await Busy.EnterAsync(nameof(LoadFonts)))
        {
            FontFamilyGroupCollection families = await Task.Run(() => FontFamilyGroupCollection.CreateInstance(Settings.UserSettings));
            FontFamilies.FontFamilyGroups = families;
        }
    }

    #endregion Font Info Loading

    #region Event Handlers

    private async void OnMetricsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ReferenceEquals(e, MetricsViewModel.FontFamilyChangedEventArgs))
        {
            using (await Busy.EnterAsync(nameof(OnMetricsPropertyChanged)))
            {
                GlyphCollection glyphs = null;

                try
                {
                    if (Metrics.FontFamily is not null)
                    {
                        glyphs = await Task.Run(() => GlyphCollection.CreateInstance(Metrics.FontFamily));
                    }
                }
                finally
                {
                    FontGlyphs.Glyphs = glyphs;
                    // Intent: Clear the selected bookmark after selection to 
                    // ensure there are not two selections active in FontFamiliesView.
                    // We're doing it here to avoid reentrancy when a bookmark is selected
                    // in the FontFamiliesView.
                    FontFamilies.SelectedBookmark = null;
                }
            }
        }
    }

    #endregion Event Handlers
}
