using Loqui;
using Loqui.Generation;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Plugin
{
    public class MajorRecordLinkEqualityModule : GenerationModule
    {
        public override async Task PostLoad(ObjectGeneration obj)
        {
            await base.PostLoad(obj);
            if (!(await obj.IsMajorRecord())) return;
            obj.GenerateEquals = false;
        }

        public override Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            return Generate(obj, fg);
        }

        public static async Task Generate(ObjectGeneration obj, FileGeneration fg)
        {
            if (!(await obj.IsMajorRecord())) return;
            using (new RegionWrapper(fg, "Equals and Hash"))
            {
                fg.AppendLine("public override bool Equals(object? obj)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("if (obj is IFormLinkGetter formLink)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return formLink.Equals(this);");
                    }
                    fg.AppendLine($"if (obj is not {obj.Interface(getter: true, internalInterface: true)} rhs) return false;");
                    fg.AppendLine($"return {obj.CommonClassInstance("this", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Equals(this, rhs, crystal: null);");
                }
                fg.AppendLine();

                fg.AppendLine($"public bool Equals({obj.Interface(getter: true, internalInterface: true)}? obj)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"return {obj.CommonClassInstance("this", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Equals(this, obj, crystal: null);");
                }
                fg.AppendLine();

                fg.AppendLine($"public override int GetHashCode() => {obj.CommonClassInstance("this", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.GetHashCode(this);");
                fg.AppendLine();
            }
        }
    }
}
