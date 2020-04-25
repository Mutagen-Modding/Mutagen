using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public interface IOwner : IMajorRecordCommon, IOwnerGetter
    {
    }

    public interface IOwnerGetter : IMajorRecordCommonGetter
    {
    }
}
