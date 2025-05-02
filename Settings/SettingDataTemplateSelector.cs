namespace GlyphViewer.Settings;

using GlyphViewer.Settings.Properties;

/// <summary>
/// Provides a <see cref="DataTemplateSelector"/> for selecting the appropriate <see cref="DataTemplate"/> 
/// various <see cref="ISetting"/> instances.
/// </summary>
internal class SettingDataTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="FontSetting"/>.
    /// </summary>
    public DataTemplate Font
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="GlyphSetting"/>.
    /// </summary>
    public DataTemplate Glyph
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="DoubleProperty"/>.
    /// </summary>
    public DataTemplate Double
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a specified <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The <see cref="ISetting"/> item to query.</param>
    /// <param name="container">not used.</param>
    /// <returns>The <see cref="DataTemplate"/> for the specified <paramref name="item"/>.</returns>
    /// <exception cref="NotSupportedException">
    /// The type of the <paramref name="item"/> does not implement <see cref="ISetting"/> or is 
    /// an unsupported <see cref="ISetting"/>.
    /// </exception>
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is FontSetting)
        {
            return Font;
        }
        if (item is GlyphSetting)
        {
            return Glyph;
        }
        if (item is DoubleProperty)
        {
            return Double;
        }
        throw new NotSupportedException($"The type {item.GetType()} is not supported.");
    }
}
