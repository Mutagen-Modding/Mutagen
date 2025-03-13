namespace Mutagen.Bethesda.Starfield;

partial class UnknownConditionData : IConditionParameters
{
    string? IConditionParametersGetter.StringParameter1 => FirstUnusedStringParameter;

    string? IConditionParametersGetter.StringParameter2 => SecondUnusedStringParameter;

    string? IConditionParameters.StringParameter1
    {
        get => FirstUnusedStringParameter;
        set => FirstUnusedStringParameter = value;
    }

    string? IConditionParameters.StringParameter2
    {
        get => SecondUnusedStringParameter;
        set => SecondUnusedStringParameter = value;
    }
}