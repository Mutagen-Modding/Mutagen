using Noggog;
using System;

namespace Mutagen.Bethesda
{
    public abstract class BasePersistentFormKeyAllocator : BaseFormKeyAllocator, IPersistentFormKeyAllocator
    {
        protected IPath SaveLocation;

        protected BasePersistentFormKeyAllocator(IMod mod, IPath saveLocation) : base(mod)
        {
            this.SaveLocation = saveLocation;
        }

        public abstract void Save();

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// An interface for something that can allocate new FormKeys when requested shared between multiple programs
    /// </summary>
    public interface IPersistentFormKeyAllocator : IFormKeyAllocator, IDisposable
    {
        public void Save();
    }
}
