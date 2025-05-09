namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using GlyphViewer.Settings.Properties;
using System.ComponentModel;

/// <summary>
/// Provides an abstract base class for a cell layout option.
/// </summary>
public abstract class CellLayoutOption : ObservableObject
{
    bool _isSelected;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="text">The text to display for the option.</param>
    /// <param name="description">The description of the option.</param>
    /// <param name="isSelected">true if the option is selected by default; otherwise, false.</param>
    protected CellLayoutOption(string text, string description, bool isSelected)
    {
        Text = text;
        Description = description;
        _isSelected = isSelected;
    }

    /// <summary>
    /// Gets the text to dispaly for the option.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the description of the option.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets or sets the value indicating if this option is selected.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnSelectedChanged();
                OnPropertyChanged(SelectedChangedEventArgs);
            }
        }
    }

    /// <summary>
    /// Called when IsSelected changes.
    /// </summary>
    protected abstract void OnSelectedChanged();

    static readonly PropertyChangedEventArgs SelectedChangedEventArgs = new(nameof(IsSelected));
}

/// <summary>
/// Provides a view model for editing <see cref="CellLayoutStyle"/>.
/// </summary>
public sealed class CellLayoutModel
{
    #region Private Classes

    class CellLayoutOption<T> : CellLayoutOption
    {
        public delegate void SetValueDelegate(T value);
        readonly SetValueDelegate _setValue;

        public CellLayoutOption(SetValueDelegate setValue, string glyph, string description, T value, bool isSelected)
            : base(glyph, description, isSelected)
        {
            Value = value;
            _setValue = setValue;
        }

        public T Value { get; }

        protected override void OnSelectedChanged()
        {
            if (IsSelected)
            {
                _setValue(Value);
            }
        }
    }

    #endregion Private Classes

    #region Fields

    readonly CellLayoutProperty _cellLayoutProperty;
    CellLayoutStyle _cellLayout;
    readonly List<CellLayoutOption<CellHeightLayout>> _heightOptions;
    readonly List<CellLayoutOption<CellWidthLayout>> _widthOptions;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="setting">The <see cref="GlyphSetting"/> containing the <see cref="CellLayoutOption"/>  to edit.</param>
    public CellLayoutModel(GlyphSetting setting)
    {
        _cellLayoutProperty = setting.CellLayout;

        _cellLayout = _cellLayoutProperty.Value;

        // Handle changes on the setting property.
        _cellLayoutProperty.PropertyChanged += OnCellLayoutPropertyChanged;

        HeightOptions = _heightOptions = 
        [
            new CellLayoutOption<CellHeightLayout>
            (
                Set,
                FluentUI.LineHorizontal3,
                Strings.CellHeightDefaultDescription,
                CellHeightLayout.Default,
                _cellLayout.Height == CellHeightLayout.Default
            ),
            new CellLayoutOption<CellHeightLayout>
            (
                Set, 
                FluentUI.LineThickness,
                Strings.CellHeightDynamicDescription,
                CellHeightLayout.Dynamic,
                _cellLayout.Height == CellHeightLayout.Dynamic
            )
        ];
        WidthOptions = _widthOptions =
        [
            new CellLayoutOption<CellWidthLayout>
            (
                Set, 
                FluentUI.TextColumnThree,
                Strings.CellWidthDefaultDescription,
                CellWidthLayout.Default,
                _cellLayout.Width == CellWidthLayout.Default
            ),
            new CellLayoutOption<CellWidthLayout>
            (
                Set, 
                FluentUI.TextAlignLeft,
                Strings.CellWidthRowDescription,
                CellWidthLayout.Width,
                _cellLayout.Width == CellWidthLayout.Width
            )
            /*
            new CellLayoutOption<CellWidthLayout>
            (
                Set, 
                FluentUI.TextColumnTwoLeft,
                Strings.CellWidthDynamicDescription,
                CellWidthLayout.Dynamic,
                _cellLayout.Width == CellWidthLayout.Dynamic
            )
            */
        ];
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="IReadOnlyList{CellLayoutOption}"/> of height options.
    /// </summary>
    public IReadOnlyList<CellLayoutOption> HeightOptions
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="IReadOnlyList{CellLayoutOption}"/> of width options.
    /// </summary>
    public IReadOnlyList<CellLayoutOption> WidthOptions
    {
        get;
    }

    #endregion Properties

    #region Set

    void Set(CellHeightLayout height)
    {
        if (height != _cellLayoutProperty.Value.Height)
        {
            _cellLayout = new(_cellLayout.Width, height);
            _cellLayoutProperty.Value = _cellLayout;
            foreach (CellLayoutOption<CellHeightLayout> option in _heightOptions)
            {
                option.IsSelected = height == option.Value;
            }
        }
    }

    void Set(CellWidthLayout width)
    {
        if (width != _cellLayoutProperty.Value.Width)
        {
            _cellLayout = new(width, _cellLayout.Height);
            _cellLayoutProperty.Value = _cellLayout;
            foreach (CellLayoutOption<CellWidthLayout> option in _widthOptions)
            {
                option.IsSelected = width == option.Value;
            }
        }
    }

    private void OnCellLayoutPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ReferenceEquals(e, ObservableObject.ValueChangedEventArgs))
        {
            CellLayoutStyle cellLayout = _cellLayoutProperty.Value;
            if (cellLayout != _cellLayout)
            {
                _cellLayout = cellLayout;
                foreach (CellLayoutOption<CellHeightLayout> option in _heightOptions)
                {
                    option.IsSelected = cellLayout.Height == option.Value;
                }
                foreach (CellLayoutOption<CellWidthLayout> option in _widthOptions)
                {
                    option.IsSelected = cellLayout.Width == option.Value;
                }
            }
        }
    }

    #endregion Set
}
