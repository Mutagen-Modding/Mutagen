using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class GenericsModule : GenerationModule
    {
        private IEnumerable<string> GetGenerics(ObjectGeneration obj, FileGeneration fg)
        {
            HashSet<string> genericNames = new HashSet<string>();
            foreach (var field in obj.IterateFields())
            {
                if (!(field is LoquiType loquiType))
                {
                    if ((field is ContainerType container)
                        && container.SubTypeGeneration is LoquiType contLoquiType)
                    {
                        loquiType = contLoquiType;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (loquiType.GenericDef == null) continue;
                genericNames.Add(loquiType.GenericDef.Name);
            }
            return genericNames;
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var genName in GetGenerics(obj, fg))
            {
                fg.AppendLine($"public static readonly {nameof(RecordType)} {genName}_RecordType;");
            }
            await base.GenerateInClass(obj, fg);
        }

        public override async Task GenerateInStaticCtor(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var genName in GetGenerics(obj, fg))
            {
                fg.AppendLine($"var register = LoquiRegistration.GetRegister(typeof(T));");
                fg.AppendLine($"{genName}_RecordType = (RecordType)register.GetType().GetField(\"{Mutagen.Bethesda.Constants.TRIGGERING_RECORDTYPE_MEMBER}\").GetValue(null);");
            }
            await base.GenerateInStaticCtor(obj, fg);
        }
    }
}
