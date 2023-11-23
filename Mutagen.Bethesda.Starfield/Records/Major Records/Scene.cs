using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class Scene
{
    [Flags]
    public enum Flag
    {
        BeginOnQuestStart = 0x0001,
        StopOnQuestEnd = 0x0002,
        ShowAllText = 0x0004,
        RepeatConditionsWhileTrue = 0x0008,
        Interruptable = 0x0010,
        PreventPlayerExitDialogue = 0x0040,
        DisableDialogueCamera = 0x0800,
        NoFollowerIdleChatter = 0x1000,
    }
}

partial class SceneBinaryCreateTranslation
{
    public enum ActionType
    {
        Dialogue,
        Package,
        Timer,
        PlayerDialogue,
        StartScene,
        Radio,
        Move,
        Camera,
        FX,
        Animation,
        Timeline
    }

    private static ASceneAction ReadSceneAction(
        SubrecordFrame subRec,
        MutagenFrame frame)
    {
        frame = frame.SpawnAll();
        switch ((ActionType)subRec.AsInt16())
        {
            case ActionType.Dialogue:
                return DialogueSceneAction.CreateFromBinary(frame);
            case ActionType.Package:
                return PackageSceneAction.CreateFromBinary(frame);
            case ActionType.Timer:
                return TimerSceneAction.CreateFromBinary(frame);
            case ActionType.PlayerDialogue:
                return PlayerDialogueSceneAction.CreateFromBinary(frame);
            case ActionType.StartScene:
                return StartSceneAction.CreateFromBinary(frame);
            case ActionType.Radio:
                return RadioSceneAction.CreateFromBinary(frame);
            case ActionType.Move:
                return MoveSceneAction.CreateFromBinary(frame);
            case ActionType.Camera:
                return CameraSceneAction.CreateFromBinary(frame);
            case ActionType.FX:
                return FxSceneAction.CreateFromBinary(frame);
            case ActionType.Animation:
                return AnimationSceneAction.CreateFromBinary(frame);
            case ActionType.Timeline:
                return TimelineSceneAction.CreateFromBinary(frame);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static ExtendedList<ASceneAction> ReadSceneActions(
        MutagenFrame frame)
    {
        frame = frame.SpawnAll();
        var ret = new ExtendedList<ASceneAction>();
        while (frame.TryReadSubrecord(RecordTypes.ANAM, out var subRec))
        {
            ret.Add(ReadSceneAction(subRec, frame));

            try
            {
                
                // Footer
                frame.ReadSubrecord(RecordTypes.ANAM);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        return ret;
    }
    
    public static partial void FillBinaryActionsCustom(
        MutagenFrame frame,
        ISceneInternal item,
        PreviousParse lastParsed)
    {
        item.Actions = ReadSceneActions(frame);
    }
}

partial class SceneBinaryWriteTranslation
{
    public static partial void WriteBinaryActionsCustom(
        MutagenWriter writer,
        ISceneGetter item)
    {
        foreach (var action in item.Actions.EmptyIfNull())
        {
            WriteSceneAction(writer, action);

            using (HeaderExport.Subrecord(writer, RecordTypes.ANAM)) { }
        }
    }

    public static void WriteSceneAction(
        MutagenWriter writer,
        IASceneActionGetter sceneAction)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.ANAM))
        {
            SceneBinaryCreateTranslation.ActionType aType = sceneAction switch
            {
                IAnimationSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.Animation,
                ITimerSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.Timer,
                ICameraSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.Camera,
                IDialogueSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.Dialogue,
                IFxSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.FX,
                IMoveSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.Move,
                IPackageSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.Package,
                IPlayerDialogueSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.PlayerDialogue,
                IRadioSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.Radio,
                IStartSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.StartScene,
                ITimelineSceneActionGetter _ => SceneBinaryCreateTranslation.ActionType.Timeline,
                _ => throw new NotImplementedException()
            };
            writer.Write((ushort)aType);
        }
        sceneAction.WriteToBinary(writer);
    }
}

partial class SceneBinaryOverlay
{
    public IReadOnlyList<IASceneActionGetter>? Actions => throw new NotImplementedException();
}