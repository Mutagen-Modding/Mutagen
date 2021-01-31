using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class HasIconsAspect : AspectInterfaceDefinition
    {
        public HasIconsAspect()
            : base("HasIcons", ApplicabilityTest)
        {
            Interfaces = (o) =>
            {
                var ret = new List<(LoquiInterfaceDefinitionType Type, string Interface)>();
                ret.Add((LoquiInterfaceDefinitionType.IGetter, "IHasIconsGetter"));
                ret.Add((LoquiInterfaceDefinitionType.ISetter, "IHasIcons"));
                return ret;
            };
            FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, Loqui.FileGeneration> Actions)>()
            {
                (LoquiInterfaceType.Direct, "Icons", (o, fg) =>
                {
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine("IIconsGetter? IHasIconsGetter.Icons => this.Icons;");
                })
            };
        }

        public static bool ApplicabilityTest(ObjectGeneration o)
        {
            if (o.Fields.FirstOrDefault(x => x.Name == "Icons") is not LoquiType loqui) return false;
            return loqui.TargetObjectGeneration.Name == "Icons";
        }
    }
}
