using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlyphViewer.Text;

public sealed class GlyphComparer : IComparer<Glyph>
{
    public static readonly GlyphComparer Comparer = new GlyphComparer();

    private GlyphComparer()
    {
    }

    public int Compare(Glyph x, Glyph y)
    {
        if (x is null && y is null)
        {
            return 0;
        }
        else if (x is null)
        {
            return -1;
        }
        else if (y is null)
        {
            return 1;
        }
        else
        {
            return x.CodePoint - y.CodePoint;
        }
    }
}


