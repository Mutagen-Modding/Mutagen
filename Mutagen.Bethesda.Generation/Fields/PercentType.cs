using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class PercentType : Loqui.Generation.PercentType
    {
        public FloatIntegerType IntegerType;

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            this.IntegerType = node.GetAttribute("integerType", FloatIntegerType.UInt);
        }
    }
}
