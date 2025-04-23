namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;
using GlyphViewer.Text;
using SkiaSharp;
using System.ComponentModel;

/// <summary>
/// Provides a view model for managing <see cref="Glyph"/>, <see cref="FontFamily"/>, <see cref="FontSize"/>,
/// and the metrics and property collections.
/// </summary>
internal sealed class MetricsModel : ObservableObject, IDisposable
{
    #region Fields

    SKFont _font;
    string _fontFamily = string.Empty;
    double _fontSize;
    FontMetricsProperties _fontProperties;

    Glyph _glyph = Glyph.Empty;
    GlyphMetricProperties _glyphProperties;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="fontSize">The initial font size in points.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fontSize"/> is less than <see cref="UserSettings.MinimumItemFontSize"/>.</exception>
    public MetricsModel(double fontSize)
    {
        if (fontSize < UserSettings.MinimumItemFontSize)
        {
            throw new ArgumentOutOfRangeException(nameof(fontSize), fontSize, $"Font size must be greater than {UserSettings.MinimumItemFontSize}.");
        }
        _fontFamily = string.Empty;
        _glyph = Glyph.Empty;
        _fontSize = fontSize;
    }

    #endregion Constructors

    #region Font Properties

    /// <summary>
    /// Gets the <see cref="SKFont"/> used to generate the <see cref="FontMetrics"/> and <see cref="GlyphMetrics"/>.
    /// </summary>
    /// <value>
    /// The <see cref="SKFont"/> for the <see cref="FontFamily"/> and <see cref="FontSize"/>; otherwise,
    /// a null reference if the <see cref="FontFamily"/> is not set.
    /// </value>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="FontChangedEventArgs"/>.
    /// </remarks>
    public SKFont Font
    {
        get => _font;
        set => SetProperty(ref _font, value, ReferenceComparer, FontChangedEventArgs);
    }

    /// <summary>
    /// The <see cref="SKTypeface"/> for the current <see cref="Font"/>.
    /// </summary>
    /// <value>see cref="SKTypeface"/> for the current <see cref="Font"/>; otherwise, 
    /// a null reference if the <see cref="Font"/> is not set.</value>
    public SKTypeface Typeface
    {
        get => _font?.Typeface;
    }

    /// <summary>
    /// Gets or sets the font size in points.
    /// </summary>
    /// <value>
    /// The font size in points; otherwise, 0 if the font size has not been set.
    /// </value>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="FontSizeChangedEventArgs"/>.
    /// </remarks>
    public double FontSize
    {
        get => _fontSize;
        set
        {
            if (value != _fontSize)
            {
                _fontSize = value;
                Update(ChangedProperty.FontSize);
            }
        }
    }

    /// <summary>
    /// Gets the font family name.
    /// </summary>
    /// <value>The font family name; otherwise, <see cref="String.Empty"/> if the family name has not been set.</value>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="FontFamilyChangedEventArgs"/>.
    /// </remarks>
    public string FontFamily
    {
        get => _fontFamily;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }
            if (value != _fontFamily)
            {
                _fontFamily = value;
                Update(ChangedProperty.FontFamily);
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="FontMetricsProperties"/> for the <see cref="FontFamily"/>.
    /// </summary>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="FontPropertiesChangedEventArgs"/>.
    /// </remarks>
    public FontMetricsProperties FontProperties
    {
        get => _fontProperties;
    }

    #endregion Font Properties

    #region Glyph Properties

    /// <summary>
    /// Gets the current <see cref="Glyph"/>.
    /// </summary>
    /// <value>
    /// The current <see cref="Glyph"/>; otherwise,
    /// <see cref="Glyph.Empty"/> if the <see cref="Glyph"/> is not set.
    /// </value>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="GlyphChangedEventArgs"/>.
    /// </remarks>
    public Glyph Glyph
    {
        get => _glyph;
        set
        {
            if (value != _glyph)
            {
                _glyph = value;
                Update(ChangedProperty.Glyph);
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="GlyphMetricProperties"/> for the <see cref="Glyph"/>
    /// </summary>
    /// <value>
    /// The <see cref="GlyphMetricProperties"/> for the <see cref="Glyph"/>; otherwise, 
    /// <see cref="Glyph.Empty"/> if the <see cref="Glyph"/> is not set.
    /// </value>
    /// <remarks>
    /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with <see cref="GlyphPropertiesChangedEventArgs"/>.
    /// </remarks>
    public GlyphMetricProperties GlyphProperties
    {
        get => _glyphProperties;
    }

    #endregion Glyph Properties

    #region Update

    /// <summary>
    /// Provides flags to identify properties that need to be updated by <see cref="Update"/>.
    /// </summary>
    [Flags]
    enum ChangedProperty
    {
        None = 0,
        FontFamily = 1,
        FontSize = 1 << 1,
        Glyph = 1 << 3,

        FontProperties = FontFamily | FontSize,
        GlyphProperties = FontFamily | FontSize | Glyph
    }

    class PropertyChanges(MetricsModel.ChangedProperty changed)
    {
        public ChangedProperty Changed { get; } = changed;

        public ChangedProperty Changes
        {
            get;
            private set;
        } = changed;

        public void Set(ChangedProperty changed)
        {
            Changes |= changed;
        }

        public bool IsSet(ChangedProperty flag)
        {
            return (Changes & flag) == flag;
        }

        public bool ContainsAny(ChangedProperty flag)
        {
            return (Changes & flag) != 0;
        }
    }

    /// <summary>
    /// Updates the various properties.
    /// </summary>
    /// <param name="changed">The <see cref="ChangedProperty"/> indentifying the proeprty that changed.</param>
    void Update(ChangedProperty changed)
    {
        if 
        (
            changed.HasFlag(ChangedProperty.FontProperties) 
            ||
            changed.HasFlag(ChangedProperty.GlyphProperties))
        {
            // Internal error: Property implementations should only pass FontFamily, FontSize or Glyph 
            // flags. All others are handled here.
            throw new ArgumentOutOfRangeException(nameof(changed), changed, "Internal Error: Invalid property flag.");
        }

        PropertyChanges changes = new(changed);

        // NOTE: Before raising any PropertyChanged event,
        // update all  side-effect properties to ensure
        // PropertyChanged subscribers see a consistent state.
        // Additionally, avoid raising PropertyChanged events for 
        // properties that do not change.

        //
        // Determine which properties need to be updated.
        //
        if (changes.ContainsAny(ChangedProperty.FontProperties))
        {
            if (changed == ChangedProperty.FontFamily)
            {
                // reset the glyph
                _glyph = Glyph.Empty;
                changes.Set(ChangedProperty.Glyph);
            }
            if (changed == ChangedProperty.FontSize)
            {
                // reset the glyph properties
                changes.Set(ChangedProperty.GlyphProperties);
            }
        }

        if (changes.IsSet(ChangedProperty.Glyph))
        {
            changes.Set(ChangedProperty.GlyphProperties);
        }

        //
        // Update the necessary properties.
        //
        if (changes.ContainsAny(ChangedProperty.FontProperties))
        {
            UpdateFont();
        }
        if (changes.ContainsAny(ChangedProperty.GlyphProperties))
        {
            UpdateGlyph();
        }

        //
        // Raise PropertyChanged events for properties that changed.
        //
        if (changes.IsSet(ChangedProperty.FontFamily))
        {
            OnPropertyChanged(FontFamilyChangedEventArgs);
        }

        if (changes.IsSet(ChangedProperty.FontSize))
        {
            OnPropertyChanged(FontSizeChangedEventArgs);
        }

        if (changes.ContainsAny(ChangedProperty.FontProperties))
        {
            OnPropertyChanged(FontPropertiesChangedEventArgs);
        }

        if (changes.IsSet(ChangedProperty.Glyph))
        {
            OnPropertyChanged(GlyphChangedEventArgs);
        }

        if (changes.ContainsAny(ChangedProperty.GlyphProperties))
        {
            OnPropertyChanged(GlyphPropertiesChangedEventArgs);
        }
    }

    void UpdateFont()
    {
        _font?.Dispose();
        _font = null;
        if (!string.IsNullOrEmpty(_fontFamily))
        {
            _font = Fonts.CreateFont(FontFamily, (float)_fontSize);
            _fontProperties = new FontMetricsProperties(_font);
        }
        else
        {
            _fontProperties = null;
        }
    }

    void UpdateGlyph()
    {
        if (_font is null || _glyph.IsEmpty)
        {
            _glyphProperties = null;
        }
        else
        {
            using (SKPaint paint = new() { IsAntialias = true })
            {
                GlyphMetrics metrics = GlyphMetrics.CreateInstance(_glyph, _font, null);
                _glyphProperties = new GlyphMetricProperties(metrics);
            }
        }
    }

    #endregion Update

    #region IDisposable

    /// <summary>
    /// Releases the resources used by the <see cref="MetricsModel"/> class.
    /// </summary>
    public void Dispose()
    {
        if (_font is not null)
        {
            _font.Dispose();
            _font = null;

            _glyph = Glyph.Empty;
            _glyphProperties = null;
            _fontProperties = null;

            GC.SuppressFinalize(this);
        }
    }

    #endregion IDisposable

    #region PropertyChangedEventArgs

    /// <summary>
    /// The <see cref="PropertyChangedEventArgs"/> for the <see cref="Font"/> property.
    /// </summary>
    public static readonly PropertyChangedEventArgs FontChangedEventArgs = new(nameof(Font));

    /// <summary>
    /// The <see cref="PropertyChangedEventArgs"/> for the <see cref="FontFamily"/> property.
    /// </summary>
    public static readonly PropertyChangedEventArgs FontFamilyChangedEventArgs = new(nameof(FontFamily));

    /// <summary>
    /// The <see cref="PropertyChangedEventArgs"/> for the <see cref="FontSize"/> property.
    /// </summary>
    public static readonly PropertyChangedEventArgs FontSizeChangedEventArgs = new(nameof(Size));

    /// <summary>
    /// The <see cref="PropertyChangedEventArgs"/> for the <see cref="FontProperties"/> property.
    /// </summary>
    public static readonly PropertyChangedEventArgs FontPropertiesChangedEventArgs = new(nameof(FontProperties));

    /// <summary>
    /// The <see cref="PropertyChangedEventArgs"/> for the <see cref="Glyph"/> property.
    /// </summary>
    public static readonly PropertyChangedEventArgs GlyphChangedEventArgs = new(nameof(Glyph));

    /// <summary>
    /// The <see cref="PropertyChangedEventArgs"/> for the <see cref="GlyphProperties"/> property.
    /// </summary>
    public static readonly PropertyChangedEventArgs GlyphPropertiesChangedEventArgs = new(nameof(GlyphProperties));

    #endregion PropertyChangedEventArgs
}
