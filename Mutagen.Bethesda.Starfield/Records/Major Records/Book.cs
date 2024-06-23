using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class Book
{
    [Flags]
    public enum Flag
    {
        CantBeTaken = 0x02
    }
}

partial class BookBinaryCreateTranslation
{
    public const byte SkillFlag = 0x01;
    public const byte SpellFlag = 0x04;
    public const byte PerkFlag = 0x10;

    public enum TeachesOption
    {
        None,
        Skill,
        Spell,
        Perk
    }

    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, IBookInternal item)
    {
        item.Flags = (Book.Flag)frame.ReadUInt8();
    }

    public static TeachesOption GetTeachingOption(int flags)
    {
        var avFlag = Enums.HasFlag(flags, SkillFlag);
        var perkFlag = Enums.HasFlag(flags, PerkFlag);
        var spellFlag = Enums.HasFlag(flags, SpellFlag);
        var numFlags = avFlag ? 1 : 0;
        numFlags += perkFlag ? 1 : 0;
        numFlags += spellFlag ? 1 : 0;
        if (numFlags > 1)
        {
            throw new MalformedDataException($"Multiple teaching flags on at the same time.");
        }
        if (avFlag)
        {
            return TeachesOption.Skill;
        }
        else if (spellFlag)
        {
            return TeachesOption.Spell;
        }
        else if (perkFlag)
        {
            return TeachesOption.Perk;
        }
        else
        {
            return TeachesOption.None;
        }
    }

    public static partial void FillBinaryTeachesCustom(MutagenFrame frame, IBookInternal item)
    {
        switch (GetTeachingOption((int)item.Flags))
        {
            case TeachesOption.None:
                item.Teaches = new BookTeachesNothing()
                {
                    RawContent = frame.ReadUInt32()
                };
                break;
            case TeachesOption.Skill:
                item.Teaches = new BookActorValue()
                {
                    ActorValue = new FormLink<IActorValueInformationGetter>(FormLinkBinaryTranslation.Instance.Parse(frame))
                };
                break;
            case TeachesOption.Spell:
                item.Teaches = new BookSpell()
                {
                    Spell = new FormLink<ISpellGetter>(FormLinkBinaryTranslation.Instance.Parse(frame))
                };
                break;
            case TeachesOption.Perk:
                item.Teaches = new BookPerk()
                {
                    Perk = new FormLink<IPerkGetter>(FormLinkBinaryTranslation.Instance.Parse(frame))
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

partial class BookBinaryWriteTranslation
{
    public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IBookGetter item)
    {
        byte flags = (byte)item.Flags;
        switch (item.Teaches)
        {
            case IBookSpellGetter _:
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.SkillFlag, false);
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.SpellFlag, true);
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.PerkFlag, false);
                break;
            case IBookActorValueGetter _:
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.SkillFlag, true);
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.SpellFlag, false);
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.PerkFlag, false);
                break;
            case IBookPerkGetter _:
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.SkillFlag, false);
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.SpellFlag, false);
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.PerkFlag, true);
                break;
            case IBookTeachesNothingGetter _:
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.SkillFlag, false);
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.SpellFlag, false);
                flags = Enums.SetFlag(flags, BookBinaryCreateTranslation.PerkFlag, false);
                break;
            default:
                break;
        }
        writer.Write(flags);
    }
    
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

partial class BookBinaryOverlay
{
    public partial Book.Flag GetFlagsCustom()
    {
        if (!_DATALocation.HasValue) return default;
        return (Book.Flag)_recordData[_FlagsLocation];
    }
    
    public partial IBookTeachTargetGetter? GetTeachesCustom()
    {
        if (!_DATALocation.HasValue) return default;

        switch (BookBinaryCreateTranslation.GetTeachingOption((int)this.Flags))
        {
            case BookBinaryCreateTranslation.TeachesOption.None:
                return new BookTeachesNothing()
                {
                    RawContent = BinaryPrimitives.ReadUInt32LittleEndian(_recordData.Slice(_TeachesLocation, 4))
                };
                break;
            case BookBinaryCreateTranslation.TeachesOption.Skill:
                return new BookActorValue()
                {
                    ActorValue = new FormLink<IActorValueInformationGetter>(FormKeyBinaryTranslation.Instance.Parse(_recordData.Slice(_TeachesLocation, 4), _package.MetaData.MasterReferences.Raw))
                };
                break;
            case BookBinaryCreateTranslation.TeachesOption.Spell:
                return new BookSpell()
                {
                    Spell = new FormLink<ISpellGetter>(FormKeyBinaryTranslation.Instance.Parse(_recordData.Slice(_TeachesLocation, 4), _package.MetaData.MasterReferences.Raw))
                };
                break;
            case BookBinaryCreateTranslation.TeachesOption.Perk:
                return new BookPerk()
                {
                    Perk = new FormLink<IPerkGetter>(FormKeyBinaryTranslation.Instance.Parse(_recordData.Slice(_TeachesLocation, 4), _package.MetaData.MasterReferences.Raw))
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}