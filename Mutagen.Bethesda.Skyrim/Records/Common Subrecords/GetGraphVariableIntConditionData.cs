namespace Mutagen.Bethesda.Skyrim;

public partial class GetGraphVariableIntConditionData : IConditionStringParameter
{
    string? IConditionStringParameterGetter.FirstStringParameter => FirstParameter;

    string? IConditionStringParameterGetter.SecondStringParameter => SecondUnusedStringParameter;

    string? IConditionStringParameter.FirstStringParameter
    {
        get => FirstParameter;
        set => FirstParameter = value;
    }

    string? IConditionStringParameter.SecondStringParameter
    {
        get => SecondUnusedStringParameter;
        set => SecondUnusedStringParameter = value;
    }

}

internal partial class GetGraphVariableIntConditionDataBinaryOverlay
{
    public string? FirstParameter => ParameterOneString;

    public string? SecondUnusedStringParameter => ParameterTwoString;

}

