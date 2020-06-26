using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [LeveledNpc, Npc]
    /// </summary>
    public partial interface INpcSpawn :
        ISkyrimMajorRecordInternal,
        INpcSpawnGetter
    {
    }

    /// <summary>
    /// Implemented by: [LeveledNpc, Npc]
    /// </summary>
    public partial interface INpcSpawnGetter : ISkyrimMajorRecordGetter
    {
    }
}
