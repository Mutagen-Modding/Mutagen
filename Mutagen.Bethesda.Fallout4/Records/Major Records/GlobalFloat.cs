using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class GlobalFloat
    {
        public const char TRIGGER_CHAR = 'f';
        public override char TypeChar { get; set; }

        public override float? RawFloat
        {
            get => this.Data;
            set => this.Data = value;
        }

        internal static GlobalFloat Factory()
        {
            var ret = new GlobalFloat();
            ret.TypeChar = TRIGGER_CHAR;
            return ret;
        }
    }

    namespace Internals
    {
        public partial class GlobalFloatBinaryOverlay
        {
            public override char TypeChar { get { return base.TypeChar; } set { base.TypeChar = value; } }
            public override float? RawFloat => this.Data;
        }
    }
}
