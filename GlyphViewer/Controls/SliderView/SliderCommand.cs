namespace GlyphViewer.Controls;

using GlyphViewer.ObjectModel;

/// <summary>
/// Provides a command for the <see cref="SliderView"/> control to handle previous and next actions.
/// </summary>
internal sealed class SliderCommand : Command
{
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="action">The <see cref="Action"/> to perform.</param>
    /// <param name="isNext">true if the command increments the <see cref="Slider.Value"/>; otherwise, 
    /// false if it decrements the <see cref="Slider.Value"/></param>
    public SliderCommand(Action action, bool isNext)
        : base(action)
    {
        IsNext = isNext;
        IsEnabled = false;
    }

    /// <summary>
    /// Gets the value indicating whether this command increments the <see cref="Slider.Value"/>.
    /// </summary>
    /// <value>
    /// true if the command increments the <see cref="Slider.Value"/>; otherwise, 
    /// false if it decrements the <see cref="Slider.Value"/>
    /// </value>
    public bool IsNext
    {
        get;
    }
}
