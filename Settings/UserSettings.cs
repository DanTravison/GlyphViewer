namespace GlyphViewer.Settings;

using GlyphViewer.Converter;
using GlyphViewer.Diagnostics;
using GlyphViewer.Resources;
using GlyphViewer.Settings.Properties;
using GlyphViewer.Views;

using System.Text.Json;

/// <summary>
/// Provides a model for managing user settings.
/// </summary>
internal sealed class UserSettings : SettingPropertyCollection, ISetting
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public UserSettings()
        : base(null, nameof(UserSettings))
    {
        Glyph = AddItem(new GlyphSetting(this));
        ItemFont = AddItem(new ItemFontSetting(this));
        ItemHeaderFont = AddItem(new ItemHeaderFontSetting(this));
        TitleFont = AddItem(new TitleFontSetting(this));
        Bookmarks = AddItem(new Bookmarks(this));
        Fonts = AddItem(new FileFonts(this));
    }

    #region Properties

    /// <summary>
    /// Gets or sets the desired width of the <see cref="GlyphView"/>
    /// </summary>
    public GlyphSetting Glyph
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
    /// Gets the <see cref="Bookmarks"/>.
    /// </summary>
    public Bookmarks Bookmarks
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="FileFonts"/> for managing fonts on the local file system.
    /// </summary>
    public FileFonts Fonts
    {
        get;
    }

    /// <summary>
    /// Gets the parent <see cref="ISetting"/>.
    /// </summary>
    /// <value>
    /// This property always returns a null reference.
    /// </value>
    public ISetting Parent
    {
        get;
    }

    #endregion Properties

    #region ISettings Properties

    /// <summary>
    /// Gets the name to display in the UI.
    /// </summary>
    public string DisplayName
    {
        get => Strings.UserSettingsLabel;
    }

    /// <summary>
    /// Gets the description of the setting.
    /// </summary>
    public string Description
    {
        get => Strings.UserSettingsDescription;
    }

    /// <summary>
    /// Gets the value indicating if the instance is user editable.
    /// </summary>
    /// <value>
    /// this property is alwasy false.
    /// </value>
    public bool CanEdit
    {
        get;
    }

    #endregion ISettings Properties

    #region FileIO

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
                string message = $"Unable to read settings file {fileInfo.FullName}";
                Trace.Exception(nameof(UserSettings), nameof(Load), ex, message);
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
                string message = $"Unable to parse settings file {fileInfo.FullName}";
                Trace.Exception(nameof(UserSettings), nameof(Load), ex, message);
            }

        } while (false);

        settings ??= new();
        settings.HasChanges = false;
        return settings;
    }

    /// <summary>
    /// Saves the <see cref="UserSettings"/>.
    /// </summary>
    public void Save()
    {
        if (HasChanges)
        {
            FileInfo fileInfo = SettingsFile;
            string content = JsonSerializer.Serialize(this, UserSettingsJsonConverter.Options);
            try
            {
                File.WriteAllText(fileInfo.FullName, content, System.Text.Encoding.UTF8);
            }
            catch (IOException ex)
            {
                string message = $"Unable to save settings to {fileInfo.FullName}";
                Trace.Exception(nameof(UserSettings), nameof(Save), ex, message);
            }
            HasChanges = false;
        }
    }

    #endregion FileIO
}
