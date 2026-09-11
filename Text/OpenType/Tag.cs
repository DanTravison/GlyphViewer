namespace GlyphViewer.Text.OpenType;

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides a struct for an Open Type tag.
/// </summary>
/// <remarks>
/// An Open Type tag is a 4 byte sequence of ASCII characters in the range of
/// 0x21 through 0x7E inclusive.
/// </remarks>
internal readonly struct Tag : IEquatable<Tag>
{
    private readonly string _text;     // original string, e.g. "cmap"
    private readonly byte[] _buffer;    // always length 4, ASCII

    /// <summary>
    /// Initializes a new instance of this struct.
    /// </summary>
    /// <param name="value">The text to encapsulate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is a null reference.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The text must be 4 characters in length.
    /// -or-
    /// <paramref name="value"/> contains a character that is not valid for a tag.
    /// </exception>
    public Tag(string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        if (value.Length != 4)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"{value} must be exactly 4 characters.");
        }

        _text = value;
        _buffer = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            char ch = value[i];
            // ASCII character range for tags.
            if (ch >= 0x21 && ch <= 0x7E)
            {
                _buffer[i] = (byte)ch;
            }
            else
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(value), value,
                    $"{value} has an invalid character at offset {i}"
                );
            }
        }
    }

    /// <summary>
    /// Gets the tag as a <see cref="ReadOnlySpan{Byte}"/>.
    /// </summary>
    /// <returns>The <see cref="ReadOnlySpan{Byte}"/> containing the tag.</returns>
    public ReadOnlySpan<byte> AsSpan()
    {
        return _buffer;
    }

    /// <summary>
    /// Gets the tag as a string.
    /// </summary>
    /// <returns>The string form of the tag.</returns>
    public override string ToString()
    { 
        return _text;
    }

    #region Equality

    /// <summary>
    /// Determines if the specified <paramref name="obj"/> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>true if <paramref name="obj"/> is equal to this instance.</returns>
    public override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is string value)
        {
            return Equals(value);
        }
        if (obj is Tag tag)
        {
            return Equals(tag);
        }

        return false;
    }

    /// <summary>
    /// Determines if the specified <paramref name="buffer"/> is equal to this instance.
    /// </summary>
    /// <param name="buffer">The object to compare.</param>
    /// <returns>true if <paramref name="buffer"/> is equal to this instance.</returns>
    public bool Equals(ReadOnlySpan<byte> buffer)
    {
        return buffer.Length == _buffer.Length && _buffer.SequenceEqual(buffer);
    }

    /// <summary>
    /// Determines if the specified string <paramref name="value"/> is equal to this instance.
    /// </summary>
    /// <param name="value">The string value to compare.</param>
    /// <returns>true if <paramref name="value"/> is equal to this instance's string value.</returns>
    public bool Equals(string value)
    {
        return value is not null && string.Equals(_text, value, StringComparison.Ordinal); 
    }

    /// <summary>
    /// Determines if the specified <paramref name="tag"/> is equal to this instance.
    /// </summary>
    /// <param name="tag">The <see cref="Tag"/> to compare.</param>
    /// <returns>true if <paramref name="tag"/> is equal to this instance.</returns>
    public bool Equals(Tag tag)
    {
        return _buffer.SequenceEqual(tag._buffer);
    }

    /// <summary>
    /// Serves as a hash function for this instance.
    /// </summary>
    /// <returns>The <see cref="Int32"/> hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return _text.GetHashCode();
    }

    #endregion Equality

    #region Validation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsValid(byte b)
    {
        return b >= 0x21 && b <= 0x7E;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsValid(char ch)
    {
        return ch >= 0x21 && ch <= 0x7E;
    }

    /// <summary>
    /// Validates a buffer's contents is valid for a <see cref="Tag"/>.
    /// </summary>
    /// <param name="buffer">The <see cref="ReadOnlySpan{Byte}"/> to validate.</param>
    /// <returns>true if the buffer contains valid <see cref="Tag"/> contents; otherwise, false.</returns>
    public static bool Validate(ReadOnlySpan<byte> buffer)
    {
        for (int i = 0; i < 4; i++)
        {
            if (!IsValid(buffer[i]))
            {
                return false;
            }
        }
        return true;
    }

    #endregion Validation

    #region public Fields

    /// <summary>
    /// Defines the <see cref="Tag"/> for the 'ttcf' header.
    /// </summary>
    public static readonly Tag Ttcf = new("ttcf");
    /// <summary>
    /// Defines the <see cref="Tag"/> for the 'cmap' table.
    /// </summary>
    public static readonly Tag Cmap = new("cmap");
    /// <summary>
    /// Defines the <see cref="Tag"/> for the 'post'' table.
    /// </summary>
    public static readonly Tag Post = new("post");
    /// <summary>
    /// Defines the <see cref="Tag"/> for the 'name' table.
    /// </summary>
    public static readonly Tag Name = new("name");
    /// <summary>
    /// Defines the <see cref="Tag"/> for the 'OS/2' table.
    /// </summary>
    public static readonly Tag OS2 = new("OS/2");


    #endregion public Fields     
}
