namespace GlyphViewer.Controls;

using GlyphViewer.ObjectModel;
using GlyphViewer.Resources;

public partial class SliderView : ContentView
{
    public SliderView()
	{
        NextCommand = new(OnNext)
        {
            IsEnabled = false
        };
        PreviousCommand = new(OnPrevious)
        {
           IsEnabled = false
        };
        InitializeComponent();

        Slider.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Slider.Value) ||
                e.PropertyName == nameof(Slider.Minimum) ||
                e.PropertyName == nameof(Slider.Maximum))
            {
                UpdateButtons();
            }
        };
    }

    #region Properties

    public SliderView Context
    {
        get => this;
    }

    /// <summary>
    /// Gets the command to decrease the <see cref="Slider.Value"/>.
    /// </summary>
    public Command PreviousCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command to increase the <see cref="Slider.Value"/>.
    /// </summary>
    public Command NextCommand
    {
        get;
    }

    // TODO: Determine if there is a better way to reflect the Slider properties
    // instead of duplicating them.

    #region Value

    /// <summary>
    /// Gets or sets value.
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Value"/>.
    /// </summary>
    public static readonly BindableProperty ValueProperty = BindableProperty.Create
    (
        nameof(Value),
        typeof(double),
        typeof(SliderView),
        0.0,
        BindingMode.TwoWay
    );

    #endregion Value

    #region Interval

    /// <summary>
    /// Gets or sets the value to use to increase or decrease the <see cref="Value"/>
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    /// <remarks>
    /// When this property is greater than zero, the <see cref="Value"/> is rounded
    /// to the nearest interval.
    /// </remarks>
    public double Interval
    {
        get => (double)GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Interval"/>.
    /// </summary>
    public static readonly BindableProperty IntervalProperty = BindableProperty.Create
    (
        nameof(Interval),
        typeof(double),
        typeof(SliderView),
        1.0
    );

    #endregion Interval

    #region Minimum

    /// <summary>
    /// Gets or sets the minimum <see cref="Value"/>.
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Minimum"/>.
    /// </summary>
    public static readonly BindableProperty MinimumProperty = BindableProperty.Create
    (
        nameof(Minimum),
        typeof(double),
        typeof(SliderView),
        0.0
    );

    #endregion Minimum

    #region Maximum

    /// <summary>
    /// Gets or sets the maximum <see cref="Value"/>.
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Maximum"/>.
    /// </summary>
    public static readonly BindableProperty MaximumProperty = BindableProperty.Create
    (
        nameof(Maximum),
        typeof(double),
        typeof(SliderView),
        double.MaxValue
    );
    #endregion Maximum

    #region ThumbStyle

    /// <summary>
    /// Gets or sets the maximum <see cref="Value"/>.
    /// </summary>
    /// <value>
    /// A double value in the range of 0 to 100f.
    /// </value>
    public ThumbStyle ThumbStyle
    {
        get => (ThumbStyle)GetValue(ThumbStyleProperty);
        set => SetValue(ThumbStyleProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="ThumbStyle"/>.
    /// </summary>
    public static readonly BindableProperty ThumbStyleProperty = BindableProperty.Create
    (
        nameof(ThumbStyle),
        typeof(ThumbStyle),
        typeof(SliderView),
        ThumbStyle.Circle
     );

    #endregion ThumbStyle

    #region Orientation

    /// <summary>
    /// Gets or sets the <see cref="StackOrientation"/> of the thumb.
    /// </summary>
    public StackOrientation Orientation
    {
        get => (StackOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Provides the <see cref="BindableProperty"/> for <see cref="Orientation"/>.
    /// </summary>
    public static readonly BindableProperty OrientationProperty = BindableProperty.Create
    (
        nameof(Orientation),
        typeof(StackOrientation),
        typeof(SliderView),
        Slider.DefaultOrientation
    );

    #endregion Orientation   

    #endregion Properties

    #region Methods

    /// <summary>
    /// Advances the slider's value by its interval, up to the maximum value.
    /// </summary>
    void OnNext()
    {
        SetSliderValue(Slider.Interval);
    }

    /// <summary>
    /// Decreases the slider's value by its interval, ensuring the value does not go below the minimum.
    /// </summary>
    void OnPrevious()
    {
        SetSliderValue(-Slider.Interval);
    }

    void SetSliderValue(double interval)
    {
        double value = Math.Clamp(Slider.Value + interval, Slider.Minimum, Slider.Maximum);
        if (value != Slider.Value)
        {
            Slider.Value = value;
        }
    }

    void UpdateButtons()
    {
        PreviousCommand.IsEnabled = Value > Minimum;
        NextCommand.IsEnabled = Value < Maximum;
        if (Orientation == StackOrientation.Horizontal)
        {
            NextButton.Text = FluentUI.CaretRightFilled;
            PreviousButton.Text = FluentUI.CaretLeftFilled;
        }
        else
        {
            NextButton.Text = FluentUI.CaretDownFilled;
            PreviousButton.Text = FluentUI.CaretUpFilled;
        }
    }

    #endregion Methods
}