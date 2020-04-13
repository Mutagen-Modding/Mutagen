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
    public class DataTypeModule : GenerationModule
    {
        public const string VersioningEnumName = "VersioningBreaks";
        public const string VersioningFieldName = "Versioning";

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
                elem.Add(new XAttribute(Loqui.Generation.Constants.HAS_BEEN_SET, "false"));
                await obj.LoadField(elem, requireName: true, add: true);
            }

            // Add object level versioning enum
            if (obj.Fields.Any(f => f is BreakType))
            {
                XElement elem = new XElement("Enum");
                elem.Add(new XAttribute(Loqui.Generation.Constants.NAME, VersioningFieldName));
                elem.Add(new XAttribute(Loqui.Generation.Constants.ENUM_NAME, $"{obj.ObjectName}.{VersioningEnumName}"));
                elem.Add(new XAttribute("binary", nameof(BinaryGenerationType.NoGeneration)));
                elem.Add(new XAttribute(Loqui.Generation.Constants.HAS_BEEN_SET, "false"));
                await obj.LoadField(elem, requireName: true, add: true);
            }
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            List<string> enumTypes;
            int breaks;
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
            {
                if (!(field is DataType dataType)) continue;
                enumTypes = new List<string>()
                {
                    "Has"
                };
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
                fg.AppendLine("[Flags]");
                fg.AppendLine($"public enum {dataType.EnumName}");
                using (new BraceWrapper(fg))
                {
                    using (var comma = new CommaWrapper(fg))
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

            // Breaks in main object
            enumTypes = new List<string>();
            breaks = 0;
            foreach (var field in obj.Fields)
            {
                if (field is BreakType breakType)
                {
                    enumTypes.Add("Break" + breaks++);
                }
            }

            if (enumTypes.Count <= 0) return;
            fg.AppendLine("[Flags]");
            fg.AppendLine($"public enum {VersioningEnumName}");
            using (new BraceWrapper(fg))
            {
                using (var comma = new CommaWrapper(fg))
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
            int? breaks = null;
            foreach (var field in obj.Fields)
            {
                if (field is BreakType breakType)
                {
                    if (breaks == null)
                    {
                        breaks = 0;
                    }
                    else
                    {
                        breaks++;
                    }
                }
                else if (breaks != null)
                {
                    field.GetFieldData().BreakIndex = breaks;
                }
            }
        }
    }
}
