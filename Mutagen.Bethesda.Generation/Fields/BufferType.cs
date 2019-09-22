using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class BufferType : ByteArrayType
    {
        public bool Static;

        public override async Task Load(XElement node, bool requireName = true)
        {
            this.Singleton = SingletonLevel.NotNull;
            await base.Load(node, requireName);
            this.IntegrateField = false;
            this.Static = node.GetAttribute<bool>("static");
            this.NotifyingProperty.OnNext(Loqui.NotifyingType.None);
            this.ObjectCentralizedProperty.OnNext(false);
            this.HasBeenSetProperty.OnNext(false);
        }
    }
}
