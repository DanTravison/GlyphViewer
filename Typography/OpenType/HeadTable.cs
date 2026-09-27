namespace CodeCadence.Typography.OpenType;

using CodeCadence.Typography.Text;
using System.Runtime.InteropServices;

/// <summary>
/// Represents the OpenType <see cref="Tag.Head"/> table.
/// </summary>
internal class HeadTable : OpenTypeStruct<HeadTable.Raw>
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Raw
    {
        public ushort MajorVersion;
        public ushort MinorVersion;
        public uint FontRevision;
        public uint CheckSumAdjustment;
        public uint MagicNumber;          // 0x5F0F3CF5
        public ushort Flags;
        public ushort UnitsPerEm;
        public long Created;              // LONGDATETIME (64-bit)
        public long Modified;             // LONGDATETIME (64-bit)
        public short XMin;
        public short YMin;
        public short XMax;
        public short YMax;
        public ushort MacStyle;
        public ushort LowestRecPPEM;
        public short FontDirectionHint;
        public short IndexToLocFormat;
        public short GlyphDataFormat;
    }

    #region Properties

    /// <summary>
    /// Get the <see cref="MacStyle"/>.
    /// </summary>
    /// <remarks>
    /// This value must agree <see cref="Os2Table.SelectionFlags"/>.
    /// and <see cref="SelectionFlags"/> are used in Windows.
    /// </remarks>
    public MacStyle MacStyle { get; private set; }

    /// <summary>
    /// Gets the units per em.
    /// </summary>
    /// <value>
    /// a value in the range of 16 to 16384.
    /// <para>
    /// If the font has a TrueType outline, a power of 2 is recommended.
    /// </para>
    /// </value>
    public ushort UnitsPerEm { get; private set; }

    /// <summary>
    /// Gets the minimum X coordinate across all glyph bounding boxes
    /// </summary>
    public short MinX { get; private set; }

    /// <summary>
    /// Gets the maximum X coordinate across all glyph bounding boxes
    /// </summary>
    public short MaxX { get; private set; }

    /// <summary>
    /// Gets the minimum Y coordinate across all glyph bounding boxes
    /// </summary>
    public short MinY { get; private set; }

    /// <summary>
    /// Gets the maximum Y coordinate across all glyph bounding boxes
    /// </summary>
    public short MaxY { get; private set; }

    #endregion Properties

    #region Public Utilities

    /// <summary>
    /// Gets the scale to use for a given font size.
    /// </summary>
    /// <param name="fontSize">The font size to use to scale the bounds.</param>
    /// <returns>A new instance of a GlyphBounds.</returns>
    public GlyphBounds GetBounds(float fontSize)
    {
        return new(fontSize, UnitsPerEm, MinX, MaxX, MinY, MaxY);
    }

    #endregion Public Utilities

    /// <summary>
    /// Reads the table from the specified <see cref="FontReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="FontReader"/> to read.</param>
    /// <param name="faceIndex">The zero-based face index.</param>
    /// <returns>A new instance of a <see cref="HeadTable"/>; otherwise, a null reference if the name table could not be found.</returns>
    internal static HeadTable Read(FontReader reader, int faceIndex)
    {
        long faceOffset = reader.Faces[faceIndex];

        long headOffset = OpenTypeParser.FindTableOffset(reader, faceOffset, Tag.Head);
        if (headOffset < 0)
        {
            return null;
        }

        HeadTable table = new();
        table.Initialize(reader, headOffset);

        return table;
    }


    protected override void OnInitialize(FontReader reader)
    {
        MacStyle = (MacStyle)ReadUInt16(OffsetOf(nameof(Raw.MacStyle)));
        UnitsPerEm = ReadUInt16(OffsetOf(nameof(Raw.UnitsPerEm)));
        MinX = ReadInt16(OffsetOf(nameof(Raw.XMin)));
        MaxX = ReadInt16(OffsetOf(nameof(Raw.XMax)));
        MinY = ReadInt16(OffsetOf(nameof(Raw.YMin)));
        MaxY = ReadInt16(OffsetOf(nameof(Raw.YMax)));
    }
}
