namespace GlyphViewer.Controls;

public enum ColumnUnitType
{
    /// <summary>
    /// Interpret the <see cref="ColumnDefinition.Width"/> property value as the count of device-specific units.
    /// </summary>
    Absolute,

    /// <summary>
    /// Ignore the see cref="ColumnDefinition.Width"/> and choose a size that fits the column contents.
    /// </summary>
    Auto,

    /// <summary>
    /// Interpret the <see cref="ColumnDefinition.Width"/> as a proportional width.
    /// </summary>
    Star,

    /// <summary>
    /// Interpret the <see cref="ColumnDefinition.Width"/> as a character count.
    /// </summary>
    /// <example>
    /// Define a column three characters wide, using OpenSansRegular, 24pt, Italic
    /// <para>
    /// 3#, OpenSansRegular, 24, Italic
    /// </para>
    /// </example>
    Character,

    /// <summary>
    /// Calculate the <see cref="ColumnDefinition.Width"/> based on a literal string.
    /// </summary>
    /// <example>
    /// Define a column based on a string literal, using OpenSansRegular, 24pt, Italic
    /// <para>
    /// @000.00, OpenSansRegular, 24, Bold
    /// </para>
    /// </example>
    Literal
}
