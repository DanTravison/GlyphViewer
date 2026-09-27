namespace GlyphViewer.Settings;

using GlyphViewer.Settings.Properties;
using GlyphViewer.Text;
using System.ComponentModel;

/// <summary>
/// Provides a <see cref="Setting"/> for font properties.
/// </summary>
public abstract class FontSetting : Setting
{
    #region Fields

    string _sample = string.Empty;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="parent">The parent <see cref="ISetting"/>.</param>
    /// <param name="name">The <see cref="ISettingProperty.Name"/> of the setting..</param>
    /// <param name="displayName">The <see cref="ISettingProperty.DisplayName"/> of the setting..</param>
    /// <param name="description">The <see cref="ISettingProperty.Description"/> of the setting.</param>
    /// <param name="defaultFontFamily">The <see cref="FontFamily"/> default value.</param>
    /// <param name="defaultFontSize">The <see cref="FontSize"/> default value.</param>
    /// <param name="minimumFontSize">The <see cref="FontSize"/> minimum value.</param>
    /// <param name="maximumFonSize">The <see cref="FontSize"/> maximum value.</param>
    /// <param name="defualtFontAttributes">The <see cref="FontAttributes"/> default value.</param>
    /// <param name="sample">The <see cref="Sample"/> to display in the UI.</param>
    protected FontSetting
    (
        ISetting parent,
        string name,
        string displayName,
        string description,
        FontFamily defaultFontFamily,
        double defaultFontSize,
        double minimumFontSize,
        double maximumFonSize,
        FontAttributes defualtFontAttributes,
        string sample = null
    )
        : base(parent, name, displayName, description)
    {
        FontFamily = new(defaultFontFamily);
        FontSize = new
        (
            defaultFontSize,
            minimumFontSize,
            maximumFonSize
        );
        FontAttributes = new
        (
            defualtFontAttributes
        );
        AddRange(FontFamily, FontSize, FontAttributes);

        // Consider making this a StringProperty
        Sample = sample;
        FontFamily.ValueChanged += OnFamilyNameChanged;
    }

    #endregion Constructors

    #region Event Handlers

    private void OnFamilyNameChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_sample))
        {
            OnPropertyChanged(SampleChangedEventArgs);
        }
    }

    #endregion Event Handlers

    #region Properties

    /// <summary>
    /// Gets the <see cref="FontFamily"/>.
    /// </summary>
    public FontFamilyProperty FontFamily
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="Double"/> font size.
    /// </summary>
    public FontSizeProperty FontSize
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="FontAttributes"/> font attributes.
    /// </summary>
    public FontAttributesProperty FontAttributes
    {
        get;
    }

    /// <summary>
    /// Gets or sets the sample text to display in the UI when configuring the font.
    /// </summary>
    /// <remarks>
    /// The default value is the <see cref="FontFamily"/> value.
    /// </remarks>
    public string Sample
    {
        get => string.IsNullOrEmpty(_sample) ? FontFamily.Value.Name : _sample;
        set => SetProperty(ref _sample, value, SampleChangedEventArgs);
    }

    #endregion Properties

    #region PropertyChangedEventArgs

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="Sample"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs SampleChangedEventArgs = new(nameof(Sample));

    #endregion PropertyChangedEventArgs
}
