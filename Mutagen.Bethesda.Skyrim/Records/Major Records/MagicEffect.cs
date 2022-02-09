using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffect
    {
        [Flags]
        public enum Flag
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
            SheathDraw = 0,
            Charge = 1,
            Ready = 2,
            Release = 3,
            ConcentrationCastLoop = 4,
            OnHit = 5,
        }
    }

    namespace Internals
    {
        public partial class MagicEffectBinaryCreateTranslation
        {
            public static partial void FillBinaryCounterEffectLogicCustom(MutagenFrame frame, IMagicEffectInternal item)
            {
                // Don't care about counter
                frame.Position += 2;
            }

            public static partial void FillBinaryConditionsCustom(MutagenFrame frame, IMagicEffectInternal item)
            {
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame);
            }

            public static partial void FillBinaryAssociatedItemCustom(MutagenFrame frame, IMagicEffectInternal item)
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
                        archetype = new MagicEffectSummonCreatureArchetype();
                        break;
                    case MagicEffectArchetype.TypeEnum.Guide:
                        archetype = new MagicEffectGuideArchetype();
                        break;
                    case MagicEffectArchetype.TypeEnum.SpawnHazard:
                        archetype = new MagicEffectSpawnHazardArchetype();
                        break;
                    case MagicEffectArchetype.TypeEnum.PeakValueModifier:
                        archetype = new MagicEffectPeakValueModArchetype();
                        break;
                    case MagicEffectArchetype.TypeEnum.Cloak:
                        archetype = new MagicEffectCloakArchetype();
                        break;
                    case MagicEffectArchetype.TypeEnum.Werewolf:
                        archetype = new MagicEffectWerewolfArchetype();
                        break;
                    case MagicEffectArchetype.TypeEnum.VampireLord:
                        archetype = new MagicEffectVampireArchetype();
                        break;
                    case MagicEffectArchetype.TypeEnum.EnhanceWeapon:
                        archetype = new MagicEffectEnhanceWeaponArchetype();
                        break;
                    case MagicEffectArchetype.TypeEnum.Calm:
                    case MagicEffectArchetype.TypeEnum.Frenzy:
                        archetype = new MagicEffectArchetype(archetypeEnum)
                        {
                            ActorValue = ActorValue.Aggression
                        };
                        break;
                    case MagicEffectArchetype.TypeEnum.Invisibility:
                        archetype = new MagicEffectArchetype(archetypeEnum)
                        {
                            ActorValue = ActorValue.Invisibility
                        };
                        break;
                    case MagicEffectArchetype.TypeEnum.Paralysis:
                        archetype = new MagicEffectArchetype(archetypeEnum)
                        {
                            ActorValue = ActorValue.Paralysis
                        };
                        break;
                    case MagicEffectArchetype.TypeEnum.Demoralize:
                    case MagicEffectArchetype.TypeEnum.TurnUndead:
                    case MagicEffectArchetype.TypeEnum.Rally:
                    case MagicEffectArchetype.TypeEnum.Banish:
                        archetype = new MagicEffectArchetype(archetypeEnum)
                        {
                            ActorValue = ActorValue.Confidence
                        };
                        break;
                    default:
                        archetype = new MagicEffectArchetype(archetypeEnum)
                        {
                            ActorValue = ActorValue.None
                        };
                        break;
                }
                archetype.AssociationKey = associatedItemKey;
                archetype.ActorValue = (ActorValue)frame.ReadInt32();
                return archetype;
            }

            public static partial void FillBinaryArchetypeCustom(MutagenFrame frame, IMagicEffectInternal item)
            {
                item.Archetype = ReadArchetype(frame);
            }
        }

        public partial class MagicEffectBinaryWriteTranslation
        {
            public static partial void WriteBinaryCounterEffectLogicCustom(MutagenWriter writer, IMagicEffectGetter item)
            {
                writer.Write((ushort)item.CounterEffects.Count);
            }

            public static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IMagicEffectGetter item)
            {
                ConditionBinaryWriteTranslation.WriteConditionsList(item.Conditions, writer);
            }

            public static partial void WriteBinaryArchetypeCustom(MutagenWriter writer, IMagicEffectGetter item)
            {
                writer.Write((int)item.Archetype.Type);
                writer.Write((int)item.Archetype.ActorValue);
            }

            public static partial void WriteBinaryAssociatedItemCustom(MutagenWriter writer, IMagicEffectGetter item)
            {
                writer.Write(writer.MetaData.MasterReferences!.GetFormID(item.Archetype.AssociationKey).Raw);
            }
        }

        public partial class MagicEffectBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }

            partial void CounterEffectLogicCustomParse(OverlayStream stream, int offset)
            {
                // Don't care about counter
                stream.Position += 2;
            }

            public IMagicEffectArchetypeGetter GetArchetypeCustom()
            {
                if (!_DATALocation.HasValue) return new MagicEffectArchetype();
                var frame = new MutagenFrame(new MutagenMemoryReadStream(_data, _package.MetaData))
                {
                    Position = _ArchetypeLocation
                };
                return MagicEffectBinaryCreateTranslation.ReadArchetype(frame);
            }
        }
    }
}
