using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Fallout4
{
    // Keep non-static so users can extend to add their own ModKeys cleanly.  No harm no foul
    public class Constants
    {
        public static readonly ModKey Fallout4 = new ModKey("Fallout4", type: ModType.Master);
        public static IEnumerable<ModKey> BaseGameModKeys => EnumerateBaseGameModKeys();

        public static readonly FormKey Player = new FormKey(Fallout4, id: 0x14);

        private static IEnumerable<ModKey> EnumerateBaseGameModKeys()
        {
            yield return Fallout4;
        }
    }
}
