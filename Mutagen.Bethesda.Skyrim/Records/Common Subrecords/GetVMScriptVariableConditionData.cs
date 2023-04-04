namespace Mutagen.Bethesda.Skyrim;

public partial class GetVMScriptVariableConditionData : IConditionStringParameter
{
    string? IConditionStringParameterGetter.FirstStringParameter => FirstUnusedStringParameter;

    string? IConditionStringParameterGetter.SecondStringParameter => VariableName;

    string? IConditionStringParameter.FirstStringParameter
    {
        get => FirstUnusedStringParameter;
        set => FirstUnusedStringParameter = value;
    }

    string? IConditionStringParameter.SecondStringParameter
    {
        get => VariableName;
        set => VariableName = value;
    }

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetVMScriptVariable;
}

