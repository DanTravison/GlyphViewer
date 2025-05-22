namespace GlyphViewer.Settings;
using GlyphViewer.Views.Renderers;
using System.Diagnostics;

/// <summary>
/// Defines the <see cref="CellLayoutStyle.Height"/> values..
/// </summary>
public enum CellHeightLayout
{
    /// <summary>
    ///  The cell height is defined by the maximum glyph height in the font.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The cell height is defined by the maximum glyph height in the row.
    /// </summary>
    Dynamic
}

/// <summary>
/// Defines the <see cref="CellLayoutStyle.Width"/> values.
/// </summary>
public enum CellWidthLayout
{
    /// <summary>
    /// The cell width is defined by the maximum glyph width in the font.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The cell width is defined by the maximum glyph width in the row.
    /// </summary>
    Width,

    /// <summary>
    /// The cell width is defined by the width of the individual glyph.
    /// </summary>
    Dynamic
}

/// <summary>
/// Defines the layout style for <see cref="IGlyphRow"/> cells.
/// </summary>
[DebuggerDisplay("{Width,nq},{Height,nq}")]
public readonly struct CellLayoutStyle : IEquatable<CellLayoutStyle>
{
    #region Fields

    /// <summary>
    /// Gets the default <see cref="CellLayoutStyle"/>.
    /// </summary>
    /// <remarks>
    /// A <see cref="CellLayoutStyle"/> with <see cref="Width"/> set to <see cref="CellWidthLayout.Default"/>
    /// and <see cref="Height"/> set to <see cref="CellHeightLayout.Default"/>.
    /// </remarks>
    public static readonly CellLayoutStyle Default = new();

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes the default instance of this class.
    /// </summary>
    public CellLayoutStyle()
        : this(CellWidthLayout.Default, CellHeightLayout.Default)
    { }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="width">The <see cref="CellWidthLayout"/> for the <see cref="Width"/>.</param>
    /// <param name="height">The <see cref="CellHeightLayout"/> for the <see cref="Height"/>.</param>
    public CellLayoutStyle(CellWidthLayout width, CellHeightLayout height)
    {
        Height = height;
        Width = width;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the <see cref="CellHeightLayout"/> for determining the cell width.
    /// </summary>
    public readonly CellWidthLayout Width;

    /// <summary>
    /// Gets the <see cref="CellHeightLayout"/> for determining the cell height.
    /// </summary>
    public readonly CellHeightLayout Height;


    #endregion Properties

    #region Equality

    /// <summary>
    /// Determines if the specified <paramref name="obj"/> is equal to this struct.
    /// </summary>
    /// <param name="obj">The object to compare..</param>
    /// <returns>
    /// true <paramref name="obj"/> is a <see cref="CellLayoutStyle"/> and is equal to this struct;
    /// otherwise, false.
    /// </returns>
    public override readonly bool Equals(object obj)
    {
        if (obj is CellLayoutStyle cellStyle)
        {
            return Equals(cellStyle);
        }
        return false;
    }

    /// <summary>
    /// Determines if the specified <paramref name="other"/> is equal to this struct.
    /// </summary>
    /// <param name="other">The <see cref="CellLayoutStyle"/> to compare..</param>
    /// <returns>
    /// true if <paramref name="other"/> is equal to this struct; otherwise, false.
    /// </returns>
    public readonly bool Equals(CellLayoutStyle other)
    {
        return Width == other.Width && Height == other.Height;
    }

    /// <summary>
    ///  Gets a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    #endregion Equality

    #region ToString/Parse

    /// <summary>
    /// Gets the current instance as a string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Format("{0},{1}", Enum.GetName(Width), Enum.GetName(Height));
    }

    /// <summary>
    /// Tries to parse a string value into a <see cref="CellLayoutStyle"/>.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="cellStyle">The <see cref="CellLayoutStyle"/>.</param>
    /// <returns>
    /// true if <paramref name="value"/> is a null, empty or whitespace-only string
    /// -or-
    /// true if <paramref name="value"/> was successfully parsed; otherwise, 
    /// false if the <paramref name="value"/> could not be parsed.
    /// </returns>
    public static bool TryParse(string value, out CellLayoutStyle cellStyle)
    {
        bool result = false;
        cellStyle = Default;

        do
        {
            if (value is not null)
            {
                value = value.Trim();
            }
            if (string.IsNullOrEmpty(value))
            {
                // An empty string equates to the default value.
                result = true;
                break;
            }

            // tokenize on comma or white space
            string[] parts = value.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                break;
            }

            if (!Enum.TryParse(parts[0], out CellWidthLayout width))
            {
                break;
            }

            if (!Enum.TryParse(parts[1], out CellHeightLayout height))
            {
                break;
            }

            cellStyle = new(width, height);
            result = true;

        } while (false);

        return result;
    }

    #endregion ToString/Parse

    #region Operator overloads

    /// <summary>
    /// Tests whether two <see cref="CellLayoutStyle"/> structures are equal.
    /// </summary>
    /// <param name="left">The <see cref="CellLayoutStyle"/> structure that is to the left of the equality operator.</param>
    /// <param name="right">The <see cref="CellLayoutStyle"/> structure that is to the right of the equality operator.</param>
    /// <returns>true if the two <see cref="CellLayoutStyle"/> structures are equal; otherwise, false.</returns>
    public static bool operator ==(CellLayoutStyle left, CellLayoutStyle right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Tests whether two <see cref="CellLayoutStyle"/> structures are not equal.
    /// </summary>
    /// <param name="left">The <see cref="CellLayoutStyle"/> structure that is to the left of the equality operator.</param>
    /// <param name="right">The <see cref="CellLayoutStyle"/> structure that is to the right of the equality operator.</param>
    /// <returns>true if the two <see cref="CellLayoutStyle"/> structures are not equal; otherwise, false.</returns>
    public static bool operator !=(CellLayoutStyle left, CellLayoutStyle right)
    {
        return !left.Equals(right);
    }

    #endregion Operator overloads
}
