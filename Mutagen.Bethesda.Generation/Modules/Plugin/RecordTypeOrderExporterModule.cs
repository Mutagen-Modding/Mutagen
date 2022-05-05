using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog.IO;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class RecordTypeOrderExporterModule : GenerationModule
{
    public override async Task FinalizeGeneration(ProtocolGeneration proto)
    {
        var basePath = Path.Combine("RecordTypeOrdering", proto.Protocol.Namespace);
        Directory.CreateDirectory(basePath);
        foreach (var obj in proto.ObjectGenerationsByID.Values)
        {
            var path = Path.Combine(basePath, $"{obj.Name}.txt");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            StructuredStringBuilder sb = new();
            var set = new HashSet<RecordType>();
            await WriteRecordTypes(obj, sb, null, set);
            ExportStringToFile exportStringToFile = new();
            exportStringToFile.ExportToFile(path, sb.GetString());
        }
    }

    public async Task WriteRecordTypes(ObjectGeneration obj, StructuredStringBuilder sb, RecordTypeConverter? converter, HashSet<RecordType> set)
    {
        var fields = obj.IterateFields(includeBaseClass: true).ToArray();
        foreach (var f in fields)
        {
            await WriteRecordTypes(f, sb, converter, set);
        }
        if (obj.GetObjectData().EndMarkerType.HasValue
            && set.Add(obj.GetObjectData().EndMarkerType.Value))
        {
            sb.AppendLine($"RecordTypes.{obj.GetObjectData().EndMarkerType.Value},");
        }
    }

    public async Task WriteRecordTypes(TypeGeneration typeGen, StructuredStringBuilder sb, RecordTypeConverter? converter, HashSet<RecordType> set)
    {
        if (!typeGen.GetFieldData().HasTrigger) return;
        foreach (var item in typeGen.GetFieldData().TriggeringRecordTypes)
        {
            var toAdd = converter.ConvertToCustom(item);
            if (set.Add(toAdd))
            {
                sb.AppendLine($"RecordTypes.{toAdd},");
            }
        }
        if (typeGen is ContainerType cont)
        {
            await WriteRecordTypes(cont.SubTypeGeneration, sb, typeGen.GetFieldData().RecordTypeConverter, set);
        }
        else if (typeGen is LoquiType loqui
                && loqui.TargetObjectGeneration != null)
        {
            await WriteRecordTypes(loqui.TargetObjectGeneration, sb, typeGen.GetFieldData().RecordTypeConverter, set);
        }
    }
}
