using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public static class TypeGenerationExt
    {
        public static MutagenFieldData GetFieldData(this TypeGeneration typeGen)
        {
            TryGetFieldData(typeGen, out var data);
            return data;
        }

        public static bool TryGetFieldData(this TypeGeneration typeGen, out MutagenFieldData fieldData)
        {
            if (typeGen.CustomData.TryGetValue(MutagenFieldData.DATA_KEY, out var dataObj))
            {
                fieldData = (MutagenFieldData)dataObj;
                return true;
            }
            fieldData = null;
            return false;
        }
    }
}
