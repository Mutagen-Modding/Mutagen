using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class RefAspect : AspectFieldInterfaceDefinition
{
    public string InterfaceNickName;
    public string MemberName;
    public string LoquiName;

    public RefAspect(
        string interfaceNickName,
        string memberName,
        string loquiName)
        : base(interfaceNickName, AspectSubInterfaceDefinition.Factory(interfaceNickName))
    {
        InterfaceNickName = interfaceNickName;
        MemberName = memberName;
        LoquiName = loquiName;

        FieldActions = new()
        {
            new (LoquiInterfaceType.Direct, memberName, (o, tg, fg) =>
            {
                fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                fg.AppendLine($"I{loquiName}Getter? {interfaceNickName}Getter.{memberName} => this.{memberName};");
            })
        };
    }

    public override bool Test(ObjectGeneration o, Dictionary<string, TypeGeneration> allFields)
        => allFields.TryGetValue(MemberName, out var field)
           && field is LoquiType loqui
           && loqui.TargetObjectGeneration.Name == LoquiName;
}