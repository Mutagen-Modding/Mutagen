using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IDuplicatable
    {
        object Duplicate(Func<FormKey> getNextFormKey);
    }
}
