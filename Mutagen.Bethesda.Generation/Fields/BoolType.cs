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
        public int ByteLength { get; private set; }

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            if (node.TryGetAttribute("boolAsMarker", out var boolAsMarker))
            {
                BoolAsMarker = new RecordType(boolAsMarker.Value);
                this.NullableProperty.OnNext((false, true));
            }
            this.GetFieldData().RecordType = BoolAsMarker;
            ByteLength = node.GetAttribute<int>(Constants.ByteLength, 1);
        }
    }
}
