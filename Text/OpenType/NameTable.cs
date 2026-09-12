namespace GlyphViewer.Text.OpenType;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Represents the OpenType 'name' table.
/// </summary>
internal sealed class NameTable : OpenTypeStruct<NameTable.Raw>
{
    #region Raw Struct

    /// <summary>
    /// Defines the raw binary layout of the 'name' table header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Raw
    {
        public ushort Format;          // 0 or 1
        public ushort Count;           // number of name records
        public ushort StringOffset;    // offset to string storage (from start of table)
    }

    /// <summary>
    /// Define the <see cref="Raw"/> field offsets.
    /// </summary>
    private static class Offsets
    {
        public static readonly int Format =
            OffsetOf(nameof(Raw.Format));

        public static readonly int Count =
            OffsetOf(nameof(Raw.Count));

        public static readonly int StringOffset =
            OffsetOf(nameof(Raw.StringOffset));
    }
    
    /// <summary>
    /// Defines the properties of a name record.
    /// </summary>
    internal readonly struct NameRecord
    {
        public NameRecord(FontReader reader)
        {
            PlatformID = reader.ReadUInt16();
            EncodingID = reader.ReadUInt16();
            LanguageID = reader.ReadUInt16();
            NameID = reader.ReadUInt16();
            Length = reader.ReadUInt16();
            Offset = reader.ReadUInt16();
        }

        public readonly ushort PlatformID;
        public readonly ushort EncodingID;
        public readonly ushort LanguageID;
        public readonly ushort NameID;
        public readonly ushort Length;
        public readonly ushort Offset;
    }

    #endregion Raw Struct

    #region Constructor

    private NameTable() : base()
    {
    }

    #endregion Constructor

    #region Properties

    private NameRecord[] _records;

    public ushort Format { get; private set; }
    public ushort Count { get; private set; }
    public ushort StringStorageOffset { get; private set; }

    public string FamilyName { get; private set; }
    public string SubfamilyName { get; private set; }
    public string TypographicFamilyName { get; private set; }
    public string TypographicSubfamilyName { get; private set; }
    public string PostScriptName { get; private set; }

    #endregion Properties

    #region Read

    internal static NameTable Read(FontReader reader, int faceIndex)
    {
        long faceOffset = reader.Faces[faceIndex];

        long nameTableOffset = OpenTypeParser.FindTableOffset(reader, faceOffset, Tag.Name);
        if (nameTableOffset < 0)
        {
            return null;
        }

        NameTable table = new();

        // Read fixed header
        table.Initialize(reader, nameTableOffset);
        return table;
    }

    protected override void OnInitialize(FontReader reader)
    {
        long nameTableStart = StartPosition;
        Format = ReadUInt16(Offsets.Format);
        Count = ReadUInt16(Offsets.Count);
        StringStorageOffset = ReadUInt16(Offsets.StringOffset);

        // Read name records sequentially
        NameRecord[] records = new NameRecord[Count];
        for (int i = 0; i < records.Length; i++)
        {
            records[i] = new(reader);
        }
        _records = records;

        // Resolve the names
        FamilyName = ResolvePreferredName(reader, nameTableStart, 1);
        SubfamilyName = ResolvePreferredName(reader, nameTableStart, 2);
        TypographicFamilyName = ResolvePreferredName(reader, nameTableStart, 16);
        TypographicSubfamilyName = ResolvePreferredName(reader, nameTableStart, 17);
        PostScriptName = ResolvePreferredName(reader, nameTableStart, 6);

        _records = null;
    }

    #endregion Read

    #region Name Resolution

    private string ResolvePreferredName(FontReader reader, long nameTableStart, params ushort[] nameIds)
    {
        // 1. Prefer Windows Unicode (Platform 3)
        foreach (ushort nameId in nameIds)
        {
            string value = GetNameForPlatform(reader, nameTableStart, nameId, platformID: 3);
            if (value is not null)
            {
                return value;
            }
        }

        // 2. Fallback to Macintosh Roman (Platform 1)
        foreach (ushort nameId in nameIds)
        {
            string value = GetNameForPlatform(reader, nameTableStart, nameId, platformID: 1);
            if (value is not null)
            {
                return value;
            }
        }

        return null;
    }

    private string GetNameForPlatform(FontReader reader, long nameTableStart, ushort nameId, ushort platformID)
    {
        foreach (NameRecord record in _records)
        {
            if (record.NameID != nameId || record.PlatformID != platformID)
            {
                continue;
            }

            return ReadNameString(reader, nameTableStart, record);
        }

        return null;
    }

    private string ReadNameString(FontReader reader, long nameTableStart, NameRecord record)
    {
        long stringStorageStart = nameTableStart + StringStorageOffset;
        long absoluteOffset = stringStorageStart + record.Offset;

        long restore = reader.Position;

        reader.Seek(absoluteOffset);

        string result = DecodeNameString(reader, record);

        reader.Seek(restore);

        return result;
    }

    private string DecodeNameString(FontReader reader, NameRecord record)
    {
        // Windows Unicode (UTF-16BE)
        if (record.PlatformID == 3)
        {
            Span<byte> buffer = stackalloc byte[record.Length];
            reader.ReadBytes(buffer);
            return DecodeUtf16BE(buffer);
        }

        // Macintosh Roman (ASCII-ish)
        if (record.PlatformID == 1)
        {
            return reader.ReadText(record.Length);
        }

        return null;
    }

    private static string DecodeUtf16BE(ReadOnlySpan<byte> buffer)
    {
        int charCount = buffer.Length / 2;
        Span<char> chars = stackalloc char[charCount];

        for (int i = 0; i < charCount; i++)
        {
            int hi = buffer[2 * i];
            int lo = buffer[2 * i + 1];
            chars[i] = (char)((hi << 8) | lo);
        }

        return new string(chars);
    }

    #endregion Name Resolution
}
