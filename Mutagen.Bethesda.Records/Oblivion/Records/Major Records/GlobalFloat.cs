using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GlobalFloat
    {
        public const char TRIGGER_CHAR = 'f';
        public override char TypeChar => TRIGGER_CHAR;

        public override float RawFloat
        {
            get => this.Data;
            set
            {
                if (this.Data != value)
                {
                    this.Data = value;
                }
                else
                {
                    this.Data_IsSet = true;
                }
            }
        }

        internal static GlobalFloat Factory()
        {
            return new GlobalFloat();
        }
    }

    namespace Internals
    {
        public partial class GlobalFloatBinaryOverlay
        {
            public override char TypeChar => GlobalFloat.TRIGGER_CHAR;
            public override float RawFloat => this.Data;
        }
    }
}
