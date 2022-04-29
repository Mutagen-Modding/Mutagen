using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Noggog;
using System.Xml.Linq;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class VersioningModule : GenerationModule
{
    public const string VersioningEnumName = "VersioningBreaks";
    public const string VersioningFieldName = "Versioning";

    public override async Task LoadWrapup(ObjectGeneration obj)
    {
        await base.LoadWrapup(obj);
        if (obj.Fields.Any(f => f is BreakType))
        {
            XElement elem = new XElement("Enum");
            elem.Add(new XAttribute(Loqui.Generation.Constants.NAME, VersioningFieldName));
            elem.Add(new XAttribute(Loqui.Generation.Constants.ENUM_NAME, $"{obj.ObjectName}.{VersioningEnumName}"));
            elem.Add(new XAttribute("binary", nameof(BinaryGenerationType.NoGeneration)));
            elem.Add(new XAttribute(Loqui.Generation.Constants.NULLABLE, "false"));
            var gen = await obj.LoadField(elem, requireName: true, add: false);
            if (gen.Failed) throw new ArgumentException();
            gen.Value.SetObjectGeneration(obj, setDefaults: true);
            gen.Value.GetFieldData().Binary = BinaryGenerationType.NoGeneration;
            gen.Value.GetFieldData().BinaryOverlay = BinaryGenerationType.NoGeneration;
            obj.Fields.Insert(0, gen.Value);
        }
    }

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        var enumTypes = new List<string>();
        var breaks = 0;
        foreach (var field in obj.Fields)
        {
            if (field is BreakType breakType)
            {
                enumTypes.Add("Break" + breaks++);
            }
        }

        if (enumTypes.Count <= 0) return;
        sb.AppendLine("[Flags]");
        sb.AppendLine($"public enum {VersioningEnumName}");
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

    public override async Task PostLoad(ObjectGeneration obj)
    {
        await base.PostLoad(obj);
        int breaks = 0;
        foreach (var field in obj.Fields)
        {
            if (field is BreakType breakType)
            {
                breakType.Index = breaks++;
            }
        }
    }

    public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
    {
        await base.PostFieldLoad(obj, field, node);
        var data = field.GetFieldData();
        var customVersion = node.Elements(XName.Get("CustomVersion", LoquiGenerator.Namespace)).FirstOrDefault();
        if (customVersion != null)
        {
            data.CustomVersion = ushort.Parse(customVersion.Value);
        }
        data.Versioning.AddRange(node.Elements(XName.Get("Versioning", LoquiGenerator.Namespace))
            .Select(versioning =>
                (Version: versioning.GetAttribute<ushort>("formVersion", throwException: true),
                    Action: versioning.GetAttribute("action", VersionAction.Add)))
            .OrderBy(versioning => versioning.Version));
        if (data.Versioning.Count > 2)
        {
            throw new NotImplementedException();
        }
        else if (data.Versioning.Count == 2)
        {
            if (data.Versioning[0].Action == data.Versioning[1].Action)
            {
                throw new ArgumentException("Versioning has non-sensical instructions.");
            }
        }
    }

    public static string GetVersionIfCheck(MutagenFieldData data, Accessor versionAccessor)
    {
        if (!data.HasVersioning)
        {
            throw new ArgumentException();
        }
        if (data.Versioning.Count <= 2)
        {
            return string.Join(" && ", data.Versioning.Select(v => $"{versionAccessor} {(v.Action == VersionAction.Add ? ">=" : "<")} {v.Version}"));
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static void AddVersionOffset(StructuredStringBuilder sb, TypeGeneration field, int expectedLen, TypeGeneration lastVersionedField, Accessor versionAccessor)
    {
        var data = field.GetFieldData();
        string offsetStr;
        if (data.Versioning.Count == 1)
        {
            if (data.Versioning[0].Action == VersionAction.Add)
            {
                offsetStr = $"{versionAccessor} < {data.Versioning[0].Version} ? -{expectedLen} : 0";
            }
            else
            {
                offsetStr = $"{versionAccessor} >= {data.Versioning[0].Version} ? -{expectedLen} : 0";
            }
        }
        else if (data.Versioning.Count == 2)
        {
            if (data.Versioning[0].Action == VersionAction.Add)
            {
                // Add first
                offsetStr = $"{versionAccessor} < {data.Versioning[0].Version} || {versionAccessor} >= {data.Versioning[1].Version} ? -{expectedLen} : 0";
            }
            else
            {
                // Remove first
                offsetStr = $"{versionAccessor} >= {data.Versioning[0].Version} || {versionAccessor} < {data.Versioning[1].Version} ? -{expectedLen} : 0";
            }
        }
        else
        {
            throw new NotImplementedException();
        }
        sb.AppendLine($"int {field.Name}VersioningOffset => {(lastVersionedField == null ? null : $"{lastVersionedField.Name}VersioningOffset + (")}{offsetStr}{(lastVersionedField == null ? null : ")")};");
    }
}