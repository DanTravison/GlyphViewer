namespace GlyphViewer.Controls;

using GlyphViewer.ObjectModel;
using System.ComponentModel;

/// <summary>
/// Provides a column definition for a <see cref="Grid"/>.
/// </summary>
public sealed class ColumnDefinition : ObservableObject
{
    #region Fields

    ColumnWidth _width;
    DataTemplate _itemTemplate;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public ColumnDefinition()
    {
        _width = ColumnWidth.Auto;
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="width">The <see cref="ColumnWidth"/> defining the width of the column.</param>
    public ColumnDefinition(ColumnWidth width)
    {
        _width = width;
    }

    #region Properties

    /// <summary>
    /// Gets or sets width of the column.
    /// </summary>
    [TypeConverter(typeof(ColumnWidthTypeConverter))]
    public ColumnWidth Width
    {
        get => _width;
        set
        {
            _width = value;
            OnPropertyChanged(WidthChangedEventArgs);
        }
    }

    /// <summary>
    /// Gets or sets <see cref="DataTemplate"/> for contents of the column.
    /// </summary>
    public DataTemplate ItemTemplate
    {
        get => _itemTemplate;
        set
        {
            if (!ReferenceEquals(_itemTemplate, value))
            {
                _itemTemplate = value;
                OnPropertyChanged(ItemTemplateChangedEventArgs);
            }
        }
    }

    #endregion Properties

    #region Cached PropertyChangedEventArgs

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="Width"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs WidthChangedEventArgs = new(nameof(Width));

    /// <summary>
    /// Provides <see cref="PropertyChangedEventArgs"/> when <see cref="ItemTemplate"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs ItemTemplateChangedEventArgs = new(nameof(ItemTemplate));

    #endregion Cached PropertyChangedEventArgs
}
