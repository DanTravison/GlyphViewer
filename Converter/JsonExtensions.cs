namespace GlyphViewer.Converter;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides <see cref="System.Text.Json"/> extension methods.
/// </summary>
public static class JsonExtensions
{
    #region Read

    /// <summary>
    /// Reads the next token
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read.</param>
    /// <param name="expectedToken">The expected <see cref="JsonTokenType"/>.</param>
    /// <remarks>
    /// This method is used to verify and read various tokens
    /// such as <see cref="JsonTokenType.StartArray"/> and <see cref="JsonTokenType.EndObject"/>.
    /// </remarks>
    static public void Read(ref this Utf8JsonReader reader, JsonTokenType expectedToken)
    {
        Verify(reader, expectedToken);
        reader.Read();
    }

    /// <summary>
    /// Reads and returns a property name from the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read.</param>
    /// <returns>The property name read from the reader.</returns>
    /// <exception cref="JsonException">The value of <see cref="Utf8JsonReader.TokenType"/> is not <see cref="JsonTokenType.PropertyName"/>.</exception>
    public static string ReadPropertyName(this ref Utf8JsonReader reader)
    {
        reader.Verify(JsonTokenType.PropertyName);
        string propertyName = reader.GetString();
        reader.Read();
        return propertyName;
    }

    /// <summary>
    /// Reads and returns a property name from the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read.</param>
    /// <returns>The property name read from the reader.</returns>
    /// <exception cref="JsonException">The value of <see cref="Utf8JsonReader.TokenType"/> is not <see cref="JsonTokenType.PropertyName"/>.</exception>
    public static string GetPropertyName(this ref Utf8JsonReader reader)
    {
        reader.Verify(JsonTokenType.PropertyName);
        return reader.GetString();
    }


    #endregion Read

    #region Add

    /// <summary>
    /// Adds one or more <see cref="JsonConverter"/> elements to <see cref="JsonSerializerOptions.Converters"/>.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to update.</param>
    /// <param name="converters">The <see cref="IEnumerable{JsonConverter}"/> of
    /// the converters to add.</param>
    public static void Add
    (
        this JsonSerializerOptions options,
        params JsonConverter[] converters
    )
    {
        foreach (JsonConverter converter in converters)
        {
            options.Converters.Add(converter);
        }
    }

    #endregion Add

    #region Verify

    /// <summary>
    /// Verifies the current token is the expected type.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="expected"></param>
    /// <exception cref="JsonException">The value of <see cref="Utf8JsonReader.TokenType"/> is does not match the <paramref name="expected"/>.</exception>
    public static void Verify(this Utf8JsonReader reader, JsonTokenType expected)
    {
        if (reader.TokenType != expected)
        {
            reader.Unexpected(expected);
        }
    }

    #endregion Verify

    #region Unexpected

    /// <summary>
    /// Throws a <see cref="JsonException"/> for the unexpected <see cref="JsonTokenType"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> being read.</param>
    /// <param name="expected">The expected <see cref="JsonTokenType"/>.</param>
    /// <exception cref="JsonException">The value of <see cref="Utf8JsonReader.TokenType"/> is does not match the <paramref name="expected"/>.</exception>
    public static void Unexpected(this Utf8JsonReader reader, JsonTokenType expected)
    {
        throw new JsonException
        (
            string.Format
            (
                "Unexpected token {0} at {1}. {2} Expected",
                reader.TokenType,
                reader.TokenStartIndex,
                expected
            )
        );
    }

   #endregion Unexpected

    #region InvalidValue

    /// <summary>
    /// Throws a <see cref="JsonException"/> when an invalid value.
    /// </summary>
    /// <param name="tokenType">The <see cref="JsonTokenType"/> being read.</param>
    /// <param name="name">The name of the token.</param>
    /// <param name="value">The property value that was read.</param>
    /// <exception cref="JsonException">The value of property is not valid.</exception>
    public static void InvalidValue(this Utf8JsonReader reader, JsonTokenType tokenType, string name, string value)
    {
        throw new JsonException
        (
            string.Format("Unexpected value for {0} {1} at {2}:{3}.", tokenType, name, reader.TokenStartIndex, value)
        );
    }

    #endregion InvalidValue
}
