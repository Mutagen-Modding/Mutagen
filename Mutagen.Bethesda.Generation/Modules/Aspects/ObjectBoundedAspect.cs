using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class ObjectBoundedAspect : AspectInterfaceDefinition
    {
        public ObjectBoundedAspect()
            : base("IObjectBounded", ApplicabilityTest)
        {
            Interfaces = (o) =>
            {
                var field = o.IterateFields(includeBaseClass: true).FirstOrDefault(x => x.Name == "ObjectBounds") as LoquiType;

                var ret = new List<(LoquiInterfaceDefinitionType Type, string Interface)>();
                ret.Add((LoquiInterfaceDefinitionType.IGetter, $"IObjectBoundedOptionalGetter"));
                ret.Add((LoquiInterfaceDefinitionType.ISetter, $"IObjectBoundedOptional"));
                if (!field.Nullable)
                {
                    ret.Add((LoquiInterfaceDefinitionType.IGetter, $"IObjectBoundedGetter"));
                    ret.Add((LoquiInterfaceDefinitionType.ISetter, $"IObjectBounded"));
                }
                return ret;
            };
            FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, Loqui.FileGeneration> Actions)>()
            {
                (LoquiInterfaceType.Direct, "ObjectBounds", (o, fg) =>
                {
                    var field = o.IterateFields(includeBaseClass: true).FirstOrDefault(x => x.Name == "ObjectBounds") as LoquiType;

                    if (!field.Nullable)
                    {
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine("ObjectBounds? IObjectBoundedOptional.ObjectBounds");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"get => this.ObjectBounds;");
                            fg.AppendLine($"set => this.ObjectBounds = value ?? new ObjectBounds();");
                        }
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine("IObjectBoundsGetter IObjectBoundedGetter.ObjectBounds => this.ObjectBounds;");
                    }
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine("IObjectBoundsGetter? IObjectBoundedOptionalGetter.ObjectBounds => this.ObjectBounds;");
                })
            };
            IdentifyFields = (o) => o.Fields
                .Where(x => x.Name == "ObjectBounds")
                .OfType<LoquiType>()
                .Where(x => x.TargetObjectGeneration.Name == "ObjectBounds");
        }

        public static bool ApplicabilityTest(ObjectGeneration o)
        {
            if (o.Fields.FirstOrDefault(x => x.Name == "ObjectBounds") is not LoquiType loqui) return false;
            return loqui.TargetObjectGeneration.Name == "ObjectBounds";
        }
    }
}
