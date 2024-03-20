namespace Mutagen.Bethesda.Starfield;

public partial class HasMagicEffectOrSpellKeywordConditionData : IConditionStringParameter
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

    Condition.Function IConditionDataGetter.Function => Condition.Function.HasMagicEffectOrSpellKeyword;

}

