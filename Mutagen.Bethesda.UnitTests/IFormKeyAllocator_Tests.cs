using Mutagen.Bethesda.Oblivion;
using Noggog.Utility;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public abstract class IFormKeyAllocator_Tests
    {
        public static readonly string Patcher1 = "Patcher1";

        public static readonly string Patcher2 = "Patcher2";

        protected Lazy<TempFolder> tempFolder = new(() => TempFolder.FactoryByPath(path: Utility.TempFolderPath));

        protected Lazy<TempFile> tempFile = new(() => new TempFile(extraDirectoryPaths: Utility.TempFolderPath));

        protected abstract IFormKeyAllocator CreateFormKeyAllocator(IMod mod);

        protected abstract IFormKeyAllocator CreateFormKeyAllocator(IMod mod, string patcherName);

        protected abstract IFormKeyAllocator LoadFormKeyAllocator(IMod mod);

        protected abstract IFormKeyAllocator LoadFormKeyAllocator(IMod mod, string patcherName);

        protected abstract void DisposeFormKeyAllocator(IFormKeyAllocator allocator);

        protected abstract void DisposeFormKeyAllocator(IFormKeyAllocator allocator, string patcherName);

        protected abstract void SaveFormKeyAllocator(IFormKeyAllocator allocator);

        protected abstract void SaveFormKeyAllocator(IFormKeyAllocator allocator, string patcherName);

        [Fact]
        public void OutOfOrderAllocationReturnsSameIdentifiers()
        {
            uint formID1, formID2;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = LoadFormKeyAllocator(mod, Patcher1);
                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                formID1 = formKey1.ID;

                allocator.GetNextFormKey();

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                formID2 = formKey2.ID;

                SaveFormKeyAllocator(allocator, Patcher1);
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = LoadFormKeyAllocator(mod, "Patcher1");

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                Assert.Equal(formID2, formKey2.ID);

                allocator.GetNextFormKey();

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                Assert.Equal(formID1, formKey1.ID);

                DisposeFormKeyAllocator(allocator, Patcher1);
            }
        }

        [Fact]
        public void OutOfOrderAllocationReturnsSameIdentifiers2()
        {
            uint formID1, formID2;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = CreateFormKeyAllocator(mod);
                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                formID1 = formKey1.ID;

                allocator.GetNextFormKey();

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                formID2 = formKey2.ID;

                SaveFormKeyAllocator(allocator);
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = LoadFormKeyAllocator(mod);

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                Assert.Equal(formID2, formKey2.ID);

                allocator.GetNextFormKey();

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                Assert.Equal(formID1, formKey1.ID);

                DisposeFormKeyAllocator(allocator);
            }
        }

        [Fact]
        public void ParallelAllocation()
        {
            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();
            var mod = new OblivionMod(Utility.PluginModKey);

            {
                var allocator = CreateFormKeyAllocator(mod, Patcher1);

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

                SaveFormKeyAllocator(allocator, Patcher1);
            }

            {
                var allocator = LoadFormKeyAllocator(mod, Patcher1);

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

                DisposeFormKeyAllocator(allocator, Patcher1);
            }
        }

        [Fact]
        public void ParallelAllocation2()
        {
            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();

            var mod = new OblivionMod(Utility.PluginModKey);
            {
                var allocator = CreateFormKeyAllocator(mod);

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

                SaveFormKeyAllocator(allocator);
            }

            {
                var allocator = LoadFormKeyAllocator(mod);

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

                DisposeFormKeyAllocator(allocator);
            }
        }

        [Fact]
        public void DuplicateAllocationWithinOnePatcherThrows()
        {
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = CreateFormKeyAllocator(mod);

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);

                FormKey? formKey2;
                var e = Assert.Throws<ConstraintException>(() =>
                {
                    formKey2 = allocator.GetNextFormKey(Utility.Edid1);
                });

                DisposeFormKeyAllocator(allocator);
            }
        }

        [Fact]
        public void DuplicateAllocationBetweenTwoPatchersThrows()
        {
            var mod = new OblivionMod(Utility.PluginModKey);
            {
                var allocator = CreateFormKeyAllocator(mod, Patcher1);

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);

                SaveFormKeyAllocator(allocator, Patcher1);
            }

            {
                var allocator = LoadFormKeyAllocator(mod, Patcher2);

                FormKey? formKey2;
                var e = Assert.Throws<ConstraintException>(() =>
                {
                    formKey2 = allocator.GetNextFormKey(Utility.Edid1);
                });

                DisposeFormKeyAllocator(allocator, Patcher1);
            }
        }
    }
}
