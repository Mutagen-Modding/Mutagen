using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Text;
using static Mutagen.Bethesda.Skyrim.MagicEffect;

namespace Mutagen.Bethesda.Skyrim.Internals
{
    public partial class MagicEffectDataBinaryCreateTranslation
    {
        static partial void FillBinaryAssociatedItemCustom(MutagenFrame frame, IMagicEffectData item)
        {
            // Skip for now.  Will be parsed by Archetype.
            frame.Position += 4;
        }

        public static MagicEffectArchetype ReadArchetype(MutagenFrame frame)
        {
            // Jump back and read in association FormKey
            var curPos = frame.Position;
            frame.Position -= 56;
            FormKey associatedItemKey = FormKeyBinaryTranslation.Instance.Parse(frame);
            frame.Position = curPos;

            // Finish reading archetype
            MagicEffectArchetype.TypeEnum archetypeEnum = (MagicEffectArchetype.TypeEnum)frame.ReadInt32();
            MagicEffectArchetype archetype;
            switch (archetypeEnum)
            {
                case MagicEffectArchetype.TypeEnum.Light:
                    archetype = new MagicEffectLightArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.Bound:
                    archetype = new MagicEffectBoundArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.SummonCreature:
                    archetype = new MagicEffectNpcArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.Guide:
                    archetype = new MagicEffectGuideArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.SpawnHazard:
                    archetype = new MagicEffectSpawnHazardArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.PeakValueModifier:
                    archetype = new MagicEffectKeywordArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.Cloak:
                    archetype = new MagicEffectSpellArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.Werewolf:
                    archetype = new MagicEffectWerewolfArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.VampireLord:
                    archetype = new MagicEffectVampireArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.EnhanceWeapon:
                    archetype = new MagicEffectEnchantmentArchetype();
                    break;
                case MagicEffectArchetype.TypeEnum.Calm:
                case MagicEffectArchetype.TypeEnum.Frenzy:
                    archetype = new MagicEffectArchetype(archetypeEnum)
                    {
                        ActorValue = ActorValueExtended.Aggression
                    };
                    break;
                case MagicEffectArchetype.TypeEnum.Invisibility:
                    archetype = new MagicEffectArchetype(archetypeEnum)
                    {
                        ActorValue = ActorValueExtended.Invisibility
                    };
                    break;
                case MagicEffectArchetype.TypeEnum.Paralysis:
                    archetype = new MagicEffectArchetype(archetypeEnum)
                    {
                        ActorValue = ActorValueExtended.Paralysis
                    };
                    break;
                case MagicEffectArchetype.TypeEnum.Demoralize:
                case MagicEffectArchetype.TypeEnum.TurnUndead:
                case MagicEffectArchetype.TypeEnum.Rally:
                case MagicEffectArchetype.TypeEnum.Banish:
                    archetype = new MagicEffectArchetype(archetypeEnum)
                    {
                        ActorValue = ActorValueExtended.Confidence
                    };
                    break;
                default:
                    archetype = new MagicEffectArchetype(archetypeEnum)
                    {
                        ActorValue = ActorValueExtended.None
                    };
                    break;
            }
            archetype.AssociationKey = associatedItemKey;
            archetype.ActorValue = (ActorValueExtended)frame.ReadInt32();
            return archetype;
        }

        static partial void FillBinaryArchetypeCustom(MutagenFrame frame, IMagicEffectData item)
        {
            item.Archetype = ReadArchetype(frame);
        }
    }

    public partial class MagicEffectDataBinaryWriteTranslation
    {
        static partial void WriteBinaryArchetypeCustom(MutagenWriter writer, IMagicEffectDataGetter item)
        {
            writer.Write((int)item.Archetype.Type);
            writer.Write((int)item.Archetype.ActorValue);
        }

        static partial void WriteBinaryAssociatedItemCustom(MutagenWriter writer, IMagicEffectDataGetter item)
        {
            writer.Write(writer.MasterReferences!.GetFormID(item.Archetype.AssociationKey).Raw);
        }
    }

    public partial class MagicEffectDataBinaryOverlay
    {
        public IMagicEffectArchetypeGetter GetArchetypeCustom(int location)
        {
            var frame = new MutagenFrame(new MutagenMemoryReadStream(_data, _package.Meta, _package.MasterReferences))
            {
                Position = location
            };
            return MagicEffectDataBinaryCreateTranslation.ReadArchetype(frame);
        }
    }
}
