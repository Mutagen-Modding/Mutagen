using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Plugins.Records
{
    public interface IFormVersionGetter
    {
        ushort? FormVersion { get; }
    }
}
