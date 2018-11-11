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

        partial void CustomCtor()
        {
            this.WhenAny(x => x.RawFloat)
                .DistinctUntilChanged()
                .BindTo(this, x => x.Data);
            this.WhenAny(x => x.Data)
                .DistinctUntilChanged()
                .BindTo(this, x => x.RawFloat);
        }

        internal static GlobalFloat Factory()
        {
            return new GlobalFloat();
        }
    }
}
