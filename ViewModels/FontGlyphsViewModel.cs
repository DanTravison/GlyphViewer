namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Settings;
using GlyphViewer.Text;
using SkiaSharp;
using System.ComponentModel;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
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
    Command _writeFileCommand;
    Command _writeClipboardCommand;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="settings">The <see cref="UserSettings"/> to use for bookmarks.</param>
    /// <param name="metrics">The <see cref="MetricsViewModel"/> to use for the current font family.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="settings"/> or <paramref name="metrics"/> is a null reference.
    /// </exception>
    public FontGlyphsViewModel(UserSettings settings, MetricsViewModel metrics)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        ArgumentNullException.ThrowIfNull(metrics, nameof(metrics));

        Metrics = metrics;
        Search = new SearchViewModel(metrics);
        UserSettings = settings;
        _writeFileCommand = new(OnWriteFile)
        {
            IsEnabled = false
        };
        WriteFileCommand = _writeFileCommand;
        _writeClipboardCommand = new(OnWriteClipboard)
        {
            IsEnabled = false
        };
        WriteClipboardCommand = _writeClipboardCommand;
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
                OnContentChanged();
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
    public UserSettings UserSettings
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

    #region HasContent

    bool HasContent
    {
        get => Glyphs != null && Glyphs.Count > 0;
    }

    void OnContentChanged()
    {
        IsJumpListOpen = false;
        _writeClipboardCommand.IsEnabled = HasContent;
        _writeFileCommand.IsEnabled = HasContent;
        Search.Glyphs = Glyphs;
        Metrics.FontProperties.GlyphCount = Glyphs?.Count ?? 0;
    }

    #endregion HasContent

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

    #region Write Content

    /// <summary>
    /// Writes the <see cref="Glyphs"/> to a JSON file.
    /// </summary>
    public ICommand WriteFileCommand
    {
        get;
    }

    /// <summary>
    /// Writes the <see cref="Glyphs"/> to the clipboard as JSON.
    /// </summary>
    public ICommand WriteClipboardCommand
    {
        get;
    }

    async void OnWriteClipboard()
    {
        if (HasContent)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteContent(stream);
                string json = Encoding.UTF8.GetString(stream.ToArray());
                await Clipboard.Default.SetTextAsync(json);
            }
        }
    }

    async void OnWriteFile()
    {
        if (HasContent)
        {
            string fileName = Metrics.FontFamily + ".json";
            using (MemoryStream stream = new MemoryStream())
            {
                WriteContent(stream);
                stream.Position = 0;
                FileInfo info = await App.SaveAs(fileName, stream);
            }
        }
    }

    /// <summary>
    /// Writes the <see cref="FontMetricsProperties"/> and <see cref="GlyphMetricProperties"/>
    /// for the <see cref="Glyphs"/> to stream as JSON using the current 
    /// <see cref="MetricsViewModel.Font"/>.
    /// </summary>
    /// <param name="stream">The stream to write the content.</param>
    void WriteContent(Stream stream)
    {
        GlyphCollection glyphs = Glyphs;
        SKFont font = Metrics.Font;

        FontMetricsProperties fontProperties = new(font);
        List<GlyphMetricProperties> glyphProperties = [];
        fontProperties.GlyphCount = glyphs.Count;

        for (int i = 0; i < Glyphs.Count; i++)
        {
            GlyphMetrics metric = GlyphMetrics.CreateInstance(Glyphs[i], font);
            GlyphMetricProperties properties = new(metric);
            glyphProperties.Add(properties);
        }

        // NOTE Using UnsafeRelaxedJsonEscaping to allow for non-escaped characters in the JSON output.
        // for example GlyphMetrics.Code has a string U+XXXX and the writer will escape the '+' character.
        JsonWriterOptions options = new()
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, options))
        {
            writer.WriteStartObject();

            writer.WriteStartObject(nameof(Metrics.Font));
            Write(writer, fontProperties);
            writer.WriteEndObject();

            writer.WriteStartArray(nameof(Glyphs));
            foreach (GlyphMetricProperties property in glyphProperties)
            {
                writer.WriteStartObject();
                Write(writer, property.Properties);
                Write(writer, property.ExtendedProperties);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
            writer.Flush();
        }
    }

    void Write(Utf8JsonWriter writer, IEnumerable<NamedValue> values)
    {
        foreach (NamedValue property in values)
        {
            if (property.Value is null)
            {
                continue;
            }
            else if (property.Value is float value)
            {
                writer.WriteNumber(property.Name, (float)property.Value);
            }
            else if (property.Value is string)
            {
                writer.WriteString(property.Name, (string)property.Value);
            }
            else if (property.Value is Enum)
            {
                string name = Enum.GetName(property.Value.GetType(), property.Value);
                writer.WriteString(property.Name, name);
            }
            else if (property.Value is bool)
            {
                writer.WriteBoolean(property.Name, (bool)property.Value);
            }
            else if (property.Value is int)
            {
                writer.WriteNumber(property.Name, (int)property.Value);
            }
            else
            {
                writer.WriteString(property.Name, property.Value.ToString());
            }
        }
    }

    #endregion Write Content

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
