using System.Xml.Linq;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Generation.Fields;

public class FloatType : Loqui.Generation.FloatType
{
    public FloatIntegerType? IntegerType;
    public double Multiplier = 1;
    public double Divisor = 1;
    public bool IsRotation;

    public bool HasMultiplier => !Multiplier.EqualsWithin(1);
    public bool HasDivisor => !Divisor.EqualsWithin(1);
    public bool HasTransformations => HasMultiplier || HasDivisor;
    public string MultiplierString => HasMultiplier ? $"{Multiplier}f" : "null";
    public string DivisorString => HasDivisor ? $"{Divisor}f" : "null";

    public override async Task Load(XElement node, bool requireName = true)
    {
        await base.Load(node, requireName);
        var data = this.GetFieldData();
        Multiplier = node.GetAttribute("multiplier", 1d);
        if (node.TryGetAttribute("divisor", out double div))
        {
            Divisor = div;
        }
        IntegerType = node.GetAttribute("integerType", default(FloatIntegerType?));
        IsRotation = node.GetAttribute("isRotation", false);
        if (IsRotation)
        {
            // Passthrough tests require 4 digits precision 
            // Intentionally not using Math.PI
            Multiplier = 57.2958f; //180f / Math.PI; 
        }
        if (IntegerType.HasValue)
        {
            switch (IntegerType)
            {
                case FloatIntegerType.UInt:
                    Min = "0";
                    Max = $"{uint.MaxValue * Multiplier}f";
                    data.Length = 4;
                    break;
                case FloatIntegerType.UShort:
                    Min = "0";
                    Max = $"{ushort.MaxValue * Multiplier}f";
                    data.Length = 2;
                    break;
                case FloatIntegerType.Byte:
                    Min = "0";
                    Max = $"{byte.MaxValue * Multiplier}f";
                    data.Length = 1;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}