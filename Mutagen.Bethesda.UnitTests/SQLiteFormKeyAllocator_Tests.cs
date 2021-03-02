using Mutagen.Bethesda.Core.Persistance;
using Mutagen.Bethesda.Oblivion;
using Noggog.Utility;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class SQLiteFormKeyAllocator_Tests
    {
        [Fact]
        public void OutOfOrderAllocationReturnsSameIdentifiers()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            uint formID1, formID2;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path, "Patcher1");
                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                formID1 = formKey1.ID;

                var formKey = allocator.GetNextFormKey();

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                formID2 = formKey2.ID;

                allocator.Commit();
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path, "Patcher1");

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                Assert.Equal(formID2, formKey2.ID);

                var formKey = allocator.GetNextFormKey();

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                Assert.Equal(formID1, formKey1.ID);
            }
        }

        [Fact]
        public void OutOfOrderAllocationReturnsSameIdentifiers2()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            uint formID1, formID2;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path);
                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                formID1 = formKey1.ID;

                var formKey = allocator.GetNextFormKey();

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                formID2 = formKey2.ID;

                allocator.Commit();
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path);

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                Assert.Equal(formID2, formKey2.ID);

                var formKey = allocator.GetNextFormKey();

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                Assert.Equal(formID1, formKey1.ID);
            }
        }

        [Fact]
        public void ParallelAllocation()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);

            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path, "Patcher1");

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
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path, "Patcher1");

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
        public void ParallelAllocation2()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);

            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path);

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
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path);

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
        public void DuplicateAllocationWithinOnePatcherThrows()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path);

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);

                FormKey? formKey2;
                var e = Assert.Throws<ConstraintException>(() => {
                    formKey2 = allocator.GetNextFormKey(Utility.Edid1);
                });
            }

        }

        [Fact]
        public void DuplicateAllocationBetweenTwoPatchersThrows()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path, "Patcher1");

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);

                allocator.Commit();
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = new SQLiteFormKeyAllocator(mod, file.File.Path, "Patcher2");

                FormKey? formKey2;
                var e = Assert.Throws<ConstraintException>(() => {
                    formKey2 = allocator.GetNextFormKey(Utility.Edid1);
                });
            }

        }

    }
}
