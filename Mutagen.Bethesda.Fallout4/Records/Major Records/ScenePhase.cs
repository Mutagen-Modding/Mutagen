using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

partial class ScenePhase
{
    [Flags]
    public enum Flag
    {
        StartWalkAwayPhase = 0x01,
        DoNotRunEndScriptsOnSceneJump = 0x02,
        StartInheritInTemplatedScenes = 0x04,
    }
}

partial class ScenePhaseBinaryCreateTranslation
{
    public static partial void FillBinaryStartConditionsCustom(MutagenFrame frame, IScenePhase item, PreviousParse lastParsed)
    {
        ConditionBinaryCreateTranslation.FillConditionsList(item.StartConditions, frame);
    }

    public static partial void FillBinaryCompletionConditionsCustom(MutagenFrame frame, IScenePhase item, PreviousParse lastParsed)
    {
        frame.ReadSubrecord(RecordTypes.NEXT);
        ConditionBinaryCreateTranslation.FillConditionsList(item.CompletionConditions, frame);
        frame.ReadSubrecord(RecordTypes.NEXT);
    }
}

partial class ScenePhaseBinaryWriteTranslation
{
    public static partial void WriteBinaryStartConditionsCustom(MutagenWriter writer, IScenePhaseGetter item)
    {
        ConditionBinaryWriteTranslation.WriteConditionsList(item.StartConditions, writer);
    }

    public static partial void WriteBinaryCompletionConditionsCustom(MutagenWriter writer, IScenePhaseGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.NEXT)) { }
        ConditionBinaryWriteTranslation.WriteConditionsList(item.CompletionConditions, writer);
        using (HeaderExport.Subrecord(writer, RecordTypes.NEXT)) { }
    }
}

partial class ScenePhaseBinaryOverlay
{
    public IReadOnlyList<IConditionGetter> StartConditions { get; private set; } = Array.Empty<IConditionGetter>();

    public IReadOnlyList<IConditionGetter> CompletionConditions { get; private set; } = Array.Empty<IConditionGetter>();

    partial void StartConditionsCustomParse(OverlayStream stream, int finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        StartConditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
    }

    partial void CompletionConditionsCustomParse(OverlayStream stream, int finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        stream.ReadSubrecord(RecordTypes.NEXT);
        CompletionConditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
        stream.ReadSubrecord(RecordTypes.NEXT);
    }
}
