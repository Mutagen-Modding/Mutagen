using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using System.Xml.Linq;
using Mutagen.Bethesda.Generation.Fields;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class DataTypeModule : GenerationModule
{
    public override async Task LoadWrapup(ObjectGeneration obj)
    {
        await base.LoadWrapup(obj);
        foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
        {
            if (!(field is DataType dataType)) continue;
            XElement elem = new XElement("Enum");
            elem.Add(new XAttribute(Loqui.Generation.Constants.NAME, dataType.StateName));
            elem.Add(new XAttribute(Loqui.Generation.Constants.ENUM_NAME, $"{obj.ObjectName}.{dataType.EnumName}"));
            elem.Add(new XAttribute("binary", nameof(BinaryGenerationType.NoGeneration)));
            elem.Add(new XAttribute(Loqui.Generation.Constants.NULLABLE, "false"));
            await obj.LoadField(elem, requireName: true, add: true);
        }
        obj.GetObjectData().DataTypeModuleComplete.SetResult();
    }

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        await base.GenerateInClass(obj, sb);

        List<string> enumTypes;
        int breaks;
        foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
        {
            if (!(field is DataType dataType)) continue;
            enumTypes = new List<string>();
            breaks = 0;
            int ranges = 0;
            foreach (var node in dataType.Node.Element(XName.Get(Loqui.Generation.Constants.FIELDS, LoquiGenerator.Namespace)).Elements())
            {
                switch (node.Name.LocalName)
                {
                    case DataType.BREAK:
                        enumTypes.Add("Break" + breaks++);
                        break;
                    case DataType.RANGE:
                        enumTypes.Add("Range" + ranges++);
                        break;
                    default:
                        break;
                }
            }
            sb.AppendLine("[Flags]");
            sb.AppendLine($"public enum {dataType.EnumName}");
            using (sb.CurlyBrace())
            {
                using (var comma = sb.CommaCollection())
                {
                    var term = 1;
                    for (int i = 0; i < enumTypes.Count; i++)
                    {
                        comma.Add($"{enumTypes[i]} = {term}");
                        term *= 2;
                    }
                }
            }
        }
    }
}