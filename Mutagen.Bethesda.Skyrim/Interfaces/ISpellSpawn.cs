using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for records that can spawn in a leveled spell list
    /// Implemented by: [Spell, LeveledSpell]
    /// </summary>
    public interface ISpellSpawn : IMajorRecordCommon, ISpellSpawnGetter
    {
    }

    /// <summary>
    /// Common interface for records that can spawn in a leveled spell list
    /// Implemented by: [Spell, LeveledSpell]
    /// </summary>
    public interface ISpellSpawnGetter : IMajorRecordCommonGetter
    {
    }
}
