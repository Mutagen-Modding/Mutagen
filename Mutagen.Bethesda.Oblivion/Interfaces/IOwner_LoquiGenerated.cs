using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Oblivion
{
    /// <summary>
    /// Implemented by: [Faction, Npc]
    /// </summary>
    public partial interface IOwner :
        IOblivionMajorRecordInternal,
        IOwnerGetter
    {
    }

    /// <summary>
    /// Implemented by: [Faction, Npc]
    /// </summary>
    public partial interface IOwnerGetter : IOblivionMajorRecordGetter
    {
    }
}
