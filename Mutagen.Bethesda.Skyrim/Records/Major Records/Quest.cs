using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class Quest
{
    [Flags]
    public enum Flag
    {
        StartGameEnabled = 0x0001,
        AllowRepeatedStages = 0x0008,
        RunOnce = 0x0100,
        ExcludeFromDialogExport = 0x0200,
        WarnOnAliasFillFailure = 0x0400
    }

    public enum TypeEnum
    { 
        None = 0,
        MainQuest = 1,
        MageGuild = 2,
        ThievesGuild = 3,
        DarkBrotherhood = 4,
        CompanionQuests = 5,
        Misc = 6,
        Daedric = 7,
        SideQuest = 8,
        CivilWar = 9,
        Vampire = 10,
        Dragonborn = 11,
    }

    [Flags]
    public enum TargetFlag
    {
        CompassMarkerIgnoresLocks = 0x1,
    }
}

partial class QuestSetterCommon
{
    private static partial void RemapInferredAssetLinks(
        IQuest obj,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping,
        AssetLinkQuery queryCategories)
    {
        throw new NotImplementedException();
    }
}

partial class QuestCommon
{
    public static partial IEnumerable<IAssetLinkGetter> GetInferredAssetLinks(IQuestGetter obj, Type? assetType)
    {
        if (obj.Flags.HasFlag(Quest.Flag.StartGameEnabled))
        {
            yield return new AssetLink<SkyrimSeqAssetType>(
                Path.Combine($"{obj.FormKey.ModKey.Name}.seq"));
        }
    }
}

partial class QuestBinaryCreateTranslation
{
    public static partial void FillBinaryDialogConditionsCustom(MutagenFrame frame, IQuestInternal item)
    {
        ConditionBinaryCreateTranslation.FillConditionsList(item.DialogConditions, frame);
    }

    public static partial ParseResult FillBinaryUnusedConditionsLogicCustom(MutagenFrame frame, IQuestInternal item)
    {
        var nextHeader = frame.ReadSubrecord();
        if (nextHeader.RecordType != RecordTypes.NEXT
            || nextHeader.Content.Length != 0)
        {
            throw new ArgumentException("Unexpected NEXT header");
        }
        ConditionBinaryCreateTranslation.FillConditionsList(item.EventConditions, frame);
        return null;
    }

    public static partial ParseResult FillBinaryNextAliasIDCustom(MutagenFrame frame, IQuestInternal item)
    {
        // Skip
        frame.ReadSubrecord();
        return null;
    }
}

partial class QuestBinaryWriteTranslation
{
    public static partial void WriteBinaryDialogConditionsCustom(MutagenWriter writer, IQuestGetter item)
    {
        ConditionBinaryWriteTranslation.WriteConditionsList(item.DialogConditions, writer);
    }

    public static partial void WriteBinaryUnusedConditionsLogicCustom(MutagenWriter writer, IQuestGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.NEXT)) { }
        ConditionBinaryWriteTranslation.WriteConditionsList(item.EventConditions, writer);
    }

    public static partial void WriteBinaryNextAliasIDCustom(MutagenWriter writer, IQuestGetter item)
    {
        var aliases = item.Aliases;
        using (HeaderExport.Subrecord(writer, RecordTypes.ANAM))
        {
            if (aliases.Count == 0)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(aliases.Select(x => x.ID).Max() + 1);
            }
        }
    }
}

partial class QuestBinaryOverlay
{
    public IReadOnlyList<IConditionGetter> DialogConditions { get; private set; } = Array.Empty<IConditionGetter>();
    public IReadOnlyList<IConditionGetter> EventConditions { get; private set; } = Array.Empty<IConditionGetter>();

    partial void DialogConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        DialogConditions = ConditionBinaryOverlay.ConstructBinaryOverlayList(stream, _package);
    }

    public partial ParseResult UnusedConditionsLogicCustomParse(OverlayStream stream, int offset)
    {
        var nextHeader = stream.ReadSubrecord();
        if (nextHeader.RecordType != RecordTypes.NEXT
            || nextHeader.Content.Length != 0)
        {
            throw new ArgumentException("Unexpected NEXT header");
        }
        EventConditions = ConditionBinaryOverlay.ConstructBinaryOverlayList(stream, _package);

        return null;
    }

    public partial ParseResult NextAliasIDCustomParse(OverlayStream stream, int offset)
    {
        stream.ReadSubrecord();
        return null;
    }
}