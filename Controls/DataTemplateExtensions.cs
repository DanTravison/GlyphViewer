namespace GlyphViewer.Controls;

static public class DataTemplateExtensions
{
    /// <summary>
    /// Creates a View from a DataTemplate.
    /// </summary>
    /// <param name="template">The <see cref="DataTemplate"/> or <see cref="DataTemplateSelector"/>.</param>
    /// <param name="container">The containing <see cref="BindableObject"/>.</param>
    /// <param name="value">The value to set for the view's <see cref="BindableObject.BindingContext"/>.</param>
    /// <returns>A new instance of a <see cref="View"/>; otherwise, a null reference.</returns>
    public static View CreateView(this DataTemplate template, BindableObject container, object value)
    {
        if (template is DataTemplateSelector selector)
        {
            template = selector.SelectTemplate(value, container);
        }
        if (template is not null && template.CreateContent() is View view)
        {
            view.BindingContext = value;
            return view;
        }
        return null;
    }
}
