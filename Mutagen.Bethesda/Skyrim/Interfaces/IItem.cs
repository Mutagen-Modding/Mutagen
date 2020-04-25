using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public interface IItem : IMajorRecordCommon, IItemGetter
    {
    }

    public interface IItemGetter : IMajorRecordCommonGetter
    {
    }
}
