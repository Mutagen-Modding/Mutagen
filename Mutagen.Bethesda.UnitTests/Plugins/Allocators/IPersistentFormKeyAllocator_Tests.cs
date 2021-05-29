using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Allocators;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.Utility;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.IO;
using System.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Allocators
{

    public class SQLiteFormKeyAllocator_Tests : ISharedFormKeyAllocator_Tests<SQLiteFormKeyAllocator>
    {
        protected override string ConstructTypicalPath()
        {
            return tempFile.Value.File.Path;
        }

        protected override SQLiteFormKeyAllocator CreateAllocator(IMod mod, string path)
        {
            return new(mod, path);
        }

        protected override SQLiteFormKeyAllocator CreateNamedAllocator(IMod mod, string path, string patcherName)
        {
            return new(mod, path, patcherName);
        }
    }

    public abstract class IPersistentFormKeyAllocator_Tests<TFormKeyAllocator> : IFormKeyAllocator_Tests<TFormKeyAllocator>, IDisposable
        where TFormKeyAllocator : BasePersistentFormKeyAllocator
    {
        protected Lazy<TempFolder> tempFolder;

        protected Lazy<TempFile> tempFile;

        private bool disposedValue;

        protected IPersistentFormKeyAllocator_Tests()
        {
            tempFolder = new(() => TempFolder.Factory());
            tempFile = new(() => new TempFile(extraDirectoryPaths: Utility.TempFolderPath));
        }

        protected override TFormKeyAllocator CreateAllocator(IMod mod) => CreateAllocator(mod, ConstructTypicalPath());

        protected abstract TFormKeyAllocator CreateAllocator(IMod mod, string path);

        protected abstract string ConstructTypicalPath();

        [Fact]
        public void CanCommit()
        {
            var mod = new OblivionMod(Utility.PluginModKey);

            using var allocator = CreateAllocator(mod);

            allocator.Commit();
        }

        [Fact]
        public void CanRollback()
        {
            var mod = new OblivionMod(Utility.PluginModKey);

            using var allocator = CreateAllocator(mod);

            allocator.Rollback();
        }

        [Fact]
        public void AllocatesSameFormKeyAfterLoad()
        {
            uint nextFormID = 123;

            FormKey expectedFormKey;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                ((IMod)mod).NextFormID = nextFormID;

                using var allocator = CreateAllocator(mod);

                expectedFormKey = allocator.GetNextFormKey();

                allocator.Commit();
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                ((IMod)mod).NextFormID = nextFormID;

                using var allocator = CreateAllocator(mod);

                var formKey = allocator.GetNextFormKey();

                Assert.Equal(expectedFormKey, formKey);
            }
        }

        [Fact]
        public void AllocatesSameFormKeyAfterRollback()
        {
            uint nextFormID = 123;

            var mod = new OblivionMod(Utility.PluginModKey);
            ((IMod)mod).NextFormID = nextFormID;

            using var allocator = CreateAllocator(mod);

            var expectedFormKey = allocator.GetNextFormKey();

            allocator.Rollback();

            var formKey = allocator.GetNextFormKey();

            Assert.Equal(expectedFormKey, formKey);
        }

        [Fact]
        public void AllocatesSameFormKeyForNullEditorIDAfterLoad()
        {
            uint nextFormID = 123;

            FormKey expectedFormKey;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                ((IMod)mod).NextFormID = nextFormID;

                using var allocator = CreateAllocator(mod);

                expectedFormKey = allocator.GetNextFormKey(null);

                allocator.Commit();
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                ((IMod)mod).NextFormID = nextFormID;

                using var allocator = CreateAllocator(mod);

                var formKey = allocator.GetNextFormKey(null);

                Assert.Equal(expectedFormKey, formKey);
            }
        }

        [Fact]
        public void AllocatesSameFormKeyForNullEditorIDAfterRollback()
        {
            uint nextFormID = 123;

            var mod = new OblivionMod(Utility.PluginModKey);
            ((IMod)mod).NextFormID = nextFormID;

            using var allocator = CreateAllocator(mod);

            var expectedFormKey = allocator.GetNextFormKey(null);

            allocator.Rollback();

            var formKey = allocator.GetNextFormKey(null);

            Assert.Equal(expectedFormKey, formKey);
        }

        [Fact]
        public void AllocatesSameFormKeyForEditorIDAfterLoad()
        {
            FormKey expectedFormKey;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = CreateAllocator(mod);

                expectedFormKey = allocator.GetNextFormKey(Utility.Edid1);

                allocator.Commit();
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = CreateAllocator(mod);

                var formKey = allocator.GetNextFormKey(Utility.Edid1);

                Assert.Equal(expectedFormKey, formKey);
            }
        }

        //[Fact]
        public void AllocatesSameFormKeyForEditorIDAfterRollback()
        {
            uint nextFormID = 123;
            var mod = new OblivionMod(Utility.PluginModKey);
            ((IMod)mod).NextFormID = nextFormID;
            using var allocator = CreateAllocator(mod);

            var expectedFormKey = allocator.GetNextFormKey(Utility.Edid1);

            allocator.Rollback();

            var formKey = allocator.GetNextFormKey(Utility.Edid1);

            Assert.Equal(expectedFormKey, formKey);
        }

        [Fact]
        public void OutOfOrderAllocationReturnsSameIdentifiers()
        {
            uint formID1, formID2;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = CreateAllocator(mod);
                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                formID1 = formKey1.ID;

                var formKey = allocator.GetNextFormKey();

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                formID2 = formKey2.ID;

                Assert.NotEqual(formKey, formKey1);
                Assert.NotEqual(formKey, formKey2);
                Assert.NotEqual(formKey1, formKey2);

                allocator.Commit();
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = CreateAllocator(mod);

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                Assert.Equal(formID2, formKey2.ID);

                var formKey = allocator.GetNextFormKey();

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                Assert.Equal(formID1, formKey1.ID);

                Assert.NotEqual(formKey, formKey1);
                Assert.NotEqual(formKey, formKey2);
                Assert.NotEqual(formKey1, formKey2);
            }
        }

        [Fact]
        public void ParallelAllocation()
        {
            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();

            var mod = new OblivionMod(Utility.PluginModKey);
            {
                using var allocator = CreateAllocator(mod);

                void apply((int i, string s) x)
                {
                    // "Randomly" allocate some non-unique FormIDs.
                    if (x.i % 3 == 0)
                        allocator.GetNextFormKey();
                    else
                    {
                        var key = allocator.GetNextFormKey(x.s);
                        output1.TryAdd(x.i, key.ID);
                    }
                }

                input.AsParallel().ForAll(apply);

                allocator.Commit();
            }

            {
                using var allocator = CreateAllocator(mod);

                void check((int i, string s) x)
                {
                    if (x.i % 3 != 0)
                    {
                        var key = allocator.GetNextFormKey(x.s);
                        if (!output1.TryGetValue(x.i, out var expectedID))
                            throw new Exception("?");
                        Assert.Equal(expectedID, key.ID);
                    }
                }

                input.AsParallel().ForAll(check);
            }
        }

        [Fact]
        public void ParallelAllocationWithCommits()
        {
            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();

            var mod = new OblivionMod(Utility.PluginModKey);
            {
                using var allocator = CreateAllocator(mod);

                void apply((int i, string s) x)
                {
                    // "Randomly" allocate some non-unique FormIDs.
                    if (x.i % 3 == 0)
                        allocator.Commit();
                    else
                    {
                        var key = allocator.GetNextFormKey(x.s);
                        output1.TryAdd(x.i, key.ID);
                    }
                }

                input.AsParallel().ForAll(apply);

                allocator.Commit();
            }

            {
                using var allocator = CreateAllocator(mod);

                void check((int i, string s) x)
                {
                    if (x.i % 3 != 0)
                    {
                        var key = allocator.GetNextFormKey(x.s);
                        if (!output1.TryGetValue(x.i, out var expectedID))
                            throw new Exception("?");
                        Assert.Equal(expectedID, key.ID);
                    }
                }

                input.AsParallel().ForAll(check);
            }
        }

        [Fact]
        public void NonExistentEndpointProperlyConstructs()
        {
            using var temp = Utility.GetTempFolder(this.GetType().Name, nameof(NonExistentEndpointProperlyConstructs));
            var mod = new OblivionMod(Utility.PluginModKey);
            using var allocator = CreateAllocator(mod, Path.Combine(temp.Dir.Path, "DoesntExist"));
        }

        [Fact]
        public void NonExistentParentDirThrows()
        {
            using var temp = Utility.GetTempFolder(this.GetType().Name, nameof(NonExistentEndpointProperlyConstructs));
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.ThrowsAny<Exception>(() =>
            {
                CreateAllocator(mod, Path.Combine(temp.Dir.Path, "DoesntExist", "AlsoDoesntExist"));
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (tempFile.IsValueCreated)
                        tempFile.Value.Dispose();
                    if (tempFolder.IsValueCreated)
                        tempFile.Value.Dispose();
                }
                disposedValue = true;
            }
        }

        ~IPersistentFormKeyAllocator_Tests()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

