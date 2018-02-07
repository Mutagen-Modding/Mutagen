using Loqui;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IFormIDLink<T> : ILink<T>
       where T : MajorRecord
    {
        FormID? UnlinkedForm { get; }
    }

    public interface IFormIDSetLink<T> : IFormIDLink<T>, ISetLink<T>
       where T : MajorRecord
    {
    }
}
