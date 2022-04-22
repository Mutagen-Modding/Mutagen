using System.Xml.Linq;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Generation.Fields;

public class PercentType : Loqui.Generation.PercentType
{
    public FloatIntegerType IntegerType;

    public override async Task Load(XElement node, bool requireName = true)
    {
        await base.Load(node, requireName);
        this.IntegerType = node.GetAttribute("integerType", FloatIntegerType.UInt);
    }
}