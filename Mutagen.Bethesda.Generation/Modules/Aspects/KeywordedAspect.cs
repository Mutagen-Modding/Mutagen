using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class KeywordedAspect : AspectInterfaceDefinition
    {
        public KeywordedAspect()
            : base("IKeyworded", ApplicabilityTest)
        {
            Interfaces = (o) => new List<(LoquiInterfaceDefinitionType Type, string Interface)>()
            {
                (LoquiInterfaceDefinitionType.IGetter, $"IKeywordedGetter<IKeywordGetter>"),
                (LoquiInterfaceDefinitionType.ISetter, $"IKeyworded<IKeywordGetter>"),
            };
            FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, Loqui.FileGeneration> Actions)>()
            {
                (LoquiInterfaceType.Direct, "Keywords", (o, fg) =>
                {
                    fg.AppendLine("IReadOnlyList<IFormLink<IKeywordGetter>>? IKeywordedGetter<IKeywordGetter>.Keywords => this.Keywords;");
                    fg.AppendLine("IReadOnlyList<IFormLink<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
                }),
                (LoquiInterfaceType.IGetter, "Keywords", (o, fg) =>
                {
                    fg.AppendLine("IReadOnlyList<IFormLink<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
                })
            };
        }

        private static bool ApplicabilityTest(ObjectGeneration o)
        {
            if (o.Fields.FirstOrDefault(x => x.Name == "Keywords") is not ContainerType cont) return false;
            return typeof(FormLinkType).Equals(cont.SubTypeGeneration.GetType());
        }
    }
}
