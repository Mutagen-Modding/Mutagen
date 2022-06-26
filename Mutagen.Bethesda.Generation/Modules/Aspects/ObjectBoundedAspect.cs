using Loqui.Generation;
using Noggog.StructuredStrings.CSharp;

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
            new (LoquiInterfaceType.Direct, _ => "ObjectBounds", (o, tg, sb) =>
            {
                if (tg is not LoquiType field) throw new ArgumentException("ObjectBounds is not LoquiType", nameof(tg));

                if (!field.Nullable)
                {
                    sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    sb.AppendLine("ObjectBounds? IObjectBoundedOptional.ObjectBounds");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine($"get => this.ObjectBounds;");
                        sb.AppendLine($"set => this.ObjectBounds = value ?? new ObjectBounds();");
                    }
                    sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    sb.AppendLine("IObjectBoundsGetter IObjectBoundedGetter.ObjectBounds => this.ObjectBounds;");
                }
                sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                sb.AppendLine("IObjectBoundsGetter? IObjectBoundedOptionalGetter.ObjectBounds => this.ObjectBounds;");
            })
        };
    }

    public static bool Test(Dictionary<string, TypeGeneration> allFields, bool nullable)
        => allFields.TryGetValue("ObjectBounds", out var field)
           && field is LoquiType loqui
           && loqui.TargetObjectGeneration.Name == "ObjectBounds"
           && loqui.Nullable == nullable;
}