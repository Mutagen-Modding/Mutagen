using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout3;

public partial class MagicEffect
{
    public enum MagicFlag
    {
        Hostile = 0x00000001,
        Recover = 0x00000002,
        Detrimental = 0x00000004,
        Self = 0x00000010,
        Touch = 0x00000020,
        Target = 0x00000040,
        NoDuration = 0x00000080,
        NoMagnitude = 0x00000100,
        NoArea = 0x00000200,
        FXPersist = 0x00000400,
        GoryVisuals = 0x00001000,
        DisplayNameOnly = 0x00002000,
        RadioBroadcast = 0x0008000,
        UseSkill = 0x00080000,
        UseAttribute = 0x00100000,
        Painless = 0x01000000,
        SprayProjectileOrFog = 0x02000000,
        BoltProjectile = 0x04000000,
        NoHitEffect = 0x08000000,
        NoDeathDispel = 0x10000000,
    }

    public enum ArchetypeEnum
    {
        ValueModifier = 0,
        Script = 1,
        Dispel = 2,
        CureDisease = 3,
        Invisibility = 11,
        Chameleon = 12,
        Light = 13,
        Lock = 16,
        Open = 17,
        BoundItem = 18,
        SummonCreature = 19,
        Paralysis = 24,
        CureParalysis = 30,
        CureAddiction = 31,
        CurePoison = 32,
        Concussion = 33,
        ValueAndParts = 34,
        LimbCondition = 35,
        Turbo = 36,
    }
}

partial class MagicEffectDataBinaryWriteTranslation
{
    public static partial void WriteBinaryAssociatedItemCustom(
        MutagenWriter writer,
        IMagicEffectDataGetter item)
    {
         FormKeyBinaryTranslation.Instance.Write(writer, item.Archetype.AssociationKey);
    }
}