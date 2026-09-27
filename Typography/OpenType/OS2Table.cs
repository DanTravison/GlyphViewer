namespace CodeCadence.Typography.OpenType;

using CodeCadence.Typography.Text;
using System.Runtime.InteropServices;

/// <summary>
/// Represents the OpenType <see cref="Tag.OS2"/> table.
/// </summary>
internal sealed class Os2Table : OpenTypeStruct<Os2Table.Raw>
{
    #region Raw Struct

    /// <summary>
    /// Defines the structure of the <see cref="Os2Table"/> panose field.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PanoseRaw
    {
        public byte FamilyType;
        public byte SerifStyle;
        public byte Weight;
        public byte Proportion;
        public byte Contrast;
        public byte StrokeVariation;
        public byte ArmStyle;
        public byte Letterform;
        public byte Midline;
        public byte XHeight;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Raw
    {
        public ushort Version;
        public short xAvgCharWidth;
        public ushort usWeightClass;
        public ushort usWidthClass;
        public ushort fsType;
        public short ySubscriptXSize;
        public short ySubscriptYSize;
        public short ySubscriptXOffset;
        public short ySubscriptYOffset;
        public short ySuperscriptXSize;
        public short ySuperscriptYSize;
        public short ySuperscriptXOffset;
        public short ySuperscriptYOffset;
        public short yStrikeoutSize;
        public short yStrikeoutPosition;
        public short sFamilyClass;
        public PanoseRaw Panose;
        public uint ulUnicodeRange1;
        public uint ulUnicodeRange2;
        public uint ulUnicodeRange3;
        public uint ulUnicodeRange4;
        public uint Tag;
        public ushort fsSelection;
        // unreferenced follow…
    }

    #endregion Raw Struct

    #region Properties

    public ushort Version { get; private set; }
    public ushort WeightClass { get; private set; }
    public ushort WidthClass { get; private set; }
    public SelectionFlags SelectionFlags { get; private set; }
    public Panose Panose { get; private set; }

    #endregion Properties

    #region Read

    /// <summary>
    /// Reads the table from the specified <see cref="FontReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="FontReader"/> to read.</param>
    /// <param name="faceIndex">The zero-based face index.</param>
    /// <returns>A new instance of a <see cref="Os2Table"/>; otherwise, a null reference if the name table could not be found.</returns>
    internal static Os2Table Read(FontReader reader, int faceIndex)
    {
        long faceOffset = reader.Faces[faceIndex];
        long os2Offset = OpenTypeParser.FindTableOffset(reader, faceOffset, Tag.OS2);
        if (os2Offset < 0)
        {
            return null;
        }

        var table = new Os2Table();
        table.Initialize(reader, faceOffset);
        return table;
    }

    protected override void OnInitialize(FontReader reader)
    {
        Version = ReadUInt16(0);
        WeightClass = ReadUInt16(OffsetOf(nameof(Raw.usWeightClass)));
        WidthClass = ReadUInt16(OffsetOf(nameof(Raw.usWidthClass)));
        SelectionFlags = (SelectionFlags)ReadUInt16(OffsetOf(nameof(Raw.fsSelection)));

        PanoseRaw raw = GetField<PanoseRaw>(OffsetOf(nameof(Raw.Panose)));
        Panose = new(raw);
    }

    #endregion Read
}
