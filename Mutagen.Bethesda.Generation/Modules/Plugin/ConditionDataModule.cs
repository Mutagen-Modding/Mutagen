using Loqui.Generation;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;
namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class ConditionDataModule : GenerationModule
{
    public override Task PreLoad(ObjectGeneration obj)
    {
        var data = obj.GetObjectData();
        data.GenerateConditionData = obj.Node.GetAttribute("generateConditionData", defaultVal: true);
        return base.PreLoad(obj);
    }
    
    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        await base.GenerateInClass(obj, sb);

        if (obj.Abstract) return;
        if (obj.GetObjectType() != ObjectType.Subrecord) return;
        if (obj.BaseClass is null) return;
        if (obj.BaseClassName != "ConditionData") return;
        if (obj.GetObjectData().GenerateConditionData == false) return;

        var i = 1;
        foreach (var field in obj.Fields)
        {
            if (i > 2) break;
            if (field.Name.Contains("UnusedStringParameter")) continue;
            var isUnusedParameter = field.Name.Contains("Unused");

            // Parameter property
            sb.AppendLine($"public override object? Parameter{i}");
            using (sb.CurlyBrace())
            {
                if (isUnusedParameter)
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
                    sb.AppendLine($"set => {name} = (value is {setterTypeName} v ? v : throw new ArgumentException());");
                }

            }

            // ParameterType property
            sb.AppendLine($"public override Type? Parameter{i}Type");
            using (sb.CurlyBrace())
            {
                if (isUnusedParameter)
                {
                    sb.AppendLine("get => null;");
                }
                else
                {
                    var name = field.TypeName(true);
                    sb.AppendLine($"get => typeof({name});");
                }
            }

            i++;
        }
    }
}
