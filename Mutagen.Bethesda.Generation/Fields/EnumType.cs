using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class EnumType : Loqui.Generation.EnumType
    {
        public int ByteLength;

        public override void Load(XElement node, bool requireName = true)
        {
            base.Load(node, requireName);
            ByteLength = node.GetAttribute<int>("byteLength", 4);
        }
    }
}
