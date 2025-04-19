namespace GlyphViewer.Settings;

/// <summary>
/// Provides a <see cref="DataTemplateSelector"/> for selecting the appropriate <see cref="DataTemplate"/> 
/// various <see cref="ISetting"/> instances.
/// </summary>
internal class SettingDataTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="FontSizeSetting"/>.
    /// </summary>
    public DataTemplate FontSize
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="GlyphWidthSetting"/>.
    /// </summary>
    public DataTemplate GlyphWidth
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> for a <see cref="DoubleSetting"/>.
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
        if (item is FontSizeSetting)
        {
            return FontSize;
        }
        if (item is GlyphWidthSetting)
        {
            return GlyphWidth;
        }
        if (item is DoubleSetting)
        {
            return Double;
        }
        throw new NotSupportedException($"The type {item.GetType()} is not supported.");
    }
}
