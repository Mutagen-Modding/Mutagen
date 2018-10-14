using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface ILink
    {
        bool Linked { get; }
        FormKey FormKey { get; }
        Type TargetType { get; }
        bool Link<M>(
            ModList<M> modList,
            M sourceMod,
            NotifyingFireParameters cmds = null)
            where M : IMod<M>;
#if DEBUG
        bool AttemptedLink { get; set; }
#endif
    }

    public interface ILink<T> : ILink, INotifyingItem<T>
        where T : IMajorRecord
    {
    }

    public interface ISetLink<T> : ILink<T>, INotifyingSetItem<T>
        where T : IMajorRecord
    {
    }
}
