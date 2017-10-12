using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public partial class GlobalShort
    {
        protected override byte TriggerByte => TRIGGER_BYTE;
        public const byte TRIGGER_BYTE = 0x73;

        partial void CustomCtor()
        {
            this.RawFloat_Property.Subscribe((change) => this.Data = (short)Math.Round(change.New));
            this.Data_Property.Subscribe((change) => this.RawFloat = change.New, fireInitial: false);
        }
    }
}
