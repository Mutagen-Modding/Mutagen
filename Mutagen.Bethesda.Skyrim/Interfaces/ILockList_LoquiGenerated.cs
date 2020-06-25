using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [FormList, Npc]
    /// </summary>
    public partial interface ILockList :
        ISkyrimMajorRecordInternal,
        ILockListGetter
    {
    }

    /// <summary>
    /// Implemented by: [FormList, Npc]
    /// </summary>
    public partial interface ILockListGetter : ISkyrimMajorRecordGetter
    {
    }
}
