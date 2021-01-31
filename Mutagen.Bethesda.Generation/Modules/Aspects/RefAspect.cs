using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class RefAspect : AspectInterfaceDefinition
    {
        public string InterfaceName;
        public string MemberName;
        public string LoquiName;

        public RefAspect(
            string aspectName,
            string interfaceName,
            string memberName,
            string loquiName)
            : base(aspectName, null!)
        {
            InterfaceName = interfaceName;
            MemberName = memberName;
            LoquiName = loquiName;
            Test = ApplicabilityTest;
            Interfaces = (o) =>
            {
                var ret = new List<(LoquiInterfaceDefinitionType Type, string Interface)>();
                ret.Add((LoquiInterfaceDefinitionType.IGetter, $"{interfaceName}Getter"));
                ret.Add((LoquiInterfaceDefinitionType.ISetter, interfaceName));
                return ret;
            };
            FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, Loqui.FileGeneration> Actions)>()
            {
                (LoquiInterfaceType.Direct, memberName, (o, fg) =>
                {
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine($"I{loquiName}Getter? {interfaceName}Getter.{memberName} => this.{memberName};");
                })
            };
        }

        public bool ApplicabilityTest(ObjectGeneration o)
        {
            if (o.Fields.FirstOrDefault(x => x.Name == MemberName) is not LoquiType loqui) return false;
            return loqui.TargetObjectGeneration.Name == LoquiName;
        }
    }
}
