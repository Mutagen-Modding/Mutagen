using Loqui.Generation;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;
namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class ConditionDataModule : GenerationModule
{
    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        await base.GenerateInClass(obj, sb);

        if (obj.Abstract) return;
        if (obj.GetObjectType() != ObjectType.Subrecord) return;
        if (obj.BaseClassName != "ConditionData") return;

        var i = 1;
        foreach (var field in obj.Fields)
        {
            if (i > 2) break;
            if (field.Name.Contains("UnusedStringParameter")) continue;

            sb.AppendLine($"public object? Parameter{i}");
            using (sb.CurlyBrace())
            {
                if (field.Name.Contains("Unused"))
                {
                    sb.AppendLine("get => null;");
                    sb.AppendLine("set");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine();
                    }
                }
                else
                {
                    var name = field.Name;
                    var setterTypeName = field.TypeName(false);
                    sb.AppendLine($"get => {name};");
                    sb.AppendLine($"set => {name} = value as {setterTypeName} ?? throw new ArgumentNullException();");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine();
                    }
                }

            }

            i++;
        }

        if (obj.Fields.Exists(f => !f.Name.Contains("Unused")))
        {
            Console.WriteLine();
            Console.WriteLine(obj.Name);
            foreach (var f in obj.Fields.Where(f => !f.Name.Contains("Unused"))) {
                Console.WriteLine(f.Name);
            }
        }
    }
}
