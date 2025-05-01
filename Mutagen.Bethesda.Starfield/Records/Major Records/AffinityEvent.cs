namespace Mutagen.Bethesda.Starfield;

public partial class AffinityEvent
{
    [Flags]
    public enum Flag
    {
        AllowRepeatedEvents = 0x0001,
        TemplateAffinityEvent = 0x0002,
        FillCompanionDialogue = 0x0004,
    }
}