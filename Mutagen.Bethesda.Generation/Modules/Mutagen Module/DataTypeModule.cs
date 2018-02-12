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
        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
            {
                if (!(field is DataType dataType)) continue;
                if (!dataType.HasStateLogic) continue;
                List<string> enumTypes = new List<string>();
                int breaks = 0;
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
                fg.AppendLine($"public {dataType.EnumName} {dataType.StateName};");
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
        }
    }
}
