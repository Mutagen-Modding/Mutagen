using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public interface IHarvestTarget : IHarvestTargetGetter, IMajorRecordCommon
    {
    }

    public interface IHarvestTargetGetter : IMajorRecordCommonGetter
    {
    }
}
