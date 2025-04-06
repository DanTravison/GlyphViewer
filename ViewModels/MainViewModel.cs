namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Text;
using SkiaSharp;
using System.ComponentModel;

internal sealed class MainViewModel : ObservableObject
{
    #region Fields

    IEnumerable<string> _fontFamilies;
    string _selectedFontFamily;
    GlyphCollection _glyphs;
    Glyph _selectedGlyph;
    GlyphMetrics _selectedGlyphMetrics;
    readonly IDispatcher _dispatcher;
    int _row;
    int _rows;

    #endregion Fields

    public MainViewModel(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    #region Properties

    public int Row
    {
        get => _row;
        set => SetProperty(ref _row, value, RowChangedEventArgs);
    }

    /// <summary>
    /// Gets or sets the number of rows.
    /// </summary>
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
    public int MaxRow
    {
        get => _rows > 0 ? _rows - 1 : 0;
    }

    public IEnumerable<string> FontFamilies
    {
        get => _fontFamilies;
        private set
        {
            if (SetProperty(ref _fontFamilies, value, FontFamiliesChangedEventArgs))
            {
                SelectedFontFamily = null;
            }   
        }
    }

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

    public GlyphCollection Glyphs
    {
        get => _glyphs;
        private set
        {
            if (SetProperty(ref _glyphs, value, ReferenceEqualityComparer.Instance, GlyphsChangedEventArgs))
            {
                SelectedGlyph = Glyph.Empty;
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected <see cref="Glyph"/>.
    /// </summary>
    public Glyph SelectedGlyph
    {
        get => _selectedGlyph;
        set => SetProperty(ref _selectedGlyph, value, SelectedGlyphChangedEventArgs);
    }

    /// <summary>
    /// Gets or sets the <see cref="GlyphMetrics"/> for the <see cref="SelectedGlyph"/>.
    /// </summary>
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
    public GlyphMetricProperties SelectedGlyphProperties
    {
        get;
        private set;
    }

    #endregion Properties

    #region Font Info Loading

    public void LoadFonts(IDispatcher dispatcher)
    {
        List<string> fontFamilies = Fonts.GetFontFamilies();
        fontFamilies.Sort(StringComparer.OrdinalIgnoreCase);
        _ = dispatcher.DispatchAsync(() =>
        {
            FontFamilies = fontFamilies as IEnumerable<string>;
        });
    }

    public void LoadGlyphs(IDispatcher dispatcher)
    {
        GlyphCollection glyphs = null;
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
            }
        }
        _ = dispatcher.DispatchAsync(() =>
        {
            Glyphs = glyphs;
        });
    }

    #endregion Font Info Loading

    #region PropertyChangedEventArgs

    static readonly PropertyChangedEventArgs FontFamiliesChangedEventArgs = new(nameof(FontFamilies));
    static readonly PropertyChangedEventArgs SelectedFontChangedEventArgs = new(nameof(SelectedFontFamily));
    static readonly PropertyChangedEventArgs GlyphsChangedEventArgs = new(nameof(Glyphs));

    static readonly PropertyChangedEventArgs SelectedGlyphChangedEventArgs = new(nameof(SelectedGlyph));
    static readonly PropertyChangedEventArgs SelectedGlyphPropertiesChangedEventArgs = new(nameof(SelectedGlyphProperties));

    static readonly PropertyChangedEventArgs RowChangedEventArgs = new(nameof(Row));
    static readonly PropertyChangedEventArgs RowsChangedEventArgs = new(nameof(Rows));
    static readonly PropertyChangedEventArgs MaxRowChangedEventArgs = new(nameof(MaxRow));

    #endregion PropertyChangedEventArgs

}
