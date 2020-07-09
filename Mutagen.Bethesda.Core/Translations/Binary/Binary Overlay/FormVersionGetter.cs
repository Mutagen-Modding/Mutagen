using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class FormVersionGetter : IFormVersionGetter
    {
        public ushort? FormVersion { get; set; }
    }
}
