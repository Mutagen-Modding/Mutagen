using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface INamed
    {
        String Name { get; set; }
        INotifyingSetItem<String> Name_Property { get; }
    }
}
