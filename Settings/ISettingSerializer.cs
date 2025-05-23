﻿namespace GlyphViewer.Settings;

using System.Text.Json;

/// <summary>
/// Provides an interface for serializing an <see cref="ISetting"/> to/from JSON.
/// </summary>
public interface ISettingSerializer
{
    /// <summary>
    /// Gets the value indicating if the setting has the default value.
    /// </summary>
    /// <value>
    /// True if this instance is in the default state; otherwise, false.
    /// </value>
    bool IsDefault
    {
        get;
    }

    /// <summary>
    /// Resets the setting to its default state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Reads the setting from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    void Read(ref Utf8JsonReader reader, JsonSerializerOptions options);

    /// <summary>
    /// Writes the setting to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    void Write(Utf8JsonWriter writer, JsonSerializerOptions options);
}
