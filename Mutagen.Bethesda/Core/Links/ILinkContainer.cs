using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface ILinkSubContainer : ILinkContainer
    {
        void Link<M>(
            ModList<M> modList,
            M sourceMod)
            where M : IMod;
    }

    public interface ILinkContainer
    {
        IEnumerable<ILink> Links { get; }
    }
}
