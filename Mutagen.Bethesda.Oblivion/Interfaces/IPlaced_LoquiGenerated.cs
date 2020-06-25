using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Oblivion
{
    /// <summary>
    /// Implemented by: [Landscape, PlacedCreature, PlacedNpc, PlacedObject]
    /// </summary>
    public partial interface IPlaced :
        IOblivionMajorRecordInternal,
        IPlacedGetter
    {
    }

    /// <summary>
    /// Implemented by: [Landscape, PlacedCreature, PlacedNpc, PlacedObject]
    /// </summary>
    public partial interface IPlacedGetter : IOblivionMajorRecordGetter
    {
    }
}
