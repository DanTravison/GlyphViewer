namespace GlyphViewer.Converter;

using System.ComponentModel;
using System.Globalization;

/// <summary>
/// Provides an abstract base class for a <see cref="TypeConverter"/>.
/// </summary>
/// <typeparam name="T">The target type to convert to (the type of this converter).</typeparam>
/// <typeparam name="S">The source type to convert from.</typeparam>
public abstract class TypeConverter<T, S> : TypeConverter
{
    /// <summary>
    /// Returns whether this converter can convert an object of the given <paramref name="sourceType"/> to the
    /// type of this converter, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType"> A System.Type that represents the type you want to convert from.</param>
    /// <returns>true if <paramref name="sourceType"/> is of type <typeparamref name="S"/>; otherwise, false.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(S);
    }

    /// <summary>
    /// Returns whether this converter can convert the object to the specified type.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="destinationType">A Type that represents the type to convert to.</param>
    /// <returns>true if the <paramref name="destinationType"/> is of type <typeparamref name="S"/>; otherwise, false.</returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(S);
    }

    /// <summary>
    /// Converts the given value to the type of this converter.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">A <typeparamref name="S"/> to convert from.</param>
    /// <returns>A <typeparamref name="T"/> value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is a null reference
    /// -or-
    /// <paramref name="culture"/> is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not a value of type <typeparamref name="S"/>.</exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        culture ??= CultureInfo.CurrentCulture;

        if (value is S source)
        {
            return ConvertFrom(culture, source);
        }

        throw new ArgumentException
        (
            $"[{value.GetType().FullName}]'{value}' must be a {typeof(S).FullName}.",
            nameof(value)
        );
    }

    /// <summary>
    /// Converts the given value object to the specified type.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The <typeparamref name="T"/> value to convert.</param>
    /// <param name="destinationType">The Type to convert the value parameter to.</param>
    /// <returns>An <see cref="int"/> value for the boolean value where 
    /// true returns 1 and false returns 0.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is a null reference
    /// -or-
    /// <paramref name="culture"/> is a null reference.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="destinationType"/> is not a <typeparamref name="S"/>
    /// -or-
    /// <paramref name="value"/> is not of type <typeparamref name="T"/>.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(culture);
        if (destinationType != typeof(S))
        {
            throw new ArgumentException($"{destinationType.FullName} must be a {typeof(S).FullName}", nameof(destinationType));
        }

        if (value is T target)
        {
            return ConvertTo(culture, target);
        }
        throw new ArgumentException($"Cannot convert a [{value.GetType().FullName}]'{value}' to a {typeof(T).FullName}", nameof(value));
    }

    /// <summary>
    /// Overridden in the derived class to convert from the <typeparamref name="S"/> type
    /// to the <typeparamref name="T"/> type; the type of this converter.
    /// </summary>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <param name="value">The value to convert from.</param>
    /// <returns>An object of type <typeparamref name="T"/>.</returns>
    protected abstract T ConvertFrom(CultureInfo culture, S value);

    /// <summary>
    /// Overridden in the derived class to convert from the <typeparamref name="T"/> type, 
    /// the type of this converter, to the <typeparamref name="S"/> type;
    /// </summary>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <param name="value">The value to convert.</param>
    /// <returns>An object of type <typeparamref name="S"/>.</returns>
    protected abstract S ConvertTo(CultureInfo culture, T value);
}
