using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class QuestAlias
{
    public enum TypeEnum
    {
        Reference,
        Location,
    }

    [Flags]
    public enum Flag
    {
        ReservesLocationOrReference = 0x0000_0001,
        Optional = 0x0000_0002,
        QuestObject = 0x0000_0004,
        AllowReuseInQuest = 0x0000_0008,
        AllowDead = 0x0000_0010,
        MatchingRefInLoadedArea = 0x0000_0020,
        Essential = 0x0000_0040,
        AllowDisabled = 0x0000_0080,
        StoresText = 0x0000_0100,
        AllowReserved = 0x0000_0200,
        Protected = 0x0000_0400,
        ForcedByAliases = 0x0000_0800,
        AllowDestroyed = 0x0000_1000,
        MatchingRefClosest = 0x0000_2000,
        UsesStoredText = 0x0000_4000,
        InitiallyDisabled = 0x0000_8000,
        AllowCleared = 0x0001_0000,
        ClearNamesWhenRemoved = 0x0002_0000,
    }
}

partial class QuestAliasBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryIDParseCustom(MutagenFrame frame, IQuestAlias item, PreviousParse lastParsed)
    {
        var subMeta = frame.ReadSubrecordHeader();
        item.Type = subMeta.RecordTypeInt switch
        {
            RecordTypeInts.ALST => QuestAlias.TypeEnum.Reference,
            RecordTypeInts.ALLS => QuestAlias.TypeEnum.Location,
            _ => throw new NotImplementedException(),
        };
        item.ID = frame.ReadUInt32();
        return lastParsed;
    }
}

partial class QuestAliasBinaryWriteTranslation
{
    public static partial void WriteBinaryIDParseCustom(MutagenWriter writer, IQuestAliasGetter item)
    {
        RecordType type = item.Type switch
        {
            QuestAlias.TypeEnum.Reference => RecordTypes.ALST,
            QuestAlias.TypeEnum.Location => RecordTypes.ALLS,
            _ => throw new NotImplementedException(),
        };
        using (HeaderExport.Subrecord(writer, type))
        {
            writer.Write(item.ID);
        }
    }
}

partial class QuestAliasBinaryOverlay
{
    public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();
    public uint ID { get; private set; }
    public QuestAlias.TypeEnum Type { get; private set; }

    partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
    }

    public partial ParseResult IDParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        var subMeta = stream.ReadSubrecordHeader();
        this.Type = subMeta.RecordTypeInt switch
        {
            RecordTypeInts.ALST => QuestAlias.TypeEnum.Reference,
            RecordTypeInts.ALLS => QuestAlias.TypeEnum.Location,
            _ => throw new NotImplementedException(),
        };
        this.ID = stream.ReadUInt32();
        return lastParsed;
    }
}