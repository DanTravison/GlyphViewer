using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GlyphViewer.Text.OpenType;

internal abstract class OpenTypeStruct<T> where T : struct
{
    private byte[] _buffer;

    protected OpenTypeStruct()
    {
        _buffer = new byte[Marshal.SizeOf<T>()];
    }

    public void ReadBytes(FontReader reader)
    {
        reader.ReadBytes(_buffer);
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

    // Helper for derived classes
    protected static int OffsetOf(string fieldName)
        => (int)Marshal.OffsetOf<T>(fieldName);
}

