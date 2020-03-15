using Loqui;
using Noggog;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Generation
{
    public class RecordTypeConverterModule : GenerationModule
    {
        public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            if (!(field is LoquiType loquiType)) return;
            var recTypeOverrides = node.Element(XName.Get("RecordTypeOverrides", LoquiGenerator.Namespace));
            if (recTypeOverrides == null) return;
            var recConversions = recTypeOverrides.Elements(XName.Get("Mapping", LoquiGenerator.Namespace));
            if (recConversions == null || !recConversions.Any()) return;
            var data = loquiType.GetFieldData();
            data.RecordTypeConverter = new RecordTypeConverter(
                recConversions.Select((n) =>
                {
                    return new KeyValuePair<RecordType, RecordType>(
                        new RecordType(n.GetAttribute("From")),
                        new RecordType(n.GetAttribute("To")));
                }).ToArray());
        }

        public override async Task PreLoad(ObjectGeneration obj)
        {
            var recTypeOverrides = obj.Node.Element(XName.Get("BaseRecordTypeOverrides", LoquiGenerator.Namespace));
            if (recTypeOverrides == null) return;
            var recConversions = recTypeOverrides.Elements(XName.Get("Mapping", LoquiGenerator.Namespace));
            if (recConversions == null || !recConversions.Any()) return;
            var objData = obj.GetObjectData();
            objData.BaseRecordTypeConverter = new RecordTypeConverter(
                recConversions.Select((n) =>
                {
                    return new KeyValuePair<RecordType, RecordType>(
                        new RecordType(n.GetAttribute("From")),
                        new RecordType(n.GetAttribute("To")));
                }).ToArray());
            await base.PreLoad(obj);
        }

        public override Task GenerateInRegistration(ObjectGeneration obj, FileGeneration fg)
        {
            var objData = obj.GetObjectData();
            if (objData.BaseRecordTypeConverter?.FromConversions.Count > 0)
            {
                using (var args = new ArgsWrapper(fg,
                    $"public static RecordTypeConverter BaseConverter = new RecordTypeConverter"))
                {
                    foreach (var conv in objData.BaseRecordTypeConverter.FromConversions)
                    {
                        args.Add((gen) =>
                        {
                            using (var args2 = new FunctionWrapper(gen,
                                "new KeyValuePair<RecordType, RecordType>"))
                            {
                                args2.Add($"{obj.BaseClass.RecordTypeHeaderName(conv.Key)}");
                                args2.Add($"new RecordType(\"{conv.Value.Type}\")");
                            }
                        });
                    }
                }
            }
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
                if (fieldData.RecordTypeConverter == null || fieldData.RecordTypeConverter.FromConversions.Count == 0) continue;
                using (var args = new ArgsWrapper(fg,
                    $"public static RecordTypeConverter {field.Name}Converter = new RecordTypeConverter"))
                {
                    foreach (var conv in fieldData.RecordTypeConverter.FromConversions)
                    {
                        args.Add((gen) =>
                        {
                            using (var args2 = new FunctionWrapper(gen,
                                "new KeyValuePair<RecordType, RecordType>"))
                            {
                                args2.Add($"{loquiType.TargetObjectGeneration.RecordTypeHeaderName(conv.Key)}");
                                args2.Add($"new RecordType(\"{conv.Value.Type}\")");
                            }
                        });
                    }
                }
            }
            return base.GenerateInRegistration(obj, fg);
        }
    }
}
