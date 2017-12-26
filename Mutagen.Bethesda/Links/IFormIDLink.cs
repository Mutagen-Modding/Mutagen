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
        RawFormID? UnlinkedForm { get; }
    }
}
