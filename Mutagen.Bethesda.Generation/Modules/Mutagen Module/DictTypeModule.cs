using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class DictTypeModule : GenerationModule
    {
        public override async  Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            if (!(field is DictType list)) return;
            if (node.TryGetAttribute("lengthLength", out int len))
            {
                list.CustomData["lengthLength"] = len;
            }
        }
    }
}
