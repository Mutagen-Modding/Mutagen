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

        partial void CustomCtor()
        {
            this.TypeChar = TRIGGER_CHAR;
            this.RawFloat_Property.Subscribe((change) => this.Data = (int)Math.Round(change.New));
            this.Data_Property.Subscribe((change) => this.RawFloat = change.New, fireInitial: false);
        }
    }
}
