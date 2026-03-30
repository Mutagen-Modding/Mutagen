namespace Mutagen.Bethesda.Fallout3;

public partial class NpcAIData
{
    public MoodFnv MoodFnv
    {
        get => (MoodFnv)(uint)Mood;
        set => Mood = (Mood)(uint)value;
    }
}
