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
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation
{
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
                            args2.Add($"new RecordType(\"{conv.Key.Type}\")");
                            args2.Add($"new RecordType(\"{conv.Value.Type}\")");
                        }
                    });
                }
            }
        }
    }
}
