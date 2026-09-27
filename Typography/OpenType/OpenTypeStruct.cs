namespace CodeCadence.Typography.OpenType;

using CodeCadence.Typography.Text;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Provides a base class for open type structures that are read as a block.
/// </summary>
/// <typeparam name="T">The type of the structure.</typeparam>
internal abstract class OpenTypeStruct<T> where T : struct
{
    private byte[] _buffer;
    private bool _isInitialized;

    protected OpenTypeStruct()
    {
        _buffer = new byte[Marshal.SizeOf<T>()];
    }

    /// <summary>
    /// Reads the <typeparamref name="T"/> struct from the stream.
    /// </summary>
    /// <param name="reader">The <see cref="FontReader"/> to read.</param>
    /// <param name="startPosition">The zero-based offset into the font contents where the <typeparamref name="T"/> is located.</param>
    protected bool Initialize(FontReader reader, long startPosition)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException("Initialize may only be called once.");
        }
        _isInitialized = true;
        StartPosition = startPosition;
        if (startPosition + _buffer.Length > reader.Length)
        {
            return false;
        }
        reader.Seek(startPosition);
        reader.ReadBytes(_buffer);
        OnInitialize(reader);
        return true;
    }

    /// <summary>
    /// Override in the derived class to interpret the raw <typeparamref name="T"/> struct
    /// and perform any additional table-specific parsing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is called by <see cref="Initialize"/> after the raw <typeparamref name="T"/>
    /// bytes have been read.
    /// </para>
    /// 
    /// <para>
    /// When <see cref="OnInitialize"/> is invoked:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <see cref="FontReader.Position"/> is positioned immediately after the
    ///       <typeparamref name="T"/> struct in the font stream.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <see cref="StartPosition"/> contains the absolute position of the start of the
    ///       <typeparamref name="T"/> struct.
    ///     </description>
    ///   </item>
    /// </list>
    /// </para>
    /// 
    /// <para>
    /// Tables that parse additional data sequentially (e.g., <c>cmap</c>, <c>kern</c>,
    /// <c>GSUB</c>, <c>GPOS</c>) may continue reading from the current position.
    /// </para>
    /// 
    /// <para>
    /// Tables that require offsets relative to the start of the table (e.g., <c>name</c>,
    /// <c>post</c> format 2) should use <see cref="StartPosition"/> to seek back to the
    /// beginning before performing additional reads.
    /// </para>
    /// 
    /// <para>
    /// A typical static <c>Read(FontReader)</c> method in a derived class simply constructs
    /// the instance, calls <c>Initialize(reader, startOffset)</c>, and returns the instance.
    /// All table-specific processing should occur inside this method.
    /// </para>
    /// </remarks>
    protected virtual void OnInitialize(FontReader reader)
    {
        // default: do nothing
    }

    protected long StartPosition
    {
        get;
        private set;
    }

    // Strongly typed BigEndian accessors
    protected ushort ReadUInt16(int offset)
        => BinaryPrimitives.ReadUInt16BigEndian(_buffer.AsSpan(offset, 2));

    protected short ReadInt16(int offset)
        => BinaryPrimitives.ReadInt16BigEndian(_buffer.AsSpan(offset, 2));

    protected uint ReadUInt32(int offset)
        => BinaryPrimitives.ReadUInt32BigEndian(_buffer.AsSpan(offset, 4));

    protected int ReadInt32(int offset)
        => BinaryPrimitives.ReadInt32BigEndian(_buffer.AsSpan(offset, 4));

    protected float ReadSingle(int offset)
        => ReadInt32(offset) / 65536f;

    protected ReadOnlySpan<byte> ReadTag(int offset)
        => _buffer.AsSpan(offset, 4);

    protected bool ReadTag(int offset, Tag expected)
    {
        return ReadTag(offset).SequenceEqual(expected.AsSpan());
    }

    protected string ReadString(int offset, int count)
        => Encoding.ASCII.GetString(_buffer, offset, count);

    /// <summary>
    /// Gets bytes from the buffer as a strongly typed, blittable struct.
    /// </summary>
    /// <typeparam name="F">The type of the struct to get.</typeparam>
    /// <param name="offset">The offset into the buffer.</param>
    /// <returns>A <typeparamref name="F"/> struct.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The <paramref name="offset"/> plus the size of <typeparamref name="T"/>
    /// exceeds the size of the buffer.
    /// </exception>
    /// <exception cref="NotSupportedException"><typeparamref name="F"/> must be a pure, unmanaged struct with only primitive fields.</exception>
    protected F GetField<F>(int offset)
        where F : struct
    {
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<F>())
        {
            ReadOnlySpan<byte> span = GetBytes(offset, Marshal.SizeOf<F>());
            return MemoryMarshal.Read<F>(span);
        }
        throw new NotSupportedException
        (
            $"Type {typeof(F)} is not blittable and cannot be read from raw bytes."
        );
    }

    /// <summary>
    /// Reads bytes from the buffer into a <see cref="ReadOnlySpan{Char}"/>.
    /// </summary>
    /// <param name="offset">The offset into the buffer.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <returns>A <see cref="ReadOnlySpan{Char}"/> for the bytes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The <paramref name="offset"/> plus <paramref name="size"/> 
    /// exceeds the size of the buffer.
    /// </exception>
    protected ReadOnlySpan<byte> GetBytes(int offset, int size)
    {
        if (offset + size > _buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, $"The offset plus the request size exceeds the buffer length.");
        }

        return _buffer.AsSpan(offset, size);
    }

    // Helper for derived classes
    protected static int OffsetOf(string fieldName)
        => (int)Marshal.OffsetOf<T>(fieldName);
}

