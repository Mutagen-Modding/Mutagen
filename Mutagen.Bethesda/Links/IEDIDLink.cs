using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IEDIDLink<T> : ILink<T>
       where T : IMajorRecord
    {
        RecordType EDID { get; }
    }

    public interface IEDIDSetLink<T> : IEDIDLink<T>, ISetLink<T>
       where T : IMajorRecord
    {
    }
}
