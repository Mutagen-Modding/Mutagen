using Noggog;
using System.Diagnostics;

namespace Mutagen.Bethesda
{
    [DebuggerDisplay("SharedFormKeyAllocator {Mod} {PatcherName}")]
    public abstract class BaseSharedFormKeyAllocator : BasePersistentFormKeyAllocator, ISharedFormKeyAllocator
    {
        public static readonly string DefaultPatcherName = "default";

        protected readonly string PatcherName;

        protected BaseSharedFormKeyAllocator(IMod mod, IPath saveLocation) : this(mod, saveLocation, DefaultPatcherName) { }

        protected BaseSharedFormKeyAllocator(IMod mod, IPath saveLocation, string patcherName) : base(mod, saveLocation)
        {
            PatcherName = patcherName;
        }

    }

    public interface ISharedFormKeyAllocator : IPersistentFormKeyAllocator
    {

    }
}
