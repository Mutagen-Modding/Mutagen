using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Skyrim;

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

    public object? Parameter1
    {
        get => (int)Function << 16 | (int)Member;
        set => (Function, Member) = value is null
            ? (EventFunction.GetIsID, EventMember.None)
            : ((EventFunction)((int)value >> 16), (EventMember)((int)value & 0xFFFF));
    }
    public Type? Parameter1Type => typeof(int);

    public object? Parameter2
    {
        get => Record;
        set => Record = value as IFormLink<ISkyrimMajorRecordGetter> ?? throw new ArgumentNullException();
    }
    public Type? Parameter2Type => typeof(object);

    public enum EventMember : ushort
    {
        None = 0x0000,
        CreatedObject = 0x314F,
        OldLocation = 0x314C,
        NewLocation = 0x324C,
        Keyword = 0x314B,
        Form = 0x3146,
        Value1 = 0x3156,
        Value2 = 0x3256
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
