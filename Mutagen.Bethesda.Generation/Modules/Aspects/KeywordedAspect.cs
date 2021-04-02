using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class KeywordedAspect : AspectFieldInterfaceDefinition
    {
        public KeywordedAspect()
            : base("IKeyworded")
        {
            FieldActions = new()
            {
                (LoquiInterfaceType.Direct, "Keywords", (o, tg, fg) =>
                {
                    fg.AppendLine("IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? IKeywordedGetter<IKeywordGetter>.Keywords => this.Keywords;");
                    fg.AppendLine("IReadOnlyList<IFormLinkGetter<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
                }),
                (LoquiInterfaceType.IGetter, "Keywords", (o, tg, fg) =>
                {
                    fg.AppendLine("IReadOnlyList<IFormLinkGetter<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
                })
            };
        }

        public override bool Test(ObjectGeneration o, Dictionary<string, TypeGeneration> allFields) => allFields
            .TryGetValue("Keywords", out var field)
            && field is ContainerType cont
            && typeof(FormLinkType).Equals(cont.SubTypeGeneration.GetType());

        public override List<AspectInterfaceData> Interfaces(ObjectGeneration obj)
        {
            return new List<AspectInterfaceData>()
            {
                (LoquiInterfaceDefinitionType.IGetter, $"IKeywordedGetter<IKeywordGetter>"),
                (LoquiInterfaceDefinitionType.ISetter, $"IKeyworded<IKeywordGetter>"),
            };
        }

    }
}
