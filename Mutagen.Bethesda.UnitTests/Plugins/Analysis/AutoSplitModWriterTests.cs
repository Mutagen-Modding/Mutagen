using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FormList = Mutagen.Bethesda.Skyrim.FormList;
using MiscItem = Mutagen.Bethesda.Skyrim.MiscItem;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class AutoSplitModWriterTests
{
    #region Util

    public class Payload
    {
        private readonly Func<FormKey> _formKeyGen;
        private readonly Func<string> _edidFunc;
        public SkyrimMod Mod { get; }

        // just to be able to create unique edids
        private int lastEdidIndex = 0;

        /// <summary>
        /// Stores EDIDs of generated forms in the input class, to be able to track them in the generated files
        /// </summary>
        private HashSet<string> expectedEdids = new();

        public Payload(SkyrimMod mod, Func<ModKey> modKeyGen, Func<string> edidFunc)
        {
            _formKeyGen = () => new FormKey(modKeyGen(), 0x800);
            _edidFunc = edidFunc;
            Mod = mod;
        }

        public string GetNewEdid(string baseEdid, bool addToExpected = true)
        {
            var newEdid = baseEdid + lastEdidIndex;
            lastEdidIndex++;
            if (addToExpected)
            {
                expectedEdids.Add(newEdid);
            }
            return newEdid;
        }

        /// <summary>
        /// Generates MISC items from NOT within the current file
        /// </summary>
        public void FillFormListWithRemoteRecords(FormList flst, int numFiles)
        {
            for (uint i = 0; i < numFiles; i++)
            {
                flst.Items.Add(_formKeyGen());
            }
        }

        public FormList CreateFormListWithContents(int numFiles)
        {
            var flst = Mod.FormLists.AddNew();
            // Set EditorID to FormKey so we can track it after split
            flst.EditorID = flst.FormKey.ToString();
            FillFormListWithRemoteRecords(flst, numFiles);
            expectedEdids.Add(flst.EditorID);
            return flst;
        }

        public IEnumerable<string> GetExpectedEdids() => expectedEdids;
    }

    private static HashSet<ModKey> GetAllMasters(IModGetter mod)
    {
        var recs = mod.EnumerateMajorRecords();
        var result = new HashSet<ModKey>();

        foreach (var majorRecord in recs)
        {
            if (mod.ModKey != majorRecord.FormKey.ModKey)
            {
                result.Add(majorRecord.FormKey.ModKey);
            }
            var formLinks = majorRecord.EnumerateFormLinks();
            foreach (var formLink in formLinks)
            {
                result.Add(formLink.FormKey.ModKey);
            }
        }

        return result;
    }
    #endregion

    [Theory, MutagenModAutoData]
    public void BasicAutoSplitTest(Payload payload, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Combine mod's ModKey and DirectoryPath to create a valid output path
        var outputPath = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);

        // Create a mod that exceeds master limit
        var originalFormLists = new List<IFormListGetter>();
        for (uint i = 0; i < 5; i++)
        {
            var flst = payload.CreateFormListWithContents(70); // Each has 70 masters
            originalFormLists.Add(flst);
        }

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        // This should trigger auto-split since we have way more than 254 masters
        sut.Write<ISkyrimMod, ISkyrimModGetter>(
            payload.Mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Verify split files were created (first file keeps original name, then _2, _3, etc.)
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(payload.Mod.ModKey.FileName);
        var extension = Path.GetExtension(payload.Mod.ModKey.FileName);
        var splitFile1Path = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);  // Base file (no suffix)
        var splitFile2Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");
        var splitFile3Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_3{extension}");

        fileSystem.File.Exists(splitFile1Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile2Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile3Path).ShouldBeFalse();

        // Re-import the split mods and verify content
        var splitMod1 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile1Path,
            (SkyrimRelease)payload.Mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });
        var splitMod2 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile2Path,
            (SkyrimRelease)payload.Mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });

        // Collect all FormLists from both split mods
        var reimportedFormLists = new List<IFormListGetter>();
        reimportedFormLists.AddRange(splitMod1.FormLists);
        reimportedFormLists.AddRange(splitMod2.FormLists);

        // Verify we have the same number of FormLists
        reimportedFormLists.Count.ShouldBe(originalFormLists.Count);

        // Verify each FormList by EditorID (which is set to FormKey)
        var expectedEdids = payload.GetExpectedEdids().ToHashSet();
        var reimportedEdids = reimportedFormLists.Select(f => f.EditorID).ToHashSet();

        reimportedEdids.ShouldBe(expectedEdids);

        // Verify the content of each FormList matches
        foreach (var originalFormList in originalFormLists)
        {
            var reimportedFormList = reimportedFormLists.FirstOrDefault(f => f.EditorID == originalFormList.EditorID);
            reimportedFormList.ShouldNotBeNull();

            // Verify the items are the same
            reimportedFormList.Items.Count.ShouldBe(originalFormList.Items.Count);
            for (int i = 0; i < originalFormList.Items.Count; i++)
            {
                reimportedFormList.Items[i].FormKey.ShouldBe(originalFormList.Items[i].FormKey);
            }
        }
    }

    [Theory, MutagenModAutoData]
    public void NoSplitWhenNotNeededTest(SkyrimMod mod, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Combine mod's ModKey and DirectoryPath to create a valid output path
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        // Write should succeed without splitting
        sut.Write<ISkyrimMod, ISkyrimModGetter>(
            mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // File should be written
        fileSystem.File.Exists(outputPath).ShouldBeTrue();

        // No split files should exist - check for _2 suffix (base file is the original, so only check _2)
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(mod.ModKey.FileName);
        var extension = Path.GetExtension(mod.ModKey.FileName);

        fileSystem.File.Exists(Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}")).ShouldBeFalse();
    }

    [Theory, MutagenModAutoData]
    public void CleansUpOldSplitFilesWithGaps(Payload payload, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(payload.Mod.ModKey.FileName);
        var extension = Path.GetExtension(payload.Mod.ModKey.FileName);

        // Create stale split files with gaps to simulate a previous export with more files
        var staleFile3 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_3{extension}");
        var staleFile5 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_5{extension}");
        var staleFile7 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_7{extension}");

        fileSystem.File.WriteAllText(staleFile3, "stale");
        fileSystem.File.WriteAllText(staleFile5, "stale");
        fileSystem.File.WriteAllText(staleFile7, "stale");

        // Create a mod that will split into 2 files (need >254 masters to trigger split)
        for (uint i = 0; i < 5; i++)
        {
            payload.CreateFormListWithContents(70); // 5 x 70 = 350 masters
        }

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        sut.Write<ISkyrimMod, ISkyrimModGetter>(
            payload.Mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Verify split files were created (base file + _2)
        var splitFile1Path = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);  // Base file (no suffix)
        var splitFile2Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        fileSystem.File.Exists(splitFile1Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile2Path).ShouldBeTrue();

        // Verify stale files were cleaned up despite gaps in numbering
        fileSystem.File.Exists(staleFile3).ShouldBeFalse();
        fileSystem.File.Exists(staleFile5).ShouldBeFalse();
        fileSystem.File.Exists(staleFile7).ShouldBeFalse();
    }

    #region Load Order with Split Tests

    /// <summary>
    /// Helper to create a mod with cross-cluster internal references that will trigger
    /// the MissingModException when written with a load order that excludes the output mod.
    ///
    /// Creates:
    /// - MiscItem: no external refs → lands in cluster 1
    /// - FormList_A: 200 unique external masters → cluster 1
    /// - FormList_B: 200 different external masters + ref to MiscItem → cluster 2
    ///
    /// When split, FormList_B (in Synthesis_2.esp) has a FormLink back to MiscItem
    /// (in Synthesis.esp), creating a cross-split master dependency.
    /// </summary>
    private static SkyrimMod CreateModWithCrossClusterReferences(ModKey modKey)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);

        var miscItem = mod.MiscItems.AddNew();
        miscItem.EditorID = "CrossRefTarget";

        var formListA = mod.FormLists.AddNew();
        formListA.EditorID = "ClusterAList";
        for (int i = 0; i < 200; i++)
        {
            formListA.Items.Add(new FormKey(new ModKey($"MasterA_{i:D3}", ModType.Plugin), 0x800));
        }

        var formListB = mod.FormLists.AddNew();
        formListB.EditorID = "ClusterBList";
        for (int i = 0; i < 200; i++)
        {
            formListB.Items.Add(new FormKey(new ModKey($"MasterB_{i:D3}", ModType.Plugin), 0x800));
        }
        // Cross-reference: points to MiscItem's FormKey (ModKey = mod's own key)
        // After split, this makes Synthesis_2.esp depend on Synthesis.esp as a master
        formListB.Items.Add(miscItem.FormKey);

        return mod;
    }

    /// <summary>
    /// Builds a load order from all external masters in the mod, excluding the mod itself.
    /// This matches Synthesis PostRunProcessor behavior where the load order is trimmed
    /// before the output patch.
    /// </summary>
    private static BinaryWriteParameters CreateParamsWithLoadOrder(
        SkyrimMod mod, IFileSystem fileSystem)
    {
        var masterKeys = new HashSet<ModKey>();
        foreach (var rec in mod.EnumerateMajorRecords())
        {
            foreach (var link in rec.EnumerateFormLinks())
            {
                if (link.FormKey.ModKey != mod.ModKey)
                    masterKeys.Add(link.FormKey.ModKey);
            }
        }

        var orderedMasters = masterKeys.OrderBy(k => k.FileName.String).ToList();

        return BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            MastersListOrdering = new MastersListOrderingByLoadOrder(orderedMasters)
        };
    }

    [Fact]
    public void WriteWithSplit_WithLoadOrder_ExcludingOutputMod()
    {
        var fileSystem = new MockFileSystem();
        var outputDir = "C:/Output";
        fileSystem.Directory.CreateDirectory(outputDir);

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        var param = CreateParamsWithLoadOrder(mod, fileSystem);

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());
        sut.Write<ISkyrimMod, ISkyrimModGetter>(mod, outputPath, param);

        // Re-import split files and verify all records are present
        var split1 = SkyrimMod.CreateFromBinaryOverlay(
            outputPath, SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });
        var split2 = SkyrimMod.CreateFromBinaryOverlay(
            Path.Combine(outputDir, "Synthesis_2.esp"), SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });

        var allFormLists = split1.FormLists.Concat(split2.FormLists).ToList();
        var allMiscItems = split1.MiscItems.Concat(split2.MiscItems).ToList();

        // Original mod had 2 FormLists and 1 MiscItem
        allFormLists.Count.ShouldBe(2);
        allMiscItems.Count.ShouldBe(1);

        allFormLists.Select(f => f.EditorID).ToHashSet()
            .ShouldBe(new HashSet<string?> { "ClusterAList", "ClusterBList" });
        allMiscItems.First().EditorID.ShouldBe("CrossRefTarget");
    }

    [Fact]
    public void WriteWithSplit_SplitKeysInLoadOrder_CorrectOrder_MasterOrderPreserved()
    {
        var fileSystem = new MockFileSystem();
        var outputDir = "C:/Output";
        fileSystem.Directory.CreateDirectory(outputDir);

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        // Build load order with external masters, then split keys in correct order:
        // ... MasterA_xxx, MasterB_xxx ..., Synthesis.esp, Synthesis_2.esp
        var externalMasters = CollectExternalMasters(mod);
        var loadOrderKeys = externalMasters.ToList();
        loadOrderKeys.Add(new ModKey("Synthesis", ModType.Plugin));
        loadOrderKeys.Add(new ModKey("Synthesis_2", ModType.Plugin));

        var param = BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrderKeys)
        };

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());
        sut.Write<ISkyrimMod, ISkyrimModGetter>(mod, outputPath, param);

        // Verify split2 has Synthesis.esp as a master and it appears before any
        // other split sibling in the master list
        var split2 = SkyrimMod.CreateFromBinaryOverlay(
            Path.Combine(outputDir, "Synthesis_2.esp"), SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });

        var mastersList = split2.MasterReferences.Select(m => m.Master).ToList();
        mastersList.ShouldContain(modKey);

        // Synthesis.esp should appear as a master (cross-cluster ref)
        var synthIndex = mastersList.IndexOf(modKey);
        synthIndex.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void WriteWithSplit_SplitKeysInLoadOrder_ReversedOrder_Throws()
    {
        var fileSystem = new MockFileSystem();
        var outputDir = "C:/Output";
        fileSystem.Directory.CreateDirectory(outputDir);

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        // Build load order with split keys in REVERSED order:
        // ... MasterA_xxx, MasterB_xxx ..., Synthesis_2.esp, Synthesis.esp
        var externalMasters = CollectExternalMasters(mod);
        var loadOrderKeys = externalMasters.ToList();
        loadOrderKeys.Add(new ModKey("Synthesis_2", ModType.Plugin));
        loadOrderKeys.Add(new ModKey("Synthesis", ModType.Plugin));

        var param = BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrderKeys)
        };

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        var ex = Should.Throw<SplitModException>(() =>
        {
            sut.Write<ISkyrimMod, ISkyrimModGetter>(mod, outputPath, param);
        });

        ex.Message.ShouldContain("out of order");
    }

    [Fact]
    public void WriteWithSplit_OnlyBaseModInLoadOrder_SplitKeyMissing_StillSucceeds()
    {
        var fileSystem = new MockFileSystem();
        var outputDir = "C:/Output";
        fileSystem.Directory.CreateDirectory(outputDir);

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        // Load order includes the base mod but NOT Synthesis_2.esp
        var externalMasters = CollectExternalMasters(mod);
        var loadOrderKeys = externalMasters.ToList();
        loadOrderKeys.Add(new ModKey("Synthesis", ModType.Plugin));
        // Synthesis_2.esp intentionally omitted

        var param = BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrderKeys)
        };

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        // Should still succeed — the augmentation adds missing split keys
        Should.NotThrow(() =>
        {
            sut.Write<ISkyrimMod, ISkyrimModGetter>(mod, outputPath, param);
        });

        fileSystem.File.Exists(outputPath).ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(outputDir, "Synthesis_2.esp")).ShouldBeTrue();
    }

    [Fact]
    public void WriteWithSplit_SplitKeysInLoadOrder_CorrectOrder_ExternalMastersNotReordered()
    {
        var fileSystem = new MockFileSystem();
        var outputDir = "C:/Output";
        fileSystem.Directory.CreateDirectory(outputDir);

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        // Build load order: external masters in specific order, then split keys
        var externalMasters = CollectExternalMasters(mod);
        var loadOrderKeys = externalMasters.ToList();
        loadOrderKeys.Add(new ModKey("Synthesis", ModType.Plugin));
        loadOrderKeys.Add(new ModKey("Synthesis_2", ModType.Plugin));

        var param = BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrderKeys)
        };

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());
        sut.Write<ISkyrimMod, ISkyrimModGetter>(mod, outputPath, param);

        // Verify that external masters in each split file follow the load order
        var split1 = SkyrimMod.CreateFromBinaryOverlay(
            outputPath, SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });

        var split1Masters = split1.MasterReferences.Select(m => m.Master).ToList();

        // Masters should be sorted by their position in the load order
        for (int i = 1; i < split1Masters.Count; i++)
        {
            var prevIndex = loadOrderKeys.IndexOf(split1Masters[i - 1]);
            var currIndex = loadOrderKeys.IndexOf(split1Masters[i]);
            prevIndex.ShouldBeLessThan(currIndex,
                $"Master {split1Masters[i - 1]} (LO index {prevIndex}) should appear before " +
                $"{split1Masters[i]} (LO index {currIndex}) in the master list");
        }
    }

    /// <summary>
    /// Creates a mod that splits into 3 clusters, with cross-references between them.
    /// Cluster 1: MiscItem + FormList_A (200 MasterA refs)
    /// Cluster 2: FormList_B (200 MasterB refs + ref to MiscItem)
    /// Cluster 3: FormList_C (200 MasterC refs + ref to MiscItem)
    /// </summary>
    private static SkyrimMod CreateModWithThreeClusters(ModKey modKey)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);

        var miscItem = mod.MiscItems.AddNew();
        miscItem.EditorID = "CrossRefTarget";

        var formListA = mod.FormLists.AddNew();
        formListA.EditorID = "ClusterAList";
        for (int i = 0; i < 200; i++)
            formListA.Items.Add(new FormKey(new ModKey($"MasterA_{i:D3}", ModType.Plugin), 0x800));

        var formListB = mod.FormLists.AddNew();
        formListB.EditorID = "ClusterBList";
        for (int i = 0; i < 200; i++)
            formListB.Items.Add(new FormKey(new ModKey($"MasterB_{i:D3}", ModType.Plugin), 0x800));
        formListB.Items.Add(miscItem.FormKey);

        var formListC = mod.FormLists.AddNew();
        formListC.EditorID = "ClusterCList";
        for (int i = 0; i < 200; i++)
            formListC.Items.Add(new FormKey(new ModKey($"MasterC_{i:D3}", ModType.Plugin), 0x800));
        formListC.Items.Add(miscItem.FormKey);

        return mod;
    }

    [Fact]
    public void WriteWithSplit_ThreeSplits_2After3InLoadOrder_Throws()
    {
        var fileSystem = new MockFileSystem();
        var outputDir = "C:/Output";
        fileSystem.Directory.CreateDirectory(outputDir);

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithThreeClusters(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        // Load order has _3 before _2: Synthesis.esp, Synthesis_3.esp, Synthesis_2.esp
        var externalMasters = CollectExternalMasters(mod);
        var loadOrderKeys = externalMasters.ToList();
        loadOrderKeys.Add(new ModKey("Synthesis", ModType.Plugin));
        loadOrderKeys.Add(new ModKey("Synthesis_3", ModType.Plugin));
        loadOrderKeys.Add(new ModKey("Synthesis_2", ModType.Plugin));

        var param = BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrderKeys)
        };

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        var ex = Should.Throw<SplitModException>(() =>
        {
            sut.Write<ISkyrimMod, ISkyrimModGetter>(mod, outputPath, param);
        });

        ex.Message.ShouldContain("out of order");
    }

    [Fact]
    public void WriteWithSplit_ThreeSplits_CorrectOrder_Succeeds()
    {
        var fileSystem = new MockFileSystem();
        var outputDir = "C:/Output";
        fileSystem.Directory.CreateDirectory(outputDir);

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithThreeClusters(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        // Load order has split keys in correct order
        var externalMasters = CollectExternalMasters(mod);
        var loadOrderKeys = externalMasters.ToList();
        loadOrderKeys.Add(new ModKey("Synthesis", ModType.Plugin));
        loadOrderKeys.Add(new ModKey("Synthesis_2", ModType.Plugin));
        loadOrderKeys.Add(new ModKey("Synthesis_3", ModType.Plugin));

        var param = BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrderKeys)
        };

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        Should.NotThrow(() =>
        {
            sut.Write<ISkyrimMod, ISkyrimModGetter>(mod, outputPath, param);
        });

        fileSystem.File.Exists(outputPath).ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(outputDir, "Synthesis_2.esp")).ShouldBeTrue();
        fileSystem.File.Exists(Path.Combine(outputDir, "Synthesis_3.esp")).ShouldBeTrue();
    }

    /// <summary>
    /// Collects all external ModKeys referenced by the mod, sorted alphabetically.
    /// </summary>
    private static List<ModKey> CollectExternalMasters(SkyrimMod mod)
    {
        var masterKeys = new HashSet<ModKey>();
        foreach (var rec in mod.EnumerateMajorRecords())
        {
            foreach (var link in rec.EnumerateFormLinks())
            {
                if (link.FormKey.ModKey != mod.ModKey)
                    masterKeys.Add(link.FormKey.ModKey);
            }
        }
        return masterKeys.OrderBy(k => k.FileName.String).ToList();
    }

    #endregion
}

