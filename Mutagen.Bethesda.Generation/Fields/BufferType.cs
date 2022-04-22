using System.Xml.Linq;
using Loqui.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation.Fields;

public class BufferType : ByteArrayType
{
    public bool Static;

    public override CopyLevel CopyLevel => CopyLevel.None;

    public override async Task Load(XElement node, bool requireName = true)
    {
        this.NullableProperty.OnNext((false, true));
        await base.Load(node, requireName);
        this.IntegrateField = false;
        this.Static = node.GetAttribute<bool>("static");
        this.NotifyingProperty.OnNext((Loqui.NotifyingType.None, true));
    }
}