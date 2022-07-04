using Loqui.Generation;
using System.Xml.Linq;
using Noggog;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class CircularHandlingModule : GenerationModule
{
    public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
    {
        field.GetFieldData().Circular = node.GetAttribute<bool>("circular", defaultVal: false);
    }
}