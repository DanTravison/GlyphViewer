namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using GlyphViewer.ViewModels;
using GlyphViewer.Views;
using System.Collections;
using System.ComponentModel;

/// <summary>
/// Provides a view model for managing user settings.
/// </summary>
public sealed class UserSettings : ObservableObject
{
    class SettingCollection : IReadOnlyCollection<ISetting>
    {
        readonly List<ISetting> _settings = [];
        public SettingCollection()
        {
        }

        public int Count
        {
            get => _settings.Count;
        }

        public S Add<S>(S setting)
            where S : ISetting
        {
            ArgumentNullException.ThrowIfNull(setting);
            _settings.Add(setting);
            return setting;
        }

        public void Reset()
        {
            foreach (var setting in _settings)
            {
                setting.Reset();
            }
        }

        public IEnumerator<ISetting> GetEnumerator()
        {
            return _settings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_settings).GetEnumerator();
        }   
    }

    #region Fields

    readonly SettingCollection _settings = new SettingCollection();

    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double MinimumGlyphWidth = 200;

    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double MaximumGlyphWidth = 500;

    /// <summary>
    /// Defines the default width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double DefaultGlyphWidth = 300;
    GlyphWidthSetting _glyphWidth;

    /// <summary>
    /// Defines the minimum <see cref="GlyphsView.ItemFontSize"/>.
    /// </summary>
    public const double MinimumItemFontSize = 12;

    /// <summary>
    /// Defines the maximum <see cref="GlyphsView.ItemFontSize"/>.
    /// </summary>
    public const double MaximumItemFontSize = 40;

    /// <summary>
    /// Defines the default <see cref="GlyphsView.ItemFontSize"/>.
    /// </summary>
    public const double DefaultItemFontSize = 32;
    FontSizeSetting _itemFontSize;

    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double MinimumTitleFontSize = 20;

    /// <summary>
    /// Defines the minimum width of the <see cref="GlyphView"/>.
    /// </summary>
    public const double MaximumTitleFontSize = 50;

    /// <summary>
    /// Define the default font size of the main page header text.
    /// </summary>
    public const double DefaultTitleFontSize = 32;
    FontSizeSetting _titleFontSize;

    /// <summary>
    /// Defines the minimum <see cref="GlyphsView.HeaderFontSize"/>.
    /// </summary>
    public const double MinimumItemHeaderFontSize = 8;

    /// <summary>
    /// Defines the maximum <see cref="GlyphsView.HeaderFontSize"/>.
    /// </summary>
    public const double MaximumItemHeaderFontSize = 40;

    /// <summary>
    /// Define the default font size of the <see cref="GlyphsView.HeaderFontSize"/>.
    /// </summary>
    public const double DefaultItemHeaderFontSize = 20;
    FontSizeSetting _itemHeaderFontSize;

    /// <summary>
    /// Defines the default color for the <see cref="GlyphsView.HeaderColor"/>.
    /// </summary>
    public static readonly Color DefaultItemHeaderColor = Colors.Black;

    /// <summary>
    /// Defines the default color Glyphs in the <see cref="GlyphsView.Items"/> collection.
    /// </summary>
    public static readonly Color DefaultItemColor = Colors.Black;

    /// <summary>
    /// Defines the default border color for the <see cref="GlyphsView.SelectedItem"/>
    /// </summary>
    public static readonly Color DefaultSelectedItemColor = Colors.Plum;

    #endregion Fields

    const string SampleItemFontText =
        FluentUI.ArrowExportRTL + " "
        + FluentUI.MusicNote1 + " "
        + FluentUI.MusicNote2 + " "
        + FluentUI.ArrowExportLTR;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public UserSettings()
    {
        Properties = _settings;

        _glyphWidth = _settings.Add(new GlyphWidthSetting
        (
            OnPropertyChanged, GlyphWidthChangedEventArgs, 
            DefaultGlyphWidth,
            Strings.GlyphWidthLabel, Strings.GlyphWidthDescription
        )
        {
            MininumValue = MinimumGlyphWidth,
            MaximumValue = MaximumGlyphWidth,
            Increment = 10
        });

        _itemFontSize = _settings.Add(new FontSizeSetting
        (
            OnPropertyChanged, ItemFontSizeChangedEventArgs, 
            DefaultItemFontSize,
            Strings.ItemFontSizeLabel, Strings.ItemFontSizeDescription            
        )
        {
            MininumValue = MinimumItemFontSize,
            MaximumValue = MaximumItemFontSize,
            Increment = 1,
            Text = SampleItemFontText,
            FontFamily = FluentUI.FamilyName
        });

        _itemHeaderFontSize = _settings.Add(new FontSizeSetting
        (
            OnPropertyChanged, ItemHeaderFontSizeChangedEventArgs,
            DefaultItemHeaderFontSize,
            Strings.ItemHeaderFontSizeLabel, Strings.ItemHeaderFontSizeDescription
        )
        {
            MininumValue = MinimumItemHeaderFontSize,
            MaximumValue = MaximumItemHeaderFontSize,
            Increment = 1,
            Text = Text.Unicode.Ranges.Latin1Supplement.Name,
            FontFamily = Strings.DefaultFontFamily
        }); 
        
        _titleFontSize = _settings.Add(new FontSizeSetting
        (
            OnPropertyChanged, TitleFontSizeChangedEventArgs, 
            DefaultTitleFontSize,
            Strings.TitleFontSizeLabel, Strings.TitleFontSizeDescription
        )
        {
            MininumValue = MinimumTitleFontSize,
            MaximumValue = MaximumTitleFontSize,
            Increment = 1,
            Text = Strings.ApplicationName,
            FontFamily=Strings.DefaultFontFamily
        });

        Navigator = new PageNavigator<SettingsPage>(true, this);

        ResetCommand = new Command(Reset)
        {
            IsEnabled = true
        };
    }

    #region Properties

    /// <summary>
    /// Gets or sets the desired width of the <see cref="GlyphView"/>
    /// </summary>
    public double GlyphWidth
    {
        get => _glyphWidth.Value;
        set => _glyphWidth.Value = value;
    }

    /// <summary>
    /// Gets or sets the font size for the Glyphs in the <see cref="GlyphsView"/>.
    /// </summary>
    public double ItemFontSize
    {
        get => _itemFontSize.Value;
        set => _itemFontSize.Value = value;
     }

    /// <summary>
    /// Gets or sets the font size for the item header for Glyph groups in the <see cref="GlyphsView"/>.
    /// </summary>
    public double ItemHeaderFontSize
    {
        get => _itemHeaderFontSize.Value;
        set => _itemHeaderFontSize.Value = value;
    }

    /// <summary>
    /// Gets or sets the font size for the main page title area.
    /// </summary>
    public double TitleFontSize
    {
        get => _titleFontSize.Value;
        set => _titleFontSize.Value = value;
    }

    /// <summary>
    /// Gets the <see cref="ISetting"/> properties as a collection.
    /// </summary>
    public IReadOnlyCollection<ISetting> Properties
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="PageNavigator"/> for opening and closing the settings page.
    /// </summary>
    public PageNavigator<SettingsPage> Navigator
    {
        get;
    }

    /// <summary>
    /// Gets the command to reset the settings to the default values.
    /// </summary>
    public Command ResetCommand
    {
        get;
    }

    #endregion Properties

    /// <summary>
    /// Reset the settings to the default values.
    /// </summary>
    public void Reset()
    {
        _settings.Reset();
    }

    public static double Constrain(object newValue, double minimum, double maximum, double defaultValue)
    {
        if (newValue is double value)
        {
            if (value < minimum)
            {
                value = minimum;
            }
            else if (value > maximum)
            {
                value = maximum;
            }
            return value;
        }
        return defaultValue;
    }

    #region PropertyChangedEventArgs

    public static PropertyChangedEventArgs GlyphWidthChangedEventArgs = new(nameof(GlyphWidth));
    public static PropertyChangedEventArgs ItemFontSizeChangedEventArgs = new(nameof(ItemFontSize));
    public static PropertyChangedEventArgs ItemHeaderFontSizeChangedEventArgs = new(nameof(ItemHeaderFontSize));
    public static PropertyChangedEventArgs TitleFontSizeChangedEventArgs = new(nameof(TitleFontSize));

    #endregion PropertyChangedEventArgs

}
