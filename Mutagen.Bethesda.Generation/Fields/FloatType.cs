using System.Globalization;
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
    public string MultiplierString => HasMultiplier ? GetValueString(Multiplier) : "null";
    public string MultiplierFloatString => HasMultiplier ? GetValueString((float)Multiplier) : "null";
    public string DivisorString => HasDivisor ? GetValueString(Divisor) : "null";
    public string DivisorFloatString => HasDivisor ? GetValueString((float)Divisor) : "null";

    public override async Task Load(XElement node, bool requireName = true)
    {
        await base.Load(node, requireName);
        var data = this.GetFieldData();
        Multiplier = node.GetAttribute("multiplier", 1d, culture: CultureInfo.InvariantCulture);
        if (node.TryGetAttribute("divisor", out double div, culture: CultureInfo.InvariantCulture))
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
                    Max = GetValueString(uint.MaxValue * Multiplier);
                    data.Length = 4;
                    break;
                case FloatIntegerType.UShort:
                    Min = "0";
                    Max = GetValueString(ushort.MaxValue * Multiplier);
                    data.Length = 2;
                    break;
                case FloatIntegerType.Byte:
                    Min = "0";
                    Max = GetValueString(byte.MaxValue * Multiplier);
                    data.Length = 1;
                    break;
                case FloatIntegerType.ByteHundred:
                    Min = "0";
                    Max = GetValueString(100 * Multiplier);
                    data.Length = 1;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    private static string GetValueString(float value) => value.ToString(CultureInfo.InvariantCulture) + "f";
    private static string GetValueString(double value) => value.ToString(CultureInfo.InvariantCulture) + "f";
}