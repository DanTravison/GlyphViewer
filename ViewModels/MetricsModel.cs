namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;
using GlyphViewer.Settings.Properties;
using GlyphViewer.Text;
using SkiaSharp;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

/// <summary>
/// Provides a view model for managing <see cref="Glyph"/>, <see cref="FontFamily"/>, <see cref="FontSize"/>,
/// and the metrics and property collections.
/// </summary>
internal sealed class MetricsModel : ObservableObject, IDisposable
{
    #region Fields

    SKFont _font;
    string _fontFamily = string.Empty;
    FontMetricsProperties _fontProperties;

    Glyph _glyph = Glyph.Empty;
    GlyphMetricProperties _glyphProperties;

    readonly Command _clipboardCommand;
    readonly FontSizeProperty _fontSize;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="fontSize">The <see cref="FontSetting.FontSize"/> from <see cref="ItemFontSetting"/>.
    /// <para>
    /// This parameter is used to determine the font size when calculating metrics.
    /// </para>
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fontSize"/> is less than <see cref="ItemFontSetting.MinimumFontSize"/>.</exception>
    public MetricsModel(FontSizeProperty fontSize)
    {
        _fontSize = fontSize;
        _fontSize.PropertyChanged += OnFontSizeChanged;
        _fontFamily = string.Empty;
        _glyph = Glyph.Empty;

        ClipboardCommand = _clipboardCommand = new Command(OnCopyToClipboard)
        {
            IsEnabled = false
        };
    }

    #endregion Constructors

    #region Font Properties

    /// <summary>
    /// Gets the <see cref="SKFont"/> used to generate the <see cref="FontProperties"/> and <see cref="GlyphProperties"/>.
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
        get => _fontSize.Value;
    }

    /// <summary>
    /// Handles changes to <see cref="FontSizeProperty"/> of <see cref="ItemFontSetting"/>.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> identifying the property that changed.</param>
    private void OnFontSizeChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ReferenceEquals(e, ObservableProperty.ValueChangedEventArgs))
        {
            OnPropertyChanged(FontSizeChangedEventArgs);
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

    #region Clipboard

    /// <summary>
    /// Gets the <see cref="ICommand"/> to copy the metrics to the clipboard.
    /// </summary>
    public ICommand ClipboardCommand
    {
        get;
    }

    /// <summary>
    /// Copies the metrics properties to the clipboard.
    /// </summary>
    void OnCopyToClipboard()
    {
        if (!_glyph.IsEmpty)
        {
            string text = ToClipboardString();
            _ = Clipboard.Default.SetTextAsync(text);
         }
    }

    static void Append(StringBuilder sb, IEnumerable<NamedValue> properties)
    {
        foreach (NamedValue property in properties)
        {
            sb.AppendLine($"\t{property.Name}: {property.Value}");
        }
    }

    /// <summary>
    /// Returns a string that represents this instance.
    /// </summary>
    /// <returns>
    /// A new line delimited string containing the name:value pairs of <see cref="GlyphMetricProperties"/>
    /// <see cref="FontMetricsProperties"/>.
    /// </returns>
    string ToClipboardString()
    {
        StringBuilder sb = new();

        sb.AppendLine("Glyph Metrics");
        Append(sb, _glyphProperties.Properties);
        Append(sb, _glyphProperties.ExtendedProperties);

        sb.AppendLine(); 
        sb.AppendLine("Font Metrics");
        foreach (NamedValue property in _fontProperties)
        {
            sb.AppendLine($"\t{property.Name}: {property.Value}");
        }
        return sb.ToString();
    }

    #endregion Clipboard

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
        Glyph = 1 << 2,
        GlyphMetrics = 1 << 3,

        GlyphProperties = Glyph | GlyphMetrics,
        FontProperties = FontFamily | FontSize,
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
            changed.HasFlag(ChangedProperty.GlyphMetrics))
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
        if (changed == ChangedProperty.FontFamily)
        {
            // reset the glyph
            _glyph = Glyph.Empty;
            changes.Set(ChangedProperty.Glyph);
        }
        else if (changed == ChangedProperty.FontSize)
        {
            // Recalculate the glyph properties
            changes.Set(ChangedProperty.GlyphMetrics);
        }

        if (changes.IsSet(ChangedProperty.Glyph))
        {
            // Recalculate the glyph properties
            changes.Set(ChangedProperty.GlyphMetrics);
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
            // FontProperties are updated if FontFamily or FontSize changed.
            OnPropertyChanged(FontPropertiesChangedEventArgs);
            OnPropertyChanged(FontChangedEventArgs);
        }

        if (changes.IsSet(ChangedProperty.Glyph))
        {
            OnPropertyChanged(GlyphChangedEventArgs);
        }

        if (changes.IsSet(ChangedProperty.GlyphMetrics))
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
            _font = FontFamily.CreateFont((float)_fontSize.Value);
            _fontProperties = new FontMetricsProperties(_font);
        }
        else
        {
            _fontProperties = null;
        }
    }

    void UpdateGlyph()
    {
        _clipboardCommand.IsEnabled = !_glyph.IsEmpty;
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
    public static readonly PropertyChangedEventArgs FontSizeChangedEventArgs = new(nameof(FontSize));

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
