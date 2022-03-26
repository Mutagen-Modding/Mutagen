using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Book
    {
        [Flags]
        public enum Flag
        {
            AdvanceActorValue = 0x01,
            CantBeTaken = 0x02,
            AddSpell = 0x04,
            AddPerk = 0x10
        }
    }

    namespace Internals
    {
        public partial class BookBinaryCreateTranslation
        {
            public const byte AvFlag = 0x10;
            public const byte PerkFlag = 0x10;
            public const byte SpellFlag = 0x04;

            public static partial void FillBinaryTeachesCustom(MutagenFrame frame, IBookInternal item)
            {
                if ((((int)item.Flags) & AvFlag) > 0)
                {
                    item.Teaches = new BookActorValue()
                    {
                        ActorValue = new FormLink<IActorValueInformationGetter>(FormLinkBinaryTranslation.Instance.Parse(frame))
                    };
                }
                else if ((((int)item.Flags) & SpellFlag) > 0)
                {
                    item.Teaches = new BookSpell()
                    {
                        Spell = new FormLink<ISpellGetter>(FormLinkBinaryTranslation.Instance.Parse(frame))
                    };
                }
                else if ((((int)item.Flags) & PerkFlag) > 0)
                {
                    item.Teaches = new BookPerk()
                    {
                        Perk = new FormLink<IPerkGetter>(FormLinkBinaryTranslation.Instance.Parse(frame))
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
            public static partial void WriteBinaryTeachesCustom(MutagenWriter writer, IBookGetter item)
            {
                switch (item.Teaches)
                {
                    case IBookActorValueGetter av:
                        FormLinkBinaryTranslation.Instance.Write(writer, av.ActorValue);
                        break;
                    case IBookSpellGetter spell:
                        FormLinkBinaryTranslation.Instance.Write(writer, spell.Spell);
                        break;
                    case IBookPerkGetter perk:
                        FormLinkBinaryTranslation.Instance.Write(writer, perk.Perk);
                        break;
                    case IBookTeachesNothingGetter nothing:
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
            public partial IBookTeachTargetGetter? GetTeachesCustom()
            {
                if (!_DATALocation.HasValue) return default;
                int flags = (int)this.Flags;
                if ((flags & BookBinaryCreateTranslation.AvFlag) > 0)
                {
                    return new BookActorValue()
                    {
                        ActorValue = new FormLink<IActorValueInformationGetter>(FormKeyBinaryTranslation.Instance.Parse(_data.Slice(_TeachesLocation, 4), _package.MetaData.MasterReferences!))
                    };
                }
                else if ((flags & BookBinaryCreateTranslation.SpellFlag) > 0)
                {
                    return new BookSpell()
                    {
                        Spell = new FormLink<ISpellGetter>(FormKeyBinaryTranslation.Instance.Parse(_data.Slice(_TeachesLocation, 4), _package.MetaData.MasterReferences!))
                    };
                }
                else if ((flags & BookBinaryCreateTranslation.PerkFlag) > 0)
                {
                    return new BookPerk()
                    {
                        Perk = new FormLink<IPerkGetter>(FormKeyBinaryTranslation.Instance.Parse(_data.Slice(_TeachesLocation, 4), _package.MetaData.MasterReferences!))
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
