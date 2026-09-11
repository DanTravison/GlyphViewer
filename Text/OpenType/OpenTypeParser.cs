namespace GlyphViewer.Text.OpenType;

using System;
using System.Collections.Generic;

internal static class OpenTypeParser
{
    #region EnumerateCmap

    public static IEnumerable<(int codepoint, uint glyphId)> EnumerateCmap(FontReader reader, int faceIndex)
    {
        long faceOffset = reader.Faces[faceIndex];

        long cmapTableOffset = FindTableOffset(reader, faceOffset, Tag.Cmap);
        if (cmapTableOffset < 0)
        {
            yield break;
        }

        long unicodeSubtableOffset = FindUnicodeCmapSubtable(reader, cmapTableOffset);
        if (unicodeSubtableOffset < 0)
        {
            yield break;
        }

        ushort format = reader.ReadUInt16(unicodeSubtableOffset);
        if (format == 4)
        {
            foreach ((int codepoint, uint glyphId) in ParseCmapFormat4(reader, unicodeSubtableOffset))
            {
                if (glyphId != 0)
                {
                    yield return (codepoint, glyphId);
                }
            }
        }
        else if (format == 12)
        {
            foreach ((int codepoint, uint glyphId) in ParseCmapFormat12(reader, unicodeSubtableOffset))
            {
                if (glyphId != 0)
                {
                    yield return (codepoint, glyphId);
                }
            }
        }
    }

    static long FindUnicodeCmapSubtable(FontReader reader, long cmapOffset)
    {
        // cmap:
        // u16 version,
        // u16 numTables,
        // then encodingRecords (platformID u16, encodingID u16, offset u32)
        if (cmapOffset < 0 || cmapOffset + 4 > reader.Length)
        {
            return -1;
        }

        reader.Seek(cmapOffset);

        ushort version = reader.ReadUInt16();
        ushort numTables = reader.ReadUInt16();

        long encodingRecordPos = cmapOffset + 4;

        long bestFormat4 = -1;
        long bestFormat12 = -1;

        for (int i = 0; i < numTables; i++)
        {
            long rec = encodingRecordPos + i * 8;
            if (rec + 8 > reader.Length)
            {
                break;
            }
            reader.Seek(rec);
            ushort platformID = reader.ReadUInt16();
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
                if (bestFormat12 < 0 || (platformID == 3 && encodingID == 10))
                {
                    bestFormat12 = subtable;
                    if (platformID == 3 && encodingID == 10)
                    {
                        break; // ideal
                    }
                }
            }
            else if (format == 4)
            {
                if (bestFormat4 < 0 || (platformID == 3 && encodingID == 1))
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

    static IEnumerable<(int Codepoint, uint GlyphId)> ParseCmapFormat12(FontReader reader, long offset)
    {
        // format (u16) + reserved (u16) + length (u32) + language (u32) + nGroups (u32)
        if (offset + 16 > reader.Length)
        {
            yield break;
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
                yield return ((int)codepoint, glyphId);
            }

            p += 12;
        }
    }

    static IEnumerable<(int Codepoint, uint GlyphId)> ParseCmapFormat4(FontReader reader, long offset)
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
            yield break;
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
            yield break;
        }

        int[] endCode = new int[segCount];
        int[] startCode = new int[segCount];
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
            int start = startCode[i];
            int end = endCode[i];
            if (start == 0xFFFF) continue;
            if (start > end) continue;

            for (int codepoint = start; codepoint <= end; codepoint++)
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

                if (glyphId != 0)
                {
                    yield return (codepoint, glyphId);
                }
            }
        }
    }

    #endregion EnumerateCmap

    #region ReadGlyphNames

    internal static IReadOnlyList<string> ReadGlyphNames(FontReader reader)
    {
        PostTable table = PostTable.Read(reader, reader.FaceIndex);
        return table?.Names;
    }

    internal static IReadOnlyList<string> ReadGlyphNames(FontReader reader, int faceIndex)
    {
        PostTable table = PostTable.Read(reader, faceIndex);
        return table?.Names;
    }

    #endregion ReadGlyphNames

    #region Utilities

    internal static long[] ReadFontHeader(FontReader reader)
    {
        long[] offsets = null;
        reader.Seek(0);

        Span<byte> buffer = stackalloc byte[4];
        reader.ReadBytes(buffer);
        if (Tag.Ttcf.Equals(buffer))
        {
            ushort majorVersion = reader.ReadUInt16();
            ushort minorVersion = reader.ReadUInt16();
            int numFonts = (int)reader.ReadUInt32();
            offsets = new long[numFonts];
            for (int i = 0; i < numFonts; i++)
            {
                uint offset = reader.ReadUInt32();
                offsets[i] = (long)offset;
            }
        }
        else
        {
            offsets = [0];
        }
        // Restore to the start of the content.
        reader.Seek(0);
        return offsets;
    }

    static internal long FindTableOffset(FontReader reader, long faceOffset, Tag tag)
    {
        // OffsetTable: scaler (4), numTables (u16) at offset 4, table records start at 12; each record = 16 bytes
        if (reader.Length < faceOffset + 12)
        {
            return -1;
        }
        reader.Seek(faceOffset + 4);
        ushort numTables = reader.ReadUInt16();

        long tableDir = faceOffset + 12;

        for (int i = 0; i < numTables; i++)
        {
            long rec = tableDir + i * 16;
            if (rec + 16 > reader.Length)
            {
                break;
            }
            reader.Seek(rec);
            if (reader.ReadTag(tag))
            {
                reader.Seek(rec + 8);
                long offset = (uint)reader.ReadUInt32();
                return offset;
            }
        }
        return -1;
    }

    #endregion Utilities

#if (false)
    static byte[] ReadAll(SKStream stream)
    {
        // Read all bytes from SKStream into a byte[] (SKStream is not System.IO.Stream)
        List<byte> buf = [];
        if (stream.HasLength)
        {
            int len = stream.Length;
            byte[] outBuf = new byte[len];
            int read = 0;
            while (read < len)
            {
                int n = stream.Read(outBuf, len - read);
                if (n <= 0) break;
                if (n < len - read)
                {
                    buf.AddRange(outBuf.AsSpan(0, n).ToArray());
                }
                else
                {
                    buf.AddRange(outBuf);
                }
                read += n;
            }
        }
        else
        {
            byte[] chunk = new byte[8192];
            int r;
            while ((r = stream.Read(chunk, chunk.Length)) > 0)
            {
                buf.AddRange(chunk.AsSpan(0, r).ToArray());
            }
        }

        return buf.ToArray();
    }

#endif
}
