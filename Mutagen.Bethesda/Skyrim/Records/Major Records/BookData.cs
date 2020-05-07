using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class BookData
    {
        [Flags]
        public enum Flag
        {
            CantBeTaken = 0x02
        }

        public enum BookType : byte
        {
            BookOrTome = 0,
            NoteOrScroll = 255
        }
    }

    namespace Internals
    {
        public partial class BookDataBinaryCreateTranslation
        {
            public const byte SkillFlag = 0x01;
            public const byte SpellFlag = 0x04;

            static partial void FillBinaryFlagsCustom(MutagenFrame frame, IBookData item)
            {
                item.Flags = (BookData.Flag)frame.ReadUInt8();
            }

            static partial void FillBinaryTeachesCustom(MutagenFrame frame, IBookData item)
            {
                if ((((int)item.Flags) & SpellFlag) > 0)
                {
                    var spell = new BookSpell();
                    spell.Spell.FormKey = FormLinkBinaryTranslation.Instance.Parse(frame);
                    item.Teaches = spell;
                }
                else if ((((int)item.Flags) & SkillFlag) > 0)
                {
                    item.Teaches = new BookSkill
                    {
                        Skill = (Skill)frame.ReadInt32()
                    };
                }
                else
                {
                    item.Teaches = new BookTeachesNothing()
                    {
                        RawContent = frame.ReadUInt32()
                    };
                }
            }
        }

        public partial class BookDataBinaryWriteTranslation
        {
            static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IBookDataGetter item)
            {
                byte flags = (byte)item.Flags;
                switch (item.Teaches)
                {
                    case BookSpell _:
                        flags = (byte)EnumExt.SetFlag(flags, BookDataBinaryCreateTranslation.SkillFlag, false);
                        flags = (byte)EnumExt.SetFlag(flags, BookDataBinaryCreateTranslation.SpellFlag, true);
                        break;
                    case BookSkill _:
                        flags = (byte)EnumExt.SetFlag(flags, BookDataBinaryCreateTranslation.SkillFlag, true);
                        flags = (byte)EnumExt.SetFlag(flags, BookDataBinaryCreateTranslation.SpellFlag, false);
                        break;
                    default:
                        break;
                }
                writer.Write(flags);
            }

            static partial void WriteBinaryTeachesCustom(MutagenWriter writer, IBookDataGetter item)
            {
                switch (item.Teaches)
                {
                    case BookSpell spell:
                        FormLinkBinaryTranslation.Instance.Write(writer, spell.Spell);
                        break;
                    case BookSkill skill:
                        var skillVal = skill.Skill;
                        if (skillVal == null)
                        {
                            writer.Write(-1);
                        }
                        else
                        {
                            writer.Write((int)skillVal);
                        }
                        break;
                    case BookTeachesNothing nothing:
                        writer.Write(nothing.RawContent);
                        break;
                    default:
                        writer.WriteZeros(4);
                        break;
                }
            }
        }

        public partial class BookDataBinaryOverlay
        {
            public BookData.Flag GetFlagsCustom(int location) => (BookData.Flag)_data[location];
            public IBookTeachTargetGetter? GetTeachesCustom(int location)
            {
                int flags = (int)this.Flags;
                if ((flags & BookDataBinaryCreateTranslation.SpellFlag) > 0)
                {
                    var spell = new BookSpell();
                    spell.Spell.FormKey = FormKeyBinaryTranslation.Instance.Parse(_data.Slice(location, 4), _package.MasterReferences);
                    return spell;
                }
                else if ((flags & BookDataBinaryCreateTranslation.SkillFlag) > 0)
                {
                    return new BookSkill
                    {
                        Skill = (Skill)BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(location, 4))
                    };
                }
                else
                {
                    return new BookTeachesNothing()
                    {
                        RawContent = BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(location, 4))
                    };
                }
            }
        }
    }
}
