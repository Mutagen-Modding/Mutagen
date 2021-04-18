using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class KeywordAspect : AspectInterfaceDefinition
    {
        public KeywordAspect() : base("IKeywordCommon") { }

        public override bool Test(ObjectGeneration o, Dictionary<string, TypeGeneration> allFields) => o.Name == "Keyword";
    }
}
