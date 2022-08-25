using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;
using System.Text.RegularExpressions;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Skyrim.Assets;

namespace Mutagen.Bethesda.Skyrim;

public partial class Book
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

partial class BookBinaryCreateTranslation
{
    public const byte SkillFlag = 0x01;
    public const byte SpellFlag = 0x04;

    public enum TeachesOption
    {
        None,
        Skill,
        Spell
    }

    public static TeachesOption GetTeachingOption(int flags)
    {
        var avFlag = EnumExt.HasFlag(flags, SkillFlag);
        var spellFlag = EnumExt.HasFlag(flags, SpellFlag);
        var numFlags = avFlag ? 1 : 0;
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
        else
        {
            return TeachesOption.None;
        }
    }

    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, IBookInternal item)
    {
        item.Flags = (Book.Flag)frame.ReadUInt8();
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
                item.Teaches = new BookSkill
                {
                    Skill = (Skill)frame.ReadInt32()
                };
                break;
            case TeachesOption.Spell:
                item.Teaches = new BookSpell()
                {
                    Spell = new FormLink<ISpellGetter>(FormLinkBinaryTranslation.Instance.Parse(frame))
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
                flags = EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SkillFlag, false);
                flags = EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SpellFlag, true);
                break;
            case IBookSkillGetter _:
                flags = EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SkillFlag, true);
                flags = EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SpellFlag, false);
                break;
            case IBookTeachesNothingGetter _:
                flags = EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SkillFlag, false);
                flags = EnumExt.SetFlag(flags, BookBinaryCreateTranslation.SpellFlag, false);
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
            case IBookSpellGetter spell:
                FormLinkBinaryTranslation.Instance.Write(writer, spell.Spell);
                break;
            case IBookSkillGetter skill:
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
            case IBookTeachesNothingGetter nothing:
                writer.Write(nothing.RawContent);
                break;
            default:
                writer.WriteZeros(4);
                break;
        }
    }
}

partial class BookCommon
{
    private const string Pattern = @"<img[\w\s='/,.:]*src='img:\/\/([\w\s=/,.:]*)'[\w\s='/,.:]*>";
    private static readonly Regex Regex = new(Pattern);
    
    public static partial IEnumerable<IAssetLink> GetInferredAssetLinks(IBookGetter obj, Type? assetType)
    {
        if (assetType != null && assetType != typeof(SkyrimTextureAssetType)) yield break;
        
        var text = obj.BookText.String;
        if (string.IsNullOrWhiteSpace(text)) yield break;

        var match = Regex.Match(text);
        if (match.Success)
        {
            yield return new AssetLink<SkyrimTextureAssetType>(match.Groups[1].Value);
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
                return new BookSkill
                {
                    Skill = (Skill)BinaryPrimitives.ReadInt32LittleEndian(_recordData.Slice(_TeachesLocation, 4))
                };
                break;
            case BookBinaryCreateTranslation.TeachesOption.Spell:
                return new BookSpell()
                {
                    Spell = new FormLink<ISpellGetter>(FormKeyBinaryTranslation.Instance.Parse(_recordData.Slice(_TeachesLocation, 4), _package.MetaData.MasterReferences!))
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}