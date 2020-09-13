using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui;

namespace Mutagen.Bethesda.Generation
{
    public class AspectInterfaceModule : GenerationModule
    {
        public List<AspectInterfaceDefinition> Definitions = new List<AspectInterfaceDefinition>();

        public AspectInterfaceModule()
        {
            Definitions.AddReturn(new AspectInterfaceDefinition(o => 
                typeof(FormLinkType).Equals(o.Fields.FirstOrDefault(x => x.Name == "Keywords")?.GetType()))
            {
                Interfaces = new List<(LoquiInterfaceDefinitionType Type, string Interface)>()
                {
                    (LoquiInterfaceDefinitionType.IGetter, $"IKeywordedGetter<IKeywordGetter>"),
                    (LoquiInterfaceDefinitionType.ISetter, $"IKeyworded<IKeywordGetter>"),
                },
                FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<Loqui.FileGeneration> Actions)>()
                {
                    (LoquiInterfaceType.Direct, "Keywords", (fg) =>
                    {
                        fg.AppendLine("IReadOnlyList<IFormLink<IKeywordGetter>>? IKeywordedGetter<IKeywordGetter>.Keywords => this.Keywords;");
                        fg.AppendLine("IReadOnlyList<IFormLink<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
                    }),
                    (LoquiInterfaceType.IGetter, "Keywords", (fg) =>
                    {
                        fg.AppendLine("IReadOnlyList<IFormLink<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
                    })
                }
            });
        }

        public override async Task LoadWrapup(ObjectGeneration obj)
        {
            foreach (var def in Definitions)
            {
                if (!def.Test(obj)) continue;
                def.Interfaces.ForEach(x => obj.Interfaces.Add(x.Type, x.Interface));
            }
        }

        public override async Task GenerateInField(ObjectGeneration obj, TypeGeneration typeGeneration, FileGeneration fg, LoquiInterfaceType type)
        {
            using (new RegionWrapper(fg, "Aspects")
            {
                AppendExtraLine = false,
                SkipIfOnlyOneLine = true,
            })
            {
                foreach (var def in Definitions)
                {
                    if (!def.Test(obj)) continue;
                    def.FieldActions
                        .Where(x => x.Type == type && typeGeneration.Name == x.Name)
                        .ForEach(x => x.Actions(fg));
                }
            }
        }
    }
}
