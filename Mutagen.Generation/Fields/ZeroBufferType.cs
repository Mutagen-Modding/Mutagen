using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Generation
{
    public class ZeroBufferType : NothingType
    {
        public int Length;

        public override void Load(XElement node, bool requireName = true)
        {
            this.Length = node.GetAttribute<int>("length", throwException: true);
            base.Load(node, requireName);
        }
    }
}
