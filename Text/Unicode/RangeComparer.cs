namespace GlyphViewer.Text.Unicode;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

/// <summary>
/// Provides an <see cref="IComparer{Range}"/>.
/// </summary>
public sealed class RangeComparer : IComparer<Range>, IEqualityComparer<Range>
{
    public static readonly RangeComparer Comparer = new();

    private RangeComparer()
    {
    }

    public int Compare(Range x, Range y)
    {
        if (x.First > y.First)
        {
            return 1;
        }
        if (x.First < y.First)
        {
            return -1;
        }
        if (x.Last > y.Last)
        {
            return 1;
        }
        if (x.Last < y.Last)
        {
            return -1;
        }
        return 0;
    }

    public bool Equals(Range x, Range y)
    {
        return x.Equals(y);
    }

    public int GetHashCode([DisallowNull] Range obj)
    {
        return obj.GetHashCode();
    }
}
