using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class FloatType : Loqui.Generation.FloatType
    {
        public FloatIntegerType? IntegerType;
        public double Multiplier;
        public bool IsRotation;

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            var data = this.GetFieldData();
            this.Multiplier = node.GetAttribute("multiplier", 1d);
            if (node.TryGetAttribute("divisor", out double div))
            {
                this.Multiplier *= 1 / div;
            }
            this.IntegerType = node.GetAttribute("integerType", default(FloatIntegerType?));
            this.IsRotation = node.GetAttribute("isRotation", false);
            if (IsRotation)
            {
                // Passthrough tests require 4 digits precision
                this.Multiplier = 57.2958f; //180f / Math.PI;
            }
            if (this.IntegerType.HasValue)
            {
                switch (this.IntegerType)
                {
                    case FloatIntegerType.UInt:
                        this.Min = "0";
                        this.Max = $"{uint.MaxValue * Multiplier}f";
                        data.Length = 4;
                        break;
                    case FloatIntegerType.UShort:
                        this.Min = "0";
                        this.Max = $"{ushort.MaxValue * Multiplier}f";
                        data.Length = 2;
                        break;
                    case FloatIntegerType.Byte:
                        this.Min = "0";
                        this.Max = $"{byte.MaxValue * Multiplier}f";
                        data.Length = 1;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
