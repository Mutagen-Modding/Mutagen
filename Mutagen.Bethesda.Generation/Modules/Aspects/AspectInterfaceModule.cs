using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class AspectInterfaceModule : GenerationModule
    {
        public List<AspectInterfaceDefinition> Definitions = new List<AspectInterfaceDefinition>();
        public static Dictionary<ProtocolKey, Dictionary<AspectInterfaceDefinition, List<ObjectGeneration>>> ObjectMappings = new Dictionary<ProtocolKey, Dictionary<AspectInterfaceDefinition, List<ObjectGeneration>>>();

        public AspectInterfaceModule()
        {
            Definitions.Add(new KeywordedAspect());
            Definitions.Add(new KeywordAspect());
            Definitions.Add(new NamedAspect());
            Definitions.Add(new ObjectBoundedAspect());
        }

        public override async Task LoadWrapup(ObjectGeneration obj)
        {
            await obj.GetObjectData().WiringComplete.Task;
            foreach (var def in Definitions)
            {
                if (!def.Test(obj)) continue;
                if (def.Interfaces != null)
                {
                    def.Interfaces(obj).ForEach(x => obj.Interfaces.Add(x.Type, x.Interface));
                }
                lock (ObjectMappings)
                {
                    ObjectMappings.GetOrAdd(obj.ProtoGen.Protocol).GetOrAdd(def).Add(obj);
                }
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
                        .ForEach(x => x.Actions(obj, fg));
                }
            }
        }
    }
}
