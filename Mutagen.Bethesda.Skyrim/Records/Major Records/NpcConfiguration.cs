using System;
using System.Diagnostics;
using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class NpcConfiguration
    {
        /// <summary>
        /// Flags for NpcConfiguration
        /// - PcLevelMult is disabled/hidden, as that functionality is exposed via a customizedNpcConfiguration's Level field.
        ///   Instantiate either a NpcLevel or PcLevelMult object to specify what type you want.
        /// </summary>
        [Flags]
        public enum Flag : uint
        {
            Female = 0x0000_0001,
            Essential = 0x0000_0002,
            IsCharGenFacePreset = 0x0000_0004,
            Respawn = 0x0000_0008,
            AutoCalcStats = 0x0000_0010,
            Unique = 0x0000_0020,
            DoesntAffectStealthMeter = 0x0000_0040,
            //PcLevelMult = 0x0000_0080,
            UseTemplate = 0x0000_0100,
            Protected = 0x0000_0800,
            Summonable = 0x0000_4000,
            DoesNotBleed = 0x0001_0000,
            BleedoutOverride = 0x0004_0000,
            OppositeGenderAnims = 0x0008_0000,
            SimpleActor = 0x0010_0000,
            LoopedScript = 0x0020_0000,
            LoopedAudio = 0x1000_0000,
            IsGhost = 0x2000_0000,
            Invulnerable = 0x8000_0000
        }

        [Flags]
        public enum TemplateFlag
        {
            Traits = 0x0001,
            Stats = 0x0002,
            Factions = 0x0004,
            SpellList = 0x0008,
            AIData = 0x0010,
            AIPackages = 0x0020,
            ModelAnimation = 0x0040,
            BaseData = 0x0080,
            Inventory = 0x0100,
            Script = 0x0200,
            DefPackList = 0x0400,
            AttackData = 0x0800,
            Keywords = 0x1000,
        }

        public ANpcLevel Level { get; set; } = new NpcLevel();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IANpcLevelGetter INpcConfigurationGetter.Level => Level;
    }

    namespace Internals
    {
        public partial class NpcConfigurationBinaryCreateTranslation
        {
            public const uint PcLevelMultFlag = 0x0000_0080;

            static partial void FillBinaryFlagsCustom(MutagenFrame frame, INpcConfiguration item)
            {
                // Read normally
                item.Flags = (NpcConfiguration.Flag)frame.ReadUInt32();
            }

            static partial void FillBinaryLevelCustom(MutagenFrame frame, INpcConfiguration item)
            {
                if (EnumExt.HasFlag((uint)item.Flags, PcLevelMultFlag))
                {
                    var raw = frame.ReadUInt16();
                    float f = (float)raw;
                    f /= 1000;
                    item.Level = new PcLevelMult()
                    {
                        LevelMult = f
                    };
                }
                else
                {
                    item.Level = new NpcLevel()
                    {
                        Level = frame.ReadInt16()
                    };
                }

                // Clear out PcLevelMult flag, as that information is kept in the field type above
                uint rawFlags = (uint)item.Flags;
                rawFlags &= ~PcLevelMultFlag;
                item.Flags = (NpcConfiguration.Flag)rawFlags;
            }
        }

        public partial class NpcConfigurationBinaryWriteTranslation
        {
            static partial void WriteBinaryFlagsCustom(MutagenWriter writer, INpcConfigurationGetter item)
            {
                // Add back PcLevelMult flag
                uint raw = (uint)item.Flags;
                switch (item.Level)
                {
                    case IPcLevelMultGetter levelMult:
                        raw |= NpcConfigurationBinaryCreateTranslation.PcLevelMultFlag;
                        break;
                    case INpcLevelGetter level:
                        raw &= ~NpcConfigurationBinaryCreateTranslation.PcLevelMultFlag;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                writer.Write(raw);
            }

            static partial void WriteBinaryLevelCustom(MutagenWriter writer, INpcConfigurationGetter item)
            {
                switch (item.Level)
                {
                    case IPcLevelMultGetter levelMult:
                        var f = levelMult.LevelMult;
                        f *= 1000;
                        writer.Write((ushort)f);
                        break;
                    case INpcLevelGetter level:
                        writer.Write(level.Level);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public partial class NpcConfigurationBinaryOverlay
        {
            IANpcLevelGetter GetLevelCustom(int location)
            {
                uint rawFlags = BinaryPrimitives.ReadUInt32LittleEndian(_data);
                if (EnumExt.HasFlag(rawFlags, NpcConfigurationBinaryCreateTranslation.PcLevelMultFlag))
                {
                    var raw = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location));
                    float f = (float)raw;
                    f /= 1000;
                    return new PcLevelMult()
                    {
                        LevelMult = f
                    };
                }
                else
                {
                    return new NpcLevel()
                    {
                        Level = BinaryPrimitives.ReadInt16LittleEndian(_data.Slice(location))
                    };
                }
            }

            NpcConfiguration.Flag GetFlagsCustom(int location)
            {
                uint rawFlags = BinaryPrimitives.ReadUInt32LittleEndian(_data);
                // Clear out PcLevelMult flag, as that information is kept in the field type above
                rawFlags &= ~NpcConfigurationBinaryCreateTranslation.PcLevelMultFlag;
                return (NpcConfiguration.Flag)rawFlags;
            }
        }
    }
}
