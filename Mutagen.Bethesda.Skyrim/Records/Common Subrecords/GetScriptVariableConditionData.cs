namespace Mutagen.Bethesda.Skyrim;

[Obsolete("This function is deprecated and will not work in current versions of the Creation Kit. Use GetVMScriptVariable instead.")]
public partial class GetScriptVariableConditionData : IConditionParameters
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

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetScriptVariable;
}

