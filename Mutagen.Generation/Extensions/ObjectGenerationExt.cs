using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public static class ObjectGenerationExt
    {
        public static RecordType GetRecordType(this ObjectGeneration objGen)
        {
            TryGetRecordType(objGen, out var data);
            return data;
        }

        public static bool TryGetRecordType(this ObjectGeneration objGen, out RecordType recType)
        {
            if (objGen.CustomData.TryGetValue(typeof(RecordType), out var dataObj))
            {
                recType = (RecordType)dataObj;
                return true;
            }
            recType = default(RecordType);
            return false;
        }
    }
}
