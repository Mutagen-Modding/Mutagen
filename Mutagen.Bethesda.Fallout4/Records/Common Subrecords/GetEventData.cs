namespace Mutagen.Bethesda.Fallout4;

public partial class GetEventData
{
    public enum EventMember
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
