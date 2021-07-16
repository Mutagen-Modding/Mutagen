using System;
using System.Collections.Concurrent;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Core.UnitTests.AutoData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Allocators;
using Mutagen.Bethesda.Plugins.Records;
using NSubstitute;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.Core.UnitTests.Plugins.Allocators
{
    public abstract class IPersistentFormKeyAllocator_Tests<TFormKeyAllocator> : IFormKeyAllocator_Tests<TFormKeyAllocator>
        where TFormKeyAllocator : BasePersistentFormKeyAllocator
    {
        protected override TFormKeyAllocator CreateAllocator(IFileSystem fileSystem, IMod mod) => CreateAllocator(fileSystem, mod, ConstructTypicalPath(fileSystem));

        protected abstract TFormKeyAllocator CreateAllocator(IFileSystem fileSystem, IMod mod, string path);

        protected abstract string ConstructTypicalPath(IFileSystem fileSystem);

        [Fact]
        public void CanCommit()
        {
            var fileSystem = GetFileSystem();
            var mod = Substitute.For<IMod>();

            using var allocator = CreateAllocator(fileSystem, mod);

            allocator.Commit();
        }

        [Fact]
        public void CanRollback()
        {
            var fileSystem = GetFileSystem();
            var mod = Substitute.For<IMod>();

            using var allocator = CreateAllocator(fileSystem, mod);

            allocator.Rollback();
        }

        [Fact]
        public void AllocatesSameFormKeyAfterLoad()
        {
            uint nextFormID = 123;

            var fileSystem = GetFileSystem();
            FormKey expectedFormKey;
            {
                var mod = Substitute.For<IMod>();
                ((IMod)mod).NextFormID = nextFormID;

                using var allocator = CreateAllocator(fileSystem, mod);

                expectedFormKey = allocator.GetNextFormKey();

                allocator.Commit();
            }

            {
                var mod = Substitute.For<IMod>();
                ((IMod)mod).NextFormID = nextFormID;

                using var allocator = CreateAllocator(fileSystem, mod);

                var formKey = allocator.GetNextFormKey();

                Assert.Equal(expectedFormKey, formKey);
            }
        }

        [Fact]
        public void AllocatesSameFormKeyAfterRollback()
        {
            uint nextFormID = 123;

            var fileSystem = GetFileSystem();
            var mod = Substitute.For<IMod>();
            mod.NextFormID = nextFormID;

            using var allocator = CreateAllocator(fileSystem, mod);

            var expectedFormKey = allocator.GetNextFormKey();

            allocator.Rollback();

            var formKey = allocator.GetNextFormKey();

            Assert.Equal(expectedFormKey, formKey);
        }

        [Fact]
        public void AllocatesSameFormKeyForNullEditorIDAfterLoad()
        {
            uint nextFormID = 123;

            var fileSystem = GetFileSystem();
            FormKey expectedFormKey;
            {
                var mod = Substitute.For<IMod>();
                mod.NextFormID = nextFormID;

                using var allocator = CreateAllocator(fileSystem, mod);

                expectedFormKey = allocator.GetNextFormKey(null);

                allocator.Commit();
            }

            {
                var mod = Substitute.For<IMod>();
                mod.NextFormID = nextFormID;

                using var allocator = CreateAllocator(fileSystem, mod);

                var formKey = allocator.GetNextFormKey(null);

                Assert.Equal(expectedFormKey, formKey);
            }
        }

        [Fact]
        public void AllocatesSameFormKeyForNullEditorIDAfterRollback()
        {
            uint nextFormID = 123;

            var fileSystem = GetFileSystem();
            var mod = Substitute.For<IMod>();
            mod.NextFormID = nextFormID;

            using var allocator = CreateAllocator(fileSystem, mod);

            var expectedFormKey = allocator.GetNextFormKey(null);

            allocator.Rollback();

            var formKey = allocator.GetNextFormKey(null);

            Assert.Equal(expectedFormKey, formKey);
        }

        [Theory, MutagenAutoData]
        public void AllocatesSameFormKeyForEditorIdAfterLoad(
            IMod modA,
            IMod modB)
        {
            var fileSystem = GetFileSystem();
            FormKey expectedFormKey;
            {
                using var allocator = CreateAllocator(fileSystem, modA);

                expectedFormKey = allocator.GetNextFormKey(Utility.Edid1);

                allocator.Commit();
            }

            {
                using var allocator = CreateAllocator(fileSystem, modB);

                var formKey = allocator.GetNextFormKey(Utility.Edid1);

                Assert.Equal(expectedFormKey, formKey);
            }
        }

        //[Fact]
        public void AllocatesSameFormKeyForEditorIDAfterRollback()
        {
            var fileSystem = GetFileSystem();
            uint nextFormID = 123;
            var mod = Substitute.For<IMod>();
            ((IMod)mod).NextFormID = nextFormID;
            using var allocator = CreateAllocator(fileSystem, mod);

            var expectedFormKey = allocator.GetNextFormKey(Utility.Edid1);

            allocator.Rollback();

            var formKey = allocator.GetNextFormKey(Utility.Edid1);

            Assert.Equal(expectedFormKey, formKey);
        }

        [Theory, MutagenAutoData]
        public void OutOfOrderAllocationReturnsSameIdentifiers(IMod modA, IMod modB)
        {
            var fileSystem = GetFileSystem();
            uint formID1, formID2;
            {
                using var allocator = CreateAllocator(fileSystem, modA);
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
                using var allocator = CreateAllocator(fileSystem, modB);

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
            var fileSystem = GetFileSystem();
            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();

            var mod = Substitute.For<IMod>();
            {
                using var allocator = CreateAllocator(fileSystem, mod);

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
                using var allocator = CreateAllocator(fileSystem, mod);

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

        [Theory, MutagenAutoData]
        public void ParallelAllocationWithCommits(IMod mod)
        {
            var fileSystem = GetFileSystem();
            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();

            {
                using var allocator = CreateAllocator(fileSystem, mod);

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
                using var allocator = CreateAllocator(fileSystem, mod);

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
            var fileSystem = GetFileSystem();
            var someFolder = "C:/SomeFolder";
            fileSystem.Directory.CreateDirectory(someFolder);
            var mod = Substitute.For<IMod>();
            using var allocator = CreateAllocator(fileSystem, mod, Path.Combine(someFolder, "DoesntExist"));
        }

        [Fact]
        public void NonExistentParentDirThrows()
        {
            var fileSystem = GetFileSystem();
            var someFolder = "C:/SomeFolder";
            var mod = Substitute.For<IMod>();
            Assert.ThrowsAny<Exception>(() =>
            {
                CreateAllocator(fileSystem, mod, Path.Combine(someFolder, "DoesntExist", "AlsoDoesntExist"));
            });
        }
    }
}

