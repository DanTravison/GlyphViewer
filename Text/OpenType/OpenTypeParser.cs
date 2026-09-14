namespace GlyphViewer.Text.OpenType;

using System;
using System.Collections.Generic;

internal static class OpenTypeParser
{
    internal static IReadOnlyDictionary<uint, string> ReadGlyphNames(FontReader reader, int faceIndex)
    {
        PostTable table = PostTable.Read(reader, faceIndex);
        return table?.Names;
    }

    /// <summary>
    /// Enumerates the glyphs in a font.
    /// </summary>
    /// <param name="reader">The <see cref="FontReader"/> to read.</param>
    /// <param name="faceIndex">The index of the font face to read.</param>
    /// <returns>An <see cref="IEnumerable{GlyphInfo}"/> of the glyphs in the font.
    /// <para>
    /// If the font cannot be read, the enumerator will return zero items.
    /// </para>
    /// </returns>
    internal static IEnumerable<GlyphInfo> GetGlyphs(FontReader reader, int faceIndex)
    {
        IReadOnlyDictionary<uint, string> glyphNames = ReadGlyphNames(reader, faceIndex);
        CmapTable cmap = CmapTable.Read(reader, faceIndex);
        if (cmap is null)
        {
            yield break;
        }
        MaxpTable maxp = MaxpTable.Read(reader, faceIndex);
        if (maxp is null || maxp.Count == 0)
        {
            yield break;
        }

        for (uint glyphId = 0; glyphId < maxp.Count; glyphId++)
        {
            if (!cmap.Glyphs.TryGetValue(glyphId, out var glyph))
            {
                glyph = new(glyphId);
            }

            if (glyphNames.TryGetValue(glyphId, out string name))
            {
                glyph.Name = name;
            }
            yield return glyph;
        }
    }

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
    // NOTE: If we encounter the case where a stream is not seekable,
    // use this logic to create a MemoryStream from it and provide another
    // IFontStream implementation to support it.

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
