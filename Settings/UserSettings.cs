namespace GlyphViewer.Settings;

using GlyphViewer.Converter;
using GlyphViewer.ObjectModel;
using GlyphViewer.ViewModels;
using GlyphViewer.Views;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

/// <summary>
/// Provides a model for managing user settings.
/// </summary>
public sealed class UserSettings : ObservableObject, IEnumerable<ISetting>
{
    #region Fields

    readonly SettingCollection _settings;

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

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public UserSettings()
    {
        _settings = new(this.OnPropertyChanged);
        Properties = _settings.Properties;

        GlyphWidth = _settings.Add(new GlyphWidthSetting(_settings, GlyphWidthChangedEventArgs));
        ItemFont = _settings.Add(new ItemFontSetting(_settings, ItemFontSizeChangedEventArgs));
        ItemHeaderFont = _settings.Add(new ItemHeaderFontSetting(_settings, ItemHeaderFontSizeChangedEventArgs));
        TitleFont = _settings.Add(new TitleFontSetting(_settings, TitleFontSizeChangedEventArgs));
        Bookmarks = _settings.Add(new Bookmarks(_settings), false);

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
    public GlyphWidthSetting GlyphWidth
    {
        get;
    }

    /// <summary>
    /// Gets or sets the font size for the Glyphs in the <see cref="GlyphsView"/>.
    /// </summary>
    public ItemFontSetting ItemFont
    {
        get;
    }

    /// <summary>
    /// Gets or sets the font size for the item header for Glyph groups in the <see cref="GlyphsView"/>.
    /// </summary>
    public ItemHeaderFontSetting ItemHeaderFont
    {
        get;
    }

    /// <summary>
    /// Gets or sets the font size for the main page title area.
    /// </summary>
    public TitleFontSetting TitleFont
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="ISetting"/> properties as a collection.
    /// </summary>
    public IReadOnlyCollection<ISetting> Properties
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="ISetting"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="ISetting"/> to get.</param>
    /// <returns>
    /// The <see cref="ISetting"/> with the specified <paramref name="name"/>;
    /// otherwise, a null reference.
    /// </returns>
    internal ISetting this[string name]
    {
        get => _settings[name];
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

    /// <summary>
    /// Gets the <see cref="Bookmarks"/>.
    /// </summary>
    internal Bookmarks Bookmarks
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

    #region Serialization

    const string SettingsFileName = "settings.json";

    static FileInfo SettingsFile
    {
        get => new FileInfo(Path.Combine(FileSystem.AppDataDirectory, SettingsFileName));
    }
    
    /// <summary>
    /// Loads the <see cref="UserSettings"/>.
    /// </summary>
    /// <returns>A new instance of a <see cref="UserSettings"/>.</returns>
    public static UserSettings Load()
    {
        UserSettings settings = null;
        FileInfo fileInfo = SettingsFile;
        string content;

        do
        {
            if (!fileInfo.Exists)
            {
                break;
            }

            try
            {
                content = File.ReadAllText(fileInfo.FullName, System.Text.Encoding.UTF8);
            }
            catch (IOException ex)
            {
                string message = $"Unable to read settings file {fileInfo.FullName}.\n{ex.Message}";
                Trace.WriteLine(message);
                break;
            }

            if (string.IsNullOrEmpty(content))
            {
                break;
            }

            try
            {
                settings = JsonSerializer.Deserialize<UserSettings>(content, UserSettingsJsonConverter.Options);
            }
            catch (JsonException ex)
            {
                string message = $"Unable to parse settings file {fileInfo.FullName}.\n{ex.Message}";
                Trace.WriteLine(message);
            }

        } while (false);

        settings ??= new();
        settings._settings.HasChanges = false;
        return settings;
    }

    /// <summary>
    /// Saves the <see cref="UserSettings"/>.
    /// </summary>
    public void Save()
    {
        if (_settings.HasChanges)
        {
            FileInfo fileInfo = SettingsFile;
            string content = JsonSerializer.Serialize(this, UserSettingsJsonConverter.Options);
            try
            {
                File.WriteAllText(fileInfo.FullName, content, System.Text.Encoding.UTF8);
            }
            catch (IOException ex)
            {
                string message = $"Unable to save settings to {fileInfo.FullName}.\n{ex.Message}";
                Trace.WriteLine(message);
            }
            _settings.HasChanges = false;
        }
    }

    #endregion Serialization

    #region IEnumerable

    /// <summary>
    /// Gets an  <see cref="IEnumerator{ISetting}"/> to enumerate all <see cref="ISetting"/> instances.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{ISetting}"/>.</returns>
    public IEnumerator<ISetting> GetEnumerator()
    {
       return _settings.GetEnumerator();
    }

    /// <summary>
    /// Gets an  <see cref="IEnumerator"/> to enumerate all <see cref="ISetting"/> instances.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/>.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_settings).GetEnumerator();
    }

    #endregion IEnumerable

    #region PropertyChangedEventArgs

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="GlyphWidth"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs GlyphWidthChangedEventArgs = new(nameof(GlyphWidth));
    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="ItemFont"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs ItemFontSizeChangedEventArgs = new(nameof(ItemFont));
    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="ItemHeaderFont"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs ItemHeaderFontSizeChangedEventArgs = new(nameof(ItemHeaderFont));
    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="TitleFont"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs TitleFontSizeChangedEventArgs = new(nameof(TitleFont));

    #endregion PropertyChangedEventArgs
}
