namespace GlyphViewer.Text.OpenType;

using System.Runtime.InteropServices;
using static GlyphViewer.Text.OpenType.Os2Table;

public enum PanoseFamilyKind : byte
{
    Any = 0,
    NoFit = 1,

    LatinText = 2,
    LatinHandWritten = 3,
    Script = 3,
    LatinDecorative = 4,
    LatinSymbol = 5,

    // Values 6–15 are reserved
}

public enum PanoseSerifStyle : byte
{
    Any = 0,
    NoFit = 1,

    Cove = 2,
    ObtuseCove = 3,
    SquareCove = 4,
    ObtuseSquareCove = 5,
    Square = 6,
    Thin = 7,
    Bone = 8,
    Exaggerated = 9,
    Triangle = 10,

    NormalSans = 11,
    ObtuseSans = 12,
    PerpendicularSans = 13,
    Flared = 14,
    Rounded = 15,

    // 16–255 reserved
}

public enum PanoseWeight : byte
{
    Any = 0,
    NoFit = 1,

    VeryLight = 2,
    Light = 3,
    Thin = 4,
    Book = 5,
    Medium = 6,
    Demi = 7,
    Bold = 8,
    Heavy = 9,
    Black = 10,
    ExtraBlack = 11,

    // 12–255 reserved
}

public enum PanoseProportion : byte
{
    Any = 0,
    NoFit = 1,

    OldStyle = 2,
    Modern = 3,
    EvenWidth = 4,
    Expanded = 5,
    Condensed = 6,
    VeryExpanded = 7,
    VeryCondensed = 8,
    Monospaced = 9,

    // 10–255 reserved
}

public enum PanoseContrast : byte
{
    Any = 0,
    NoFit = 1,

    None = 2,
    VeryLow = 3,
    Low = 4,
    MediumLow = 5,
    Medium = 6,
    MediumHigh = 7,
    High = 8,
    VeryHigh = 9,

    // 10–255 reserved
}

public enum PanoseStrokeVariation : byte
{
    Any = 0,
    NoFit = 1,

    NoVariation = 2,
    GradualDiagonal = 3,
    GradualTransitional = 4,
    GradualVertical = 5,
    GradualHorizontal = 6,
    RapidVertical = 7,
    RapidHorizontal = 8,
    InstantVertical = 9,
    InstantHorizontal = 10,

    // 11–255 reserved
}

public enum PanoseArmStyle : byte
{
    Any = 0,
    NoFit = 1,

    StraightArmsHorz = 2,
    StraightArmsWedge = 3,
    StraightArmsVert = 4,
    StraightArmsSingleSerif = 5,
    StraightArmsDoubleSerif = 6,

    NonStraightArmsHorz = 7,
    NonStraightArmsWedge = 8,
    NonStraightArmsVert = 9,
    NonStraightArmsSingleSerif = 10,
    NonStraightArmsDoubleSerif = 11,

    // 12–255 reserved
}

public enum PanoseLetterform : byte
{
    Any = 0,
    NoFit = 1,

    NormalContact = 2,
    NormalWeighted = 3,
    NormalBoxed = 4,
    NormalFlattened = 5,

    NormalRounded = 6,
    NormalOffCenter = 7,
    NormalSquare = 8,

    ObliqueContact = 9,
    ObliqueWeighted = 10,
    ObliqueBoxed = 11,
    ObliqueFlattened = 12,

    ObliqueRounded = 13,
    ObliqueOffCenter = 14,
    ObliqueSquare = 15,

    // 16–255 reserved
}

public enum PanoseMidline : byte
{
    Any = 0,
    NoFit = 1,

    StandardTrimmed = 2,
    StandardPointed = 3,
    StandardSerifed = 4,
    HighTrimmed = 5,
    HighPointed = 6,
    HighSerifed = 7,
    ConstantTrimmed = 8,
    ConstantPointed = 9,
    ConstantSerifed = 10,
    LowTrimmed = 11,
    LowPointed = 12,
    LowSerifed = 13,

    // 14–255 reserved
}

public enum PanoseXHeight : byte
{
    Any = 0,
    NoFit = 1,

    ConstantSmall = 2,
    ConstantStandard = 3,
    ConstantLarge = 4,
    DuckingSmall = 5,
    DuckingStandard = 6,
    DuckingLarge = 7,

    // 8–255 reserved
}

public static class PanoseExtensions
{
    public static bool IsSerif(this PanoseSerifStyle style)
    {
        return style >= PanoseSerifStyle.Cove &&
               style <= PanoseSerifStyle.Triangle;
    }

    public static bool IsSansSerif(this PanoseSerifStyle style)
    {
        return style >= PanoseSerifStyle.NormalSans &&
               style <= PanoseSerifStyle.Rounded;
    }
}

/// <summary>
/// Defines the <see cref="Os2Table.Panose"/> proprety.
/// </summary>
public sealed class Panose
{
    internal Panose(PanoseRaw raw)
    {
        FamilyKind = (PanoseFamilyKind)raw.FamilyType;
        SerifStyle = (PanoseSerifStyle)raw.SerifStyle;
        Weight = (PanoseWeight)raw.Weight;
        Proportion = (PanoseProportion)raw.Proportion;

        IsMonospaced = Proportion == PanoseProportion.Monospaced;
        IsSerif = SerifStyle.IsSerif();
        IsSansSerif = SerifStyle.IsSansSerif();
        IsBold = Weight >= PanoseWeight.Bold;
        IsScript = FamilyKind == PanoseFamilyKind.Script;
    }

    public PanoseFamilyKind FamilyKind { get; }
    public PanoseSerifStyle SerifStyle { get; }
    public PanoseWeight Weight { get; }
    public PanoseProportion Proportion { get; }

    public bool IsMonospaced { get; }
    public bool IsSerif { get; }
    public bool IsSansSerif { get; }
    public bool IsBold { get; }
    public bool IsScript { get; }
}
