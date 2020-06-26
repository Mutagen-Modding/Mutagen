using Mutagen.Bethesda;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Implemented by: [Flora, LandscapeTexture, MoveableStatic, Static, Tree]
    /// </summary>
    public partial interface IRegionTarget :
        ISkyrimMajorRecordInternal,
        IRegionTargetGetter
    {
    }

    /// <summary>
    /// Implemented by: [Flora, LandscapeTexture, MoveableStatic, Static, Tree]
    /// </summary>
    public partial interface IRegionTargetGetter : ISkyrimMajorRecordGetter
    {
    }
}
