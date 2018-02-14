using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class ColorTypeModule : GenerationModule
    {
        public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            await base.PostFieldLoad(obj, field, node);
            field.CustomData["ColorExtraByte"] = node.GetAttribute<bool>("extraByte", defaultVal: true);
        }
    }
}
