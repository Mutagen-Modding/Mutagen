using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public static class TypeGenerationExt
    {
        public static MutagenFieldData GetFieldData(this TypeGeneration typeGen)
        {
            return typeGen.CustomData.TryCreateValue(Mutagen.Bethesda.Generation.Constants.DataKey, () => new MutagenFieldData(typeGen)) as MutagenFieldData;
        }

        public static bool NeedsRecordConverter(this TypeGeneration typeGen)
        {
            return typeGen is LoquiType loqui
                && loqui.GetFieldData().HasTrigger;
        }
    }
}
