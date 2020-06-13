using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for records that can spawn in a leveled npc list
    /// Implemented by: [Npc, LeveledNpc]
    /// </summary>
    public interface INpcSpawn : IMajorRecordCommon, INpcSpawnGetter
    {
    }

    /// <summary>
    /// Common interface for records that can spawn in a leveled npc list
    /// Implemented by: [Npc, LeveledNpc]
    /// </summary>
    public interface INpcSpawnGetter : IMajorRecordCommonGetter
    {
    }
}
