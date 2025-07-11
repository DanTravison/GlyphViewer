﻿namespace GlyphViewer.Settings;

using GlyphViewer.Resources;
using GlyphViewer.Views;

/// <summary>
/// Provides an <see cref="FontSetting"/> for the <see cref="GlyphsView"/> item font.
/// </summary>
public sealed class ItemFontSetting : FontSetting
{
    #region Constants

    /// <summary>
    /// Defines the minimum <see cref="FontSetting.FontSize"/>.
    /// </summary>
    public const double MinimumFontSize = 8;

    /// <summary>
    /// Defines the maximum <see cref="FontSetting.FontSize"/>.
    /// </summary>
    public const double MaximumFontSize = 72;

    /// <summary>
    /// Defines the default <see cref="FontSetting.FontSize"/>.
    /// </summary>
    public const double DefaultFontSize = 32;

    const string SampleText =
        FluentUI.ArrowExportRTL + " "
        + FluentUI.MusicNote1 + " "
        + FluentUI.MusicNote2 + " "
        + FluentUI.ArrowExportLTR;

    /// <summary>
    /// Defines the default text color to use to draw the glyph.
    /// </summary>
    public static readonly Color DefaultItemColor = Colors.Black;

    /// <summary>
    /// Defines the default color for the seleted glyph.
    /// </summary>
    public static readonly Color DefaultSelectedItemColor = Colors.DarkTurquoise;

    /// <summary>
    /// Defines the default color for the selected item background color.
    /// </summary>
    public static readonly Color DefaultSelectedItemBackgroundColor = DefaultSelectedItemColor.WithAlpha(.4f);

    #endregion Constants

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">The parent <see cref="ISetting"/>.</param>
    public ItemFontSetting(ISetting parent)
        : base
        (
            parent,
            nameof(UserSettings.ItemFont),
            Strings.ItemFontName,
            Strings.ItemFontDescription,
            Text.FontFamily.FluentUI, // NOTE: This is only used by PageSettings.
            DefaultFontSize,
            MinimumFontSize,
            MaximumFontSize,
            Microsoft.Maui.Controls.FontAttributes.None,
            SampleText
        )
    {
    }
}
