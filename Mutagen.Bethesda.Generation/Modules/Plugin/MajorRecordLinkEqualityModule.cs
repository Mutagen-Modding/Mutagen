using Loqui.Generation;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class MajorRecordLinkEqualityModule : GenerationModule
{
    public override async Task PostLoad(ObjectGeneration obj)
    {
        await base.PostLoad(obj);
        if (!(await obj.IsMajorRecord())) return;
        obj.GenerateEquals = false;
    }

    public override Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        return Generate(obj, sb);
    }

    public static async Task Generate(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (!(await obj.IsMajorRecord())) return;
        using (sb.Region("Equals and Hash"))
        {
            sb.AppendLine("public override bool Equals(object? obj)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine("if (obj is IFormLinkGetter formLink)");
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return formLink.Equals(this);");
                }
                sb.AppendLine($"if (obj is not {obj.Interface(getter: true, internalInterface: true)} rhs) return false;");
                sb.AppendLine($"return {obj.CommonClassInstance("this", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Equals(this, rhs, crystal: null);");
            }
            sb.AppendLine();

            sb.AppendLine($"public bool Equals({obj.Interface(getter: true, internalInterface: true)}? obj)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"return {obj.CommonClassInstance("this", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Equals(this, obj, crystal: null);");
            }
            sb.AppendLine();

            sb.AppendLine($"public override int GetHashCode() => {obj.CommonClassInstance("this", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.GetHashCode(this);");
            sb.AppendLine();
        }
    }
}