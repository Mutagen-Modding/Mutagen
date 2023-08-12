using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class FloatStoredAsIntegerOverflowException : Exception
{
    public float? Multiplier { get; }
    public float? Divisor { get; }
    public FloatIntegerType IntegerType { get; }
    
    public FloatStoredAsIntegerOverflowException(
        string? name,
        float? multiplier,
        float? divisor, 
        FloatIntegerType integerType)
        : base(GetMessage(name, multiplier, divisor, integerType))
    {
        Multiplier = multiplier;
        Divisor = divisor;
        IntegerType = integerType;
    }

    private static string GetMessage(
        string? name,
        float? multiplier,
        float? divisor, 
        FloatIntegerType integerType)
    {
        uint max;
        switch (integerType)
        {
            case FloatIntegerType.UInt:
                max = uint.MaxValue;
                break;
            case FloatIntegerType.UShort:
                max = ushort.MaxValue;
                break;
            case FloatIntegerType.Byte:
                max = byte.MaxValue;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(integerType), integerType, null);
        }

        max = FloatBinaryTranslation.ApplyTransformationsToUInt32(max, multiplier: multiplier, divisor: divisor);

        return $"{name ?? "This float"} does not support values smaller than 0, or greater than {max}";
    }
}