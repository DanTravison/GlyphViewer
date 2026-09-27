using System.Reflection;
using System.Runtime.ExceptionServices;

namespace CodeCadence.Typography.Unicode;

/// <summary>
/// Defines the set of Unicode ranges.
/// </summary>
public sealed class Ranges
{
    static readonly List<Range> _ranges = [];
    static readonly Dictionary<uint, Range> _codeTable = [];
    static readonly Dictionary<string, Range> _nameTable = [];

    static Ranges()
    {
        BuildTables();
        _ranges.Add(Range.None);
        _ranges.Sort(RangeComparer.Comparer);
    }

    static void Add(Range[] ranges)
    {
        foreach (Range range in ranges)
        {
            _ranges.Add(range);
            _codeTable.Add(range.Id, range);
            _nameTable.Add(range.Name, range);
        }
    }

    public static IEnumerable<Range> All
    {
        get => _ranges;
    }

    /// <summary>
    /// Find the <see cref="Range"/> contianing the specified <paramref name="codepoint"/>.
    /// </summary>
    /// <param name="codepoint">The codepoint to query.</param>
    /// <returns>
    /// The <see cref="Range"/> containing the specified <paramref name="codepoint"/>;
    /// otherwise, <see cref="Range.None"/>.
    /// </returns>
    public static Range Find(uint codepoint)
    {
        return RangeSearchComparer.Find(_ranges, codepoint);
    }

    /// <summary>
    /// Finds the <see cref="Range"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="Range"/> to find.</param>
    /// <returns>The <see cref="Range"/> with the specified <paramref name="name"/>; otherwise, 
    /// <see cref="Range.None"/>.
    /// </returns>
    public static Range Find(string name)
    {
        if (name is null)
        {
            return Range.None;
        }
        name = name.Trim();
        if (!_nameTable.TryGetValue(name, out Range range))
        {
            range = Range.None;
        }
        return range;
    }

    static void BuildTables()
    {
        Add([
            new("Basic Latin", '\u0000', '\u007F'),
            new("Latin-1 Supplement", '\u0080', '\u00FF'),
            new("Latin Extended-A", '\u0100', '\u017F'),
            new("Latin Extended-B", '\u0180', '\u024F'),
            new("IPA Extensions", '\u0250', '\u02AF'),
            new("Spacing Modifier Letters", '\u02B0', '\u02FF'),
            new("Combining Diacritical Marks", '\u0300', '\u036F'),
            new("Greek and Coptic", '\u0370', '\u03FF'),
            new("Cyrillic", '\u0400', '\u04FF'),
            new("Cyrillic Supplement", '\u0500', '\u052F'),
            new("Armenian", '\u0530', '\u058F'),
            new("Hebrew", '\u0590', '\u05FF'),
            new("Arabic", '\u0600', '\u06FF'),
            new("Syriac", '\u0700', '\u074F'),
            new("Arabic Supplement", '\u0750', '\u077F'),
            new("Thaana", '\u0780', '\u07BF'),
            new("NKo", '\u07C0', '\u07FF'),
            new("Samaritan", '\u0800', '\u083F'),
            new("Mandaic", '\u0840', '\u085F'),
            new("Syriac Supplement", '\u0860', '\u086F'),
            new("Arabic Extended-B", '\u0870', '\u089F'),
            new("Arabic Extended-A", '\u08A0', '\u08FF'),
            new("Devanagari", '\u0900', '\u097F'),
            new("Bengali", '\u0980', '\u09FF'),
            new("Gurmukhi", '\u0A00', '\u0A7F'),
            new("Gujarati", '\u0A80', '\u0AFF'),
            new("Oriya", '\u0B00', '\u0B7F'),
            new("Tamil", '\u0B80', '\u0BFF'),
            new("Telugu", '\u0C00', '\u0C7F'),
            new("Kannada", '\u0C80', '\u0CFF'),
            new("Malayalam", '\u0D00', '\u0D7F'),
            new("Sinhala", '\u0D80', '\u0DFF'),
            new("Thai", '\u0E00', '\u0E7F'),
            new("Lao", '\u0E80', '\u0EFF'),
            new("Tibetan", '\u0F00', '\u0FFF'),
            new("Myanmar", '\u1000', '\u109F'),
            new("Georgian", '\u10A0', '\u10FF'),
            new("Hangul Jamo", '\u1100', '\u11FF'),
            new("Ethiopic", '\u1200', '\u137F'),
            new("Ethiopic Supplement", '\u1380', '\u139F'),
            new("Cherokee", '\u13A0', '\u13FF'),
            new("Unified Canadian Aboriginal Syllabics", '\u1400', '\u167F'),
            new("Ogham", '\u1680', '\u169F'),
            new("Runic", '\u16A0', '\u16FF'),
            new("Tagalog", '\u1700', '\u171F'),
            new("Hanunoo", '\u1720', '\u173F'),
            new("Buhid", '\u1740', '\u175F'),
            new("Tagbanwa", '\u1760', '\u177F'),
            new("Khmer", '\u1780', '\u17FF'),
            new("Mongolian", '\u1800', '\u18AF'),
            new("Unified Canadian Aboriginal Syllabics Extended", '\u18B0', '\u18FF'),
            new("Limbu", '\u1900', '\u194F'),
            new("Tai Le", '\u1950', '\u197F'),
            new("New Tai Lue", '\u1980', '\u19DF'),
            new("Khmer Symbols", '\u19E0', '\u19FF'),
            new("Buginese", '\u1A00', '\u1A1F'),
            new("Tai Tham", '\u1A20', '\u1AAF'),
            new("Combining Diacritical Marks Extended", '\u1AB0', '\u1AFF'),
            new("Balinese", '\u1B00', '\u1B7F'),
            new("Sundanese", '\u1B80', '\u1BBF'),
            new("Batak", '\u1BC0', '\u1BFF'),
            new("Lepcha", '\u1C00', '\u1C4F'),
            new("Ol Chiki", '\u1C50', '\u1C7F'),
            new("Cyrillic Extended-C", '\u1C80', '\u1C8F'),
            new("Georgian Extended", '\u1C90', '\u1CBF'),
            new("Sundanese Supplement", '\u1CC0', '\u1CCF'),
            new("Vedic Extensions", '\u1CD0', '\u1CFF'),
            new("Phonetic Extensions", '\u1D00', '\u1D7F'),
            new("Phonetic Extensions Supplement", '\u1D80', '\u1DBF'),
            new("Combining Diacritical Marks Supplement", '\u1DC0', '\u1DFF'),
            new("Latin Extended Additional", '\u1E00', '\u1EFF'),
            new("Greek Extended", '\u1F00', '\u1FFF'),

            new("General Punctuation", '\u2000', '\u206F'),
            new("Superscripts and Subscripts", '\u2070', '\u209F'),
            new("Currency Symbols", '\u20A0', '\u20CF'),
            new("Combining Diacritical Marks for Symbols", '\u20D0', '\u20FF'),
            new("Letterlike Symbols", '\u2100', '\u214F'),
            new("Number Forms", '\u2150', '\u218F'),
            new("Arrows", '\u2190', '\u21FF'),
            new("Mathematical Operators", '\u2200', '\u22FF'),
            new("Miscellaneous Technical", '\u2300', '\u23FF'),
            new("Control Pictures", '\u2400', '\u243F'),
            new("Optical Character Recognition", '\u2440', '\u245F'),
            new("Enclosed Alphanumerics", '\u2460', '\u24FF'),
            new("Box Drawing", '\u2500', '\u257F'),
            new("Block Elements", '\u2580', '\u259F'),
            new("Geometric Shapes", '\u25A0', '\u25FF'),
            new("Miscellaneous Symbols", '\u2600', '\u26FF'),
            new("Dingbats", '\u2700', '\u27BF'),
            new("Miscellaneous Mathematical Symbols-A", '\u27C0', '\u27EF'),
            new("Supplemental Arrows-A", '\u27F0', '\u27FF'),
            new("Supplemental Arrows-B", '\u2900', '\u297F'),
            new("Miscellaneous Mathematical Symbols-B", '\u2980', '\u29FF'),
            new("Supplemental Mathematical Operators", '\u2A00', '\u2AFF'),
            new("Miscellaneous Symbols and Arrows", '\u2B00', '\u2BFF'),

            new("Latin Extended-C", '\u2C60', '\u2C7F'),
            new("Coptic", '\u2C80', '\u2CFF'),
            new("Georgian Supplement", '\u2D00', '\u2D2F'),
            new("Tifinagh", '\u2D30', '\u2D7F'),
            new("Ethiopic Extended", '\u2D80', '\u2DDF'),
            new("Cyrillic Extended-A", '\u2DE0', '\u2DFF'),
            new("Supplemental Punctuation", '\u2E00', '\u2E7F'),
            new("CJK Radicals Supplement", '\u2E80', '\u2EFF'),
            new("Kangxi Radicals", '\u2F00', '\u2FDF'),
            new("Ideographic Description Characters", '\u2FF0', '\u2FFF'),

            new("CJK Symbols and Punctuation", '\u3000', '\u303F'),
            new("Hiragana", '\u3040', '\u309F'),
            new("Katakana", '\u30A0', '\u30FF'),
            new("Bopomofo", '\u3100', '\u312F'),
            new("Hangul Compatibility Jamo", '\u3130', '\u318F'),
            new("Kanbun", '\u3190', '\u319F'),
            new("Bopomofo Extended", '\u31A0', '\u31BF'),
            new("CJK Strokes", '\u31C0', '\u31EF'),
            new("Katakana Phonetic Extensions", '\u31F0', '\u31FF'),
            new("Enclosed CJK Letters and Months", '\u3200', '\u32FF'),
            new("CJK Compatibility", '\u3300', '\u33FF'),
            new("CJK Unified Ideographs Extension-A", '\u3400', '\u4DBF'),
            new("Yijing Hexagram Symbols", '\u4DC0', '\u4DFF'),
            new("CJK Unified Ideographs", '\u4E00', '\u9FFF'),

            new("Yi Syllables", '\uA000', '\uA48F'),
            new("Yi Radicals", '\uA490', '\uA4CF'),
            new("Lisu", '\uA4D0', '\uA4FF'),
            new("Vai", '\uA500', '\uA63F'),
            new("Cyrillic Extended-B", '\uA640', '\uA69F'),
            new("Bamum", '\uA6A0', '\uA6FF'),
            new("Modifier Tone Letters", '\uA700', '\uA71F'),
            new("Latin Extended-D", '\uA720', '\uA7FF'),
            new("Syloti Nagri", '\uA800', '\uA82F'),
            new("Common Indic Number Forms", '\uA830', '\uA83F'),
            new("Phags-pa", '\uA840', '\uA87F'),
            new("Saurashtra", '\uA880', '\uA8DF'),
            new("Devanagari Extended", '\uA8E0', '\uA8FF'),
            new("Kayah Li", '\uA900', '\uA92F'),
            new("Rejang", '\uA930', '\uA95F'),
            new("Hangul Jamo Extended-A", '\uA960', '\uA97F'),
            new("Javanese", '\uA980', '\uA9DF'),
            new("Myanmar Extended-B", '\uA9E0', '\uA9FF'),
            new("Cham", '\uAA00', '\uAA5F'),
            new("Myanmar Extended-A", '\uAA60', '\uAA7F'),
            new("Tai Viet", '\uAA80', '\uAADF'),
            new("Meetei Mayek Extensions", '\uAAE0', '\uAAFF'),
            new("Ethiopic Extended-A", '\uAB00', '\uAB2F'),
            new("Latin Extended-E", '\uAB30', '\uAB6F'),
            new("Cherokee Supplement", '\uAB70', '\uABBF'),
            new("Meetei Mayek", '\uABC0', '\uABFF'),

            new("Hangul Syllables", '\uAC00', '\uD7AF'),
            new("Hangul Jamo Extended-B", '\uD7B0', '\uD7FF'),

            new("High Surrogates", '\uD800', '\uDB7F'),
            new("High Private Use Surrogates", '\uDB80', '\uDBFF'),
            new("Low Surrogates", '\uDC00', '\uDFFF'),

            new("Private Use Area", '\uE000', '\uF8FF'),
            new("CJK Compatibility Ideographs", '\uF900', '\uFAFF'),
            new("Alphabetic Presentation Forms", '\uFB00', '\uFB4F'),
            new("Arabic Presentation Forms-A", '\uFB50', '\uFDFF'),
            new("Variation Selectors", '\uFE00', '\uFE0F'),
            new("Vertical Forms", '\uFE10', '\uFE1F'),
            new("Combining Half Marks", '\uFE20', '\uFE2F'),
            new("CJK Compatibility Forms", '\uFE30', '\uFE4F'),
            new("Small Form Variants", '\uFE50', '\uFE6F'),
            new("Arabic Presentation Forms-B", '\uFE70', '\uFEFF'),

            new("Halfwidth and Fullwidth Forms", '\uFF00', '\uFFEF'),
            new("Specials", '\uFFF0', '\uFFFF'),

            new(0x10000, 0x1007F, "Linear B Syllabary"),
            new(0x10080, 0x100FF, "Linear B Ideograms"),
            new(0x10100, 0x1013F, "Aegean Numbers"),
            new(0x10140, 0x1018F, "Ancient Greek Numbers"),
            new(0x10190, 0x101CF, "Ancient Symbols"),
            new(0x101D0, 0x101FF, "Phaistos Disc"),
            new(0x10280, 0x1029F, "Lycian"),
            new(0x102A0, 0x102DF, "Carian"),
            new(0x102E0, 0x102FF, "Coptic Epact Numbers"),
            new(0x10300, 0x1032F, "Old Italic"),
            new(0x10330, 0x1034F, "Gothic"),
            new(0x10350, 0x1037F, "Old Permic"),
            new(0x10380, 0x1039F, "Ugaritic"),
            new(0x103A0, 0x103DF, "Old Persian"),
            new(0x10400, 0x1044F, "Deseret"),
            new(0x10450, 0x1047F, "Shavian"),
            new(0x10480, 0x104AF, "Osmanya"),
            new(0x104B0, 0x104FF, "Osage"),
            new(0x10500, 0x1052F, "Elbasan"),
            new(0x10530, 0x1056F, "Caucasian Albanian"),
            new(0x10570, 0x105BF, "Vithkuqi"),
            new(0x10600, 0x1077F, "Linear A"),
            new(0x10780, 0x107BF, "Latin Extended-F"),
            new(0x10800, 0x1083F, "Cypriot Syllabary"),
            new(0x10840, 0x1085F, "Imperial Aramaic"),
            new(0x10860, 0x1087F, "Palmyrene"),
            new(0x10880, 0x108AF, "Nabataean"),
            new(0x108E0, 0x108FF, "Hatran"),
            new(0x10900, 0x1091F, "Phoenician"),
            new(0x10920, 0x1093F, "Lydian"),
            new(0x10980, 0x1099F, "Meroitic Hieroglyphs"),
            new(0x109A0, 0x109FF, "Meroitic Cursive"),
            new(0x10A00, 0x10A5F, "Kharoshthi"),
            new(0x10A60, 0x10A7F, "Old South Arabian"),
            new(0x10A80, 0x10A9F, "Old North Arabian"),
            new(0x10AC0, 0x10AFF, "Manichaean"),
            new(0x10B00, 0x10B3F, "Avestan"),
            new(0x10B40, 0x10B5F, "Inscriptional Parthian"),
            new(0x10B60, 0x10B7F, "Inscriptional Pahlavi"),
            new(0x10B80, 0x10BAF, "Psalter Pahlavi"),
            new(0x10C00, 0x10C4F, "Old Turkic"),
            new(0x10C80, 0x10CFF, "Old Hungarian"),
            new(0x10D00, 0x10D3F, "Hanifi Rohingya"),
            new(0x10E60, 0x10E7F, "Rumi Numeral Symbols"),
            new(0x10E80, 0x10EBF, "Yezidi"),
            new(0x10EC0, 0x10EFF, "Arabic Extended-C"),
            new(0x10F00, 0x10F2F, "Old Sogdian"),
            new(0x10F30, 0x10F6F, "Sogdian"),
            new(0x10F70, 0x10FAF, "Old Uyghur"),
            new(0x10FB0, 0x10FDF, "Chorasmian"),
            new(0x10FE0, 0x10FFF, "Elymaic"),
            new(0x11000, 0x1107F, "Brahmi"),
            new(0x11080, 0x110CF, "Kaithi"),
            new(0x110D0, 0x110FF, "Sora Sompeng"),
            new(0x11100, 0x1114F, "Chakma"),
            new(0x11150, 0x1117F, "Mahajani"),
            new(0x11180, 0x111DF, "Sharada"),
            new(0x111E0, 0x111FF, "Sinhala Archaic Numbers"),
            new(0x11200, 0x1124F, "Khojki"),
            new(0x11280, 0x112AF, "Multani"),
            new(0x112B0, 0x112FF, "Khudawadi"),
            new(0x11300, 0x1137F, "Grantha"),
            new(0x11400, 0x1147F, "Newa"),
            new(0x11480, 0x114DF, "Tirhuta"),
            new(0x11580, 0x115FF, "Siddham"),
            new(0x11600, 0x1165F, "Modi"),
            new(0x11660, 0x1167F, "Mongolian Supplement"),
            new(0x11680, 0x116CF, "Takri"),
            new(0x11700, 0x1174F, "Ahom"),
            new(0x11800, 0x1184F, "Dogra"),
            new(0x118A0, 0x118FF, "Warang Citi"),
            new(0x11900, 0x1195F, "Dives Akuru"),
            new(0x119A0, 0x119FF, "Nandinagari"),
            new(0x11A00, 0x11A4F, "Zanabazar Square"),
            new(0x11A50, 0x11AAF, "Soyombo"),
            new(0x11AB0, 0x11ABF, "Unified Canadian Aboriginal Syllabics Extended-A"),
            new(0x11AC0, 0x11AFF, "Pau Cin Hau"),
            new(0x11B00, 0x11B5F, "Devanagari Extended-A"),
            new(0x11C00, 0x11C6F, "Bhaiksuki"),
            new(0x11C70, 0x11CBF, "Marchen"),
            new(0x11D00, 0x11D5F, "Masaram Gondi"),
            new(0x11D60, 0x11DAF, "Gunjala Gondi"),
            new(0x11EE0, 0x11EFF, "Makasar"),
            new(0x11F00, 0x11F5F, "Kawi"),
            new(0x11FB0, 0x11FBF, "Lisu Supplement"),
            new(0x11FC0, 0x11FFF, "Tamil Supplement"),
            new(0x12000, 0x123FF, "Cuneiform"),
            new(0x12400, 0x1247F, "Cuneiform Numbers and Punctuation"),
            new(0x12480, 0x1254F, "Early Dynastic Cuneiform"),
            new(0x12F90, 0x12FFF, "Cypro-Minoan"),
            new(0x13000, 0x1342F, "Egyptian Hieroglyphs"),
            new(0x13430, 0x1345F, "Egyptian Hieroglyph Format Controls"),
            new(0x14400, 0x1467F, "Anatolian Hieroglyphs"),
            new(0x16800, 0x16A3F, "Bamum Supplement"),
            new(0x16A40, 0x16A6F, "Mro"),
            new(0x16A70, 0x16ACF, "Tangsa"),
            new(0x16AD0, 0x16AFF, "Bassa Vah"),
            new(0x16B00, 0x16B8F, "Pahawh Hmong"),
            new(0x16E40, 0x16E9F, "Medefaidrin"),
            new(0x16F00, 0x16F9F, "Miao"),
            new(0x16FE0, 0x16FFF, "Ideographic Symbols and Punctuation"),
            new(0x17000, 0x187FF, "Tangut"),
            new(0x18800, 0x18AFF, "Tangut Components"),
            new(0x18B00, 0x18CFF, "Khitan Small Script"),
            new(0x18D00, 0x18D7F, "Tangut Supplement"),
            new(0x1AFF0, 0x1AFFF, "Kana Extended-B"),
            new(0x1B000, 0x1B0FF, "Kana Supplement"),
            new(0x1B100, 0x1B12F, "Kana Extended-A"),
            new(0x1B130, 0x1B16F, "Small Kana Extension"),
            new(0x1B170, 0x1B2FF, "Nushu"),
            new(0x1BC00, 0x1BC9F, "Duployan"),
            new(0x1BCA0, 0x1BCAF, "Shorthand Format Controls"),
            new(0x1CF00, 0x1CFCF, "Znamenny Musical Notation"),
            new(0x1D000, 0x1D0FF, "Byzantine Musical Symbols"),
            new(0x1D100, 0x1D1FF, "Musical Symbols"),
            new(0x1D200, 0x1D24F, "Ancient Greek Musical Notation"),
            new(0x1D2C0, 0x1D2DF, "Kaktovik Numerals"),
            new(0x1D2E0, 0x1D2FF, "Mayan Numerals"),
            new(0x1D300, 0x1D35F, "Tai Xuan Jing Symbols"),
            new(0x1D360, 0x1D37F, "Counting Rod Numerals"),
            new(0x1D400, 0x1D7FF, "Mathematical Alphanumeric Symbols"),
            new(0x1D800, 0x1DAAF, "Sutton SignWriting"),
            new(0x1DF00, 0x1DFFF, "Latin Extended-G"),
            new(0x1E000, 0x1E02F, "Glagolitic Supplement"),
            new(0x1E030, 0x1E08F, "Cyrillic Extended-D"),
            new(0x1E100, 0x1E14F, "Nyiakeng Puachue Hmong"),
            new(0x1E290, 0x1E2BF, "Toto"),
            new(0x1E2C0, 0x1E2FF, "Wancho"),
            new(0x1E4D0, 0x1E4FF, "Nag Mundari"),
            new(0x1E7E0, 0x1E7FF, "Ethiopic Extended-B"),
            new(0x1E800, 0x1E8DF, "Mende Kikakui"),
            new(0x1E900, 0x1E95F, "Adlam"),
            new(0x1EC70, 0x1ECBF, "Indic Siyaq Numbers"),
            new(0x1ED00, 0x1ED4F, "Ottoman Siyaq Numbers"),
            new(0x1EE00, 0x1EEFF, "Arabic Mathematical Alphabetic Symbols"),
            new(0x1F000, 0x1F02F, "Mahjong Tiles"),
            new(0x1F030, 0x1F09F, "Domino Tiles"),
            new(0x1F0A0, 0x1F0FF, "Playing Cards"),
            new(0x1F100, 0x1F1FF, "Enclosed Alphanumeric Supplement"),
            new(0x1F200, 0x1F2FF, "Enclosed Ideographic Supplement"),
            new(0x1F300, 0x1F5FF, "Miscellaneous Symbols and Pictographs"),
            new(0x1F600, 0x1F64F, "Emoticons"),
            new(0x1F650, 0x1F67F, "Ornamental Dingbats"),
            new(0x1F680, 0x1F6FF, "Transport and Map Symbols"),
            new(0x1F700, 0x1F77F, "Alchemical Symbols"),
            new(0x1F780, 0x1F7FF, "Geometric Shapes Extended"),
            new(0x1F800, 0x1F8FF, "Supplemental Arrows-C"),
            new(0x1F900, 0x1F9FF, "Supplemental Symbols and Pictographs"),
            new(0x1FA00, 0x1FA6F, "Chess Symbols"),
            new(0x1FA70, 0x1FAFF, "Symbols and Pictographs Extended-A"),
            new(0x1FB00, 0x1FBFF, "Symbols for Legacy Computing"),
            new(0x20000, 0x2A6DF, "CJK Unified Ideographs Extension B"),
            new(0x2A700, 0x2B73F, "CJK Unified Ideographs Extension C"),
            new(0x2B740, 0x2B81F, "CJK Unified Ideographs Extension D"),
            new(0x2B820, 0x2CEAF, "CJK Unified Ideographs Extension E"),
            new(0x2CEB0, 0x2EBEF, "CJK Unified Ideographs Extension F"),
            new(0x2EBF0, 0x2EE5F, "CJK Unified Ideographs Extension I"),
            new(0x2F800, 0x2FA1F, "CJK Compatibility Ideographs Supplement"),
            new(0x30000, 0x3134F, "CJK Unified Ideographs Extension G"),
            new(0x31350, 0x323AF, "CJK Unified Ideographs Extension H"),
            new(0xE0000, 0xE007F, "Tags"),
            new(0xE0100, 0xE01EF, "Variation Selectors Supplement"),
            new(0xF0000, 0xFFFFF, "Supplementary Private Use Area-A"),
            new(0x100000, 0x10FFFF, "Supplementary Private Use Area-B"),

        ]);
    }

}
