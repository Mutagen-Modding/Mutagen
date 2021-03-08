using Noggog;
using System;

namespace Mutagen.Bethesda
{
    public abstract class BasePersistentFormKeyAllocator : BaseFormKeyAllocator, IPersistentFormKeyAllocator
    {
        protected IPath SaveLocation;

        public bool CommitOnDispose = true;

        private bool _disposed = false;

        protected BasePersistentFormKeyAllocator(IMod mod, IPath saveLocation) : base(mod)
        {
            this.SaveLocation = saveLocation;
        }

        /// <summary>
        /// Writes state to disk.
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// Reloads last committed state from disk.
        /// </summary>
        public abstract void Rollback();

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (CommitOnDispose) Commit();
            }
            _disposed = true;
        }

        public void Dispose() => Dispose(true);

        /*
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        */
    }

    /// <summary>
    /// An interface for something that can allocate new FormKeys when requested shared between multiple programs
    /// </summary>
    public interface IPersistentFormKeyAllocator : IFormKeyAllocator, IDisposable
    {
        public void Commit();

        public void Rollback();
    }
}
