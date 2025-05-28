namespace GlyphViewer.Text;

using GlyphViewer.Diagnostics;

using SkiaSharp;

/// <summary>
/// Provides a <see cref="FontFamily"/> for a font file.
/// </summary>
internal class FileFontFamily : FontFamily
{
    FileInfo _file;

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
    /// Overrides <see cref="FontFamily.Load"/> to load a typeface from a file.
    /// </summary>
    /// <param name="style">Ignored.</param>
    /// <returns>The <see cref="SKTypeface"/> for the file; otherwise, a null reference.</returns>
    protected override SKTypeface Load(SKFontStyle style)
    {
        if (_file is not null && _file.Exists)
        {
            try
            {
                return SKTypeface.FromFile(_file.FullName);
            }
            catch (Exception ex)
            {
                Trace.Exception(this, nameof(Load), ex, "Failed to load typeface from file '{0}'", _file.FullName);
            }
        }
        _file = null;
        return null;
    }
}
