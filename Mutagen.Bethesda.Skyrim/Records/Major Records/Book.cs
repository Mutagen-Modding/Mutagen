using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Book
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IIconsGetter? IHasIconsGetter.Icons => this.Icons;
        #endregion

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
        public partial class BookBinaryCreateTranslation
        {
            public const byte SkillFlag = 0x01;
            public const byte SpellFlag = 0x04;

            static partial void FillBinaryFlagsCustom(MutagenFrame frame, IBookInternal item)
            {
                item.Flags = (Book.Flag)frame.ReadUInt8();
            }

            static partial void FillBinaryTeachesCustom(MutagenFrame frame, IBookInternal item)
            {
                if ((((int)item.Flags) & SpellFlag) > 0)
                {
                    item.Teaches = new BookSpell()
                    {
                        Spell = FormLinkBinaryTranslation.Instance.Parse(frame)
                    };
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

        public partial class BookBinaryWriteTranslation
        {
            static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IBookGetter item)
            {
                byte flags = (byte)item.Flags;
                switch (item.Teaches)
                {
                    case BookSpell _:
                        flags = (byte)EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SkillFlag, false);
                        flags = (byte)EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SpellFlag, true);
                        break;
                    case BookSkill _:
                        flags = (byte)EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SkillFlag, true);
                        flags = (byte)EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SpellFlag, false);
                        break;
                    default:
                        break;
                }
                writer.Write(flags);
            }

            static partial void WriteBinaryTeachesCustom(MutagenWriter writer, IBookGetter item)
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

        public partial class BookBinaryOverlay
        {
            public Book.Flag GetFlagsCustom()
            {
                if (!_DATALocation.HasValue) return default;
                return (Book.Flag)_data[_FlagsLocation];
            }

            public IBookTeachTargetGetter? GetTeachesCustom()
            {
                if (!_DATALocation.HasValue) return default;
                int flags = (int)this.Flags;
                if ((flags & BookBinaryCreateTranslation.SpellFlag) > 0)
                {
                    return new BookSpell()
                    {
                        Spell = FormKeyBinaryTranslation.Instance.Parse(_data.Slice(_TeachesLocation, 4), _package.MetaData.MasterReferences!)
                    };
                }
                else if ((flags & BookBinaryCreateTranslation.SkillFlag) > 0)
                {
                    return new BookSkill
                    {
                        Skill = (Skill)BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(_TeachesLocation, 4))
                    };
                }
                else
                {
                    return new BookTeachesNothing()
                    {
                        RawContent = BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(_TeachesLocation, 4))
                    };
                }
            }
        }
    }
}
