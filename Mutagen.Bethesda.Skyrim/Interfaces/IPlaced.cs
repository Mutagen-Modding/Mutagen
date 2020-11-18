using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial interface IPlaced : IPlacedThing, IPlacedSimple
    {
    }

    public partial interface IPlacedGetter : IPlacedThingGetter, IPlacedSimpleGetter
    {
    }
}
