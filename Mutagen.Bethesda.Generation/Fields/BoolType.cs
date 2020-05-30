using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class BoolType : Loqui.Generation.BoolType
    {
        public RecordType? BoolAsMarker;

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            if (node.TryGetAttribute("boolAsMarker", out var boolAsMarker))
            {
                BoolAsMarker = new RecordType(boolAsMarker.Value);
                this.HasBeenSetProperty.OnNext((false, true));
            }
            this.TryCreateFieldData().RecordType = BoolAsMarker;
        }
    }
}
