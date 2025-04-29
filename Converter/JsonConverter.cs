namespace GlyphViewer.Converter;

using System.Text.Json;

/// <summary>
/// Provides a base <see cref="System.Text.Json.Serialization.JsonConverter{T}"/> with read and verify methods.
/// </summary>
/// <typeparam name="T">The type of object to convert.</typeparam>
public abstract class JsonConverter<T> : System.Text.Json.Serialization.JsonConverter<T>
{
    #region Read

    /// <summary>
    /// Reads and converts the JSON to a <typeparamref name="T"/> instance.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read.</param>
    /// <param name="typeToConvert">The type to convert.
    /// <para>
    /// The implementation expects the type <typeparamref name="T"/>.
    /// </para>.
    /// </param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to deserialize.</param>
    /// <returns>An instance of a <paramref name="typeToConvert"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="reader"/> is using unsupported options.</exception>
    /// <exception cref="ArgumentException"><paramref name="typeToConvert"/> must be of type <typeparamref name="T"/>.</exception>
    public override sealed T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (typeToConvert != typeof(T) && !typeToConvert.IsSubclassOf(typeof(T)))
        {
            throw new ArgumentException(nameof(typeToConvert));
        }
        return OnRead(ref reader, options);
    }

    /// <summary>
    /// Implemented in the derived class to read the <typeparamref name="T"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read.</param>
    /// <returns>An instance of a <typeparamref name="T"/>.</returns>
    protected abstract T OnRead(ref Utf8JsonReader reader, JsonSerializerOptions options);

    #endregion Read

    #region Write

    /// <summary>
    /// Writes a specified value as JSON.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The <typeparamref name="T"/> value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> or <paramref name="value"/> is a null reference.</exception>
    public override sealed void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);

        OnWrite(writer, value, options);
    }

    /// <summary>
    /// Implemented in the derived class to write the <paramref name="value"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The <typeparamref name="T"/> value to write.</param>
    protected abstract void OnWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options);

    #endregion Write
}
