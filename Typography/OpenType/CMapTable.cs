namespace CodeCadence.Typography.OpenType;

using CodeCadence.Typography.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Represents the OpenType <see cref="Tag.Cmap"/> table.
/// </summary>
internal sealed class CmapTable : OpenTypeStruct<CmapTable.Raw>
{
    #region Raw

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Raw
    {
        public ushort Version;
        public ushort NumTables;
    }

    enum PlatformID : ushort
    {
        Unicode = 0,
        Macintosh = 1,
        ISO = 2, // Deprecated
        Windows = 3,
        Custom = 4
    }

    #endregion Raw

    #region Fields

    private readonly Dictionary<uint, GlyphInfo> _glyphs = [];

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets <see cref="GlyphInfo"/> dictionary.
    /// </summary>
    public IReadOnlyDictionary<uint, GlyphInfo> Glyphs
    {
        get => _glyphs;
    }

    #endregion Properties

    public static CmapTable Read(FontReader reader, int faceIndex)
    {
        long faceOffset = reader.Faces[faceIndex];
        long cmapTableOffset = OpenTypeParser.FindTableOffset(reader, faceOffset, Tag.Cmap);
        if (cmapTableOffset < 0)
        {
            return null;
        }

        CmapTable table = new();
        table.Initialize(reader, cmapTableOffset);

        if (table._glyphs.Count > 0)
        {
            return table;
        }
        return null;
    }

    protected override void OnInitialize(FontReader reader)
    {
        uint tableCount = ReadUInt16(OffsetOf(nameof(Raw.NumTables)));

        long subtableOffset = FindUnicodeSubtable(reader, tableCount);

        if (subtableOffset == -1)
        {
            // Can't find a the subtable, 
            return;
        }

        ushort format = reader.ReadUInt16(subtableOffset);

        if (format == 12)
        {
            ParseFormat12(reader, subtableOffset);
        }
        else
        {
            ParseFormat4(reader, subtableOffset);
        }
    }

    long FindUnicodeSubtable(FontReader reader, uint tableCount)
    {
        long cmapOffset = StartPosition;

        if (cmapOffset + 4 > reader.Length)
        {
            return -1;
        }

        long encodingRecordPos = cmapOffset + 4;

        reader.Seek(encodingRecordPos);

        long bestFormat4 = -1;
        long bestFormat12 = -1;

        for (int i = 0; i < tableCount; i++)
        {
            long rec = encodingRecordPos + i * 8;
            if (rec + 8 > reader.Length)
            {
                break;
            }
            reader.Seek(rec);
            PlatformID platformID = (PlatformID)reader.ReadUInt16();
            ushort encodingID = reader.ReadUInt16();
            uint off = reader.ReadUInt32();

            long subtable = cmapOffset + (int)off;
            if (subtable + 2 > reader.Length)
            {
                continue;
            }

            reader.Seek(subtable);
            ushort format = reader.ReadUInt16();

            // prefer format 12 (full Unicode) then format 4 (BMP)
            if (format == 12)
            {
                if (bestFormat12 < 0 || (platformID == PlatformID.Windows && encodingID == 10))
                {
                    bestFormat12 = subtable;
                    if (platformID == PlatformID.Windows && encodingID == 10)
                    {
                        break; // ideal
                    }
                }
            }
            else if (format == 4)
            {
                if (bestFormat4 < 0 || (platformID == PlatformID.Windows && encodingID == 1))
                {
                    bestFormat4 = subtable;
                }
            }
        }

        if (bestFormat12 >= 0)
        {
            return bestFormat12;
        }
        if (bestFormat4 >= 0)
        {
            return bestFormat4;
        }
        return -1;
    }

    void ParseFormat12(FontReader reader, long offset)
    {
        // format (u16) + reserved (u16) + length (u32) + language (u32) + nGroups (u32)
        if (offset + 16 > reader.Length)
        {
            return;
        }
        reader.Seek(offset + 12);
        uint nGroups = reader.ReadUInt32();

        long p = offset + 16;
        for (int i = 0; i < nGroups; i++)
        {
            if (p + 12 > reader.Length)
            {
                break;
            }
            reader.Seek(p);
            uint startCharCode = reader.ReadUInt32();
            uint endCharCode = reader.ReadUInt32();
            uint startGlyphId = reader.ReadUInt32();

            for (uint codepoint = startCharCode; codepoint <= endCharCode; codepoint++)
            {
                uint glyphId = startGlyphId + (codepoint - startCharCode);
                GlyphInfo info = new(glyphId, codepoint);
                // NOTE: Two codepoints can map to the same glyphid.
                // As such, we can ignore the duplicates since the outline will
                // be the same.
                if (!_glyphs.ContainsKey(glyphId))
                {
                    _glyphs.Add(info.Id, info);
                }
            }

            p += 12;
        }
    }

    void ParseFormat4(FontReader reader, long offset)
    {
        // format(2)
        // length(2)
        // language(2)
        // segCountX2(2)
        // searchRange(2)
        // entrySelector(2)
        // rangeShift(2)

        if (offset + 14 > reader.Length)
        {
            return;
        }
        reader.Seek(offset + 6);
        ushort segCountX2 = reader.ReadUInt16();

        int segCount = segCountX2 / 2;

        long endCodePos = offset + 14;
        long reservedPadPos = endCodePos + segCount * 2;
        long startCodePos = reservedPadPos + 2;
        long idDeltaPos = startCodePos + segCount * 2;
        long idRangeOffsetPos = idDeltaPos + segCount * 2;
        long glyphArrayPos = idRangeOffsetPos + segCount * 2;

        if (glyphArrayPos > reader.Length)
        {
            return;
        }

        uint[] endCode = new uint[segCount];
        uint[] startCode = new uint[segCount];
        int[] idDelta = new int[segCount];
        int[] idRangeOffset = new int[segCount];

        for (int i = 0; i < segCount; i++)
        {
            endCode[i] = reader.ReadUInt16(endCodePos + i * 2);
            startCode[i] = reader.ReadUInt16(startCodePos + i * 2);
            idDelta[i] = (short)reader.ReadUInt16(idDeltaPos + i * 2); // signed
            idRangeOffset[i] = reader.ReadUInt16(idRangeOffsetPos + i * 2);
        }

        for (int i = 0; i < segCount; i++)
        {
            uint start = startCode[i];
            uint end = endCode[i];
            if (start == 0xFFFF) continue;
            if (start > end) continue;

            for (uint codepoint = start; codepoint <= end; codepoint++)
            {
                uint glyphId = 0;
                if (idRangeOffset[i] == 0)
                {
                    // glyphId = (codepoint + idDelta) % 65536
                    glyphId = (uint)((codepoint + idDelta[i]) & 0xFFFF);
                }
                else
                {
                    // idRangeOffset is a byte offset from the location of the idRangeOffset word to the glyphIdArray
                    long idRangeOffsetWordPos = idRangeOffsetPos + i * 2;
                    long glyphIndexAddress = idRangeOffsetWordPos + idRangeOffset[i] + (codepoint - start) * 2;
                    if (glyphIndexAddress + 2 <= reader.Length)
                    {
                        ushort glyphIndex = reader.ReadUInt16(glyphIndexAddress);
                        if (glyphIndex != 0)
                        {
                            glyphId = (uint)((glyphIndex + idDelta[i]) & 0xFFFF);
                        }
                    }
                }

                if (glyphId != 0 && !_glyphs.ContainsKey(glyphId))
                {
                    GlyphInfo info = new(glyphId, codepoint);
                    _glyphs.Add(info.Id, info);
                }
            }
        }
    }
}
