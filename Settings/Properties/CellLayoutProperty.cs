﻿namespace GlyphViewer.Settings.Properties;

using GlyphViewer.Resources;
using GlyphViewer.Settings;
using System.Text.Json;

/// <summary>
/// Provides a <see cref="SettingProperty{CellLayoutStyle}"/>.
/// </summary>
public sealed class CellLayoutProperty : SettingProperty<CellLayoutStyle>
{
    readonly CellLayoutModel _model;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    public CellLayoutProperty()
        : base(nameof(GlyphSetting.CellLayout), GlyphSetting.DefaultCellLayout, Strings.CellLayoutStyleName, Strings.CellLayoutDescription)
    {
        _model = new CellLayoutModel(this);
    }

    /// <summary>
    /// Gets the <see cref="CellLayoutModel.HeightOptions"/>.
    /// </summary>
    public IReadOnlyList<CellLayoutOption> HeightOptions
    {
        get => _model.HeightOptions;
    }

    /// <summary>
    /// Gets the <see cref="CellLayoutModel.WidthOptions"/>.
    /// </summary>
    public IReadOnlyList<CellLayoutOption> WidthOptions
    {
        get => _model.WidthOptions;
    }

    #region Overrides

    /// <summary>
    /// Reads the value from the <paramref name="reader"/>
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> positioned at the value.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to read the serialized value.</param>
    /// <returns>The <see cref="CellLayoutStyle"/> value.</returns>
    protected override CellLayoutStyle ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        string value = reader.GetString();
        // NOTE: TryParse sets cellLayout to CellLayoutStyle.Default on failure.
        CellLayoutStyle.TryParse(value, out CellLayoutStyle cellLayout);
        return cellLayout;
    }

    /// <summary>
    /// Writes the <paramref name="value"/> to the <paramref name="writer"/>.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write.</param>
    /// <param name="value">The <see cref="CellLayoutStyle"/> value to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use to serialize the value.</param>
    protected override void WriteValue(Utf8JsonWriter writer, CellLayoutStyle value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    #endregion Overrides
}
