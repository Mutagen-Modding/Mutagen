using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

public partial class HeadPart
{
    [Flags]
    public enum Flag
    {
        Playable = 0x01,
        Male = 0x02,
        Female = 0x04,
        IsExtraPart = 0x08,
        UseSolidTint = 0x10,
        UsesBodyTexture = 0x40
    }

    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0004,
    }

    public enum TypeEnum
    {
        Misc,
        Face,
        Eyes,
        Hair,
        FacialHair,
        Scars,
        Eyebrows,
        Meatcaps,
        Teeth,
        HeadRear
    }
}

partial class HeadPartBinaryOverlay
{
    public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = Array.Empty<IConditionGetter>();

    partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
    }
}