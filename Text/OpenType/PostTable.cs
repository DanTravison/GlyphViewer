namespace GlyphViewer.Text.OpenType;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Represents the OpenType 'post' table.
/// </summary>
internal sealed class PostTable : OpenTypeStruct<PostTable.Raw>
{
    #region Raw Struct

    /// <summary>
    /// Defines the raw binary layout of the 'post' table.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Raw
    {
        public uint FormatFixed;
        public uint ItalicAngle;
        public short UnderlinePosition;
        public short UnderlineThickness;
        public uint IsFixedPitch;

        public uint MinMemType42;
        public uint MaxMemType42;
        public uint MinMemType1;
        public uint MaxMemType1;

        public ushort NumGlyphs;
    }

    /// <summary>
    /// Provides the byte offsets of fields within <see cref="Raw"/>.
    /// </summary>
    private static class Offsets
    {
        public static readonly int FormatFixed =
            OffsetOf(nameof(Raw.FormatFixed));

#if (false)

        public static readonly int ItalicAngle =
            OffsetOf(nameof(Raw.ItalicAngle));

        public static readonly int UnderlinePosition =
            OffsetOf(nameof(Raw.UnderlinePosition));

        public static readonly int UnderlineThickness =
            OffsetOf(nameof(Raw.UnderlineThickness));

        public static readonly int IsFixedPitch =
            OffsetOf(nameof(Raw.IsFixedPitch));

        public static readonly int MinMemType42 =
            OffsetOf(nameof(Raw.MinMemType42));

        public static readonly int MaxMemType42 =
            OffsetOf(nameof(Raw.MaxMemType42));

        public static readonly int MinMemType1 =
            OffsetOf(nameof(Raw.MinMemType1));

        public static readonly int MaxMemType1 =
            OffsetOf(nameof(Raw.MaxMemType1));
#endif

        public static readonly int NumGlyphs =
            OffsetOf(nameof(Raw.NumGlyphs));
    }

#endregion Raw Struct

    #region Constructor

    private PostTable() : base()
    {
    }

    #endregion Constructor

    #region Properties (Fixed Fields)

    // NOTE: most fields are not exposed as properties since they are
    // not used for typical usage.

    /// <summary>
    /// Gets the format.
    /// </summary>
    /// <remarks>
    /// Post table is used explicitly for retrieving glyph names and requires
    /// format 2.0;  otherwise, <see cref="Read"/> will reeturn a null reference.
    /// </remarks>
    public uint Format { get; private set; }

    /// <summary>
    /// Gets the number of glyphs.
    /// </summary>
    /// <remarks>
    /// Consumers should use the glyphId as the index.
    /// </remarks>
    public ushort Count { get; private set; }

    /// <summary>
    /// Gets the list of glyph names.
    /// </summary>
    /// <remarks>
    /// Consumers should use the glyphId as the index to the associated name.
    /// </remarks>
    public IReadOnlyList<string> Names { get; private set; }

    #endregion Properties

    #region Read

    /// <summary>
    /// Reads the 'post' table from the specified <see cref="FontReader"/>.
    /// </summary>
    /// <param name="reader">The reader positioned at the start of the 'post' table.</param>
    /// <param name="faceIndex">The zero-based index of the font face to read.</param>
    /// <returns>The populated <see cref="PostTable"/>.</returns>
    internal static PostTable Read(FontReader reader, int faceIndex)
    {
        long faceOffset = reader.Faces[faceIndex];

        // Locate the 'post' table
        long postOffset = OpenTypeParser.FindTableOffset(reader, faceOffset, Tag.Post);
        if (postOffset < 0)
        {
            return null;
        }

        PostTable table = new();
        table.Initialize(reader, postOffset);

        // Only format 2.0 contains glyph names
        if (table.Format != 2)
        {
            return null;
        }

        return table;
    }

    protected override void OnInitialize(FontReader reader)
    {
        Format = ReadUInt32(Offsets.FormatFixed) >> 16;
        Count = ReadUInt16(Offsets.NumGlyphs);

        if (Format != 2)
        {
            return;
        }

        ushort numGlyphs = Count;
        //
        // Read glyph name index array
        //
        var nameIndex = new ushort[numGlyphs];
        for (int i = 0; i < numGlyphs; i++)
        {
            nameIndex[i] = reader.ReadUInt16();
        }

        //
        // Read glyph names
        //
        var names = new List<string>(numGlyphs);

        // Buffer for reading the name length.
        Span<byte> bufferLen = stackalloc byte[1];

        // Standard Mac glyph names (first 258 entries)
        // You can embed the standard list or reference your existing one.
        for (int i = 0; i < numGlyphs; i++)
        {
            ushort idx = nameIndex[i];

            if (idx < 258)
            {
                names.Add(MacGlyphNames[idx]);
            }
            else
            {
                int customIndex = idx - 258;

                // Read the name length.
                reader.ReadBytes(bufferLen);
                int len = bufferLen[0];

                // Read the ASCII text.
                string name = reader.ReadText(len);
                names.Add(name);
            }
        }

        Names = names;
    }

    #endregion Read

    static readonly string[] MacGlyphNames =
    {
        ".notdef", "null", "CR", "space", "exclam", "quotedbl", "numbersign", "dollar",
        "percent", "ampersand", "quotesingle", "parenleft", "parenright", "asterisk",
        "plus", "comma", "hyphen", "period", "slash", "zero", "one", "two", "three",
        "four", "five", "six", "seven", "eight", "nine", "colon", "semicolon", "less",
        "equal", "greater", "question", "at", "A", "B", "C", "D", "E", "F", "G", "H",
        "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X",
        "Y", "Z", "bracketleft", "backslash", "bracketright", "asciicircum", "underscore",
        "grave", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n",
        "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "braceleft", "bar",
        "braceright", "asciitilde", "Adieresis", "Aring", "Ccedilla", "Eacute", "Ntilde",
        "Odieresis", "Udieresis", "aacute", "agrave", "acircumflex", "adieresis", "atilde",
        "aring", "ccedilla", "eacute", "egrave", "ecircumflex", "edieresis", "iacute",
        "igrave", "icircumflex", "idieresis", "ntilde", "oacute", "ograve", "ocircumflex",
        "odieresis", "otilde", "uacute", "ugrave", "ucircumflex", "udieresis", "dagger",
        "degree", "cent", "sterling", "section", "bullet", "paragraph", "germandbls",
        "registered", "copyright", "trademark", "acute", "dieresis", "notequal", "AE",
        "Oslash", "infinity", "plusminus", "lessequal", "greaterequal", "yen", "mu",
        "partialdiff", "summation", "product", "pi", "integral", "ordfeminine",
        "ordmasculine", "Omega", "ae", "oslash", "questiondown", "exclamdown", "logicalnot",
        "radical", "florin", "approxequal", "Delta", "guillemotleft", "guillemotright",
        "ellipsis", "nonbreakingspace", "Agrave", "Atilde", "Otilde", "OE", "oe",
        "endash", "emdash", "quotedblleft", "quotedblright", "quoteleft", "quoteright",
        "divide", "lozenge", "ydieresis", "Ydieresis", "fraction", "currency", "guilsinglleft",
        "guilsinglright", "fi", "fl", "daggerdbl", "periodcentered", "quotesinglbase",
        "quotedblbase", "perthousand", "Acircumflex", "Ecircumflex", "Aacute", "Edieresis",
        "Egrave", "Iacute", "Icircumflex", "Idieresis", "Igrave", "Oacute", "Ocircumflex",
        "apple", "Ograve", "Uacute", "Ucircumflex", "Ugrave", "dotlessi", "circumflex",
        "tilde", "macron", "breve", "dotaccent", "ring", "cedilla", "hungarumlaut",
        "ogonek", "caron", "Lslash", "lslash", "Scaron", "scaron", "Zcaron", "zcaron",
        "brokenbar", "Eth", "eth", "Yacute", "yacute", "Thorn", "thorn", "minus", "multiply",
        "onesuperior", "twosuperior", "threesuperior", "onehalf", "onequarter",
        "threequarters", "franc", "Gbreve", "gbreve", "Idotaccent", "Scedilla", "scedilla",
        "Cacute", "cacute", "Ccaron", "ccaron", "dcroat"
    };
}
