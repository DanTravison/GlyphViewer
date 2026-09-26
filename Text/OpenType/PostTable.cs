namespace GlyphViewer.Text.OpenType;

using GlyphViewer.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Represents the OpenType <see cref="Tag.Post"/> table.
/// </summary>
internal sealed class PostTable : OpenTypeStruct<PostTable.Raw>
{
    #region Raw Struct

    /// <summary>
    /// Defines the raw binary layout of the version 1.0 'post' table.
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

    }

    #endregion Raw Struct

    #region Constructor

    private PostTable() : base()
    {
    }

    #endregion Constructor

    #region Properties (Fixed Fields)

    /// <summary>
    /// Gets the set of glyphId to name.
    /// </summary>
    /// <remarks>
    /// Consumers should use the glyphId as the key to  retrieve the associated name.
    /// </remarks>
    public IReadOnlyDictionary<uint, string> Names { get; private set; }

    #endregion Properties

    #region Read

    /// <summary>
    /// Reads the table from the specified <see cref="FontReader"/>.
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
        if (table.Names is null)
        {
            return null;
        }

        return table;
    }

    protected override void OnInitialize(FontReader reader)
    {
        ushort format = ReadUInt16(Offsets.FormatFixed);
        if (format != 2)
        {
            return;
        }

        ushort numGlyphs = reader.ReadUInt16();

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
        Dictionary<uint, string> names = [];

        // Buffer for reading the name length.
        Span<byte> bufferLen = stackalloc byte[1];

        // Standard Mac glyph names (first 258 entries)
        for (uint i = 0; i < numGlyphs; i++)
        {
            ushort idx = nameIndex[i];

            if (idx < 258)
            {
                names.Add(i, MacGlyphNames[idx]);
            }
            else
            {
                // Read the name length.
                reader.ReadBytes(bufferLen);
                int len = bufferLen[0];

                // Read the ASCII text.
                string name = reader.ReadText(len);
                names.Add(i, name);
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
