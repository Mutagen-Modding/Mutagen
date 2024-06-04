using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Starfield;

public partial class MagicEffect
{
    [Flags]
    public enum Flag : ulong
    {
        Hostile = 0x0000_0001,
        Recover = 0x0000_0002,
        Detrimental = 0x0000_0004,
        SnapToNavmesh = 0x0000_0008,
        NoHitEvent = 0x0000_0010,
        DispelWithKeywords = 0x0000_0100,
        NoDuration = 0x0000_0200,
        NoMagnitude = 0x0000_0400,
        NoArea = 0x0000_0800,
        FXPersist = 0x0000_1000,
        GoryVisuals = 0x0000_4000,
        HideInUI = 0x0000_8000,
        NoRecast = 0x0002_0000,
        PowerAffectsMagnitude = 0x0020_0000,
        PowerAffectsDuration = 0x0040_0000,
        Painless = 0x0400_0000,
        NoHitEffect = 0x0800_0000,
        NoDeathDispel = 0x1000_0000,
    }

    public enum SoundType
    {
        Charge = 1,
        Release = 3,
        OnHit = 5,
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
        frame.Position -= 80;
        FormKey associatedItemKey = FormKeyBinaryTranslation.Instance.Parse(frame);
        frame.Position = curPos;

        // Finish reading archetype
        MagicEffectArchetype.TypeEnum archetypeEnum = (MagicEffectArchetype.TypeEnum)frame.ReadInt32();
        AMagicEffectArchetype archetype;
        switch (archetypeEnum)
        {
            case MagicEffectArchetype.TypeEnum.Light:
                archetype = new MagicEffectLightArchetype()
                {
                    Association = associatedItemKey.ToLink<ILightGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.Bound:
                archetype = new MagicEffectBoundArchetype()
                {
                    Association = associatedItemKey.ToLink<IBindableEquipmentGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.SummonCreature:
                archetype = new MagicEffectSummonCreatureArchetype()
                {
                    Association = associatedItemKey.ToLink<INpcGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.Guide:
                archetype = new MagicEffectGuideArchetype()
                {
                    Association = associatedItemKey.ToLink<IHazardGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.SpawnHazard:
                archetype = new MagicEffectSpawnHazardArchetype()
                {
                    Association = associatedItemKey.ToLink<IHazardGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.PeakValueModifier:
                archetype = new MagicEffectPeakValueModArchetype()
                {
                    Association = associatedItemKey.ToLink<IKeywordGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.Cloak:
                archetype = new MagicEffectCloakArchetype()
                {
                    Association = associatedItemKey.ToLink<ISpellGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.EnhanceWeapon:
                archetype = new MagicEffectEnhanceWeaponArchetype()
                {
                    Association = associatedItemKey.ToLink<IObjectEffectGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.Calm:
            case MagicEffectArchetype.TypeEnum.Frenzy:
                archetype = new MagicEffectArchetype(archetypeEnum)
                {
                    Association = associatedItemKey.ToLink<IStarfieldMajorRecordGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.Invisibility:
                archetype = new MagicEffectArchetype(archetypeEnum)
                {
                    Association = associatedItemKey.ToLink<IStarfieldMajorRecordGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.Paralysis:
                archetype = new MagicEffectArchetype(archetypeEnum)
                {
                    Association = associatedItemKey.ToLink<IStarfieldMajorRecordGetter>()
                };
                break;
            case MagicEffectArchetype.TypeEnum.Demoralize:
            case MagicEffectArchetype.TypeEnum.TurnUndead:
            case MagicEffectArchetype.TypeEnum.Rally:
            case MagicEffectArchetype.TypeEnum.Banish:
                archetype = new MagicEffectArchetype(archetypeEnum)
                {
                    Association = associatedItemKey.ToLink<IStarfieldMajorRecordGetter>()
                };
                break;
            default:
                archetype = new MagicEffectArchetype(archetypeEnum)
                {
                    Association = associatedItemKey.ToLink<IStarfieldMajorRecordGetter>()
                };
                break;
        }
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
    }

    public static partial void WriteBinaryAssociatedItemCustom(MutagenWriter writer, IMagicEffectGetter item)
    {
        FormKeyBinaryTranslation.Instance.Write(writer, item.Archetype.AssociationKey.FormKey);
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