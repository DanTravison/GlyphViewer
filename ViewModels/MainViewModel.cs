namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
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
    string _selectedFontFamily;
    GlyphCollection _glyphs;
    Glyph _selectedGlyph;
    GlyphMetrics _selectedGlyphMetrics;
    double _glyphFontSize = 32.0;
    readonly IDispatcher _dispatcher;
    int _row;
    int _rows;
    ICommand _pickUnicodeRangeCommand;
    FontMetricsProperties _fontProperties;

    #endregion Fields

    public MainViewModel(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    #region Properties

    /// <summary>
    /// Gets the preferred size of the GlyphView width.
    /// </summary>
    public double GlyphWidth
    {
        get => 300;
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
                SelectedFontFamily = null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected font family.
    /// </summary>
    /// <remarks>
    /// This property is updated by FontFamiliesView.
    /// </remarks>
    public string SelectedFontFamily
    {
        get => _selectedFontFamily;
        set
        {
            if (SetProperty(ref _selectedFontFamily, value, StringComparer.CurrentCulture, SelectedFontChangedEventArgs))
            {
                Task.Run(() => { LoadGlyphs(_dispatcher); });
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="FontMetricsProperties"/> for the <see cref="SelectedFontFamily"/>.
    /// </summary>
    public FontMetricsProperties FontMetrics
    {
        get => _fontProperties;
        set => SetProperty(ref _fontProperties, value, ReferenceComparer, FontMetricsChangedEventArgs);
    }

    #endregion Font Families

    #region Glyph Properties

    /// <summary>
    /// Gets the <see cref="GlyphCollection"/> for the selected font family.
    /// </summary>
    /// <remarks>
    /// This property is updated by LoadGlyphs and consumed by GlyphsView.
    /// </remarks>
    public GlyphCollection Glyphs
    {
        get => _glyphs;
        private set
        {
            if (SetProperty(ref _glyphs, value, ReferenceComparer, GlyphsChangedEventArgs))
            {
                SelectedGlyph = Glyph.Empty;
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected <see cref="Glyph"/>.
    /// </summary>
    /// <remarks>This property is updated by the GlyphsView.</remarks>
    public Glyph SelectedGlyph
    {
        get => _selectedGlyph;
        set => SetProperty(ref _selectedGlyph, value, SelectedGlyphChangedEventArgs);
    }

    /// <summary>
    /// Gets or sets the <see cref="GlyphMetrics"/> for the <see cref="SelectedGlyph"/>.
    /// </summary>
    /// <remarks>This property is update by GlyphsView.</remarks>
    public GlyphMetrics SelectedGlyphMetrics
    {
        get => _selectedGlyphMetrics;
        set
        {
            _selectedGlyphMetrics = value;
            if (!value.IsEmpty)
            {
                SelectedGlyphProperties = new GlyphMetricProperties(value);
            }
            else
            {
                SelectedGlyphProperties = null;
            }
            OnPropertyChanged(SelectedGlyphPropertiesChangedEventArgs);
        }
    }

    /// <summary>
    /// Gets the <see cref="GlyphMetricProperties"/> for the <see cref="SelectedGlyph"/>.
    /// </summary>
    /// <remarks>
    /// This property is updated by <see cref="SelectedGlyphMetrics"/>
    /// and consumed by the MetricsView.
    /// </remarks>
    public GlyphMetricProperties SelectedGlyphProperties
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the font size for displaying glyphs in the GlyphsView.
    /// </summary>
    public double GlypFontSize
    {
        get => _glyphFontSize;
        set => SetProperty(ref _glyphFontSize, value, GlyphFontSizeChangedEventArgs);
    }

    #endregion Selected Glyph Properties

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
        FontFamilyGroupCollection families = FontFamilyGroupCollection.CreateInstance();
        _ = dispatcher.DispatchAsync(() =>
        {
            FontFamilyGroups = families;
        });
    }

    public void LoadGlyphs(IDispatcher dispatcher)
    {
        GlyphCollection glyphs = null;
        FontMetricsProperties fontMetrics = null;
        if (!string.IsNullOrWhiteSpace(SelectedFontFamily))
        {
            using (SKTypeface typeface = SKTypeface.FromFamilyName
            (
                SelectedFontFamily,
                SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright
            ))
            {
                glyphs = GlyphCollection.CreateInstance(typeface);
                fontMetrics = new FontMetricsProperties(typeface, (float)GlypFontSize);
            }
        }
        _ = dispatcher.DispatchAsync(() =>
        {
            FontMetrics = fontMetrics;
            Glyphs = glyphs;
        });
    }

    #endregion Font Info Loading

    #region PropertyChangedEventArgs

    static readonly PropertyChangedEventArgs FontFamiliesChangedEventArgs = new(nameof(FontFamilyGroups));
    static readonly PropertyChangedEventArgs SelectedFontChangedEventArgs = new(nameof(SelectedFontFamily));
    static readonly PropertyChangedEventArgs FontMetricsChangedEventArgs = new(nameof(FontMetrics));
    static readonly PropertyChangedEventArgs GlyphsChangedEventArgs = new(nameof(Glyphs));
    static readonly PropertyChangedEventArgs GlyphFontSizeChangedEventArgs = new(nameof(GlypFontSize));

    public static readonly PropertyChangedEventArgs SelectedFamilyGroupChangedEventArgs = new(nameof(SelectedFamilyGroup));

    static readonly PropertyChangedEventArgs SelectedGlyphChangedEventArgs = new(nameof(SelectedGlyph));
    static readonly PropertyChangedEventArgs SelectedGlyphPropertiesChangedEventArgs = new(nameof(SelectedGlyphProperties));

    static readonly PropertyChangedEventArgs UnicodeRangesChangedEventArgs = new(nameof(UnicodeRanges));
    static readonly PropertyChangedEventArgs SelectedUnicodeRangeChangedEventArgs = new(nameof(SelectedUnicodeRange));
    static readonly PropertyChangedEventArgs PickUnicodeRangeCommandChangedEventArgs = new(nameof(PickUnicodeRangeCommand));

    static readonly PropertyChangedEventArgs RowChangedEventArgs = new(nameof(Row));
    static readonly PropertyChangedEventArgs RowsChangedEventArgs = new(nameof(Rows));
    static readonly PropertyChangedEventArgs MaxRowChangedEventArgs = new(nameof(MaxRow));

    #endregion PropertyChangedEventArgs
}
