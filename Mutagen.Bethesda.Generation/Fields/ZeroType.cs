using Loqui.Generation;
using System;
using Noggog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class ZeroType : NothingType
    {
        public int Length;

        public override Task Load(XElement node, bool requireName = true)
        {
            this.Length = node.GetAttribute<int>("byteLength", throwException: true);
            return base.Load(node, requireName);
        }

        public override string ToString()
        {
            return "Zeros";
        }
    }
}
