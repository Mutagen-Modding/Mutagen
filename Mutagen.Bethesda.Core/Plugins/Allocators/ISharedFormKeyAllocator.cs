using Mutagen.Bethesda.Plugins.Records;
using System.Diagnostics;

namespace Mutagen.Bethesda.Plugins.Allocators
{
    [DebuggerDisplay("SharedFormKeyAllocator {Mod} {PatcherName}")]
    public abstract class BaseSharedFormKeyAllocator : BasePersistentFormKeyAllocator, ISharedFormKeyAllocator
    {
        public string ActivePatcherName { get; }

        protected BaseSharedFormKeyAllocator(IMod mod, string saveLocation, string activePatcherName)
            : base(mod, saveLocation)
        {
            ActivePatcherName = activePatcherName;
        }
    }

    public interface ISharedFormKeyAllocator : IPersistentFormKeyAllocator
    {
    }
}
