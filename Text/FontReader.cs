namespace GlyphViewer.Text;

using GlyphViewer.Text.OpenType;
using SkiaSharp;
using System.Buffers.Binary;

/// <summary>
/// Provides a reader for reading a font from an <see cref="SKTypeface"/> or a file.
/// </summary>
public sealed class FontReader : IDisposable
{
    #region Fields

    IFontStream _stream;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="stream">The <see cref="IFontStream"/> to read.</param>
    private FontReader(IFontStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));
        _stream = stream;
        Faces = OpenTypeParser.ReadFontHeader(this);
        if (_stream.FaceIndex < 0 || _stream.FaceIndex >= Faces.Count)
        {
            throw new InvalidOperationException($"FaceIndex is outside the valid range of 0..{Faces.Count - 1}");
        }
        FaceIndex = stream.FaceIndex;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the default face index to use to enumerate glyphs
    /// </summary>
    /// <value>
    /// The face index to enumerate glyphs; otherwise, zero.
    /// </value>
    internal int FaceIndex
    {
        get;
    }

    /// <summary>
    /// Gets the offsets to each face in the font file.
    /// </summary>
    public IReadOnlyList<long> Faces
    {
        get;
    }

    /// <summary>
    /// Gets the current position in the content.
    /// </summary>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    internal long Position
    {
        get
        {
            ObjectDisposedException.ThrowIf(_stream is null, this);
            return _stream.GetPosition();
        }
    }

    /// <summary>
    /// Gets the length of the content.
    /// </summary>
    /// <value>
    /// The length of the content.
    /// </value>
    internal long Length
    {
        get
        {
            ObjectDisposedException.ThrowIf(_stream is null, this);
            return _stream.Length;
        }
    }

    /// <summary>
    /// Gets the value indicating if the end of the content has been reached.
    /// </summary>
    /// <value>true if the end of the content has been reached; otherwise, false.</value>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    internal bool IsEndOfContent
    {
        get
        {
            return Position >= Length;
        }
    }

    #endregion Properties

    #region public methods

    /// <summary>
    /// Gets the glyph names for the font.
    /// </summary>
    /// <returns>
    /// An <see cref="IReadOnlyDictionary{Uint32, String}"/> of glyph id to string;
    /// otherwise a null refernce if the font does not define glyph names.
    /// </returns>
    public IReadOnlyDictionary<uint, string> GetGlyphNames()
    {
        return GetGlyphNames(FaceIndex);
    }

    /// <summary>
    /// Gets the glyph names for the font.
    /// </summary>
    /// <returns>
    /// An <see cref="IReadOnlyDictionary{Uint32, String}"/> of glyph id to string;
    /// otherwise a null refernce if the font does not define glyph names.
    /// </returns>
    public IReadOnlyDictionary<uint, string> GetGlyphNames(int faceIndex)
    {
        if (faceIndex < -0 || faceIndex >= Faces.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(faceIndex), faceIndex, null);
        }
        return OpenTypeParser.ReadGlyphNames(this, faceIndex);
    }

    public IEnumerable<GlyphInfo> GetGlyphs(int faceIndex)
    {
        if (faceIndex < -0 || faceIndex >= Faces.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(faceIndex), faceIndex, null);
        }
        return OpenTypeParser.GetGlyphs(this, faceIndex);
    }

    public IEnumerable<GlyphInfo> GetGlyphs()
    {
        return GetGlyphs(FaceIndex);
    }

    #endregion public methods

    #region Seek

    /// <summary>
    /// Seeks the <see cref="Position"/> in the underlying content.
    /// </summary>
    /// <param name="position">The zero-based absolute position to seek.</param>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="position"/> is
    /// less than zero or greater than <see cref="Length"/></exception>
    internal void Seek(long position)
    {
        ObjectDisposedException.ThrowIf(_stream is null, this);
        if (position < 0 || position > Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }
        _stream.SetPosition(position);
    }

    /// <summary>
    /// Skips the count of bytes in the underling content.
    /// </summary>
    /// <param name="count">The number of bytes to skip.</param>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <see cref="Position"/> plus the 
    /// <paramref name="count"/> exceeds the <see cref="Length"/>.</exception>
    internal void Skip(long count)
    {
        ObjectDisposedException.ThrowIf(_stream is null, this);
        if (count < 0 || count + Position > Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, null);
        }
        _stream.SetPosition(count + Position);
    }

    #endregion Seek

    #region Read

    /// <summary>
    /// Read 4 bytes as a tag.
    /// </summary>
    /// <param name="buffer">The <see cref="Span{Byte}"/> to populate.</param>
    /// <returns>
    /// true if the <paramref name="buffer"/> was populated with valid <see cref="Tag"/> content; 
    /// otherwise, false.
    /// </returns>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="buffer"/> must have a length of 4.</exception>
    internal bool ReadTag(scoped Span<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(_stream is null, this);
   
        if (Position + 4 > Length)
        {
            throw new EndOfStreamException();
        }
        if (buffer.Length != 4)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), "The buffer must have a length of 4");
        }
        _stream.ReadBytes(buffer);
        return Tag.Validate(buffer);
    }

    /// <summary>
    /// Reads the next 4 bytes as a <see cref="Tag"/> and verifies the expected value.
    /// </summary>
    /// <param name="tag">The expected <see cref="Tag"/>.</param>
    /// <returns>true if the expected tag was read; otherwise, false.</returns>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    /// <exception cref="EndOfStreamException">The end of the content has been reached.</exception>
    internal bool ReadTag(Tag tag)
    {
        ObjectDisposedException.ThrowIf(_stream is null, this);
        if (Position + 4 > Length)
        {
            throw new EndOfStreamException();
        }
        Span<byte> buffer = stackalloc byte[4];
        _stream.ReadBytes(buffer);
        return tag.AsSpan().SequenceEqual(buffer);
    }

    /// <summary>
    /// Reads a <see cref="UInt16"/> from the content.
    /// </summary>
    /// <returns>The <see cref="UInt16"/> value.</returns>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    /// <exception cref="EndOfStreamException">The end of the content has been reached.</exception>
    internal ushort ReadUInt16()
    {
        ObjectDisposedException.ThrowIf(_stream is null, this);
        if (Position + 2 > Length)
        {
            throw new EndOfStreamException();
        }
        Span<byte> buffer = stackalloc byte[2];
        _stream.ReadBytes(buffer);
        return BinaryPrimitives.ReadUInt16BigEndian(buffer);
    }

    /// <summary>
    /// Reads a <see cref="UInt16"/> from the content at the specified offset.
    /// </summary>
    /// <param name="offset">The zero-based offset to read from.</param>
    /// <returns>The <see cref="UInt16"/> value.</returns>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    /// <exception cref="EndOfStreamException">The end of the content has been reached.</exception>
    internal ushort ReadUInt16(long offset)
    {
        Seek(offset);
        return ReadUInt16();
    }

    /// <summary>
    /// Reads a <see cref="UInt32"/> from the content.
    /// </summary>
    /// <returns>The <see cref="UInt32"/> value.</returns>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    /// <exception cref="EndOfStreamException">The end of the content has been reached.</exception>
    internal uint ReadUInt32()
    {
        ObjectDisposedException.ThrowIf(_stream is null, this);
        if (Position + 4 > Length)
        {
            throw new EndOfStreamException();
        }

        Span<byte> buffer = stackalloc byte[4];
        _stream.ReadBytes(buffer);
        return BinaryPrimitives.ReadUInt32BigEndian(buffer);
    }

    /// <summary>
    /// Reads a <see cref="UInt32"/> from the content from the specified offset.
    /// </summary>
    /// <param name="offset">The zero-based offset to read from.</param>
    /// <returns>The <see cref="UInt32"/> value.</returns>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    /// <exception cref="EndOfStreamException">The end of the content has been reached.</exception>
    internal uint ReadUInt32(long offset)
    {
        Seek(offset);
        return ReadUInt32();
    }

    /// <summary>
    /// Reads bytes from the content.
    /// </summary>
    /// <param name="buffer">The buffer to populate.
    /// <para>
    /// The buffer length indicates the number of bytes to read.
    /// </para>
    /// </param>
    /// <exception cref="ObjectDisposedException">This instance is disposed.</exception>
    /// <exception cref="EndOfStreamException">The end of the content has been reached.</exception>
    internal void ReadBytes(scoped Span<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(_stream is null, this);
        if (Position + buffer.Length > Length)
        {
            throw new EndOfStreamException();
        }
        _stream.ReadBytes(buffer);
    }

    /// <summary>
    /// Reads bytes as an ASCII string.
    /// </summary>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The string value.</returns>
    internal string ReadText(int length)
    {
        Span<byte> buffer = stackalloc byte[length];
        _stream.ReadBytes(buffer);
        return System.Text.Encoding.ASCII.GetString(buffer);
    }

    #endregion Read

    #region Dispose

    /// <summary>
    /// Releases all resources.
    /// </summary>
    public void Dispose()
    {
        if (_stream is not null)
        {
            IFontStream stream = _stream;
            _stream = null;
            stream.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    #endregion Dispose

    #region CreateInstance

    /// <summary>
    /// Creates a <see cref="FontReader"/> for a file-based font.
    /// </summary>
    /// <param name="filepath">The fully qualified path to the font file.</param>
    /// <returns>A new instance of a <see cref="FontReader"/>.</returns>
    public static FontReader CreateInstance(string filepath)
    {
        FileFontReader reader = new(filepath);
        return new(reader);
    }

    /// <summary>
    /// Creates a <see cref="FontReader"/> for a file-based font.
    /// </summary>
    /// <param name="typeface">The <see cref="SKTypeface"/> to read.</param>
    /// <returns>A new instance of a <see cref="FontReader"/>.</returns>    
    public static FontReader CreateInstance(SKTypeface typeface)
    {
        SkFontReader reader = new(typeface);
        return new(reader);
    }

    #endregion CreateInstance

    internal interface IFontStream : IDisposable
    {
        /// <summary>
        /// Gets the length of the content.
        /// </summary>
        /// <value>The zero-based offset into the content.</value>
        long Length
        {
            get;
        }

        /// <summary>
        /// Gets the face index when the <see cref="IFontStream"/> was created.
        /// </summary>
        /// <value>The zero-based font face index; otherwise, zero.</value>
        /// <remarks>
        /// This property is used purely for validate that the ttcIndex returned
        /// by <see cref="SKTypeface.OpenStream(out int)"/> is consistent with the number
        /// of font faces in the underlying font-collection.
        /// <para>
        /// The default value is zero.
        /// </para>
        /// </remarks>
        int FaceIndex
        {
            get;
        }

        /// <summary>
        /// Gets the current position in the underlying stream.
        /// </summary>
        /// <returns></returns>
        long GetPosition();

        /// <summary>
        /// Sets the current position in the underlying stream.
        /// </summary>
        /// <param name="position">The zero-based absolute position to set.</param>
        void SetPosition(long position);

        /// <summary>
        /// Implement in the derived class to read bytes from the stream.
        /// </summary>
        /// <param name="buffer">A buffer to populate where the buffer's length
        /// indicates the number of bytes to read.
        /// </param>
        void ReadBytes(scoped Span<byte> buffer);
    }
}


/// <summary>
/// Provides a <see cref="FontReader"/> for reading an <see cref="SKTypeface"/> stream.
/// </summary>
sealed class SkFontReader : FontReader.IFontStream
{
    #region Fields

    private SKStreamAsset _stream;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="typeface">The <see cref="SKTypeface"/> to consume.</param>
    public SkFontReader(SKTypeface typeface)
    {
        _stream = typeface.OpenStream(out int faceIndex);
        Length = _stream.Length;
        FaceIndex = faceIndex;
    }

    #endregion Constructors

    #region IFontStream

    /// <summary>
    /// Gets the length of the content.
    /// </summary>
    public long Length
    {
        get;
    }

    /// <summary>
    /// Gets the default face index for this reader.
    /// </summary>
    public int FaceIndex
    {
        get;
    }

    /// <summary>
    /// Gets the current <see cref="FontReader.Position"/> in the content.
    /// </summary>
    public long GetPosition()
    {
        return _stream.Position;
    }


    /// <summary>
    /// Sets the current <see cref="FontReader.Position"/> in the content.
    /// </summary>
    public void SetPosition(long position)
    {
        if (position > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }
        _stream.Seek((int)position);
    }

    /// <summary>
    /// Read bytes from the stream.
    /// </summary>
    /// <param name="buffer">A buffer to populate where the buffer's length
    /// indicates the number of bytes to read.
    /// </param>   
    public void ReadBytes(scoped Span<byte> buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = _stream.ReadByte();
        }
    }

    #endregion IFontStream

    #region IDisposable

    /// <summary>
    /// Releases the underlying stream.
    /// </summary>
    public void Dispose()
    {
        if (_stream is not null)
        {
            _stream.Dispose();
            _stream = null;
            GC.SuppressFinalize(this);
        }
    }

    #endregion IDisposable
}

/// <summary>
/// Provides a <see cref="FontReader"/> for a file-based font.
/// </summary>
sealed class FileFontReader : FontReader.IFontStream
{
    #region Fields

    FileStream _stream;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="path">The fully qualified path to the font file.</param>
    public FileFontReader(string path)
    {
        _stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        Length = _stream.Length;
    }

    #endregion Constructors

    #region IFontStream

    /// <summary>
    /// Gets the length of the content.
    /// </summary>
    public long Length
    {
        get;
    }

    /// <summary>
    /// Gets the face index selected when the reader was created.
    /// </summary>
    /// <value>
    /// This property always returns zero.
    /// </value>
    public int FaceIndex
    {
        get;
    } = 0;

    /// <summary>
    /// Gets the current <see cref="FontReader.Position"/> in the content.
    /// </summary>
    public long GetPosition()
    {
        return _stream.Position;
    }

    /// <summary>
    /// Sets the current <see cref="FontReader.Position"/> in the content.
    /// </summary>
    public void SetPosition(long position)
    {
        _stream.Seek(position, SeekOrigin.Begin);
    }

    /// <summary>
    /// Read bytes from the stream.
    /// </summary>
    /// <param name="buffer">A buffer to populate where the buffer's length
    /// indicates the number of bytes to read.
    /// </param>   
    public void ReadBytes(scoped Span<byte> buffer)
    {
        _stream.ReadExactly(buffer);
    }

    #endregion IFontStream

    #region IDisposable

    /// <summary>
    /// Releases the underlying stream.
    /// </summary>
    public void Dispose()
    {
        if (_stream is not null)
        {
            _stream.Dispose();
            _stream = null;
            GC.SuppressFinalize(this);
        }
    }

    #endregion IDisposable
}