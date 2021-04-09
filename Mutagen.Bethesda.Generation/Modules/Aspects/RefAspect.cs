using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class RefAspect : AspectFieldInterfaceDefinition
    {
        public string InterfaceName;
        public string MemberName;
        public string LoquiName;

        public RefAspect(
            string interfaceName,
            string memberName,
            string loquiName)
            : base(interfaceName)
        {
            InterfaceName = interfaceName;
            MemberName = memberName;
            LoquiName = loquiName;

            FieldActions = new()
            {
                (LoquiInterfaceType.Direct, memberName, (o, tg, fg) =>
                {
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine($"I{loquiName}Getter? {interfaceName}Getter.{memberName} => this.{memberName};");
                })
            };
        }

        public override bool Test(ObjectGeneration o, Dictionary<string, TypeGeneration> allFields) => allFields
            .TryGetValue(MemberName, out var field)
            && field is LoquiType loqui
            && loqui.TargetObjectGeneration.Name == LoquiName;
    }
}
