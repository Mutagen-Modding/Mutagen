namespace Mutagen.Bethesda.Starfield;

public static class ObjectModProperty
{
    public enum ValueType
    {
        Int = 0,
        Float = 1,
        Bool = 2,
        String = 3,
        FormIdInt = 4,
        Enum = 5,
        FormIdFloat = 6,
    }

    public enum FunctionType
    {
        Float,
        Bool,
        Enum,
        FormID
    }

    public static FunctionType GetFunctionType(ValueType val)
    {
        return val switch
        {
            ValueType.Int => FunctionType.Float,
            ValueType.Float => FunctionType.Float,
            ValueType.Bool => FunctionType.Bool,
            ValueType.String => FunctionType.Float,
            ValueType.FormIdInt => FunctionType.FormID,
            ValueType.Enum => FunctionType.Enum,
            ValueType.FormIdFloat => FunctionType.Float,
            _ => throw new ArgumentOutOfRangeException(nameof(val), val, null)
        };
    }
    
    public enum FloatFunctionType
    {
        Set,
        MultAndAdd,
        Add
    }

    public enum BoolFunctionType
    {
        Set,
        And,
        Or
    }

    public enum EnumFunctionType
    {
        Set
    }

    public enum FormLinkFunctionType
    {
        Set,
        Remove,
        Add
    }
}