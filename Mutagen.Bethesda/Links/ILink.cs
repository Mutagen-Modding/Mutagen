using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface ILink<T> : INotifyingItem<T>
        where T : IMajorRecord
    {
        bool Linked { get; }
        RawFormID FormID { get; }
    }
}
