namespace CodeCadence.Typography.Unicode;

using System;
using System.Collections.Generic;
using System.Text;

internal sealed class RangeSearchComparer : IComparer<Range>
{
    public static readonly RangeSearchComparer Comparer = new();

    private RangeSearchComparer()
    { }

    public static Range Find(List<Range> ranges, uint codepoint)
    {
        Range key = new("", codepoint, codepoint);
        int index = ranges.BinarySearch(key, Comparer);
        if (index >= 0)
        {
            return ranges[index];
        }
        return Range.None;
    }

    /// <summary>
    /// Compares a Range item against a target codepoint.
    /// </summary>
    /// <param name="range">The <see cref="Range"/> element from the sorted list.</param>
    /// <param name="search">The 'synthetic' <see cref="Range"/> containing the codepoint to find.</param>
    public int Compare(Range range, Range search)
    {
        uint codepoint = search.First;

        if (codepoint < range.First)
        {
            // The specified range is after the range containing the target codepoint.
            return 1;
        }
        if (codepoint > range.Last)
        {
            // The specified range is before the range containing the target codepoint.
            return -1;
        }
        return 0;
    }
}
