namespace GlyphViewer.ViewModels;

using GlyphViewer.Diagnostics;
using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using GlyphViewer.Settings;
using GlyphViewer.Text;
using GlyphViewer.Views;
using SkiaSharp;
using System.ComponentModel;

/// <summary>
/// Provides a view model for the settings page.
/// </summary>
internal class SettingsViewModel : ObservableObject
{
    #region Fields

    FontFamiliesViewModel _fontFamilies;
    FileFontFamily _selectedItem;
    static readonly PropertyChangedEventArgs SelectedItemChangedEventArgs = new(nameof(SelectedItem));

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="settings">The <see cref="UserSettings"/> to use.</param>
    /// <param name="fontFamilies">The <see cref="FontFamiliesViewModel"/> to update.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="settings"/> or <paramref name="fontFamilies"/> is a null reference.
    /// </exception>
    public SettingsViewModel(UserSettings settings, FontFamiliesViewModel fontFamilies)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        ArgumentNullException.ThrowIfNull(fontFamilies, nameof(fontFamilies));

        _fontFamilies = fontFamilies;
        AddFontCommand = new FileFontCommand(this, true);
        RemoveFontCommand = new FileFontCommand(this, false)
        {
            IsEnabled = false
        };
        UserSettings = settings;
        ResetCommand = new Command(settings.ResetEditable)
        {
            IsEnabled = true
        };
        Navigator = new PageNavigator<SettingsPage>(true, this);
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="PageNavigator"/> for opening and closing the settings page.
    /// </summary>
    public PageNavigator<SettingsPage> Navigator
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="UserSettings"/>.
    /// </summary>
    public UserSettings UserSettings
    {
        get;
    }

    /// <summary>
    /// Gets the command for loading a font from the file system.
    /// </summary>
    public Command AddFontCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command for removing a file system-based font.
    /// </summary>
    public Command RemoveFontCommand
    {
        get;
    }

    /// <summary>
    /// Gets or sets the selected <see cref="FileFontFamily"/>.
    /// </summary>
    public FileFontFamily SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value, ReferenceComparer, SelectedItemChangedEventArgs))
            {
                RemoveFontCommand.IsEnabled = value is not null;
            }
        }
    }

    /// <summary>
    /// Gets the command to reset the user editable settings to the default values.
    /// </summary>
    public Command ResetCommand
    {
        get;
    }

    #endregion Properties

    /// <summary>
    /// Saves the user settings to the file system.
    /// </summary>
    public void Save()
    {
        UserSettings.Save();
    }

    #region Font File Loading

        async void LoadFont()
    {
        FileInfo file = await PickFontFile();
        if (file is not null && file.Exists)
        {
            FileFontFamily fontFamily = new FileFontFamily(file);
            // verify we can load the font.
            try
            {
                if (fontFamily.GetTypeface(SKFontStyle.Normal) is not null)
                {
                    _fontFamilies.Add(fontFamily);
                }
            }
            catch (Exception ex)
            {
                Trace.Exception(this, nameof(LoadFont), ex, $"Cannot load {file.FullName}");
                // TODO: Notify the user.
            }
        }
    }

    void UnloadFont()
    {
        if (SelectedItem is not null)
        {
            _fontFamilies.Remove(SelectedItem);
        }
    }

    static readonly FilePickerFileType _fontFileTypes = new
    (
        new Dictionary<DevicePlatform, IEnumerable<string>>
        {
                {DevicePlatform.WinUI,   [".ttf", ".otf"] },
                {DevicePlatform.macOS,   ["ttf", "otf"] },
                {DevicePlatform.Android, ["font/ttf", "font/otf"] },
                {DevicePlatform.iOS,     ["public.opentype-font", "public.truetype-font"] }
        }
    );

    /// <summary>
    /// Picks a font file from the file system.
    /// </summary>
    /// <returns>The <see cref="FileInfo"/> for the selected file; otherwise, a null reference.</returns>
    static async Task<FileInfo> PickFontFile()
    {
        return await App.PickFile(Strings.FontFilePickerTitle, _fontFileTypes);
    }

    /// <summary>
    /// Provides a <see cref="Command"/> loading or unloading a font file.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of this class.
    /// </remarks>
    /// <param name="model">The <see cref="MainViewModel"/>.</param>
    /// <param name="isLoad">true to load a font; otherwise, false to unload a font.</param>
    class FileFontCommand(SettingsViewModel model, bool isLoad) : Command(NopAction)
    {
        #region Fields

        readonly bool _isLoad = isLoad;
        readonly SettingsViewModel _model = model;

        #endregion Fields

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="parameter">The parameter to process.</param>
        public override void Execute(object parameter)
        {
            if (_isLoad)
            {
                _model.LoadFont();
            }
            else if (_model.SelectedItem is not null)
            {
                _model.UnloadFont();
            }
        }
    }

    #endregion Font File Loading
}