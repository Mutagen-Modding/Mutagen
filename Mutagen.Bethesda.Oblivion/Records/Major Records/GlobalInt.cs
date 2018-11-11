using Noggog.Notifying;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
            this.WhenAny(x => x.RawFloat)
                .DistinctUntilChanged()
                .Select(x => (int)Math.Round(x))
                .BindTo(this, x => x.Data);
            this.WhenAny(x => x.Data)
                .DistinctUntilChanged()
                .BindTo(this, x => x.RawFloat);
        }
    }
}
