using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Noggog;

namespace Mutagen.Bethesda.Generation
{
    public class ListTypeModule : GenerationModule
    {
        public override async  Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            if (!(field is ListType list)) return;
            if (node.TryGetAttribute("lengthLength", out int len))
            {
                list.CustomData["lengthLength"] = len;
            }
        }
    }
}
