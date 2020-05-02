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
        public FloatBinaryType StorageType;
        public FloatIntegerType IntegerType;
        public ushort IntegerDivisor;

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            var data = this.TryCreateFieldData();
            this.StorageType = node.GetAttribute("binaryType", FloatBinaryType.Normal);
            this.IntegerDivisor = node.GetAttribute("integerDivisor", (ushort)0);
            this.IntegerType = node.GetAttribute("integerType", FloatIntegerType.UInt);
            if (this.StorageType == FloatBinaryType.Integer)
            {
                if (this.IntegerDivisor <= 0)
                {
                    throw new ArgumentException("Need to specify integer divisor with Integer binary type.");
                }
                switch (this.IntegerType)
                {
                    case FloatIntegerType.UInt:
                        this.Min = "0";
                        this.Max = $"{uint.MaxValue / IntegerDivisor * 1f}";
                        data.Length = 4;
                        break;
                    case FloatIntegerType.UShort:
                        this.Min = "0";
                        this.Max = $"{ushort.MaxValue / IntegerDivisor * 1f}";
                        data.Length = 2;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
