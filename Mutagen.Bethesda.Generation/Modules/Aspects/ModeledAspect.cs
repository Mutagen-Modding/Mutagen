using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class ModeledAspect : AspectInterfaceDefinition
    {
        public ModeledAspect()
            : base("Modeled", ApplicabilityTest)
        {
            Interfaces = (o) =>
            {
                var ret = new List<(LoquiInterfaceDefinitionType Type, string Interface)>();
                ret.Add((LoquiInterfaceDefinitionType.IGetter, "IModeledGetter"));
                ret.Add((LoquiInterfaceDefinitionType.ISetter, "IModeled"));
                return ret;
            };
            FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, Loqui.FileGeneration> Actions)>()
            {
                (LoquiInterfaceType.Direct, "Model", (o, fg) =>
                {
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine("IModelGetter? IModeledGetter.Model => this.Model;");
                })
            };
        }

        public static bool ApplicabilityTest(ObjectGeneration o)
        {
            if (o.Fields.FirstOrDefault(x => x.Name == "Model") is not LoquiType loqui) return false;
            return loqui.TargetObjectGeneration.Name == "Model";
        }
    }
}
