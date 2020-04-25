using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public interface IEffectRecord : IMajorRecordCommon, IEffectRecordGetter
    {
    }

    public interface IEffectRecordGetter : IMajorRecordCommonGetter
    {
    }
}
