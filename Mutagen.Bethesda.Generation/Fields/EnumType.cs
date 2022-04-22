using System.Xml.Linq;
using Noggog;

namespace Mutagen.Bethesda.Generation.Fields;

public class EnumType : Loqui.Generation.EnumType
{
    public int ByteLength { get; private set; }
    public int? NullableFallbackInt { get; private set; }

    public override async Task Load(XElement node, bool requireName = true)
    {
        await base.Load(node, requireName);
        ByteLength = node.GetAttribute<int>(Constants.ByteLength, 4);
        if (node.TryGetAttribute<int>("nullableBinaryFallback", out var i))
        {
            NullableFallbackInt = i;
        }
    }
}