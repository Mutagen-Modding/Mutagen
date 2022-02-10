using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Generation.Fields;

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