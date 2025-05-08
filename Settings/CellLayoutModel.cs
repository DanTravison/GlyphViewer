namespace GlyphViewer.Settings;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;
using GlyphViewer.Settings.Properties;
using System.ComponentModel;
using System.Diagnostics;

/// <summary>
/// Provides an abstract base class for a cell layout option.
/// </summary>
public abstract class CellLayoutOption : ObservableObject
{
    bool _isSelected;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="model">The containing <see cref="CellLayoutModel"/></param>
    /// <param name="text">The text to display for the option.</param>
    /// <param name="description">The description of the option.</param>
    /// <param name="isSelected">true if the option is selected by default; otherwise, false.</param>
    protected CellLayoutOption(CellLayoutModel model, string text, string description, bool isSelected)
    {
        Text = text;
        Description = description;
        Model = model;
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
    /// Gets the parent <see cref="CellLayoutModel"/>
    /// </summary>
    protected CellLayoutModel Model
    {
        get;
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

    [DebuggerDisplay("{IsSelected,nq}")]
    class CellWidthOption : CellLayoutOption
    {
        public CellWidthOption(CellLayoutModel model, string glyph, string description, CellWidthLayout value, bool isSelected)
            : base(model, glyph, description, isSelected)
        {
            Value = value;
        }

        public CellWidthLayout Value { get; }

        protected override void OnSelectedChanged()
        {
            if (IsSelected)
            {
                Model.Set(Value);
            }
        }
    }

    [DebuggerDisplay("{IsSelected,nq}")]
    class CellHeightOption : CellLayoutOption
    {
        public CellHeightOption(CellLayoutModel model, string glyph, string description, CellHeightLayout value, bool isSelected)
            : base(model,glyph, description, isSelected)
        {
            Value = value;
        }

        public CellHeightLayout Value { get; }

        protected override void OnSelectedChanged()
        {
            if (IsSelected)
            {
                Model.Set(Value);
            }
        }
    }

    #endregion Private Classes

    #region Fields

    readonly CellLayoutProperty _cellLayoutProperty;
    CellLayoutStyle _cellLayout;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="cellLayoutProperty">The <see cref="CellLayoutProperty"/> to edit.</param>
    public CellLayoutModel(CellLayoutProperty cellLayoutProperty)
    {
        _cellLayoutProperty = cellLayoutProperty;
        _cellLayout = cellLayoutProperty.Value;

        // Needed for reset.
        _cellLayoutProperty.PropertyChanged += OnCellLayoutPropertyChanged;

        CellLayoutStyle cellLayout = cellLayoutProperty.Value;
        HeightOptions =
        [
            new CellHeightOption
            (
                this, FluentUI.LineHorizontal3,
                Strings.CellHeightDefaultDescription,
                CellHeightLayout.Default,
                cellLayout.Height == CellHeightLayout.Default
            ),
            new CellHeightOption
            (
                this, FluentUI.LineThickness,
                Strings.CellHeightDynamicDescription,
                CellHeightLayout.Dynamic,
                cellLayout.Height == CellHeightLayout.Dynamic
            )
        ];
        WidthOptions =
        [
            new CellWidthOption
            (
                this, FluentUI.TextColumnThree,
                Strings.CellWidthDefaultDescription,
                CellWidthLayout.Default,
                cellLayout.Width == CellWidthLayout.Default
            ),
            new CellWidthOption
            (
                this, FluentUI.TextAlignLeft,
                Strings.CellWidthRowDescription,
                CellWidthLayout.Width,
                cellLayout.Width == CellWidthLayout.Width
            ),
            new CellWidthOption
            (
                this, FluentUI.TextColumnTwoLeft,
                Strings.CellWidthDynamicDescription,
                CellWidthLayout.Dynamic,
                cellLayout.Width == CellWidthLayout.Dynamic
            )
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
        }
    }

    void Set(CellWidthLayout width)
    {
        if (width != _cellLayoutProperty.Value.Width)
        {
            _cellLayout = new(width, _cellLayout.Height);
            _cellLayoutProperty.Value = _cellLayout;
        }
    }

    private void OnCellLayoutPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ReferenceEquals(e, ObservableObject.ValueChangedEventArgs))
        {
            CellLayoutStyle style = _cellLayoutProperty.Value;
            if (style != _cellLayout)
            {
                _cellLayout = style;
                foreach (CellLayoutOption option in HeightOptions)
                {
                    option.IsSelected = style.Height == ((CellHeightOption)option).Value;
                }
                foreach (CellLayoutOption option in WidthOptions)
                {
                    option.IsSelected = style.Width == ((CellWidthOption)option).Value;
                }
            }
        }
    }

    #endregion Set
}
