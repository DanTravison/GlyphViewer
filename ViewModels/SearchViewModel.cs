namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Text;
using System.ComponentModel;
using System.Windows.Input;

/// <summary>
/// Provides a search view model for the glyphs in a <see cref="GlyphCollection"/>.
/// </summary>
internal sealed class SearchViewModel : ObservableObject
{
    #region Fields

    GlyphCollection _glyphs;
    Glyph _selectedItem;

    readonly Command _searchCommand;
    readonly Command _showResultsCommand;
    string _searchText;
    IReadOnlyList<Glyph> _searchResults;
    bool _showResults;

    readonly MetricsViewModel _metrics;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="metrics">
    /// The <see cref="MetricsViewModel"/> to update when a glyph is selected
    /// from the results.
    /// </param>
    internal SearchViewModel(MetricsViewModel metrics)
    {
        ArgumentNullException.ThrowIfNull(metrics, nameof(metrics));

        _metrics = metrics;

        _metrics.PropertyChanged += OnMetricsPropertyChanged;
        // NOTE: _searchCommand is always enabled because changing the IsEnabled state
        // when the search text changes can cause reentracy and cause the command
        // to be disabled when it is not supposed to be.
        _searchCommand = new(OnSearch);
        _showResultsCommand = new(OnShowResults)
        {
            IsEnabled = false
        };
    }

    private void OnMetricsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ReferenceEquals(e, MetricsViewModel.FontFamilyChangedEventArgs))
        {
            SearchText = string.Empty;
        }
    }

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="GlyphCollection"/> to search.
    /// </summary>
    public GlyphCollection Glyphs
    {
        get => _glyphs;
        set
        {
            _glyphs = value;
            Results = null;
            SelectedItem = null;
            _metrics.FontProperties.GlyphCount = value?.Count ?? 0;
            OnPropertyChanged(CanEditChangedEventArgs);
        }
    }

    /// <summary>
    /// Gets or sets the selected <see cref="Glyph"/> from the search results.
    /// </summary>
    public Glyph SelectedItem
    {
        get => _selectedItem;
        set
        {
            if 
            (
                SetProperty(ref _selectedItem, value, SelectedItemChangedEventArgs)
                &&
                value is not null
            )
            {
                _metrics.Glyph = value;
                _selectedItem = null;
                // need to delay since we can't set SelectedItem in an event for the 
                // selection changed.
                App.Current.Dispatcher.DispatchAsync(() => OnPropertyChanged(SelectedItemChangedEventArgs));
            }
        }
    }

    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value, SearchTextChangedEventArgs))
            {
                Results = null;
            }
        }
    }

    /// <summary>
    /// Gets the search results.
    /// </summary>
    public IReadOnlyList<Glyph> Results
    {
        get => _searchResults;
        private set
        {
            if (SetProperty(ref _searchResults, value, ResultsChangedEventArgs))
            {
                _showResultsCommand.IsEnabled = value?.Count > 0;
                App.Current.Dispatcher.DispatchAsync(() =>
                {
                    _selectedItem = null;
                    ShowResults = value?.Count > 0;
                });
            }
        }
    }

    /// <summary>
    /// Gets the value indicating if the search input field is enabled.
    /// </summary>
    public bool CanEdit
    {
        get => _glyphs?.Count > 0;
    }

    /// <summary>
    /// Gets the value indicating if the search results should be visible.
    /// </summary>
    public bool ShowResults
    {
        get => _showResults && _searchResults?.Count > 0;
        set => SetProperty(ref _showResults, value, ShowResultsChangedEventArgs);
    }

    #endregion Properties

    #region Commands

    /// <summary>
    /// Gets the <see cref="ICommand"/> to use to perform a search.
    /// </summary>
    public Command SearchCommand
    {
        get => _searchCommand;
    }

    void OnSearch()
    {
        string searchText = SearchText;
        if (!string.IsNullOrEmpty(searchText))
        {
            Results = Glyphs.Search(searchText);
        }
    }

    /// <summary>
    /// Gets the <see cref="Command"/> to use to show search results.
    /// </summary>
    public Command ShowResultsCommand
    {
        get => _showResultsCommand;
    }

    void OnShowResults()
    {
        if (_searchResults?.Count > 0)
        {
            ShowResults = !ShowResults;
        }
        else
        {
            ShowResults = false;
        }
    }

    #endregion Commands

    #region PrpeprtyChangedEventArgs

    static readonly PropertyChangedEventArgs CanEditChangedEventArgs = new(nameof(CanEdit));
    static readonly PropertyChangedEventArgs SelectedItemChangedEventArgs = new(nameof(SelectedItem));
    static readonly PropertyChangedEventArgs ResultsChangedEventArgs = new(nameof(Results));
    static readonly PropertyChangedEventArgs ShowResultsChangedEventArgs = new(nameof(ShowResults));
    static readonly PropertyChangedEventArgs SearchTextChangedEventArgs = new(nameof(SearchText));

    #endregion PrpeprtyChangedEventArgs
}
