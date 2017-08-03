using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class UnknownType : LoquiType
    {
        public UnknownType()
            : base()
        {
            this.RefName = "UnknownData";
        }
    }
}
