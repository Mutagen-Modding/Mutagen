using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Generation
{
    public class StringType : Loqui.Generation.StringType
    {
        public bool NullTerminate;

        public override void Load(XElement node, bool requireName = true)
        {
            this.NullTerminate = node.GetAttribute<bool>("nullTerminate", defaultVal: true);
            base.Load(node, requireName);
        }
    }
}
