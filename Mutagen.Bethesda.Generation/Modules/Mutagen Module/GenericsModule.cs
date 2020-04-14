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
                                if (!(dict.ValueTypeGen is LoquiType keyLoqui)) continue;
                                loquiType = keyLoqui;
                                break;
                            case DictMode.KeyValue:
                                if (dict.KeyTypeGen is LoquiType || dict.ValueTypeGen is LoquiType)
                                {
                                    throw new NotImplementedException();
                                }
                                continue;
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
                fg.AppendLine($"{genName}_RecordType = (RecordType)LoquiRegistration.GetRegister(typeof(T))!.GetType().GetField(Mutagen.Bethesda.Internals.Constants.{nameof(Mutagen.Bethesda.Internals.Constants.TriggeringRecordTypeMember)}).GetValue(null);");
            }
            await base.GenerateInStaticCtor(obj, fg);
        }
    }
}
