using Mutagen.Bethesda.Oblivion;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Persistence
{
    public abstract class ISharedFormKeyAllocator_Tests<TFormKeyAllocator> : IPersistentFormKeyAllocator_Tests<TFormKeyAllocator>
        where TFormKeyAllocator : BaseSharedFormKeyAllocator
    {
        public static readonly string Patcher1 = "Patcher1";

        public static readonly string Patcher2 = "Patcher2";

        protected abstract TFormKeyAllocator CreateNamedAllocator(IMod mod, string path, string patcherName);

        protected TFormKeyAllocator CreateNamedAllocator(IMod mod, string patcherName) => CreateNamedAllocator(mod, ConstructTypicalPath(), patcherName);

        [Fact]
        public void OutOfOrderAllocationReturnsSameIdentifiersShared()
        {
            uint formID1, formID2;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = CreateNamedAllocator(mod, Patcher1);
                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                formID1 = formKey1.ID;

                allocator.GetNextFormKey();

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                formID2 = formKey2.ID;

                allocator.Commit();
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                using var allocator = CreateNamedAllocator(mod, Patcher1);

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                Assert.Equal(formID2, formKey2.ID);

                allocator.GetNextFormKey();

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                Assert.Equal(formID1, formKey1.ID);
            }
        }

        [Fact]
        public void ParallelAllocationShared()
        {
            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();
            var mod = new OblivionMod(Utility.PluginModKey);

            {
                using var allocator = CreateNamedAllocator(mod, Patcher1);

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
                using var allocator = CreateNamedAllocator(mod, Patcher1);

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
        public void DuplicateAllocationBetweenTwoPatchersThrows()
        {
            var mod = new OblivionMod(Utility.PluginModKey);
            {
                using var allocator = CreateNamedAllocator(mod, Patcher1);

                allocator.GetNextFormKey(Utility.Edid1);

                allocator.Commit();
            }

            {
                using var allocator = CreateNamedAllocator(mod, Patcher2);

                var e = Assert.Throws<ConstraintException>(() => allocator.GetNextFormKey(Utility.Edid1));
            }
        }
    }
}
