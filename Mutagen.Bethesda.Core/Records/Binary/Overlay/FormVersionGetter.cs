using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Records.Binary.Overlay
{
    public class FormVersionGetter : IFormVersionGetter
    {
        public ushort? FormVersion { get; set; }
    }
}
