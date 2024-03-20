namespace Mutagen.Bethesda.Starfield;

public partial class GetVMScriptVariableConditionData : IConditionStringParameter
{
    string? IConditionStringParameterGetter.FirstStringParameter => FirstParameter;

    string? IConditionStringParameterGetter.SecondStringParameter => SecondParameter;

    string? IConditionStringParameter.FirstStringParameter
    {
        get => FirstParameter;
        set => FirstParameter = value;
    }

    string? IConditionStringParameter.SecondStringParameter
    {
        get => SecondParameter;
        set => SecondParameter = value;
    }

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetVMScriptVariable;

}

