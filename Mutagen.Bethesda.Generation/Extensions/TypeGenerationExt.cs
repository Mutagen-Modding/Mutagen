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
        public static MutagenFieldData TryCreateFieldData(this TypeGeneration typeGen)
        {
            return typeGen.CustomData.TryCreateValue(Mutagen.Bethesda.Generation.Constants.DataKey, () => new MutagenFieldData(typeGen)) as MutagenFieldData;
        }

        public static MutagenFieldData GetFieldData(this TypeGeneration typeGen)
        {
            TryGetFieldData(typeGen, out var data);
            return data;
        }

        public static bool TryGetFieldData(this TypeGeneration typeGen, out MutagenFieldData fieldData)
        {
            if (typeGen.CustomData.TryGetValue(Constants.DataKey, out var dataObj))
            {
                fieldData = (MutagenFieldData)dataObj;
                return true;
            }
            fieldData = null;
            return false;
        }

        public static bool NeedMasters(this TypeGeneration typeGen)
        {
            return typeGen is FormLinkType
                || (typeGen is LoquiType loqui
                    && loqui.GetFieldData().HasTrigger);
        }
    }
}
