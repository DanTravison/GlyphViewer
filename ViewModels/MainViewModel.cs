namespace GlyphViewer.ViewModels;

using CommunityToolkit.Maui.Storage;
using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using GlyphViewer.Settings;
using GlyphViewer.Text;
using GlyphViewer.Views;
using SkiaSharp;
using System.ComponentModel;
using System.Runtime.Versioning;

internal sealed class MainViewModel : ObservableObject
{
    #region Fields

    readonly IDispatcher _dispatcher;
    readonly MetricsViewModel _metrics;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="dispatcher">The <see cref="IDispatcher"/> to use for invoking methods on the UI thread.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dispatcher"/> is a null reference.</exception>
    public MainViewModel(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        Settings = UserSettings.Load();

        _metrics = new MetricsViewModel(Settings);
        _metrics.PropertyChanged += OnMetricsPropertyChanged;
        BookmarkCommand = new(Settings.Bookmarks, _metrics);
        FontGlyphs = new(Settings, _metrics);
        FontFamilies = new(_metrics);
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="Settings.UserSettings"/>.
    /// </summary>
    public UserSettings Settings
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="MetricsViewModel"/>.
    /// </summary>
    public MetricsViewModel Metrics
    {
        get => _metrics;
    }

    /// <summary>
    /// Gets the command for updating the currenty selected font family in bookmarks.
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

    #endregion Properties

    #region Font Info Loading

    public void LoadFonts(IDispatcher dispatcher)
    {
        FontFamilyGroupCollection families = FontFamilyGroupCollection.CreateInstance(Settings.Bookmarks);
        _ = dispatcher.DispatchAsync(() =>
        {
            FontFamilies.FontFamilyGroups = families;
        });
    }

    void LoadGlyphs(IDispatcher dispatcher)
    {
        GlyphCollection glyphs = null;
        if (_metrics.FontFamily is not null)
        {
            glyphs = GlyphCollection.CreateInstance(_metrics.FontFamily);
        }
        _ = dispatcher.DispatchAsync(() =>
        {
            FontGlyphs.Glyphs = glyphs;
            // Intent: Clear the selected bookmark after selection to 
            // ensure there are not two selections active in FontFamiliesView.
            // We're doing it here to avoid reentrancy when a bookmark is selected
            // in the FontFamiliesView.
            FontFamilies.SelectedBookmark = null;
        });
    }

    #endregion Font Info Loading

    #region Event Handlers

    private void OnMetricsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ReferenceEquals(e, MetricsViewModel.FontFamilyChangedEventArgs))
        {
            Task.Run(() => { LoadGlyphs(_dispatcher); });
        }
    }

    #endregion Event Handlers

    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.2")]
    [SupportedOSPlatform("MacCatalyst14.2")]
    [SupportedOSPlatform("Windows")]
    public static async Task<FileInfo> SaveAs(string fileName, Stream stream)
    {
        FileSaverResult result = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);
        if (result.IsSuccessful && result.FilePath is not null)
        {
            return new FileInfo(result.FilePath);
        }
        return null;
    }

    #region Font File Loading

    async void LoadFontFile()
    {
        FileInfo file = await PickFontFile();
        if (file is not null && file.Exists)
        {
            FontFamily fontFamily = new FileFont(file);
            if (fontFamily.GetTypeface(SKFontStyle.Normal) is not null)
            {
                _metrics.FontFamily = fontFamily;
                LoadGlyphs(_dispatcher);
            }       
        }
    }

    static readonly FilePickerFileType _fontFileTypes = new
    (
        new Dictionary<DevicePlatform, IEnumerable<string>>
        {
                {DevicePlatform.WinUI,   [".ttf", ".otf"] },
                {DevicePlatform.macOS,   ["ttf", "otf"] },
                {DevicePlatform.Android, ["font/ttf", "font/otf"] },
                {DevicePlatform.iOS,     ["public.opentype-font", "public.truetype-font"] }
        }
    );

    /// <summary>
    /// Picks a font file from the file system.
    /// </summary>
    /// <returns>The <see cref="FileInfo"/> for the selected file; otherwise, a null reference.</returns>
    static async Task<FileInfo> PickFontFile()
    {
        return await App.PickFile(Strings.FontFilePickerTitle, _fontFileTypes);
    }

    #endregion Font File Loading
}
