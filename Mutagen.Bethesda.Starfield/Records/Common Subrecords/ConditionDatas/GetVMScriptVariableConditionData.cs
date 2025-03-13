namespace Mutagen.Bethesda.Starfield;

public partial class GetVMScriptVariableConditionData : IConditionParameters
{
    string? IConditionParametersGetter.StringParameter1 => FirstParameter;

    string? IConditionParametersGetter.StringParameter2 => SecondParameter;

    string? IConditionParameters.StringParameter1
    {
        get => FirstParameter;
        set => FirstParameter = value;
    }

    string? IConditionParameters.StringParameter2
    {
        get => SecondParameter;
        set => SecondParameter = value;
    }

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetVMScriptVariable;

}

