
namespace GlyphViewer.Controls;

/// <summary>
/// Provides a delegate for handling <see cref="ColumnDefinition"/> property change event. 
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void ColumnChangedEventHandler(ColumnDefinitionCollection sender, ColumnChangedEventArgs e);

/// <summary>
/// Provides an <see cref="EventArgs"/> for a <see cref="ColumnDefinition"/> property changed.
/// </summary>
public sealed class ColumnChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="column">The <see cref="ColumnDefinition"/> that changed.</param>
    /// <param name="propertyName">The name of the property that changed.</param>
    internal ColumnChangedEventArgs(ColumnDefinition column, string propertyName)
    {
        Column = column;
        PropertyName = propertyName;
    }

    /// <summary>
    /// Gets the <see cref="ColumnDefinition"/> that changed.
    /// </summary>
    public ColumnDefinition Column
    {
        get;
    }

    /// <summary>
    /// Gets the name of the property that changed.
    /// </summary>
    public string PropertyName
    {
        get;
    }
}
