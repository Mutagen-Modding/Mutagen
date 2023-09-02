namespace Mutagen.Bethesda.Starfield;

public partial class Keyword
{
    // ToDo
    // Copy Paste Risk
    [Flags]
    public enum MajorFlag
    {
        Restricted = 0x0008_0000
    }

    // ToDo
    // Copy Paste Risk
    public enum TypeEnum
    {
        None,
        ComponentTechLevel,
        AttachPoint,
        ComponentProperty,
        InstantiationFilter,
        ModAssociation,
        Sound,
        AnimArchetype,
        FunctionCall,
        RecipeFilter,
        AttractionType,
        DialogueSubtype,
        QuestTarget,
        AnimFlavor,
        AnimGender,
        AnimFace,
        QuestGroup,
        AnimInjured,
        DispelEffect,
    }
}