using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class SpecialEditionModule : GenerationModule
    {
        public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            var isSE = node.GetAttribute("isSE", false);
            if (isSE)
            {
                field.Enabled = false;
            }
        }
    }
}
