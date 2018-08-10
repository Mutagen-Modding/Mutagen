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
            this.NotifyingProperty.Item = Loqui.NotifyingType.None;
            this.ObjectCentralizedProperty.Item = false;
            this.HasBeenSetProperty.Item = false;
        }
    }
}
