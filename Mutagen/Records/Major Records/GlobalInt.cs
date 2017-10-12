using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public partial class GlobalInt
    {
        protected override byte TriggerByte => TRIGGER_BYTE;
        public const byte TRIGGER_BYTE = 0x6C;

        partial void CustomCtor()
        {
            this.RawFloat_Property.Subscribe((change) => this.Data = (int)Math.Round(change.New));
            this.Data_Property.Subscribe((change) => this.RawFloat = change.New, fireInitial: false);
        }
    }
}
