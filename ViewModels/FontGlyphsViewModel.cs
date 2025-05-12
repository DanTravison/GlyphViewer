namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;
using GlyphViewer.Text;
using System.ComponentModel;
using System.Windows.Input;
using UnicodeRange = Text.Unicode.Range;

internal sealed class FontGlyphsViewModel : ObservableObject
{
    #region Fields

    ICommand _pickUnicodeRangeCommand;
    UnicodeRange _selectedRange;
    GlyphCollection _glyphs;
    int _row;
    int _rows;
    bool _IsJumpListOpen;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="settings">The <see cref="UserSettings"/>.</param>
    /// <param name="metrics">The <see cref="MetricsViewModel"/>.</param>
    public FontGlyphsViewModel(UserSettings settings, MetricsViewModel metrics)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(metrics);

        Metrics = metrics;
        Search = new SearchViewModel(metrics);
        Settings = settings;
    }

    /// <summary>
    /// Gets or sets the <see cref="GlyphCollection"/>.
    /// </summary>
    public GlyphCollection Glyphs
    {
        get => _glyphs;
        set
        {
            if (SetProperty(ref _glyphs, value, ReferenceComparer, GlyphsChangedEventArgs))
            {
                Search.Glyphs = value;
                IsJumpListOpen = false;
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="MetricsViewModel"/>.
    /// </summary>
    public MetricsViewModel Metrics
    {
        get;
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

    /// <summary>
    /// Gets the <see cref="SearchViewModel"/> for searching the glyphs in the current font family.
    /// </summary>
    public SearchViewModel Search
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="UserSettings"/>.
    /// </summary>
    public UserSettings Settings
    {
        get;
    }

    /// <summary>
    /// Gets or sets the value indicating if the JumpList is open.
    /// </summary>
    public bool IsJumpListOpen
    {
        get => _IsJumpListOpen;
        set
        {
            if (SetProperty(ref _IsJumpListOpen, value, IsJumpListOpenChangedEventArgs))
            {
                Search.ShowResults = false;
            }
        }
    }

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
    /// Gets or sets the selected Unicode range.
    /// </summary>
    public UnicodeRange SelectedUnicodeRange
    {
        get => _selectedRange;
        set => SetProperty(ref _selectedRange, value, SelectedUnicodeRangeChangedEventArgs);
    }

    #endregion Unicode Range Properties

    #region PropertyChangedEventArgs

    static readonly PropertyChangedEventArgs SelectedUnicodeRangeChangedEventArgs = new(nameof(SelectedUnicodeRange));
    static readonly PropertyChangedEventArgs PickUnicodeRangeCommandChangedEventArgs = new(nameof(PickUnicodeRangeCommand));
    static readonly PropertyChangedEventArgs GlyphsChangedEventArgs = new(nameof(Glyphs));
    static readonly PropertyChangedEventArgs RowChangedEventArgs = new(nameof(Row));
    static readonly PropertyChangedEventArgs RowsChangedEventArgs = new(nameof(Rows));
    static readonly PropertyChangedEventArgs MaxRowChangedEventArgs = new(nameof(MaxRow));
    static readonly PropertyChangedEventArgs IsJumpListOpenChangedEventArgs = new(nameof(IsJumpListOpen));

    #endregion PropertyChangedEventArgs
}
