namespace GlyphViewer.Diagnostics;

using System.Text.Json.Serialization;

/// <summary>
/// Provides trace levels for filtering trace output.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TraceFlag : int
{
    /// <summary>
    /// No tracing is output.
    /// </summary>
    None = 0,

    /// <summary>
    /// Error tracing is output.
    /// </summary>
    Error = 1 << 0,

    /// <summary>
    /// Warning tracing is output.
    /// </summary>
    Warning = 1 << 1,

    /// <summary>
    /// Informational tracing is output.
    /// </summary>
    Info = 1 << 2,

    /// <summary>
    /// Verbose tracing is output.
    /// </summary>
    Verbose = 1 << 3,

    /// <summary>
    /// Provides diagnostic level tracing; such as method entry and exit.
    /// </summary>
    Diagnostic = 1 << 4,

    /// <summary>
    /// Visual state event tracing.
    /// </summary>
    Performance = 1 << 5,

    /// <summary>
    /// Touch event tracing
    /// </summary>
    Touch = 1 << 6,

    /// <summary>
    /// Mouse event tracing
    /// </summary>
    Mouse = 1 << 7,

    /// <summary>
    /// Keyboard event tracing
    /// </summary>
    Keyboard = 1 << 8,

    /// <summary>
    /// Tracing for application life-cycle events.
    /// </summary>
    Application = 1 << 9,

    /// <summary>
    /// Tracing for page navigation
    /// </summary>
    Navigation = 1 << 10,

    /// <summary>
    /// Tracing for settings.
    /// </summary>
    Settings = 1 << 11,

    /// <summary>
    /// Tracing for command execution.
    /// </summary>
    Command = 1 << 12,

    /// <summary>
    /// Tracing for UI control operations.
    /// </summary>
    Control = 1 << 13,

    /// <summary>
    /// Tracing for Control DataContext changes.
    /// </summary>
    DataContext = 1 << 14,

    /// <summary>
    /// UI Layout event tracing
    /// </summary>
    Layout = 1 << 15,

    /// <summary>
    /// Tracing for drawing operations.
    /// </summary>
    Drawing = 1 << 16,

    /// <summary>
    /// Menu state changes
    /// </summary>
    Menu = 1 << 17,

    /// <summary>
    /// Tracing for Control selection changes.
    /// </summary>
    Selection = 1 << 18,

    /// <summary>
    /// Tracing for drag and drop operations.
    /// </summary>
    Drag = 1 << 19,

    /// <summary>
    /// Managed resource tracing.
    /// </summary>
    Resource = 1 << 20,

    /// <summary>
    /// Tracing for file and directory enumeration
    /// </summary>
    Storage = 1 << 21,

    /// <summary>
    /// Tracing for document actions.
    /// </summary>
    Document = 1 << 22,

    /// <summary>
    /// Tracing for device operations.
    /// </summary>
    Device = 1 << 23,

    /// <summary>
    /// Tracing for platform calls
    /// </summary>
    Platform = 1 << 24,

    #region Combined Flags

    /// <summary>
    /// Touch/Mouse event tracing
    /// </summary>
    Pointer = Mouse | Touch,

    /// <summary>
    /// Pointer, Mouse and Keyboard event tracing
    /// </summary> 
    Input = Pointer | Keyboard,

    /// <summary>
    /// Defines the default trace flags.
    /// </summary>
    Default = Error | Warning,

    #endregion Combined Flags
}
