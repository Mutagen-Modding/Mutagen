namespace Mutagen.Bethesda.Fallout76;

public partial class ActionRecord
{
    [Flags]
    public enum MajorFlag
    {
        Restricted = 0x0008_0000
    }

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