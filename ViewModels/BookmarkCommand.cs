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
    /// <param name="bookmarks">The <see cref="Bookmarks"/> to update.</param>
    public BookmarkCommand(Bookmarks bookmarks, MetricsViewModel metrics)
        : base(NopAction)
    {
        _bookmarks = bookmarks;
        _bookmarks.CollectionChanged += OnBookmarksChanged;
        _metrics = metrics;
        _metrics.PropertyChanged += OnMetricsPropertyChanged;
        IsEnabled = false;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the current font family name.
    /// </summary>
    public string FamilyName
    {
        get => _metrics.FontFamily;
    }

    /// <summary>
    /// Gets the value indicating if the <see cref="FamilyName"/> is bookmarked.
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
        IsBookmarked = !string.IsNullOrEmpty(FamilyName) && _bookmarks.Contains(FamilyName);
    }

    private void OnFamilyNameChanged()
    {
        string familyName = _metrics.FontFamily;
        IsEnabled = !string.IsNullOrEmpty(familyName);
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
    /// Adds or removes the <see cref="FamilyName"/> from the bookmarks.
    /// </summary>
    /// <param name="_">Not used.</param>
    public override sealed void Execute(object _)
    {
        if (!string.IsNullOrEmpty(FamilyName))
        {
            if (IsBookmarked)
            {
                _bookmarks.Remove(FamilyName);
            }
            else
            {
                _bookmarks.Add(FamilyName);
            }
            IsBookmarked = _bookmarks.Contains(FamilyName);
        }
    }

    #endregion Execute

    #region PropertyChangedEventArgs

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="FamilyName"/> changes.
    /// </summary>
    static readonly PropertyChangedEventArgs FamilyNameChangedEventArgs = new(nameof(FamilyName));
    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="Glyph"/> changes.
    /// </summary>
    static readonly PropertyChangedEventArgs IsBookmarkedChangedEventArgs = new(nameof(IsBookmarked));

    #endregion PropertyChangedEventArgs
}
