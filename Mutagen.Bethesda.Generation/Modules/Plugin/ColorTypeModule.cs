using Loqui.Generation;
using System.Threading.Tasks;
using System.Xml.Linq;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class ColorTypeModule : GenerationModule
{
    public const string BinaryTypeStr = "ColorBinaryType";

    public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
    {
        if (!(field is ColorType)) return;
        await base.PostFieldLoad(obj, field, node);
        field.CustomData[BinaryTypeStr] = node.GetAttribute<ColorBinaryType>("binaryType", defaultVal: ColorBinaryType.Alpha);
    }
}