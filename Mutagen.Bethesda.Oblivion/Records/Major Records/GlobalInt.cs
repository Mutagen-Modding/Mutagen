using Noggog.Notifying;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GlobalInt
    {
        public const char TRIGGER_CHAR = 'l';

        internal static GlobalInt Factory()
        {
            return new GlobalInt();
        }

        partial void CustomCtor()
        {
            this.TypeChar = TRIGGER_CHAR;
            this.WhenAny(x => x.RawFloat).Subscribe((change) => this.Data = (int)Math.Round(change));
            this.WhenAny(x => x.Data).Subscribe((change) => this.RawFloat = change);
        }
    }
}
