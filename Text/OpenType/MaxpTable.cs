namespace GlyphViewer.Text.OpenType;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Represents the OpenType <see cref="Tag.Maxp"/> table.
/// </summary>
internal class MaxpTable : OpenTypeStruct<MaxpTable.Raw>
{
    #region Raw Struct

    /// <summary>
    /// Defines the raw binary layout of the 'name' table header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Raw
    {
        public uint Version;          // 0 or 1
        public ushort Count;           // number of name records
    }

    #endregion Raw Struct

    /// <summary>
    /// Gets the version of the table
    /// </summary>
    public uint Version { get; private set; }

    /// <summary>
    /// Gets the number of glyphs in the font.
    /// </summary>
    public ushort Count { get; private set; }

    /// <summary>
    /// Reads the table from the specified <see cref="FontReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="FontReader"/> to read.</param>
    /// <param name="faceIndex">The zero-based face index.</param>
    /// <returns>A new instance of a <see cref="MaxpTable"/>; otherwise, a null reference if the name table could not be found.</returns>
    static internal MaxpTable Read(FontReader reader, int faceIndex)
    {
        long faceOffset = reader.Faces[faceIndex];

        long nameTableOffset = OpenTypeParser.FindTableOffset(reader, faceOffset, Tag.Maxp);
        if (nameTableOffset < 0)
        {
            return null;
        }

        MaxpTable table = new();

        // Read fixed header
        table.Initialize(reader, nameTableOffset);
        return table;
    }

    protected override void OnInitialize(FontReader reader)
    {
        Version = ReadUInt32(OffsetOf(nameof(Raw.Version)));
        Count = ReadUInt16(OffsetOf(nameof(Raw.Count)));
    }
}
