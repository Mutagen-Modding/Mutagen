using Loqui;
using Noggog;
using Loqui.Generation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class RecordTypeConverterModule : GenerationModule
{
    public static RecordTypeConverter GetConverter(XElement node)
    {
        if (node == null) return null;
        var recConversions = node.Elements(XName.Get("Mapping", LoquiGenerator.Namespace));
        if (recConversions == null || !recConversions.Any()) return null;
        return new RecordTypeConverter(
            recConversions.Select((n) =>
            {
                return new KeyValuePair<RecordType, RecordType>(
                    new RecordType(n.GetAttribute("From")),
                    new RecordType(n.GetAttribute("To")));
            }).ToArray());
    }

    public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
    {
        if (!(field is LoquiType loquiType)) return;
        var data = loquiType.GetFieldData();
        data.RecordTypeConverter = GetConverter(node.Element(XName.Get("RecordTypeOverrides", LoquiGenerator.Namespace)));
    }

    public override async Task PreLoad(ObjectGeneration obj)
    {
        var objData = obj.GetObjectData();
        objData.BaseRecordTypeConverter = GetConverter(obj.Node.Element(XName.Get("BaseRecordTypeOverrides", LoquiGenerator.Namespace)));

        var gameModeOverrides = obj.Node.Element(XName.Get("GameModeOverride", LoquiGenerator.Namespace));
        if (gameModeOverrides != null)
        {
            var mode = gameModeOverrides.GetAttribute<GameRelease>("release", throwException: true);
            if (objData.GameReleaseConverters == null)
            {
                objData.GameReleaseConverters = new Dictionary<GameRelease, RecordTypeConverter>();
            }
            objData.GameReleaseConverters[mode] = GetConverter(gameModeOverrides);
        }

        var versionModeOverrides = obj.Node.Element(XName.Get("VersionOverride", LoquiGenerator.Namespace));
        if (versionModeOverrides != null)
        {
            var version = versionModeOverrides.GetAttribute<byte>("version", throwException: true);
            if (objData.VersionConverters == null)
            {
                objData.VersionConverters = new Dictionary<byte, RecordTypeConverter>();
            }
            objData.VersionConverters[version] = GetConverter(versionModeOverrides);
        }
        await base.PreLoad(obj);
    }

    public override Task GenerateInRegistration(ObjectGeneration obj, FileGeneration fg)
    {
        var objData = obj.GetObjectData();
        GenerateConverterMember(fg, obj.BaseClass, objData.BaseRecordTypeConverter, "Base");
        foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
        {
            LoquiType loquiType = field as LoquiType;
            if (loquiType == null)
            {
                switch (field)
                {
                    case WrapperType wrapper:
                        loquiType = wrapper.SubTypeGeneration as LoquiType;
                        if (loquiType != null) break;
                        continue;
                    default:
                        continue;
                }
            }
            var fieldData = loquiType.GetFieldData();
            GenerateConverterMember(fg, loquiType.TargetObjectGeneration, fieldData.RecordTypeConverter, field.Name);
        }
        if (objData.GameReleaseConverters != null)
        {
            foreach (var kv in objData.GameReleaseConverters)
            {
                GenerateConverterMember(fg, obj, kv.Value, kv.Key.ToString());
            }
            using (var args = new FunctionWrapper(fg,
                       $"public static {nameof(RecordTypeConverter)}? Get"))
            {
                args.Add($"{nameof(GameRelease)} release");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"return release switch");
                using (new BraceWrapper(fg) { AppendSemicolon = true })
                {
                    using (var comma = new CommaWrapper(fg))
                    {
                        foreach (var kv in objData.GameReleaseConverters)
                        {
                            comma.Add($"{nameof(GameRelease)}.{kv.Key} => {kv.Key}Converter");
                        }
                        comma.Add($"_ => default({nameof(RecordTypeConverter)})");
                    }
                }
            }
        }
        if (objData.VersionConverters != null)
        {
            foreach (var kv in objData.VersionConverters)
            {
                GenerateConverterMember(fg, obj, kv.Value, $"Version{kv.Key}");
            }
            using (var args = new FunctionWrapper(fg,
                       $"public static {nameof(RecordTypeConverter)}? Get"))
            {
                args.Add($"int? version");
            }
            using (new BraceWrapper(fg))
            {
                bool first = true;
                fg.AppendLine($"if (version == null) return default({nameof(RecordTypeConverter)});");
                foreach (var kv in objData.VersionConverters.OrderBy(kv => kv.Key))
                {
                    fg.AppendLine($"{(first ? null : "else ")}if (version.Value >= {kv.Key})");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return Version{kv.Key}Converter;");
                    }
                    first = false;
                }
                fg.AppendLine($"return default({nameof(RecordTypeConverter)});");
            }
        }
        return base.GenerateInRegistration(obj, fg);
    }

    public static void GenerateConverterMember(FileGeneration fg, ObjectGeneration objGen, RecordTypeConverter recordTypeConverter, string nickName)
    {
        if (recordTypeConverter == null || recordTypeConverter.FromConversions.Count == 0) return;
        using (var args = new ArgsWrapper(fg,
                   $"public static RecordTypeConverter {nickName}Converter = new RecordTypeConverter"))
        {
            foreach (var conv in recordTypeConverter.FromConversions)
            {
                args.Add((gen) =>
                {
                    using (var args2 = new FunctionWrapper(gen,
                               "new KeyValuePair<RecordType, RecordType>"))
                    {
                        args2.Add($"RecordTypes.{conv.Key.Type}");
                        args2.Add($"RecordTypes.{conv.Value.Type}");
                    }
                });
            }
        }
    }
}