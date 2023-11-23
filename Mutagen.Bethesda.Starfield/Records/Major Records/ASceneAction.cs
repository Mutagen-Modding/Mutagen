namespace Mutagen.Bethesda.Starfield;

public partial class ASceneAction
{
    [Flags]
    public enum Flag : uint
    {
        PlayerPositiveUseDialogueSubtype = 0x0000_0080,
        PlayerNegativeUseDialogueSubtype = 0x0000_0100,
        PlayerNeutralUseDialogueSubtype = 0x0000_0200,
        UseDialogueSubtype = 0x0000_0400,
        PlayerQuestionUseDialogueSubtype = 0x0000_0800,
        ClearTargetOnActionEnd = 0x0000_1000,
        FaceTarget = 0x0000_8000,
        Looping = 0x0001_0000,
        HeadtrackPlayer = 0x0002_0000,
        IgnoreForCompletion = 0x0008_0000,
        CameraSpeakerTarget = 0x0020_0000,
        CompleteFaceTarget = 0x0040_0000,
        NpcPositiveUseDialogueSubtype = 0x0800_0000,
        NpcNegativeUseDialogueSubtype = 0x1000_0000,
        NpcNeutralUseDialogueSubtype = 0x2000_0000,
        NpcQuestionUseDialogueSubtype = 0x4000_0000,
    }
}