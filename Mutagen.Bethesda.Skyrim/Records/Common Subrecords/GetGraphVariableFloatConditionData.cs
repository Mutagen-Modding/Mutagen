namespace Mutagen.Bethesda.Skyrim;

public partial class GetGraphVariableFloatConditionData : IConditionStringParameter
{
    string? IConditionStringParameterGetter.FirstStringParameter => GraphVariable;

    string? IConditionStringParameterGetter.SecondStringParameter => SecondUnusedStringParameter;

    string? IConditionStringParameter.FirstStringParameter
    {
        get => GraphVariable;
        set => GraphVariable = value;
    }

    string? IConditionStringParameter.SecondStringParameter
    {
        get => SecondUnusedStringParameter;
        set => SecondUnusedStringParameter = value;
    }

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetGraphVariableFloat;
}

