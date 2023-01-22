namespace Mutagen.Bethesda.Skyrim;

public partial class GetQuestVariableConditionData : IConditionStringParameter
{
    string? IConditionStringParameterGetter.FirstStringParameter => FirstUnusedStringParameter;

    string? IConditionStringParameterGetter.SecondStringParameter => SecondParameter;

    string? IConditionStringParameter.FirstStringParameter
    {
        get => FirstUnusedStringParameter;
        set => FirstUnusedStringParameter = value;
    }

    string? IConditionStringParameter.SecondStringParameter
    {
        get => SecondParameter;
        set => SecondParameter = value;
    }

}

internal partial class GetQuestVariableConditionDataBinaryOverlay
{
    public string? FirstUnusedStringParameter => ParameterOneString;

    public string? SecondParameter => ParameterTwoString;

}

