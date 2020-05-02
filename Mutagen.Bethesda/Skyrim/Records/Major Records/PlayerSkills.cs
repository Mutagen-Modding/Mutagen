using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class PlayerSkills
    {
    }

    namespace Internals
    {
        public partial class PlayerSkillsBinaryCreateTranslation
        {
            public static readonly int SkillsSize = EnumExt.GetSize<Skill>() - 1;

            static partial void FillBinarySkillOffsetsCustom(MutagenFrame frame, IPlayerSkills item)
            {
                var mem = frame.ReadMemory(SkillsSize);
                for (int i = 0; i < SkillsSize; i++)
                {
                    item.SkillOffsets[EnumExt.GetNth<Skill>(i)] = mem[i];
                }
            }

            static partial void FillBinarySkillValuesCustom(MutagenFrame frame, IPlayerSkills item)
            {
                var mem = frame.ReadMemory(SkillsSize);
                for (int i = 0; i < SkillsSize; i++)
                {
                    item.SkillValues[EnumExt.GetNth<Skill>(i)] = mem[i];
                }
            }
        }

        public partial class PlayerSkillsBinaryWriteTranslation
        {
            static partial void WriteBinarySkillOffsetsCustom(MutagenWriter writer, IPlayerSkillsGetter item)
            {
                var dict = item.SkillOffsets;
                for (int i = 0; i < PlayerSkillsBinaryCreateTranslation.SkillsSize; i++)
                {
                    if (dict.TryGetValue(EnumExt.GetNth<Skill>(i), out var val))
                    {
                        writer.Write(val);
                    }
                    else
                    {
                        writer.WriteZeros(1);
                    }
                }
            }

            static partial void WriteBinarySkillValuesCustom(MutagenWriter writer, IPlayerSkillsGetter item)
            {
                var dict = item.SkillValues;
                for (int i = 0; i < PlayerSkillsBinaryCreateTranslation.SkillsSize; i++)
                {
                    if (dict.TryGetValue(EnumExt.GetNth<Skill>(i), out var val))
                    {
                        writer.Write(val);
                    }
                    else
                    {
                        writer.WriteZeros(1);
                    }
                }
            }
        }

        public partial class PlayerSkillsBinaryOverlay
        {
            public IReadOnlyDictionary<Skill, byte> SkillValues
            {
                get
                {
                    var mem = _data;
                    var ret = new Dictionary<Skill, byte>();
                    for (int i = 0; i < PlayerSkillsBinaryCreateTranslation.SkillsSize; i++)
                    {
                        ret[EnumExt.GetNth<Skill>(i)] = mem[i];
                    }
                    return ret;
                }
            }

            public IReadOnlyDictionary<Skill, byte> SkillOffsets
            {
                get
                {
                    var mem = _data.Slice(PlayerSkillsBinaryCreateTranslation.SkillsSize);
                    var ret = new Dictionary<Skill, byte>();
                    for (int i = 0; i < PlayerSkillsBinaryCreateTranslation.SkillsSize; i++)
                    {
                        ret[EnumExt.GetNth<Skill>(i)] = mem[i];
                    }
                    return ret;
                }
            }
        }
    }
}
