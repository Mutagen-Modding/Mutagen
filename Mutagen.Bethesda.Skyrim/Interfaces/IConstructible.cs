using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Skyrim
{
    // All items that implement IConstructible also implement IItem, except LLists

    public partial interface IConstructible : IItem
    {
    }

    public partial interface IConstructibleGetter : IItemGetter
    {
    }
}
