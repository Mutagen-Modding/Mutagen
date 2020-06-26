using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [Faction, Race]
    /// </summary>
    public partial interface IRelatable :
        ISkyrimMajorRecordInternal,
        IRelatableGetter
    {
    }

    /// <summary>
    /// Implemented by: [Faction, Race]
    /// </summary>
    public partial interface IRelatableGetter : ISkyrimMajorRecordGetter
    {
    }
}
