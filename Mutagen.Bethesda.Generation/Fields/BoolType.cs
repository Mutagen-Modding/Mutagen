using System.Xml.Linq;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Generation.Fields;

public class BoolType : Loqui.Generation.BoolType
{
    public RecordType? BoolAsMarker;
    public int ByteLength { get; private set; }
    public int? ImportantByteLength { get; private set; }

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
        ImportantByteLength = node.GetAttribute<int?>(Constants.ImportantByteLength, null);
    }
}