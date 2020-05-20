using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as Region objects
    /// Implemented by: [Tree, Flora, Static, LandscapeTexture, MoveableStatic]
    /// </summary>
    public interface IRegionTarget : IRegionTargetGetter, IMajorRecordCommon
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as Region objects
    /// Implemented by: [Tree, Flora, Static, LandscapeTexture, MoveableStatic]
    /// </summary>
    public interface IRegionTargetGetter : IMajorRecordCommonGetter
    {
    }
}
