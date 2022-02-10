using Loqui.Generation;
using System.Collections.Generic;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class KeywordedAspect : AspectFieldInterfaceDefinition
    {
        public KeywordedAspect()
            : base("IKeyworded")
        {
            FieldActions = new()
            {
                new(LoquiInterfaceType.Direct, "Keywords", (o, tg, fg) =>
                {
                    fg.AppendLine("IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? IKeywordedGetter<IKeywordGetter>.Keywords => this.Keywords;");
                    fg.AppendLine("IReadOnlyList<IFormLinkGetter<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
                }),
                new(LoquiInterfaceType.IGetter, "Keywords", (o, tg, fg) =>
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
