using System.Xml.Linq;
using Loqui.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation.Fields;

public class ZeroType : NothingType
{
    public int Length;

    public override Task Load(XElement node, bool requireName = true)
    {
        this.Length = node.GetAttribute<int>(Constants.ByteLength, throwException: true);
        return base.Load(node, requireName);
    }

    public override string ToString()
    {
        return "Zeros";
    }
}