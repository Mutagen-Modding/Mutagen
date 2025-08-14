using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

public partial class MagicEffectArchetype
{
    public enum TypeEnum
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

    public MagicEffectArchetype(TypeEnum type = TypeEnum.ValueModifier)
    {
        this.Type = type;
    }

    MagicEffectArchetype.TypeEnum IAMagicEffectArchetypeGetter.Type => Type;
    public override IFormLinkIdentifier AssociationKey => Association;
}
