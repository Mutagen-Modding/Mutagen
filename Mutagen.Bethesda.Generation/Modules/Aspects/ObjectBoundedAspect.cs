using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class ObjectBoundedAspect : AspectFieldInterfaceDefinition
{
    public ObjectBoundedAspect()
        : base("IObjectBounded")
    {
        FieldActions = new()
        {
            new (LoquiInterfaceType.Direct, "ObjectBounds", (o, tg, fg) =>
            {
                if (tg is not LoquiType field) throw new ArgumentException("ObjectBounds is not LoquiType", nameof(tg));

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
    }

    public override bool Test(ObjectGeneration o, Dictionary<string, TypeGeneration> allFields)
        => allFields.TryGetValue("ObjectBounds", out var field)
           && field is LoquiType loqui
           && loqui.TargetObjectGeneration.Name == "ObjectBounds";

    public override List<AspectInterfaceData> Interfaces(ObjectGeneration obj)
    {
        var field = obj.IterateFields(includeBaseClass: true).OfType<LoquiType>().Single(x => x.Name == "ObjectBounds");

        var list = new List<AspectInterfaceData>();
        AddInterfaces(list, "IObjectBoundedOptional");

        if (!field.Nullable)
        {
            AddInterfaces(list, "IObjectBounded");
        }

        return list;
    }
}