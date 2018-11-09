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
    public partial class GlobalShort
    {
        public const char TRIGGER_CHAR = 's';

        internal static GlobalShort Factory()
        {
            return new GlobalShort();
        }

        partial void CustomCtor()
        {
            this.TypeChar = TRIGGER_CHAR;
            this.WhenAny(x => x.RawFloat).Subscribe((change) => this.Data = (short)Math.Round(change));
            this.WhenAny(x => x.Data).Subscribe((change) => this.RawFloat = change);
        }
    }
}
