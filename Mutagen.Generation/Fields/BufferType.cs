using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Generation
{
    public class BufferType : ByteArrayType
    {
        public bool Static;

        public override void Load(XElement node, bool requireName = true)
        {
            this.Nullable = false;
            base.Load(node, requireName);
            this.IntegrateField = false;
            this.Static = node.GetAttribute<bool>("static");
            this.Notifying = NotifyingOption.None;
        }
    }
}
