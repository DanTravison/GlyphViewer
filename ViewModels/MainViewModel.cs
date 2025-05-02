namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;
using GlyphViewer.Text;
using SkiaSharp;
using System.ComponentModel;
using System.Windows.Input;

internal sealed class MainViewModel : ObservableObject
{
    #region Fields

    FontFamilyGroupCollection _fontFamiliesGroups;
    FontFamilyGroup _selectedGroup;
    Text.Unicode.Range _selectedRange;
    IReadOnlyList<Text.Unicode.Range> _ranges;
    GlyphCollection _glyphs;
    readonly IDispatcher _dispatcher;
    int _row;
    int _rows;
    ICommand _pickUnicodeRangeCommand;
    readonly MetricsModel _metrics;

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

        _metrics = new MetricsModel(Settings.ItemFont.FontSize);
        _metrics.PropertyChanged += OnMetricsPropertyChanged;
        BookmarkCommand = new(Settings.Bookmarks, _metrics);
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
    /// Gets the command for updating the currenty selected font family in bookmarks.
    /// </summary>
    public BookmarkCommand BookmarkCommand
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="MetricsModel"/> for the selected <see cref="Glyph"/> and font family.
    /// </summary>
    public MetricsModel Metrics
    {
        get => _metrics;
    }

    #region Rows

    /// <summary>
    /// Gets or sets the current row.
    /// </summary>
    /// <remarks>
    /// This property is updated by GlyphsView and the GlyphsView slider.
    /// </remarks>
    public int Row
    {
        get => _row;
        set => SetProperty(ref _row, value, RowChangedEventArgs);
    }

    /// <summary>
    /// Gets or sets the number of rows.
    /// </summary>
    /// <remarks>
    /// This property is updated by the GlyphsView.
    /// </remarks>
    public int Rows
    {
        get => _rows;
        set
        {
            if (SetProperty(ref _rows, value, RowsChangedEventArgs))
            {
                OnPropertyChanged(MaxRowChangedEventArgs);
            }
        }
    }

    /// <summary>
    /// Gets or sets the maximum row number.
    /// </summary>
    /// <remarks>
    /// This property is consumed by the GlyphsView slider.
    /// </remarks>
    public int MaxRow
    {
        get => _rows > 0 ? _rows - 1 : 0;
    }

    #endregion Rows

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
        private set
        {
            if (SetProperty(ref _fontFamiliesGroups, value, FontFamiliesChangedEventArgs))
            {
                _metrics.FontFamily = null;
            }
        }
    }

    #endregion Font Families

    #region Glyphs

    /// <summary>
    /// Gets the <see cref="GlyphCollection"/> for the selected font family.
    /// </summary>
    /// <remarks>
    /// This property is updated by LoadGlyphs and consumed by GlyphsView.
    /// </remarks>
    public GlyphCollection Glyphs
    {
        get => _glyphs;
        private set => SetProperty(ref _glyphs, value, ReferenceComparer, GlyphsChangedEventArgs);
    }

    #endregion Glyphs

    #region Family Group

    /// <summary>
    /// Gets or sets the command for the jump list to pick a font family group.
    /// </summary>
    /// <remarks>
    /// Not currently used pending figuring out the correct XAML binding.
    /// Currently, invoking the FamilyGroup picker is implemented in Fontfamiliesview.OnPickGroup
    /// </remarks>
    public ICommand PickFamilyGroupCommand
    {
        get;
        set;
    }

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

    #region Unicode Range Properties

    /// <summary>
    /// Gets or sets the command to display the JumpList for picking a Unicode range.
    /// </summary>
    /// <remarks>
    /// This property is populated by the GlyphsView jump list and consumed by GlyphsView.HeaderPickCommand
    /// </remarks>
    public ICommand PickUnicodeRangeCommand
    {
        get => _pickUnicodeRangeCommand;
        set => SetProperty(ref _pickUnicodeRangeCommand, value, ReferenceComparer, PickUnicodeRangeCommandChangedEventArgs);
    }

    /// <summary>
    /// Gets or sets the list of Unicode ranges for the currently selected font family.
    /// </summary>
    public IReadOnlyList<Text.Unicode.Range> UnicodeRanges
    {
        get => _ranges;
        set => SetProperty(ref _ranges, value, ReferenceComparer, UnicodeRangesChangedEventArgs);
    }

    /// <summary>
    /// Gets or sets the selected Unicode range.
    /// </summary>
    public Text.Unicode.Range SelectedUnicodeRange
    {
        get => _selectedRange;
        set => SetProperty(ref _selectedRange, value, ReferenceComparer, SelectedUnicodeRangeChangedEventArgs);
    }

    #endregion Unicode Range Properties

    #endregion Properties

    #region Font Info Loading

    public void LoadFonts(IDispatcher dispatcher)
    {
        FontFamilyGroupCollection families = FontFamilyGroupCollection.CreateInstance(Settings.Bookmarks);
        _ = dispatcher.DispatchAsync(() =>
        {
            FontFamilyGroups = families;
        });
    }

    public void LoadGlyphs(IDispatcher dispatcher)
    {
        GlyphCollection glyphs = null;
        if (!string.IsNullOrWhiteSpace(_metrics.FontFamily))
        {
            using (SKTypeface typeface = SKTypeface.FromFamilyName
            (
                _metrics.FontFamily,
                SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright
            ))
            {
                glyphs = GlyphCollection.CreateInstance(typeface);
            }
        }
        _ = dispatcher.DispatchAsync(() =>
        {
            Glyphs = glyphs;
        });
    }

    #endregion Font Info Loading

    #region Event Handlers

    private void OnMetricsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ReferenceEquals(e, MetricsModel.FontFamilyChangedEventArgs))
        {
            Task.Run(() => { LoadGlyphs(_dispatcher); });
        }
    }

    #endregion Event Handlers

    #region PropertyChangedEventArgs

    static readonly PropertyChangedEventArgs FontFamiliesChangedEventArgs = new(nameof(FontFamilyGroups));
    static readonly PropertyChangedEventArgs GlyphsChangedEventArgs = new(nameof(Glyphs));

    public static readonly PropertyChangedEventArgs SelectedFamilyGroupChangedEventArgs = new(nameof(SelectedFamilyGroup));

    static readonly PropertyChangedEventArgs UnicodeRangesChangedEventArgs = new(nameof(UnicodeRanges));
    static readonly PropertyChangedEventArgs SelectedUnicodeRangeChangedEventArgs = new(nameof(SelectedUnicodeRange));
    static readonly PropertyChangedEventArgs PickUnicodeRangeCommandChangedEventArgs = new(nameof(PickUnicodeRangeCommand));

    static readonly PropertyChangedEventArgs RowChangedEventArgs = new(nameof(Row));
    static readonly PropertyChangedEventArgs RowsChangedEventArgs = new(nameof(Rows));
    static readonly PropertyChangedEventArgs MaxRowChangedEventArgs = new(nameof(MaxRow));

    #endregion PropertyChangedEventArgs
}
