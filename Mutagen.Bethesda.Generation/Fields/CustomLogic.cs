using Loqui.Generation;
using Mutagen.Bethesda.Records;
using Noggog;
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
            var data = this.GetFieldData();
            foreach (var item in node.Elements(XName.Get("RecordType", LoquiGenerator.Namespace)))
            {
                data.TriggeringRecordTypes.Add(new RecordType(item.Value));
            }
        }
    }
}
