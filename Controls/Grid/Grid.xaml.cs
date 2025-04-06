namespace GlyphViewer.Controls;

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MauiGrid = Microsoft.Maui.Controls.Grid;

/// <summary>
/// Provides a data-bound <see cref="Grid"/>.
/// </summary>
public partial class Grid : ContentView
{
    #region Fields

    DataTemplate _defaultItemTemplate;
    const string DefaultItemTemplate = "GridItemContentTemplate";

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public Grid()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        if (StringComparer.Ordinal.Compare(propertyName, nameof(BackgroundColor)) == 0)
        {
            CoreGrid.BackgroundColor = BackgroundColor;
        }
        else if (StringComparer.Ordinal.Compare(propertyName, nameof(Background)) == 0)
        {
            CoreGrid.Background = Background;
        }
        else if (StringComparer.Ordinal.Compare(propertyName, nameof(Padding)) == 0)
        {
            CoreGrid.Padding = Padding;
        }
        else if (StringComparer.Ordinal.Compare(propertyName, nameof(Margin)) == 0)
        {
            CoreGrid.Margin = Margin;
        }
        base.OnPropertyChanged(propertyName);
    }

    #region RowHeight

    /// <summary>
    /// Gets or sets height of each row.
    /// </summary>
    [TypeConverter(typeof(GridLengthTypeConverter))]
    public GridLength RowHeight
    {
        get { return (GridLength)GetValue(RowHeightProperty); }
        set { SetValue(RowHeightProperty, value); }
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for the <see cref="RowHeight"/> property.
    /// </summary>
    public static readonly BindableProperty RowHeightProperty = BindableProperty.Create
    (
        nameof(RowHeight),
        typeof(GridLength),
        typeof(Grid),
        GridLength.Auto,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Grid grid)
            {
                grid.CoreGrid.RowDefinitions = [];
            }
        }
    );

    #endregion RowHeight

    #region ColumnDefinitions

    /// <summary>
    /// Gets or sets the <see cref="ColumnDefinitionCollection"/> for defining columns.
    /// </summary>
    public ColumnDefinitionCollection ColumnDefinitions
    {
        get => GetValue(ColumnDefinitionsProperty) as ColumnDefinitionCollection;
        set => SetValue(ColumnDefinitionsProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for the <see cref="ColumnDefinitions"/> property.
    /// </summary>
    public static readonly BindableProperty ColumnDefinitionsProperty = BindableProperty.Create
    (
        nameof(ColumnDefinitions),
        typeof(ColumnDefinitionCollection),
        typeof(Grid),
        null,
        defaultValueCreator: bindableObject =>
        {
            return new ColumnDefinitionCollection();
        },
        validateValue: (bindableObject, newValue) =>
        {
            return newValue is not null;
        },
        propertyChanged: (bindableObject, newValue, oldValue) =>
        {
            if (bindableObject is Grid grid)
            {
                grid.ColumnsChanged(oldValue as ColumnDefinitionCollection, newValue as ColumnDefinitionCollection);
            }
        }
    );

    void ColumnsChanged(ColumnDefinitionCollection oldCollection, ColumnDefinitionCollection newCollection)
    {
        if (oldCollection is not null)
        {
            oldCollection.CollectionChanged += OnColumnsChanged;
        }
        if (newCollection is not null)
        {
            newCollection.CollectionChanged += OnColumnsChanged;
            UpdateColumns();
        }
    }

    private void OnColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        int index;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (object value in e.NewItems)
                {
                    if (value is ColumnDefinition column)
                    {
                        column.PropertyChanged += OnColumnChanged;
                        CoreGrid.ColumnDefinitions.Add(new(column.Width.ToGridLength()));
                    }
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (object value in e.NewItems)
                {
                    if (value is ColumnDefinition column)
                    {
                        column.PropertyChanged -= OnColumnChanged;
                        index = ColumnDefinitions.IndexOf(column);
                        CoreGrid.ColumnDefinitions.RemoveAt(index);
                    }
                }
                break;

            case NotifyCollectionChangedAction.Replace:
            {
                ColumnDefinition column = (ColumnDefinition)e.OldItems[0];
                column.PropertyChanged -= OnColumnChanged;
                index = e.NewStartingIndex;
                column = (ColumnDefinition)e.NewItems[0];
            }
            break;
            case NotifyCollectionChangedAction.Reset:
                CoreGrid.ColumnDefinitions = [];
                break;
        }
    }

    private void OnColumnChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is ColumnDefinition column)
        {
            int index = ColumnDefinitions.IndexOf(column);
            CoreGrid.ColumnDefinitions[index] = new(column.Width.ToGridLength());
            Populate();
        }
    }

    private void UpdateColumns()
    {
        if
        (
            CoreGrid.ColumnDefinitions is null
            ||
            CoreGrid.ColumnDefinitions.Count != ColumnDefinitions.Count
        )
        {
            Microsoft.Maui.Controls.ColumnDefinitionCollection columns = [];
            foreach (ColumnDefinition definition in ColumnDefinitions)
            {
                Microsoft.Maui.Controls.ColumnDefinition column = new(definition.Width.ToGridLength());
                columns.Add(column);
            }
            CoreGrid.ColumnDefinitions = columns;
        }
    }

    #endregion ColumnDefinitions

    #region ItemsSource

    /// <summary>
    /// Gets or sets the <see cref="IEnumerable"/> to use to populate the grid rows.
    /// </summary>
    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty) as IEnumerable;
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> backing store for <see cref="ItemsSource"/>.
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create
    (
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(Grid),
        null,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Grid grid)
            {
                grid.Populate();
            }
        }
    );

    #endregion ItemsSource

    #region RowSpacing

    /// <summary>
    /// Gets or sets the distance between rows in the Grid.
    /// </summary>
    public double RowSpacing
    {
        get => (double)GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> backing store for <see cref="RowSpacing"/>.
    /// </summary>
    public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create
    (
        nameof(RowSpacing),
        typeof(double),
        typeof(Grid),
        0d,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Grid grid)
            {
                grid.CoreGrid.RowSpacing = (double)newValue;
            }
        }
    );

    #endregion RowSpacing

    #region ColumnSpacing

    /// <summary>
    /// Gets or sets the distance between columns in the Grid.
    /// </summary>
    public double ColumnSpacing
    {
        get => (double)GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }


    /// <summary>
    /// Provides the <see cref="BindableProperty"/> backing store for <see cref="ColumnSpacing"/>.
    /// </summary>
    public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create
    (
        nameof(ColumnSpacing),
        typeof(double),
        typeof(Grid),
        0d,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (bindableObject is Grid grid)
            {
                grid.CoreGrid.ColumnSpacing = (double)newValue;
            }
        }
    );

    #endregion ColumnSpacing

    #region Populate Children

    DataTemplate DefaultChildTemplate
    {
        get
        {
            if (_defaultItemTemplate is null)
            {
                this.Resources.TryGetValue(DefaultItemTemplate, out object template);
                _defaultItemTemplate = (DataTemplate)template;
            }
            return _defaultItemTemplate;
        }
    }

    private void Populate()
    {
        CoreGrid.Clear();
        CoreGrid.RowDefinitions.Clear();
        UpdateColumns();

        if (ItemsSource is not null)
        {
            int row = 0;
            DataTemplate defaultTemplate = DefaultChildTemplate;

            if (ColumnDefinitions is null || ColumnDefinitions.Count == 0)
            {
                foreach (object value in ItemsSource)
                {
                    AddChild(value, defaultTemplate, row, 0);
                    row++;
                }
            }
            else
            {
                foreach (object value in ItemsSource)
                {
                    for (int column = 0; column < ColumnDefinitions.Count; column++)
                    {
                        ColumnDefinition definition = ColumnDefinitions[column];
                        DataTemplate template = definition.ItemTemplate ?? defaultTemplate;
                        AddChild(value, template, row, column);
                    }
                    row++;
                }
            }
        }
    }

    void AddChild(object value, DataTemplate template, int row, int column)
    {
        CoreGrid.RowDefinitions.Add(new(RowHeight));

        View child = template.CreateView(this, value);
        MauiGrid.SetRow(child, row);
        MauiGrid.SetColumn(child, column);
        CoreGrid.Add(child);
    }

    #endregion Populate Children
}