using Loqui;
using System;

namespace Mutagen.Bethesda.Pex.Records.Internals
{
    public static class GameCategoryExt
    {
        public static ILoquiRegistration? ToPexRegistration(this GameCategory category)
        {
            return category switch
            {
                GameCategory.Skyrim => Skyrim.Pex.Internals.PexFile_Registration.Instance,
                _ => null
            };
        }
    }
}
