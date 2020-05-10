using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public interface INpcSpawn : IMajorRecordCommon, INpcSpawnGetter
    {
    }

    public interface INpcSpawnGetter : IMajorRecordCommonGetter
    {
    }
}
