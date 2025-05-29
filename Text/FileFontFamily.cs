namespace GlyphViewer.Text;

using GlyphViewer.Diagnostics;

using SkiaSharp;

/// <summary>
/// Provides a <see cref="FontFamily"/> for a font file.
/// </summary>
internal class FileFontFamily : FontFamily
{
    static readonly object _lock = new();
    static readonly Dictionary<string, SKTypeface> _typefaces;
    FileInfo _file;
    private SKTypeface _typeface;

    static FileFontFamily()
    {
        IEqualityComparer<string> comparer;
#if WINDOWS
        comparer = StringComparer.OrdinalIgnoreCase;    
#else
        comparer = StringComparer.CurrentCulture;
#endif
        _typefaces = new Dictionary<string, SKTypeface>(comparer);
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="file">The <see cref="FileInfo"/> for the file.</param>
    public FileFontFamily(FileInfo file)
        : base(file.Name)
    {
        _file = file;
    }

    /// <summary>
    /// Gets the <see cref="Uri"/> for the font file.
    /// </summary>
    /// <value>
    /// The <see cref="Uri"/> for the font file; otherwise, a null reference if the file does not exist
    /// or a previous load failed.
    /// </value>
    public string FilePath
    {
        get
        {
            if (_file is not null && _file.Exists)
            {
                return _file.FullName;
            }
            return null;
        }
    }

    /// <summary>
    /// Gets the value indicating if the font file exists.
    /// </summary>
    /// <value>
    /// true if the font file exists; otherwise, false if the file does not exist
    /// or a previous load failed.
    /// </value>
    public bool Exists
    {
        get
        {
            return _file is not null && _file.Exists;
        }
    }

    /// <summary>
    /// Gets the <see cref="SKTypeface"/> for the font family.
    /// </summary>
    /// <value>
    /// The <see cref="SKTypeface"/> for the font family; otherwise, a null reference if 
    /// the font file does not exist or could not be loaded.
    /// </value>
    public override SKTypeface GetTypeface(SKFontStyle style)
    {
        lock (_lock)
        {
            // NOTE: File-based fonts do not support styles, so we can cache the typeface.
            if (_typeface is null && _file is not null && !_typefaces.TryGetValue(_file.FullName, out _typeface))
            {
                try
                {
                    _typeface = SKTypeface.FromFile(_file.FullName);
                    _typefaces.Add(_file.FullName, _typeface);
                }
                catch (Exception ex)
                {
                    Trace.Exception(this, nameof(GetTypeface), ex, "Failed to load typeface from file '{0}'", _file.FullName);
                    _file = null;
                }
                
            }
            return _typeface;
        }
    }
}
