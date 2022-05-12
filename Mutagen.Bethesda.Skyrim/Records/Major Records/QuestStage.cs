namespace Mutagen.Bethesda.Skyrim;

public partial class QuestStage
{
    [Flags]
    public enum Flag
    {
        StartUpStage = 0x02,
        ShutDownStage = 0x04,
        KeepInstanceDataFromHereOn = 0x08,
    }
}