using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GlobalFloat
    {
        public const char TRIGGER_CHAR = 'f';

        internal static GlobalFloat Factory()
        {
            return new GlobalFloat();
        }
    }
}
