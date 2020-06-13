using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class QuestObjective
    {
        [Flags]
        public enum Flag
        {
            OrWithPrevious = 0x1,
        }
    }
}
