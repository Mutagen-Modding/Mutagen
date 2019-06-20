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
        FormKey FormKey { get; set; }
        Type TargetType { get; }
        bool Link<M>(
            ModList<M> modList,
            M sourceMod)
            where M : IMod;
#if DEBUG
        bool AttemptedLink { get; set; }
#endif
    }

    public interface ILink<T> : ILink
        where T : IMajorRecordInternalGetter
    {
        T Item { get; set; }
        void Unset();
    }

    public interface ISetLink<T> : ILink<T>
        where T : IMajorRecordInternalGetter
    {
        bool HasBeenSet { get; set; }
    }
}
