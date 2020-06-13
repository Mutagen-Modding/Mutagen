using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// An interface for something that can be specified as an Alias voice type
    /// Implemented by: [Npc, FormList]
    /// </summary>
    public interface IAliasVoiceType : IMajorRecordCommon, IAliasVoiceTypeGetter
    {
    }

    /// <summary>
    /// An interface for something that can be specified as an Alias voice type
    /// Implemented by: [Npc, FormList]
    /// </summary>
    public interface IAliasVoiceTypeGetter : IMajorRecordCommonGetter
    {
    }
}
