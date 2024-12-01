using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Skyrim;

public partial class GetEventDataConditionData : IConditionParameters
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

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetEventData;

    object? IConditionParameters.Parameter1
    {
        get => (int)Function << 16 | (int)Member;
        set => (Function, Member) = value is null
            ? (EventFunction.GetIsID, EventMember.None)
            : ((EventFunction)((int)value >> 16), (EventMember)((int)value & 0xFFFF));
    }
    Type? IConditionParametersGetter.Parameter1Type => typeof(int);

    object? IConditionParameters.Parameter2
    {
        get => Record;
        set => Record = value as IFormLink<ISkyrimMajorRecordGetter> ?? throw new ArgumentNullException();
    }
    Type? IConditionParametersGetter.Parameter2Type => typeof(object);

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
