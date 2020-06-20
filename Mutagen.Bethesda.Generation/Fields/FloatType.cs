using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class FloatType : Loqui.Generation.FloatType
    {
        public FloatIntegerType? IntegerType;
        public double Multiplier;

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            var data = this.TryCreateFieldData();
            this.Multiplier = node.GetAttribute("multiplier", 1d);
            this.IntegerType = node.GetAttribute("integerType", default(FloatIntegerType?));
            if (this.IntegerType.HasValue)
            {
                switch (this.IntegerType)
                {
                    case FloatIntegerType.UInt:
                        this.Min = "0";
                        this.Max = $"{uint.MaxValue * Multiplier}";
                        data.Length = 4;
                        break;
                    case FloatIntegerType.UShort:
                        this.Min = "0";
                        this.Max = $"{ushort.MaxValue * Multiplier}";
                        data.Length = 2;
                        break;
                    case FloatIntegerType.Byte:
                        this.Min = "0";
                        this.Max = $"{byte.MaxValue * Multiplier}";
                        data.Length = 1;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
