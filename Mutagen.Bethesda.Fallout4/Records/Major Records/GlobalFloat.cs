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
        public override char TypeChar => TRIGGER_CHAR;

        public override float? RawFloat
        {
            get => this.Data;
            set => this.Data = value;
        }
    }

    namespace Internals
    {
        public partial class GlobalFloatBinaryOverlay
        {
            public override char TypeChar => GlobalFloat.TRIGGER_CHAR;
            public override float? RawFloat => this.Data;
            public bool NoTypeDeclaration { get; internal set; }
        }
    }
}
