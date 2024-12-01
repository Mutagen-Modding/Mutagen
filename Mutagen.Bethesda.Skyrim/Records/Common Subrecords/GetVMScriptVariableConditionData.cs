namespace Mutagen.Bethesda.Skyrim;

public partial class GetVMScriptVariableConditionData : IConditionParameters
{
    string? IConditionParametersGetter.StringParameter1 => FirstUnusedStringParameter;

    string? IConditionParametersGetter.StringParameter2 => VariableName;

    string? IConditionParameters.StringParameter1
    {
        get => FirstUnusedStringParameter;
        set => FirstUnusedStringParameter = value;
    }

    string? IConditionParameters.StringParameter2
    {
        get => VariableName;
        set => VariableName = value;
    }

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetVMScriptVariable;
}

