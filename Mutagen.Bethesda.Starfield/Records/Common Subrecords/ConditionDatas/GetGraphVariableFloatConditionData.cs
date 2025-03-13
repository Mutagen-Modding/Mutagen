namespace Mutagen.Bethesda.Starfield;

public partial class GetGraphVariableFloatConditionData : IConditionParameters
{
    string? IConditionParametersGetter.StringParameter1 => FirstParameter;

    string? IConditionParametersGetter.StringParameter2 => SecondUnusedStringParameter;

    string? IConditionParameters.StringParameter1
    {
        get => FirstParameter;
        set => FirstParameter = value;
    }

    string? IConditionParameters.StringParameter2
    {
        get => SecondUnusedStringParameter;
        set => SecondUnusedStringParameter = value;
    }

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetGraphVariableFloat;

}

