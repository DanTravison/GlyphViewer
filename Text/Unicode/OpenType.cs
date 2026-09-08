using SkiaSharp;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GlyphViewer.Text.Unicode;

internal class OpenType
{
    public static IEnumerable<(int codepoint, uint glyphId)> EnumerateCmap(SKTypeface typeface)
    {
        using (SKStreamAsset stream = typeface.OpenStream(out int ttcIndex))
        {
            if (stream is null)
            {
                yield break;
            }

            // Read all bytes from SKStream into a byte[] (SKStream is not System.IO.Stream)
            byte[] data = ReadAll(stream);

            int faceOffset = 0;

            string tag = ReadTag(data, 0);
            if (tag == "ttcf")
            {
                ushort majorVersion = ReadUInt16(data, 4);
                ushort minorVersion = ReadUInt16(data, 6);
                // TTC header
                uint numFonts = ReadUInt32(data, 8); // or 8..11
                if (ttcIndex >= 0 && 12 + (ttcIndex * 4) + 4 <= data.Length)
                {
                    faceOffset = (int)ReadUInt32(data, 12 + (ttcIndex * 4));
                }
                // now parse the selected face starting at faceOffset
            }
            int cmapTableOffset = FindTableOffset(data, faceOffset, "cmap");
            if (cmapTableOffset < 0)
            {
                yield break;
            }

            int unicodeSubtableOffset = FindUnicodeCmapSubtable(data, cmapTableOffset);
            if (unicodeSubtableOffset < 0)
            {
                yield break;
            }

            ushort format = ReadUInt16(data, unicodeSubtableOffset);
            if (format == 4)
            {
                foreach ((int codepoint, uint glyphId) in ParseCmapFormat4(data, unicodeSubtableOffset))
                {
                    if (glyphId != 0)
                    {
                        yield return (codepoint, glyphId);
                    }
                }
            }
            else if (format == 12)
            {
                foreach ((int codepoint, uint glyphId) in ParseCmapFormat12(data, unicodeSubtableOffset))
                {
                    if (glyphId != 0)
                    {
                        yield return (codepoint, glyphId);
                    }
                }
            }
        }
    }

    static int FindTableOffset(byte[] data, int faceOffset, string tag)
    {
        // OffsetTable: scaler (4), numTables (u16) at offset 4, table records start at 12; each record = 16 bytes
        if (data == null || data.Length < faceOffset + 12)
        {
            return -1;
        }
        ushort numTables = ReadUInt16(data, faceOffset + 4);
        int tableDir = faceOffset + 12;

        for (int i = 0; i < numTables; i++)
        {
            int rec = tableDir + i * 16;
            if (rec + 16 > data.Length)
            {
                break;
            }
            string t = ReadTag(data, rec);
            if (string.Equals(t, tag, StringComparison.Ordinal))
            {
                uint offset = ReadUInt32(data, rec + 8);
                return (int)offset;
            }
        }
        return -1;
    }

    static int FindUnicodeCmapSubtable(byte[] data, int cmapOffset)
    {
        // cmap: u16 version, u16 numTables, then encodingRecords (platformID u16, encodingID u16, offset u32)
        if (cmapOffset < 0 || cmapOffset + 4 > data.Length)
        {
            return -1;
        }

        ushort version = ReadUInt16(data, cmapOffset);
        ushort numTables = ReadUInt16(data, cmapOffset + 2);
        int encodingRecordPos = cmapOffset + 4;

        int bestFormat4 = -1;
        int bestFormat12 = -1;

        for (int i = 0; i < numTables; i++)
        {
            int rec = encodingRecordPos + i * 8;
            if (rec + 8 > data.Length)
            {
                break;
            }

            ushort platformID = ReadUInt16(data, rec);
            ushort encodingID = ReadUInt16(data, rec + 2);
            uint off = ReadUInt32(data, rec + 4);
            int subtable = cmapOffset + (int)off;
            if (subtable + 2 > data.Length)
            {
                continue;
            }

            ushort format = ReadUInt16(data, subtable);
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

    static IEnumerable<(int Codepoint, uint GlyphId)> ParseCmapFormat12(byte[] data, int offset)
    {
        // format (u16) + reserved (u16) + length (u32) + language (u32) + nGroups (u32)
        if (offset + 16 > data.Length) yield break;
        uint nGroups = ReadUInt32(data, offset + 12);

        int p = offset + 16;
        for (int i = 0; i < nGroups; i++)
        {
            if (p + 12 > data.Length) break;
            uint startCharCode = ReadUInt32(data, p);
            uint endCharCode = ReadUInt32(data, p + 4);
            uint startGlyphId = ReadUInt32(data, p + 8);

            for (uint codepoint = startCharCode; codepoint <= endCharCode; codepoint++)
            {
                uint glyphId = startGlyphId + (codepoint - startCharCode);
                yield return ((int)codepoint, glyphId);
            }

            p += 12;
        }
    }

    static IEnumerable<(int Codepoint, uint GlyphId)> ParseCmapFormat4(byte[] data, int offset)
    {
        // format(2) length(2) language(2) segCountX2(2) searchRange(2) entrySelector(2) rangeShift(2)
        if (offset + 14 > data.Length) yield break;
        ushort segCountX2 = ReadUInt16(data, offset + 6);
        int segCount = segCountX2 / 2;

        int endCodePos = offset + 14;
        int reservedPadPos = endCodePos + segCount * 2;
        int startCodePos = reservedPadPos + 2;
        int idDeltaPos = startCodePos + segCount * 2;
        int idRangeOffsetPos = idDeltaPos + segCount * 2;
        int glyphArrayPos = idRangeOffsetPos + segCount * 2;

        if (glyphArrayPos > data.Length) yield break;

        int[] endCode = new int[segCount];
        int[] startCode = new int[segCount];
        int[] idDelta = new int[segCount];
        int[] idRangeOffset = new int[segCount];

        for (int i = 0; i < segCount; i++)
        {
            endCode[i] = ReadUInt16(data, endCodePos + i * 2);
            startCode[i] = ReadUInt16(data, startCodePos + i * 2);
            idDelta[i] = (short)ReadUInt16(data, idDeltaPos + i * 2); // signed
            idRangeOffset[i] = ReadUInt16(data, idRangeOffsetPos + i * 2);
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
                    int idRangeOffsetWordPos = idRangeOffsetPos + i * 2;
                    int glyphIndexAddress = idRangeOffsetWordPos + idRangeOffset[i] + (codepoint - start) * 2;
                    if (glyphIndexAddress + 2 <= data.Length)
                    {
                        ushort glyphIndex = ReadUInt16(data, glyphIndexAddress);
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

    static string ReadTag(byte[] data, int offset)
    {
        if (offset + 4 > data.Length)
        {
            return string.Empty;
        }
        return System.Text.Encoding.ASCII.GetString(data, offset, 4);
    }

    static ushort ReadUInt16(byte[] data, int offset)
        => BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset, 2));

    static uint ReadUInt32(byte[] data, int offset)
        => BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(offset, 4));
}
