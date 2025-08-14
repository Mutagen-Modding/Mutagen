using Mutagen.Bethesda.Plugins;
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
}

partial class MagicEffectBinaryCreateTranslation
{
    public static partial void FillBinaryAssociatedItemCustom(MutagenFrame frame, IMagicEffectInternal item)
    {
        // Skip for now.  Will be parsed by Archetype.
        frame.Position += 4;
    }

    public static AMagicEffectArchetype ReadArchetype(MutagenFrame frame)
    {
        // Jump back and read in association FormKey
        var curPos = frame.Position;
        frame.Position -= 56;
        FormKey associatedItemKey = FormKeyBinaryTranslation.Instance.Parse(frame);
        frame.Position = curPos;

        // Finish reading archetype
        MagicEffectArchetype.TypeEnum archetypeEnum = (MagicEffectArchetype.TypeEnum)frame.ReadInt32();
        AMagicEffectArchetype archetype;
        switch (archetypeEnum)
        {
            case MagicEffectArchetype.TypeEnum.SummonCreature:
                archetype = new MagicEffectSummonCreatureArchetype()
                {
                    Association = associatedItemKey.ToLink<ICreatureGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.BoundItem:
                archetype = new MagicEffectBoundItemArchetype()
                {
                    Association = associatedItemKey.ToLink<IBoundItemGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.Script:
                archetype = new MagicEffectScriptArchetype()
                {
                    Association = associatedItemKey.ToLink<IScriptGetter>()
                };
                break;
            default:
                archetype = new MagicEffectArchetype(archetypeEnum)
                {
                    ActorValue = ActorValue.None,
                    Association = associatedItemKey.ToLink<IFallout3MajorRecordGetter>()
                };
                break;
        }
        archetype.ActorValue = (ActorValue)frame.ReadInt32();
        return archetype;
    }

    public static partial void FillBinaryArchetypeCustom(MutagenFrame frame, IMagicEffectInternal item)
    {
        item.Archetype = ReadArchetype(frame);
    }
}

partial class MagicEffectBinaryWriteTranslation
{
    public static partial void WriteBinaryArchetypeCustom(MutagenWriter writer, IMagicEffectGetter item)
    {
        writer.Write((int)item.Archetype.Type);
        writer.Write((int)item.Archetype.ActorValue);
    }

    public static partial void WriteBinaryAssociatedItemCustom(MutagenWriter writer, IMagicEffectGetter item)
    {
        FormKeyBinaryTranslation.Instance.Write(writer, item.Archetype.AssociationKey);
    }
}

partial class MagicEffectBinaryOverlay
{
    public partial IAMagicEffectArchetypeGetter GetArchetypeCustom()
    {
        if (!_DATALocation.HasValue) return new MagicEffectArchetype();
        var frame = new MutagenFrame(new MutagenMemoryReadStream(_recordData, _package.MetaData))
        {
            Position = _ArchetypeLocation
        };
        return MagicEffectBinaryCreateTranslation.ReadArchetype(frame);
    }
}