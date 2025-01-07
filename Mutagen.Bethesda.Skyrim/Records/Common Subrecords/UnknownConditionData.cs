namespace Mutagen.Bethesda.Skyrim;

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

    object? IConditionParameters.Parameter1
    {
        get => null;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter1Type => null;

    object? IConditionParameters.Parameter2
    {
        get => null;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter2Type => null;
}