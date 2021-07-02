using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.UnitTests
{
    public class MockFileSystemWatcherFactory : IFileSystemWatcherFactory
    {
        private readonly MockFileSystemWatcher _mock;

        public MockFileSystemWatcherFactory()
        {
            _mock = new MockFileSystemWatcher();
        }

        public MockFileSystemWatcherFactory(MockFileSystemWatcher mock)
        {
            _mock = mock;
        }

        public IFileSystemWatcher CreateNew() => _mock;

        public IFileSystemWatcher CreateNew(string path) => _mock;

        public IFileSystemWatcher CreateNew(string path, string filter) => _mock;
    }
    
    public class MockFileSystemWatcher : IFileSystemWatcher
    {
        public void Dispose()
        {
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }

        public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
        {
            throw new NotImplementedException();
        }

        public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
        {
            throw new NotImplementedException();
        }

        public bool IncludeSubdirectories { get; set; }
        public bool EnableRaisingEvents { get; set; }
        public string Filter { get; set; } = string.Empty;
        public Collection<string> Filters { get; } = new();
        public int InternalBufferSize { get; set; }
        public NotifyFilters NotifyFilter { get; set; }
        public string Path { get; set; } = string.Empty;
        public ISite Site { get; set; } = null!;
        public ISynchronizeInvoke SynchronizingObject { get; set; } = null!;
        public event FileSystemEventHandler? Changed;
        public event FileSystemEventHandler? Created;
        public event FileSystemEventHandler? Deleted;
        public event ErrorEventHandler? Error;
        public event RenamedEventHandler? Renamed;

        public void MarkCreated(string path)
        {
            if (Created == null) return;
            Created(this, new FileSystemEventArgs(
                WatcherChangeTypes.Created,
                System.IO.Path.GetDirectoryName(path)!,
                System.IO.Path.GetFileName(path)));
        }

        public void MarkDeleted(string path)
        {
            Deleted?.Invoke(this, new FileSystemEventArgs(
                WatcherChangeTypes.Deleted,
                System.IO.Path.GetDirectoryName(path)!,
                System.IO.Path.GetFileName(path)));
        }

        public void MarkChanged(string path)
        {
            Changed?.Invoke(this, new FileSystemEventArgs(
                WatcherChangeTypes.Changed,
                System.IO.Path.GetDirectoryName(path)!,
                System.IO.Path.GetFileName(path)));   
        }
    }
}