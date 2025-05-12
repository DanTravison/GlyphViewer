namespace GlyphViewer.Controls;

using System.Collections;
using System.Windows.Input;

/// <summary>
/// Provides a <see cref="ContentView"/> for displaying a list of items as a jump list.
/// </summary>
public partial class JumpList : ContentView
{
    readonly ObjectModel.Command _openCommand;
    CollectionView _items;

    public JumpList()
    {
        InitializeComponent();
        OpenCommand = _openCommand = new ObjectModel.Command(OnOpen);
    }

    protected override void OnApplyTemplate()
    {
        _items = GetTemplateChild("Items") as CollectionView;
        base.OnApplyTemplate();
    }

    #region ItemsSource

    /// <summary>
    /// Gets or sets the <see cref="IEnumerable"/> of items to display.
    /// </summary>
    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty) as IEnumerable;
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="ItemsSource"/> property.
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create
    (
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(JumpList),
        null
    );

    #endregion ItemsSource

    #region ItemTemplate

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the items to display.
    /// </summary>
    public DataTemplate ItemTemplate
    {
        get => GetValue(ItemTemplateProperty) as DataTemplate;
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="ItemTemplate"/> property.
    /// </summary>
    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create
    (
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(JumpList),
        null
    );

    #endregion ItemTemplate

    #region SelectedItem

    /// <summary>
    /// Gets or sets the selected state of the control.
    /// </summary>
    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="SelectedItem"/>.
    /// </summary>
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create
    (
        nameof(SelectedItem),
        typeof(object),
        typeof(JumpList),
        null,
        BindingMode.OneWayToSource
    );

    #endregion SelectedItem

    #region IsOpen

    /// <summary>
    /// Gets or sets the active state of the picker.
    /// </summary>
    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Provides a bindable property for the <see cref="IsOpen"/> property.
    /// </summary>
    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create
    (
        nameof(IsOpen),
        typeof(bool),
        typeof(JumpList),
        false,
        BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is JumpList picker)
            {
                picker.OnIsOpenChanged();
            }
        }
    );

    bool _isOpening;
    void OnIsOpenChanged()
    {
        if (IsOpen)
        {
            _isOpening = true;
            _items.SelectedItem = null;
            _isOpening = false;
        }
        ZIndex = IsOpen ? 1 : -1;
        IsVisible = IsOpen;
        _openCommand.IsEnabled = !IsOpen;
    }

    #endregion IsOpen

    #region OpenCommand

    /// <summary>
    /// Gets or sets the active state of the picker.
    /// </summary>
    public ICommand OpenCommand
    {
        get => GetValue(OpenCommandProperty) as ICommand;
        set => SetValue(OpenCommandProperty, value);
    }

    /// <summary>
    /// Provides a bindable property for the <see cref="OpenCommand"/> property.
    /// </summary>
    public static readonly BindableProperty OpenCommandProperty = BindableProperty.Create
    (
        nameof(OpenCommand),
        typeof(ICommand),
        typeof(JumpList),
        null,
        BindingMode.OneWayToSource
    );

    #endregion OpenCommand

    #region Open/Close methods

    void OnClose(object sender, EventArgs e)
    {
        if (!_isOpening)
        {
            IsOpen = false;
        }
    }

    void OnOpen()
    {
        IsOpen = true;
    }

    #endregion Open/Close methods
}