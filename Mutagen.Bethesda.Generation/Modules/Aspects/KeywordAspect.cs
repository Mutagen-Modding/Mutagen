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
        public KeywordAspect()
            : base("Keyword", ApplicabilityTest)
        {
            Interfaces = (o) =>
            {
                var ret = new List<(LoquiInterfaceDefinitionType Type, string Interface)>();
                ret.Add((LoquiInterfaceDefinitionType.IGetter, nameof(IKeywordCommonGetter)));
                ret.Add((LoquiInterfaceDefinitionType.ISetter, nameof(IKeywordCommon)));
                return ret;
            };
        }

        public static bool ApplicabilityTest(ObjectGeneration o)
        {
            return o.Name == "Keyword";
        }
    }
}
