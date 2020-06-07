using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class CustomLogic : NothingType
    {
        public bool IsRecordType = false;

        public override string ToString()
        {
            return "Custom";
        }

        public override async Task Load(XElement node, bool requireName = true)
        {
            await base.Load(node, requireName);
            IsRecordType = node.GetAttribute<bool>("isUntypedRecordType", false);
            var data = this.TryCreateFieldData();
            foreach (var item in node.Elements(XName.Get("RecordType", LoquiGenerator.Namespace)))
            {
                data.TriggeringRecordTypes.Add(new RecordType(item.Value));
            }
        }
    }
}
