using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as an Npc Template
    /// </summary>
    public interface INpcTemplate : IMajorRecordCommon, INpcTemplateGetter
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as an Npc Template
    /// </summary>
    public interface INpcTemplateGetter : IMajorRecordCommonGetter
    {

    }
}
