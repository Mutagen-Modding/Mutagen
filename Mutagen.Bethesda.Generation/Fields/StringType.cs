using System;
using Noggog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class StringType : Loqui.Generation.StringType
    {
        public StringBinaryType BinaryType;

        public override async Task Load(XElement node, bool requireName = true)
        {
            this.BinaryType = node.GetAttribute<StringBinaryType>("binaryType", defaultVal: StringBinaryType.NullTerminate);
            await base.Load(node, requireName);
        }
    }
}
