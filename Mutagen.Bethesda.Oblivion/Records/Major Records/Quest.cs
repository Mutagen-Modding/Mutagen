using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Quest
    {
        public enum Flag
        {
            StartGameEnabled = 0x01,
            AllowRepeatedConversationTopics = 0x04,
            AllowRepeatedStages = 0x08
        }
    }
}
