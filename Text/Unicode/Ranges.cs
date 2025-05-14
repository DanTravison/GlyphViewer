using System.Reflection;

namespace GlyphViewer.Text.Unicode;

/// <summary>
/// Defines the set of Unicode ranges.
/// </summary>
internal class Ranges
{
    static readonly List<Range> _ranges = [];
    static readonly Dictionary<uint, Range> _codeTable = [];
    static readonly Dictionary<string, Range> _nameTable = [];

    class RangeComparer : IComparer<Range>
    {
        public static readonly RangeComparer Comparer = new RangeComparer();

        private RangeComparer()
        {
        }

        public int Compare(Range x, Range y)
        {
            return x.First.CompareTo(y.First);
        }
    }

    static Ranges()
    {
        foreach (FieldInfo field in typeof(Ranges).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType == typeof(Range))
            {
                Range range = (Range)field.GetValue(null);
                _ranges.Add(range);
                _codeTable.Add(range.Id, range);
                _nameTable.Add(range.Name, range);
            }
        }
        _ranges.Sort(RangeComparer.Comparer);
    }

    public static IEnumerable<Range> All
    {
        get => _ranges;
    }

    /// <summary>
    /// Find the <see cref="Range"/> contianing the specified <paramref name="codePoint"/>.
    /// </summary>
    /// <param name="codePoint">The code point to query.</param>
    /// <returns>
    /// The <see cref="Range"/> containing the specified <paramref name="codePoint"/>;
    /// otherwise, <see cref="Range.Empty"/>.
    /// </returns>
    public static Range Find(ushort codePoint)
    {
        Range range = Range.Empty;
        int index = _ranges.BinarySearch(new Range(string.Empty, codePoint, codePoint), RangeComparer.Comparer);
        do
        {
            if (index >= 0)
            {
                range = _ranges[index];
                break;
            }
            index = ~index;
            if (index == 0)
            {
                break;
            }
            range = _ranges[index - 1];
        } while (false);

        return range;
    }

    /// <summary>
    /// Finds the <see cref="Range"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="Range"/> to find.</param>
    /// <returns>The <see cref="Range"/> with the specified <paramref name="name"/>; otherwise, 
    /// <see cref="Range.Empty"/>.
    /// </returns>
    public static Range Find(string name)
    {
        if (name is null)
        {
            return Range.Empty;
        }
        name = name.Trim();
        if (!_nameTable.TryGetValue(name, out Range range))
        {
            range = Range.Empty;
        }
        return range;
    }

    #region Public Static Fields

    /// <summary>
    /// The Unicode range for 'Basic Latin' (U+0000..U+007F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0000.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range BasicLatin = new("Basic Latin", '\u0000', '\u007F');

    /// <summary>
    /// The Unicode range for 'Latin-1 Supplement' (U+0080..U+00FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0080.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Latin1Supplement = new("Latin-1 Supplement", '\u0080', '\u00FF');

    /// <summary>
    /// The Unicode range for 'Latin Extended-A' (U+0100..U+017F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0100.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range LatinExtendedA = new("Latin Extended-A", '\u0100', '\u017F');

    /// <summary>
    /// The Unicode range for 'Latin Extended-B' (U+0180..U+024F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0180.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range LatinExtendedB = new("Latin Extended-B", '\u0180', '\u024F');

    /// <summary>
    /// The Unicode range for 'IPA Extensions' (U+0250..U+02AF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0250.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range IpaExtensions = new("IPA Extensions", '\u0250', '\u02AF');

    /// <summary>
    /// The Unicode range for 'Spacing Modifier Letters' (U+02B0..U+02FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U02B0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SpacingModifierLetters = new("Spacing Modifier Letters", '\u02B0', '\u02FF');

    /// <summary>
    /// The Unicode range for 'Combining Diacritical Marks' (U+0300..U+036F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0300.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CombiningDiacriticalMarks = new("Combining Diacritical Marks", '\u0300', '\u036F');

    /// <summary>
    /// The Unicode range for 'Greek and Coptic' (U+0370..U+03FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0370.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range GreekandCoptic = new("Greek and Coptic", '\u0370', '\u03FF');

    /// <summary>
    /// The Unicode range for 'Cyrillic' (U+0400..U+04FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0400.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Cyrillic = new("Cyrillic", '\u0400', '\u04FF');

    /// <summary>
    /// The Unicode range for 'Cyrillic Supplement' (U+0500..U+052F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0500.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CyrillicSupplement = new("Cyrillic Supplement", '\u0500', '\u052F');

    /// <summary>
    /// The Unicode range for 'Armenian' (U+0530..U+058F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0530.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Armenian = new("Armenian", '\u0530', '\u058F');

    /// <summary>
    /// The Unicode range for 'Hebrew' (U+0590..U+05FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0590.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Hebrew = new("Hebrew", '\u0590', '\u05FF');

    /// <summary>
    /// The Unicode range for 'Arabic' (U+0600..U+06FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0600.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Arabic = new("Arabic", '\u0600', '\u06FF');

    /// <summary>
    /// The Unicode range for 'Syriac' (U+0700..U+074F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0700.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Syriac = new("Syriac", '\u0700', '\u074F');

    /// <summary>
    /// The Unicode range for 'Arabic Supplement' (U+0750..U+077F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0750.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range ArabicSupplement = new("Arabic Supplement", '\u0750', '\u077F');

    /// <summary>
    /// The Unicode range for 'Thaana' (U+0780..U+07BF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0780.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Thaana = new("Thaana", '\u0780', '\u07BF');

    /// <summary>
    /// The Unicode range for 'NKo' (U+07C0..U+07FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U07C0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range NKo = new("NKo", '\u07C0', '\u07FF');

    /// <summary>
    /// The Unicode range for 'Samaritan' (U+0800..U+083F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0800.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Samaritan = new("Samaritan", '\u0800', '\u083F');

    /// <summary>
    /// The Unicode range for 'Mandaic' (U+0840..U+085F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0840.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Mandaic = new("Mandaic", '\u0840', '\u085F');

    /// <summary>
    /// The Unicode range for 'Syriac Supplement' (U+0860..U+086F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0860.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SyriacSupplement = new("Syriac Supplement", '\u0860', '\u086F');

    /// <summary>
    /// The Unicode range for 'Arabic Extended-B' (U+0870..U+089F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0870.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range ArabicExtendedB = new("Arabic Extended-B", '\u0870', '\u089F');

    /// <summary>
    /// The Unicode range for 'Arabic Extended-A' (U+08A0..U+08FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U08A0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range ArabicExtendedA = new("Arabic Extended-A", '\u08A0', '\u08FF');

    /// <summary>
    /// The Unicode range for 'Devanagari' (U+0900..U+097F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0900.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Devanagari = new("Devanagari", '\u0900', '\u097F');

    /// <summary>
    /// The Unicode range for 'Bengali' (U+0980..U+09FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0980.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Bengali = new("Bengali", '\u0980', '\u09FF');

    /// <summary>
    /// The Unicode range for 'Gurmukhi' (U+0A00..U+0A7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0A00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Gurmukhi = new("Gurmukhi", '\u0A00', '\u0A7F');

    /// <summary>
    /// The Unicode range for 'Gujarati' (U+0A80..U+0AFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0A80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Gujarati = new("Gujarati", '\u0A80', '\u0AFF');

    /// <summary>
    /// The Unicode range for 'Oriya' (U+0B00..U+0B7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0B00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Oriya = new("Oriya", '\u0B00', '\u0B7F');

    /// <summary>
    /// The Unicode range for 'Tamil' (U+0B80..U+0BFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0B80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Tamil = new("Tamil", '\u0B80', '\u0BFF');

    /// <summary>
    /// The Unicode range for 'Telugu' (U+0C00..U+0C7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0C00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Telugu = new("Telugu", '\u0C00', '\u0C7F');

    /// <summary>
    /// The Unicode range for 'Kannada' (U+0C80..U+0CFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0C80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Kannada = new("Kannada", '\u0C80', '\u0CFF');

    /// <summary>
    /// The Unicode range for 'Malayalam' (U+0D00..U+0D7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0D00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Malayalam = new("Malayalam", '\u0D00', '\u0D7F');

    /// <summary>
    /// The Unicode range for 'Sinhala' (U+0D80..U+0DFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0D80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Sinhala = new("Sinhala", '\u0D80', '\u0DFF');

    /// <summary>
    /// The Unicode range for 'Thai' (U+0E00..U+0E7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0E00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Thai = new("Thai", '\u0E00', '\u0E7F');

    /// <summary>
    /// The Unicode range for 'Lao' (U+0E80..U+0EFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0E80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Lao = new("Lao", '\u0E80', '\u0EFF');

    /// <summary>
    /// The Unicode range for 'Tibetan' (U+0F00..U+0FFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U0F00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Tibetan = new("Tibetan", '\u0F00', '\u0FFF');

    /// <summary>
    /// The Unicode range for 'Myanmar' (U+1000..U+109F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1000.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Myanmar = new("Myanmar", '\u1000', '\u109F');

    /// <summary>
    /// The Unicode range for 'Georgian' (U+10A0..U+10FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U10A0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Georgian = new("Georgian", '\u10A0', '\u10FF');

    /// <summary>
    /// The Unicode range for 'Hangul Jamo' (U+1100..U+11FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1100.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range HangulJamo = new("Hangul Jamo", '\u1100', '\u11FF');

    /// <summary>
    /// The Unicode range for 'Ethiopic' (U+1200..U+137F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1200.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Ethiopic = new("Ethiopic", '\u1200', '\u137F');

    /// <summary>
    /// The Unicode range for 'Ethiopic Supplement' (U+1380..U+139F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1380.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range EthiopicSupplement = new("Ethiopic Supplement", '\u1380', '\u139F');

    /// <summary>
    /// The Unicode range for 'Cherokee' (U+13A0..U+13FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U13A0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Cherokee = new("Cherokee", '\u13A0', '\u13FF');

    /// <summary>
    /// The Unicode range for 'Unified Canadian Aboriginal Syllabics' (U+1400..U+167F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1400.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range UnifiedCanadianAboriginalSyllabics = new("Unified Canadian Aboriginal Syllabics", '\u1400', '\u167F');

    /// <summary>
    /// The Unicode range for 'Ogham' (U+1680..U+169F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1680.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Ogham = new("Ogham", '\u1680', '\u169F');

    /// <summary>
    /// The Unicode range for 'Runic' (U+16A0..U+16FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U16A0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Runic = new("Runic", '\u16A0', '\u16FF');

    /// <summary>
    /// The Unicode range for 'Tagalog' (U+1700..U+171F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1700.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Tagalog = new("Tagalog", '\u1700', '\u171F');

    /// <summary>
    /// The Unicode range for 'Hanunoo' (U+1720..U+173F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1720.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Hanunoo = new("Hanunoo", '\u1720', '\u173F');

    /// <summary>
    /// The Unicode range for 'Buhid' (U+1740..U+175F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1740.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Buhid = new("Buhid", '\u1740', '\u175F');

    /// <summary>
    /// The Unicode range for 'Tagbanwa' (U+1760..U+177F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1760.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Tagbanwa = new("Tagbanwa", '\u1760', '\u177F');

    /// <summary>
    /// The Unicode range for 'Khmer' (U+1780..U+17FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1780.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Khmer = new("Khmer", '\u1780', '\u17FF');

    /// <summary>
    /// The Unicode range for 'Mongolian' (U+1800..U+18AF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1800.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Mongolian = new("Mongolian", '\u1800', '\u18AF');

    /// <summary>
    /// The Unicode range for 'Unified Canadian Aboriginal Syllabics Extended' (U+18B0..U+18FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U18B0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range UnifiedCanadianAboriginalSyllabicsExtended = new("Unified Canadian Aboriginal Syllabics Extended", '\u18B0', '\u18FF');

    /// <summary>
    /// The Unicode range for 'Limbu' (U+1900..U+194F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1900.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Limbu = new("Limbu", '\u1900', '\u194F');

    /// <summary>
    /// The Unicode range for 'Tai Le' (U+1950..U+197F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1950.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range TaiLe = new("Tai Le", '\u1950', '\u197F');

    /// <summary>
    /// The Unicode range for 'New Tai Lue' (U+1980..U+19DF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1980.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range NewTaiLue = new("New Tai Lue", '\u1980', '\u19DF');

    /// <summary>
    /// The Unicode range for 'Khmer Symbols' (U+19E0..U+19FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U19E0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range KhmerSymbols = new("Khmer Symbols", '\u19E0', '\u19FF');

    /// <summary>
    /// The Unicode range for 'Buginese' (U+1A00..U+1A1F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1A00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Buginese = new("Buginese", '\u1A00', '\u1A1F');

    /// <summary>
    /// The Unicode range for 'Tai Tham' (U+1A20..U+1AAF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1A20.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range TaiTham = new("Tai Tham", '\u1A20', '\u1AAF');

    /// <summary>
    /// The Unicode range for 'Combining Diacritical Marks Extended' (U+1AB0..U+1AFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1AB0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CombiningDiacriticalMarksExtended = new("Combining Diacritical Marks Extended", '\u1AB0', '\u1AFF');

    /// <summary>
    /// The Unicode range for 'Balinese' (U+1B00..U+1B7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1B00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Balinese = new("Balinese", '\u1B00', '\u1B7F');

    /// <summary>
    /// The Unicode range for 'Sundanese' (U+1B80..U+1BBF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1B80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Sundanese = new("Sundanese", '\u1B80', '\u1BBF');

    /// <summary>
    /// The Unicode range for 'Batak' (U+1BC0..U+1BFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1BC0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Batak = new("Batak", '\u1BC0', '\u1BFF');

    /// <summary>
    /// The Unicode range for 'Lepcha' (U+1C00..U+1C4F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1C00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Lepcha = new("Lepcha", '\u1C00', '\u1C4F');

    /// <summary>
    /// The Unicode range for 'Ol Chiki' (U+1C50..U+1C7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1C50.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range OlChiki = new("Ol Chiki", '\u1C50', '\u1C7F');

    /// <summary>
    /// The Unicode range for 'Cyrillic Extended-C' (U+1C80..U+1C8F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1C80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CyrillicExtendedC = new("Cyrillic Extended-C", '\u1C80', '\u1C8F');

    /// <summary>
    /// The Unicode range for 'Georgian Extended' (U+1C90..U+1CBF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1C90.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range GeorgianExtended = new("Georgian Extended", '\u1C90', '\u1CBF');

    /// <summary>
    /// The Unicode range for 'Sundanese Supplement' (U+1CC0..U+1CCF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1CC0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SundaneseSupplement = new("Sundanese Supplement", '\u1CC0', '\u1CCF');

    /// <summary>
    /// The Unicode range for 'Vedic Extensions' (U+1CD0..U+1CFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1CD0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range VedicExtensions = new("Vedic Extensions", '\u1CD0', '\u1CFF');

    /// <summary>
    /// The Unicode range for 'Phonetic Extensions' (U+1D00..U+1D7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1D00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range PhoneticExtensions = new("Phonetic Extensions", '\u1D00', '\u1D7F');

    /// <summary>
    /// The Unicode range for 'Phonetic Extensions Supplement' (U+1D80..U+1DBF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1D80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range PhoneticExtensionsSupplement = new("Phonetic Extensions Supplement", '\u1D80', '\u1DBF');

    /// <summary>
    /// The Unicode range for 'Combining Diacritical Marks Supplement' (U+1DC0..U+1DFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1DC0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CombiningDiacriticalMarksSupplement = new("Combining Diacritical Marks Supplement", '\u1DC0', '\u1DFF');

    /// <summary>
    /// The Unicode range for 'Latin Extended Additional' (U+1E00..U+1EFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1E00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range LatinExtendedAdditional = new("Latin Extended Additional", '\u1E00', '\u1EFF');

    /// <summary>
    /// The Unicode range for 'Greek Extended' (U+1F00..U+1FFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U1F00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range GreekExtended = new("Greek Extended", '\u1F00', '\u1FFF');

    /// <summary>
    /// The Unicode range for 'General Punctuation' (U+2000..U+206F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2000.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range GeneralPunctuation = new("General Punctuation", '\u2000', '\u206F');

    /// <summary>
    /// The Unicode range for 'Superscripts and Subscripts' (U+2070..U+209F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2070.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SuperscriptsAndSubscripts = new("Superscripts and Subscripts", '\u2070', '\u209F');

    /// <summary>
    /// The Unicode range for 'Currency Symbols' (U+20A0..U+20CF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U20A0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CurrencySymbols = new("Currency Symbols", '\u20A0', '\u20CF');

    /// <summary>
    /// The Unicode range for 'Combining Diacritical Marks for Symbols' (U+20D0..U+20FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U20D0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CombiningDiacriticalMarksforSymbols = new("Combining Diacritical Marks for Symbols", '\u20D0', '\u20FF');

    /// <summary>
    /// The Unicode range for 'Letterlike Symbols' (U+2100..U+214F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2100.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range LetterlikeSymbols = new("Letterlike Symbols", '\u2100', '\u214F');

    /// <summary>
    /// The Unicode range for 'Number Forms' (U+2150..U+218F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2150.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range NumberForms = new("Number Forms", '\u2150', '\u218F');

    /// <summary>
    /// The Unicode range for 'Arrows' (U+2190..U+21FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2190.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Arrows = new("Arrows", '\u2190', '\u21FF');

    /// <summary>
    /// The Unicode range for 'Mathematical Operators' (U+2200..U+22FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2200.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MathematicalOperators = new("Mathematical Operators", '\u2200', '\u22FF');

    /// <summary>
    /// The Unicode range for 'Miscellaneous Technical' (U+2300..U+23FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2300.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MiscellaneousTechnical = new("Miscellaneous Technical", '\u2300', '\u23FF');

    /// <summary>
    /// The Unicode range for 'Control Pictures' (U+2400..U+243F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2400.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range ControlPictures = new("Control Pictures", '\u2400', '\u243F');

    /// <summary>
    /// The Unicode range for 'Optical Character Recognition' (U+2440..U+245F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2440.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range OpticalCharacterRecognition = new("Optical Character Recognition", '\u2440', '\u245F');

    /// <summary>
    /// The Unicode range for 'Enclosed Alphanumerics' (U+2460..U+24FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2460.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range EnclosedAlphanumerics = new("Enclosed Alphanumerics", '\u2460', '\u24FF');

    /// <summary>
    /// The Unicode range for 'Box Drawing' (U+2500..U+257F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2500.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range BoxDrawing = new("Box Drawing", '\u2500', '\u257F');

    /// <summary>
    /// The Unicode range for 'Block Elements' (U+2580..U+259F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2580.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range BlockElements = new("Block Elements", '\u2580', '\u259F');

    /// <summary>
    /// The Unicode range for 'Geometric Shapes' (U+25A0..U+25FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U25A0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range GeometricShapes = new("Geometric Shapes", '\u25A0', '\u25FF');

    /// <summary>
    /// The Unicode range for 'Miscellaneous Symbols' (U+2600..U+26FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2600.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MiscellaneousSymbols = new("Miscellaneous Symbols", '\u2600', '\u26FF');

    /// <summary>
    /// The Unicode range for 'Dingbats' (U+2700..U+27BF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2700.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Dingbats = new("Dingbats", '\u2700', '\u27BF');

    /// <summary>
    /// The Unicode range for 'Miscellaneous Mathematical Symbols-A' (U+27C0..U+27EF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U27C0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MiscellaneousMathematicalSymbolsA = new("Miscellaneous Mathematical Symbols-A", '\u27C0', '\u27EF');

    /// <summary>
    /// The Unicode range for 'Supplemental Arrows-A' (U+27F0..U+27FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U27F0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SupplementalArrowsA = new("Supplemental Arrows-A", '\u27F0', '\u27FF');

    /// <summary>
    /// The Unicode range for 'Braille Patterns' (U+2800..U+28FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2800.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range BraillePatterns = new("Braille Patterns", '\u2800', '\u28FF');

    /// <summary>
    /// The Unicode range for 'Supplemental Arrows-B' (U+2900..U+297F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2900.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SupplementalArrowsB = new("Supplemental Arrows-B", '\u2900', '\u297F');

    /// <summary>
    /// The Unicode range for 'Miscellaneous Mathematical Symbols-B' (U+2980..U+29FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2980.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MiscellaneousMathematicalSymbolsB = new("Miscellaneous Mathematical Symbols-B", '\u2980', '\u29FF');

    /// <summary>
    /// The Unicode range for 'Supplemental Mathematical Operators' (U+2A00..U+2AFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2A00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SupplementalMathematicalOperators = new("Supplemental Mathematical Operators", '\u2A00', '\u2AFF');

    /// <summary>
    /// The Unicode range for 'Miscellaneous Symbols and Arrows' (U+2B00..U+2BFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2B00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MiscellaneousSymbolsAndArrows = new("Miscellaneous Symbols and Arrows", '\u2B00', '\u2BFF');

    /// <summary>
    /// The Unicode range for 'Glagolitic' (U+2C00..U+2C5F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2C00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Glagolitic = new("Glagolitic", '\u2C00', '\u2C5F');

    /// <summary>
    /// The Unicode range for 'Latin Extended-C' (U+2C60..U+2C7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2C60.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range LatinExtendedC = new("Latin Extended-C", '\u2C60', '\u2C7F');

    /// <summary>
    /// The Unicode range for 'Coptic' (U+2C80..U+2CFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2C80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Coptic = new("Coptic", '\u2C80', '\u2CFF');

    /// <summary>
    /// The Unicode range for 'Georgian Supplement' (U+2D00..U+2D2F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2D00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range GeorgianSupplement = new("Georgian Supplement", '\u2D00', '\u2D2F');

    /// <summary>
    /// The Unicode range for 'Tifinagh' (U+2D30..U+2D7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2D30.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Tifinagh = new("Tifinagh", '\u2D30', '\u2D7F');

    /// <summary>
    /// The Unicode range for 'Ethiopic Extended' (U+2D80..U+2DDF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2D80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range EthiopicExtended = new("Ethiopic Extended", '\u2D80', '\u2DDF');

    /// <summary>
    /// The Unicode range for 'Cyrillic Extended-A' (U+2DE0..U+2DFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2DE0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CyrillicExtendedA = new("Cyrillic Extended-A", '\u2DE0', '\u2DFF');

    /// <summary>
    /// The Unicode range for 'Supplemental Punctuation' (U+2E00..U+2E7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2E00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SupplementalPunctuation = new("Supplemental Punctuation", '\u2E00', '\u2E7F');

    /// <summary>
    /// The Unicode range for 'CJK Radicals Supplement' (U+2E80..U+2EFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2E80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CjkRadicalsSupplement = new("CJK Radicals Supplement", '\u2E80', '\u2EFF');

    /// <summary>
    /// The Unicode range for 'Kangxi Radicals' (U+2F00..U+2FDF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2F00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range KangxiRadicals = new("Kangxi Radicals", '\u2F00', '\u2FDF');

    /// <summary>
    /// The Unicode range for 'Ideographic Description Characters' (U+2FF0..U+2FFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U2FF0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range IdeographicDescriptionCharacters = new("Ideographic Description Characters", '\u2FF0', '\u2FFF');

    /// <summary>
    /// The Unicode range for 'CJK Symbols and Punctuation' (U+3000..U+303F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U3000.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CjkSymbolsAndPunctuation = new("CJK Symbols and Punctuation", '\u3000', '\u303F');

    /// <summary>
    /// The Unicode range for 'Hiragana' (U+3040..U+309F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U3040.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Hiragana = new("Hiragana", '\u3040', '\u309F');

    /// <summary>
    /// The Unicode range for 'Katakana' (U+30A0..U+30FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U30A0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Katakana = new("Katakana", '\u30A0', '\u30FF');

    /// <summary>
    /// The Unicode range for 'Bopomofo' (U+3100..U+312F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U3100.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Bopomofo = new("Bopomofo", '\u3100', '\u312F');

    /// <summary>
    /// The Unicode range for 'Hangul Compatibility Jamo' (U+3130..U+318F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U3130.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range HangulCompatibilityJamo = new("Hangul Compatibility Jamo", '\u3130', '\u318F');

    /// <summary>
    /// The Unicode range for 'Kanbun' (U+3190..U+319F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U3190.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Kanbun = new("Kanbun", '\u3190', '\u319F');

    /// <summary>
    /// The Unicode range for 'Bopomofo Extended' (U+31A0..U+31BF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U31A0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range BopomofoExtended = new("Bopomofo Extended", '\u31A0', '\u31BF');

    /// <summary>
    /// The Unicode range for 'CJK Strokes' (U+31C0..U+31EF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U31C0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CjkStrokes = new("CJK Strokes", '\u31C0', '\u31EF');

    /// <summary>
    /// The Unicode range for 'Katakana Phonetic Extensions' (U+31F0..U+31FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U31F0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range KatakanaPhoneticExtensions = new("Katakana Phonetic Extensions", '\u31F0', '\u31FF');

    /// <summary>
    /// The Unicode range for 'Enclosed CJK Letters and Months' (U+3200..U+32FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U3200.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range EnclosedCjkLettersandMonths = new("Enclosed CJK Letters and Months", '\u3200', '\u32FF');

    /// <summary>
    /// The Unicode range for 'CJK Compatibility' (U+3300..U+33FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U3300.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CjkCompatibility = new("CJK Compatibility", '\u3300', '\u33FF');

    /// <summary>
    /// The Unicode range for 'CJK Unified Ideographs Extension A' (U+3400..U+4DBF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U3400.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CjkUnifiedIdeographsExtensionA = new("CJK Unified Ideographs Extension A", '\u3400', '\u4DBF');

    /// <summary>
    /// The Unicode range for 'Yijing Hexagram Symbols' (U+4DC0..U+4DFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U4DC0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range YijingHexagramSymbols = new("Yijing Hexagram Symbols", '\u4DC0', '\u4DFF');

    /// <summary>
    /// The Unicode range for 'CJK Unified Ideographs' (U+4E00..U+9FFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/U4E00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CjkUnifiedIdeographs = new("CJK Unified Ideographs", '\u4E00', '\u9FFF');

    /// <summary>
    /// The Unicode range for 'Yi Syllables' (U+A000..U+A48F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA000.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range YiSyllables = new("Yi Syllables", '\uA000', '\uA48F');

    /// <summary>
    /// The Unicode range for 'Yi Radicals' (U+A490..U+A4CF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA490.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range YiRadicals = new("Yi Radicals", '\uA490', '\uA4CF');

    /// <summary>
    /// The Unicode range for 'Lisu' (U+A4D0..U+A4FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA4D0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Lisu = new("Lisu", '\uA4D0', '\uA4FF');

    /// <summary>
    /// The Unicode range for 'Vai' (U+A500..U+A63F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA500.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Vai = new("Vai", '\uA500', '\uA63F');

    /// <summary>
    /// The Unicode range for 'Cyrillic Extended-B' (U+A640..U+A69F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA640.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CyrillicExtendedB = new("Cyrillic Extended-B", '\uA640', '\uA69F');

    /// <summary>
    /// The Unicode range for 'Bamum' (U+A6A0..U+A6FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA6A0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Bamum = new("Bamum", '\uA6A0', '\uA6FF');

    /// <summary>
    /// The Unicode range for 'Modifier Tone Letters' (U+A700..U+A71F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA700.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range ModifierToneLetters = new("Modifier Tone Letters", '\uA700', '\uA71F');

    /// <summary>
    /// The Unicode range for 'Latin Extended-D' (U+A720..U+A7FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA720.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range LatinExtendedD = new("Latin Extended-D", '\uA720', '\uA7FF');

    /// <summary>
    /// The Unicode range for 'Syloti Nagri' (U+A800..U+A82F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA800.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SylotiNagri = new("Syloti Nagri", '\uA800', '\uA82F');

    /// <summary>
    /// The Unicode range for 'Common Indic Number Forms' (U+A830..U+A83F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA830.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CommonIndicNumberForms = new("Common Indic Number Forms", '\uA830', '\uA83F');

    /// <summary>
    /// The Unicode range for 'Phags-pa' (U+A840..U+A87F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA840.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Phagspa = new("Phags-pa", '\uA840', '\uA87F');

    /// <summary>
    /// The Unicode range for 'Saurashtra' (U+A880..U+A8DF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA880.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Saurashtra = new("Saurashtra", '\uA880', '\uA8DF');

    /// <summary>
    /// The Unicode range for 'Devanagari Extended' (U+A8E0..U+A8FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA8E0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range DevanagariExtended = new("Devanagari Extended", '\uA8E0', '\uA8FF');

    /// <summary>
    /// The Unicode range for 'Kayah Li' (U+A900..U+A92F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA900.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range KayahLi = new("Kayah Li", '\uA900', '\uA92F');

    /// <summary>
    /// The Unicode range for 'Rejang' (U+A930..U+A95F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA930.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Rejang = new("Rejang", '\uA930', '\uA95F');

    /// <summary>
    /// The Unicode range for 'Hangul Jamo Extended-A' (U+A960..U+A97F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA960.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range HangulJamoExtendedA = new("Hangul Jamo Extended-A", '\uA960', '\uA97F');

    /// <summary>
    /// The Unicode range for 'Javanese' (U+A980..U+A9DF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA980.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Javanese = new("Javanese", '\uA980', '\uA9DF');

    /// <summary>
    /// The Unicode range for 'Myanmar Extended-B' (U+A9E0..U+A9FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UA9E0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MyanmarExtendedB = new("Myanmar Extended-B", '\uA9E0', '\uA9FF');

    /// <summary>
    /// The Unicode range for 'Cham' (U+AA00..U+AA5F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UAA00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Cham = new("Cham", '\uAA00', '\uAA5F');

    /// <summary>
    /// The Unicode range for 'Myanmar Extended-A' (U+AA60..U+AA7F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UAA60.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MyanmarExtendedA = new("Myanmar Extended-A", '\uAA60', '\uAA7F');

    /// <summary>
    /// The Unicode range for 'Tai Viet' (U+AA80..U+AADF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UAA80.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range TaiViet = new("Tai Viet", '\uAA80', '\uAADF');

    /// <summary>
    /// The Unicode range for 'Meetei Mayek Extensions' (U+AAE0..U+AAFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UAAE0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MeeteiMayekExtensions = new("Meetei Mayek Extensions", '\uAAE0', '\uAAFF');

    /// <summary>
    /// The Unicode range for 'Ethiopic Extended-A' (U+AB00..U+AB2F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UAB00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range EthiopicExtendedA = new("Ethiopic Extended-A", '\uAB00', '\uAB2F');

    /// <summary>
    /// The Unicode range for 'Latin Extended-E' (U+AB30..U+AB6F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UAB30.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range LatinExtendedE = new("Latin Extended-E", '\uAB30', '\uAB6F');

    /// <summary>
    /// The Unicode range for 'Cherokee Supplement' (U+AB70..U+ABBF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UAB70.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CherokeeSupplement = new("Cherokee Supplement", '\uAB70', '\uABBF');

    /// <summary>
    /// The Unicode range for 'Meetei Mayek' (U+ABC0..U+ABFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UABC0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range MeeteiMayek = new("Meetei Mayek", '\uABC0', '\uABFF');

    /// <summary>
    /// The Unicode range for 'Hangul Syllables' (U+AC00..U+D7AF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UAC00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range HangulSyllables = new("Hangul Syllables", '\uAC00', '\uD7AF');

    /// <summary>
    /// The Unicode range for 'Hangul Jamo Extended-B' (U+D7B0..U+D7FF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UD7B0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range HangulJamoExtendedB = new("Hangul Jamo Extended-B", '\uD7B0', '\uD7FF');

    /// <summary>
    /// The Unicode range for 'CJK Compatibility Ideographs' (U+F900..U+FAFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UF900.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CjkCompatibilityIdeographs = new("CJK Compatibility Ideographs", '\uF900', '\uFAFF');

    /// <summary>
    /// The Unicode range for 'Alphabetic Presentation Forms' (U+FB00..U+FB4F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFB00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range AlphabeticPresentationForms = new("Alphabetic Presentation Forms", '\uFB00', '\uFB4F');

    /// <summary>
    /// The Unicode range for 'Arabic Presentation Forms-A' (U+FB50..U+FDFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFB50.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range ArabicPresentationFormsA = new("Arabic Presentation Forms-A", '\uFB50', '\uFDFF');

    /// <summary>
    /// The Unicode range for 'Variation Selectors' (U+FE00..U+FE0F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFE00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range VariationSelectors = new("Variation Selectors", '\uFE00', '\uFE0F');

    /// <summary>
    /// The Unicode range for 'Vertical Forms' (U+FE10..U+FE1F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFE10.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range VerticalForms = new("Vertical Forms", '\uFE10', '\uFE1F');

    /// <summary>
    /// The Unicode range for 'Combining Half Marks' (U+FE20..U+FE2F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFE20.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CombiningHalfMarks = new("Combining Half Marks", '\uFE20', '\uFE2F');

    /// <summary>
    /// The Unicode range for 'CJK Compatibility Forms' (U+FE30..U+FE4F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFE30.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range CjkCompatibilityForms = new("CJK Compatibility Forms", '\uFE30', '\uFE4F');

    /// <summary>
    /// The Unicode range for 'Small Form Variants' (U+FE50..U+FE6F).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFE50.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range SmallFormVariants = new("Small Form Variants", '\uFE50', '\uFE6F');

    /// <summary>
    /// The Unicode range for 'Arabic Presentation Forms-B' (U+FE70..U+FEFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFE70.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range ArabicPresentationFormsB = new("Arabic Presentation Forms-B", '\uFE70', '\uFEFF');

    /// <summary>
    /// The Unicode range for 'Halfwidth and Fullwidth Forms' (U+FF00..U+FFEF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFF00.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range HalfwidthandFullwidthForms = new("Halfwidth and Fullwidth Forms", '\uFF00', '\uFFEF');

    /// <summary>
    /// The Unicode range for 'Specials' (U+FFF0..U+FFFF).
    /// </summary>
    /// <remarks>
    /// See https://www.unicode.org/charts/PDF/UFFF0.pdf for the characters in the range.
    /// </remarks>
    public static readonly Range Specials = new("Specials", '\uFFF0', '\uFFFF');

    #endregion Public Static Fields
}

