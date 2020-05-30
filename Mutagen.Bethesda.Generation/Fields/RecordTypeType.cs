using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class RecordTypeType : PrimitiveType
    {
        public override Type Type(bool getter) => typeof(RecordType);
        public override bool IsClass => false;

        public override string GetDefault(bool getter)
        {
            return "RecordType.Null";
        }
    }
}
