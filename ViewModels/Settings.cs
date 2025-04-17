namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using GlyphViewer.Views;
using System.ComponentModel;

/// <summary>
/// Provides a view model for managing user settings.
/// </summary>
public sealed class Settings : ObservableObject
{
    #region Fields

    /// <summary>
    /// Defines the default width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double DefaultGlyphWidth = 300;
    double _glyphWidth;

    /// <summary>
    /// Defines the default font size of the <see cref="GlyphsView"/> Glyph items.
    /// </summary>
    public const double DefaultItemFontSize = 32;
    double _itemFontSize;

    /// <summary>
    /// Define the default font size of the main page header text.
    /// </summary>
    public const double DefaultHeaderFontSize = 32;
    double _headerFontSize;

    /// <summary>
    /// Define the default font size of the <see cref="GlyphsView"/> header items.
    /// </summary>
    public const double DefaultHeaderItemFontSize = 20;
    double _headerItemFontSize;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public Settings()
    {
        _glyphWidth = GetPreference(nameof(GlyphWidth), DefaultGlyphWidth);
        _itemFontSize = GetPreference(nameof(ItemFontSize), DefaultItemFontSize);
        _headerFontSize = GetPreference(nameof(HeaderFontSize), DefaultHeaderFontSize);
        _headerItemFontSize = GetPreference(nameof(HeaderItemFontSize), DefaultHeaderItemFontSize);
    }

    /// <summary>
    /// Gets or sets the desired width of the <see cref="GlyphView"/>
    /// </summary>
    public double GlyphWidth
    {
        get => _glyphWidth;
        set => SetPreference(ref _glyphWidth, value, GlyphWidthChangedEventArgs);
    }

    /// <summary>
    /// Gets or sets the font size for the <see cref="GlyphsView"/> Glyph items.
    /// </summary>
    public double ItemFontSize
    {
        get => _itemFontSize;
        set => SetPreference(ref _itemFontSize, value, ItemFontSizeChangedEventArgs);
     }

    /// <summary>
    /// Gets or sets the font size for the <see cref="GlyphsView"/> Glyph items.
    /// </summary>
    public double HeaderItemFontSize
    {
        get => _headerItemFontSize;
        set => SetPreference(ref _headerItemFontSize, value, HeaderItemFontSizeChangedEventArgs);
    }

    /// <summary>
    /// Gets or sets the font size for the main page header text.
    /// </summary>
    public double HeaderFontSize
    {
        get => _headerFontSize;
        set => SetPreference(ref _headerFontSize, value, HeaderFontSizeChangedEventArgs);
    }

    void SetPreference(ref double field, double value, PropertyChangedEventArgs e)
    {
        if (SetProperty(ref field, value, e))
        {
            Preferences.Set(nameof(field), value);
        }
    }

    static double GetPreference(string key, double defaultValue)
    {
        if (Preferences.ContainsKey(key))
        {
            return Preferences.Get(key, defaultValue);
        }
        return defaultValue;
    }

    #region PropertyChangedEventArgs

    public static PropertyChangedEventArgs GlyphWidthChangedEventArgs = new(nameof(GlyphWidth));
    public static PropertyChangedEventArgs ItemFontSizeChangedEventArgs = new(nameof(ItemFontSize));
    public static PropertyChangedEventArgs HeaderItemFontSizeChangedEventArgs = new(nameof(HeaderItemFontSize));
    public static PropertyChangedEventArgs HeaderFontSizeChangedEventArgs = new(nameof(HeaderFontSize));

    #endregion PropertyChangedEventArgs

}
