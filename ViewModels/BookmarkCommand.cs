namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;
using GlyphViewer.Text;
using System.Collections.Specialized;
using System.ComponentModel;

/// <summary>
/// Provides a <see cref="Command"/> for setting or clearing a font family bookmark.
/// </summary>
internal sealed class BookmarkCommand : Command
{
    #region Fields

    readonly Bookmarks _bookmarks;
    readonly MetricsViewModel _metrics;
    bool _isBookmarked;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this command.
    /// </summary>
    /// <param name="settings">The <see cref="UserSettings"/> to use for bookmarks.</param>
    /// <param name="metrics">The <see cref="MetricsViewModel"/> to use for the current font family.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="settings"/> or <paramref name="metrics"/> is a null reference.
    /// </exception>
    public BookmarkCommand(UserSettings settings, MetricsViewModel metrics)
        : base(NopAction)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        ArgumentNullException.ThrowIfNull(metrics, nameof(metrics));

        _bookmarks = settings.Bookmarks;
        _bookmarks.CollectionChanged += OnBookmarksChanged;
        _metrics = metrics;
        _metrics.PropertyChanged += OnMetricsPropertyChanged;
        IsEnabled = false;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the current font family name.
    /// </summary>
    public FontFamily FontFamily
    {
        get => _metrics.FontFamily;
    }

    /// <summary>
    /// Gets the value indicating if the <see cref="FontFamily"/> is bookmarked.
    /// </summary>
    public bool IsBookmarked
    {
        get => _isBookmarked;
        private set => SetProperty(ref _isBookmarked, value, IsBookmarkedChangedEventArgs);
    }

    #endregion Properties

    #region Event Handlers

    private void OnBookmarkChanged()
    {
        IsBookmarked = FontFamily is not null && _bookmarks.Contains(FontFamily);
    }

    private void OnFamilyNameChanged()
    {
        IsEnabled = _metrics.FontFamily is not null;
        OnBookmarkChanged();
    }

    private void OnMetricsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ReferenceEquals(e, MetricsViewModel.FontFamilyChangedEventArgs))
        {
            OnFamilyNameChanged();
        }
    }

    private void OnBookmarksChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnBookmarkChanged();
    }

    #endregion Event Handlers

    #region Execute

    /// <summary>
    /// Adds or removes the <see cref="FontFamily"/> from the bookmarks.
    /// </summary>
    /// <param name="_">Not used.</param>
    public override sealed void Execute(object _)
    {
        if (FontFamily is not null)
        {
            if (IsBookmarked)
            {
                _bookmarks.Remove(FontFamily);
            }
            else
            {
                _bookmarks.Add(FontFamily);
            }
            IsBookmarked = _bookmarks.Contains(FontFamily);
        }
    }

    #endregion Execute

    #region PropertyChangedEventArgs

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="FontFamily"/> changes.
    /// </summary>
    static readonly PropertyChangedEventArgs FamilyNameChangedEventArgs = new(nameof(FontFamily));
    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="Glyph"/> changes.
    /// </summary>
    static readonly PropertyChangedEventArgs IsBookmarkedChangedEventArgs = new(nameof(IsBookmarked));

    #endregion PropertyChangedEventArgs
}
