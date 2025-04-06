namespace GlyphViewer.Controls;

using System.Collections.ObjectModel;

/// <summary>
/// Provides a <see cref="ColumnDefinition"/> collection.
/// </summary>
public sealed class ColumnDefinitionCollection : ObservableCollection<ColumnDefinition>
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public ColumnDefinitionCollection()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="definitions">The <see cref="ColumnDefinition"/> elements to use to populate the
    /// collection.
    /// </param>
    public ColumnDefinitionCollection(params ColumnDefinition[] definitions)
        : base(definitions)
    {
    }
}

