using Noggog;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Pex
{
    public interface IHasUserFlags : IHasUserFlagsGetter
    {
        new ExtendedList<UserFlag> UserFlags { get; }
    }

    public interface IHasUserFlagsGetter
    {
        IReadOnlyList<IUserFlagGetter> UserFlags { get; }
    }
}
