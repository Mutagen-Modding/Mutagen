using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class ObjectBoundedAspect : AspectFieldInterfaceDefinition
{
    public ObjectBoundedAspect()
        : base(
            "IObjectBounded",
            AspectSubInterfaceDefinition.Factory(
                "IObjectBounded",
                (o, f) => Test(f, nullable: false)),
            AspectSubInterfaceDefinition.Factory(
                "IObjectBoundedOptional",
                (o, f) => Test(f, nullable: true)))
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

    public static bool Test(Dictionary<string, TypeGeneration> allFields, bool nullable)
        => allFields.TryGetValue("ObjectBounds", out var field)
           && field is LoquiType loqui
           && loqui.TargetObjectGeneration.Name == "ObjectBounds"
           && loqui.Nullable == nullable;
}