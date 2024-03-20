namespace Mutagen.Bethesda.Starfield;

public partial class GetEventDataConditionData : IConditionStringParameter
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

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetEventData;
    
    public enum EventMember : ushort
    {
        None = 0x0,
        Form = 0x3146,
        Global = 0x3147,
        Identifier = 0x3149,
        Keyword1 = 0x314b,
        Keyword2 = 0x324b,
        Keyword3 = 0x334b,
        OldLocation = 0x314c,
        NewLocation = 0x324c,
        CreatedObject = 0x314f,
        Player1 = 0x3150,
        Player2 = 0x3250,
        Quest1 = 0x3251,
        Reference1 = 0x3152,
        Reference2 = 0x3252,
        Reference3 = 0x3352,
        Value1 = 0x3156,
        Value2 = 0x3256,
        All = 0xffff,
    }
    
    public enum EventFunction : ushort
    {
        GetIsID,
        IsInList,
        GetValue,
        HasKeyword,
        GetItemValue
    }
}
