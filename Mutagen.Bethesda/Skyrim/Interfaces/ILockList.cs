using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as a cell lock list
    /// Implemented by: [FormList, Npc]
    /// </summary>
    public interface ILockList : ILockListGetter, IMajorRecordCommon
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as a cell lock list
    /// Implemented by: [FormList, Npc]
    /// </summary>
    public interface ILockListGetter : IMajorRecordCommonGetter
    {
    }
}
