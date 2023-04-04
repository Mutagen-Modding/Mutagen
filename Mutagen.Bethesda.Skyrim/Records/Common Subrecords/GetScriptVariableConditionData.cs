namespace Mutagen.Bethesda.Skyrim;

[Obsolete("This function is deprecated and will not work in current versions of the Creation Kit. Use GetVMScriptVariable instead.")]
public partial class GetScriptVariableConditionData : IConditionStringParameter
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

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetScriptVariable;
}

