using System.ComponentModel;

namespace GlyphViewer.Settings.Properties;

/// <summary>
/// Provides an interface for an <see cref="ISetting"/> container.
/// </summary>
public interface ISettingPropertyCollection : ISettingSerializer, INotifyPropertyChanged, IReadOnlyCollection<ISettingProperty>
{
    /// <summary>
    /// Gets the <see cref="ISettingProperty"/> with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="ISettingProperty"/> to get.</param>
    /// <returns>
    /// The <see cref="ISettingProperty"/> with the specified <paramref name="name"/>;
    /// otherwise, a null reference.
    /// </returns>    
    ISettingProperty this[string name]
    {
        get;
    }

    /// <summary>
    /// Gets or sets the value indicating if the collection needs to be serialized.
    /// </summary>
    /// <value>true if the collection needs to be serialized; otherwise, false.</value>
    bool HasChanges
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the <see cref="IReadOnlyList{ISettingProperty}"/> of instances where <see cref="ISettingProperty.CanEdit"/> is true.
    /// </summary>
    IReadOnlyList<ISettingProperty> Properties
    {
        get;
    }
}
