using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class SpecialParseType : NothingType
    {
        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            this.NotifyingProperty.OnNext((Loqui.NotifyingType.None, true));
            this.HasBeenSetProperty.OnNext((false, true));
        }
    }
}
