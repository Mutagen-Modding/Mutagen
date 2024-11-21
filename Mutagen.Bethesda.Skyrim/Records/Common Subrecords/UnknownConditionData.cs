namespace Mutagen.Bethesda.Skyrim;

partial class UnknownConditionData : IConditionStringParameter
{
    string? IConditionStringParameterGetter.FirstStringParameter => FirstUnusedStringParameter;

    string? IConditionStringParameterGetter.SecondStringParameter => SecondUnusedStringParameter;

    string? IConditionStringParameter.FirstStringParameter
    {
        get => FirstUnusedStringParameter;
        set => FirstUnusedStringParameter = value;
    }

    string? IConditionStringParameter.SecondStringParameter
    {
        get => SecondUnusedStringParameter;
        set => SecondUnusedStringParameter = value;
    }

    public override object? Parameter1
    {
        get => null;
        set
        {

        }
    }
    public override Type? Parameter1Type => null;

    public override object? Parameter2
    {
        get => null;
        set
        {

        }
    }
    public override Type? Parameter2Type => null;
}