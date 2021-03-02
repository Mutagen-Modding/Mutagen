using Mutagen.Bethesda.Core.Persistance;
using Mutagen.Bethesda.Oblivion;
using Noggog.Utility;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.IO;
using System.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class TextFileFormKeyAllocatorv2_Tests
    {
        [Fact]
        public void StaticExport()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            TextFileFormKeyAllocatorv2.WriteToFile(
                file.File.Path,
                new (string, FormKey)[]
                {
                    (Utility.Edid1, Utility.Form1),
                    (Utility.Edid2, Utility.Form2),
                });
            var lines = File.ReadAllLines(file.File.Path);
            Assert.Equal(
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid2,
                    Utility.Form2.ToString(),
                },
                lines);
        }

        [Fact]
        public void TypicalImport()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            File.WriteAllLines(
                file.File.Path,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid2,
                    Utility.Form2.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = TextFileFormKeyAllocatorv2.FromFile(mod, file.File.Path);
            var formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }

        [Fact]
        public void FailedImportTruncatedFile()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            File.WriteAllLines(
                file.File.Path,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid2,
                    //Utility.Form2.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() =>
            {
                TextFileFormKeyAllocatorv2.FromFile(mod, file.File.Path);
            });
        }

        [Fact]
        public void FailedImportDuplicateFormKey()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            File.WriteAllLines(
                file.File.Path,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid2,
                    Utility.Form1.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() =>
            {
                TextFileFormKeyAllocatorv2.FromFile(mod, file.File.Path);
            });
        }

        [Fact]
        public void FailedImportDuplicateEditorID()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            File.WriteAllLines(
                file.File.Path,
                new string[]
                {
                    Utility.Edid1,
                    Utility.Form1.ToString(),
                    Utility.Edid1,
                    Utility.Form2.ToString(),
                });
            var mod = new OblivionMod(Utility.PluginModKey);
            Assert.Throws<ArgumentException>(() =>
            {
                TextFileFormKeyAllocatorv2.FromFile(mod, file.File.Path);
            });
        }

        [Fact]
        public void TypicalReimport()
        {
            var list = new (string, FormKey)[]
            {
                (Utility.Edid1, Utility.Form1),
                (Utility.Edid2, Utility.Form2),
            };
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);
            TextFileFormKeyAllocatorv2.WriteToFile(file.File.Path, list);
            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = TextFileFormKeyAllocatorv2.FromFile(mod, file.File.Path);
            var formID = allocator.GetNextFormKey();
            Assert.Equal(Utility.PluginModKey, formID.ModKey);
            formID = allocator.GetNextFormKey(Utility.Edid1);
            Assert.Equal(formID, Utility.Form1);
            formID = allocator.GetNextFormKey(Utility.Edid2);
            Assert.Equal(formID, Utility.Form2);
        }

        [Fact]
        public void OutOfOrderAllocationReturnsSameIdentifiers()
        {
            using var folder = TempFolder.FactoryByPath(path: Utility.TempFolderPath);
            uint formID1, formID2;
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = TextFileFormKeyAllocatorv2.FromFolder(mod, folder.Dir.Path, "Patcher1");
                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                formID1 = formKey1.ID;

                var formKey = allocator.GetNextFormKey();

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                formID2 = formKey2.ID;

                allocator.WriteToFolder(folder.Dir.Path);
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = TextFileFormKeyAllocatorv2.FromFolder(mod, folder.Dir.Path, "Patcher1");

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
                var allocator = new TextFileFormKeyAllocatorv2(mod);
                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);
                formID1 = formKey1.ID;

                var formKey = allocator.GetNextFormKey();

                var formKey2 = allocator.GetNextFormKey(Utility.Edid2);
                formID2 = formKey2.ID;

                allocator.WriteToFile(file.File.Path);
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = TextFileFormKeyAllocatorv2.FromFile(mod, file.File.Path);

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
            using var folder = TempFolder.FactoryByPath(path: Utility.TempFolderPath);

            var input = Enumerable.Range(1, 100).Select(i => (i, i.ToString())).ToList();
            var output1 = new ConcurrentDictionary<int, uint>();

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = TextFileFormKeyAllocatorv2.FromFolder(mod, folder.Dir.Path, "Patcher1");

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

                allocator.WriteToFolder(folder.Dir.Path);
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = TextFileFormKeyAllocatorv2.FromFolder(mod, folder.Dir.Path, "Patcher1");

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
                var allocator = new TextFileFormKeyAllocatorv2(mod);

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

                allocator.WriteToFile(file.File.Path);
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = TextFileFormKeyAllocatorv2.FromFile(mod, file.File.Path);

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
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = new TextFileFormKeyAllocatorv2(mod);

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);

                FormKey? formKey2;
                var e = Assert.Throws<ConstraintException>(() =>
                {
                    formKey2 = allocator.GetNextFormKey(Utility.Edid1);
                });
            }
        }

        [Fact]
        public void DuplicateAllocationBetweenTwoPatchersThrows()
        {
            using var folder = TempFolder.FactoryByPath(path: Utility.TempFolderPath);
            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = TextFileFormKeyAllocatorv2.FromFolder(mod, folder.Dir.Path, "Patcher1");

                var formKey1 = allocator.GetNextFormKey(Utility.Edid1);

                allocator.WriteToFolder(folder.Dir.Path);
            }

            {
                var mod = new OblivionMod(Utility.PluginModKey);
                var allocator = TextFileFormKeyAllocatorv2.FromFolder(mod, folder.Dir.Path, "Patcher2");

                FormKey? formKey2;
                var e = Assert.Throws<ConstraintException>(() =>
                {
                    formKey2 = allocator.GetNextFormKey(Utility.Edid1);
                });
            }

        }

        [Fact]
        public void WritingMultiplePatcherStateToFileFails()
        {
            using var file = new TempFile(extraDirectoryPaths: Utility.TempFolderPath);

            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = new TextFileFormKeyAllocatorv2(mod, "Patcher1");

            Assert.Throws<InvalidOperationException>(() =>
            {
                allocator.WriteToFile(file.File.Path);
            });
        }

        [Fact]
        public void WritingSinglePatcherStateToDirectoryFails()
        {
            using var folder = TempFolder.FactoryByPath(path: Utility.TempFolderPath);

            var mod = new OblivionMod(Utility.PluginModKey);
            var allocator = new TextFileFormKeyAllocatorv2(mod);

            Assert.Throws<InvalidOperationException>(() =>
            {
                allocator.WriteToFolder(folder.Dir.Path);
            });
        }
    }
}
