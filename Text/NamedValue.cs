namespace GlyphViewer.Text;

/// <summary>
/// Provides a named value.
/// </summary>
public class NamedValue
{
    /// <summary>
    /// Initializes a new instance of this class
    /// </summary>
    /// <param name="name">The <see cref="Name"/> of the <paramref name="value"/></param>
    /// <param name="value">The associated <paramref name="value"/>.</param>
    public NamedValue(string name, object value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name
    {
        get;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public object Value
    {
        get;
    }
}
