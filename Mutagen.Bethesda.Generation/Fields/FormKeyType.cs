using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class FormKeyType : PrimitiveType
    {
        public override Type Type(bool getter) => typeof(FormKey);

        public override string GetDefault(bool getter)
        {
            return "FormKey.Null";
        }
    }
}
