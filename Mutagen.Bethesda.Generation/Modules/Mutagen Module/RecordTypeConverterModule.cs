using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public override Task GenerateInRegistration(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
            {
                if (!(field is LoquiType loquiType)) continue;
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
