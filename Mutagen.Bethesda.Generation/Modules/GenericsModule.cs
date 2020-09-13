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
                LoquiType loquiType;
                switch (field)
                {
                    case LoquiType l:
                        loquiType = l;
                        break;
                    case ContainerType container:
                        if (!(container.SubTypeGeneration is LoquiType contLoquiType)) continue;
                        loquiType = contLoquiType;
                        break;
                    case DictType dict:
                        switch (dict.Mode)
                        {
                            case DictMode.KeyedValue:
                                if (!(dict.ValueTypeGen is LoquiType loqui)) continue;
                                loquiType = loqui;
                                break;
                            case DictMode.KeyValue:
                                var keyLoqui = dict.KeyTypeGen as LoquiType;
                                var valLoqui = dict.ValueTypeGen as LoquiType;
                                if (keyLoqui != null)
                                {
                                    throw new NotImplementedException();
                                }
                                if (valLoqui == null) continue;
                                loquiType = valLoqui;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    default:
                        continue;
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
                fg.AppendLine($"{genName}_RecordType = {nameof(UtilityTranslation)}.{nameof(UtilityTranslation.GetRecordType)}<T>();");
            }
            await base.GenerateInStaticCtor(obj, fg);
        }
    }
}
