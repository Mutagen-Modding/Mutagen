using Noggog;
using System.Diagnostics;

namespace Mutagen.Bethesda
{
    [DebuggerDisplay("SharedFormKeyAllocator {Mod} {PatcherName}")]
    public abstract class BaseSharedFormKeyAllocator : BasePersistentFormKeyAllocator, ISharedFormKeyAllocator
    {
        protected readonly string PatcherName;

        protected BaseSharedFormKeyAllocator(IMod mod, string saveLocation, string patcherName) : base(mod, saveLocation)
        {
            PatcherName = patcherName;
        }
    }

    public interface ISharedFormKeyAllocator : IPersistentFormKeyAllocator
    {
    }
}
