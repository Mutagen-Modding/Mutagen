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

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class AutoSplitModWriterTests
{
    [Theory, MutagenModAutoData]
    public void BasicAutoSplitTest(SplitTestPayload payload, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);

        // 5 x 70 = 350 masters, well past the 254 limit, so the write auto-splits.
        var originalFormLists = new List<IFormListGetter>();
        for (uint i = 0; i < 5; i++)
        {
            var flst = payload.CreateFormListWithContents(70);
            originalFormLists.Add(flst);
        }

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        sut.Write<ISkyrimMod, ISkyrimModGetter>(
            payload.Mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // First split keeps the base name; subsequent files are suffixed _2, _3, ...
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(payload.Mod.ModKey.FileName);
        var extension = Path.GetExtension(payload.Mod.ModKey.FileName);
        var splitFile1Path = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);
        var splitFile2Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");
        var splitFile3Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_3{extension}");

        fileSystem.File.Exists(splitFile1Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile2Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile3Path).ShouldBeFalse();

        var splitMod1 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile1Path,
            (SkyrimRelease)payload.Mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });
        var splitMod2 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile2Path,
            (SkyrimRelease)payload.Mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });

        var reimportedFormLists = new List<IFormListGetter>();
        reimportedFormLists.AddRange(splitMod1.FormLists);
        reimportedFormLists.AddRange(splitMod2.FormLists);

        reimportedFormLists.Count.ShouldBe(originalFormLists.Count);

        var expectedEdids = payload.GetExpectedEdids().ToHashSet();
        var reimportedEdids = reimportedFormLists.Select(f => f.EditorID).ToHashSet();

        reimportedEdids.ShouldBe(expectedEdids);

        foreach (var originalFormList in originalFormLists)
        {
            var reimportedFormList = reimportedFormLists.FirstOrDefault(f => f.EditorID == originalFormList.EditorID);
            reimportedFormList.ShouldNotBeNull();

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
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        sut.Write<ISkyrimMod, ISkyrimModGetter>(
            mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        fileSystem.File.Exists(outputPath).ShouldBeTrue();

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(mod.ModKey.FileName);
        var extension = Path.GetExtension(mod.ModKey.FileName);

        fileSystem.File.Exists(Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}")).ShouldBeFalse();
    }

    [Theory, MutagenModAutoData]
    public void CleansUpOldSplitFilesWithGaps(SplitTestPayload payload, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(payload.Mod.ModKey.FileName);
        var extension = Path.GetExtension(payload.Mod.ModKey.FileName);

        // Stale files with non-contiguous numbering (_3, _5, _7) from a previous larger export; cleanup must
        // remove all of them even though _2, _4, _6 are absent.
        var staleFile3 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_3{extension}");
        var staleFile5 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_5{extension}");
        var staleFile7 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_7{extension}");

        fileSystem.File.WriteAllText(staleFile3, "stale");
        fileSystem.File.WriteAllText(staleFile5, "stale");
        fileSystem.File.WriteAllText(staleFile7, "stale");

        // 5 x 70 = 350 masters, past the 254 limit, so this splits into 2 files.
        for (uint i = 0; i < 5; i++)
        {
            payload.CreateFormListWithContents(70);
        }

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());

        sut.Write<ISkyrimMod, ISkyrimModGetter>(
            payload.Mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        var splitFile1Path = Path.Combine(existingOutputDirectory.Path, payload.Mod.ModKey.FileName);
        var splitFile2Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        fileSystem.File.Exists(splitFile1Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile2Path).ShouldBeTrue();

        fileSystem.File.Exists(staleFile3).ShouldBeFalse();
        fileSystem.File.Exists(staleFile5).ShouldBeFalse();
        fileSystem.File.Exists(staleFile7).ShouldBeFalse();
    }

    #region Load Order with Split Tests

    /// <summary>
    /// MiscItem + FormList_A (200 MasterA refs) land in cluster 1; FormList_B (200 MasterB refs) in cluster 2.
    /// FormList_B also references MiscItem, so after the split Synthesis_2.esp depends on Synthesis.esp as a
    /// master — the cross-split dependency these load-order tests hinge on.
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
        formListB.Items.Add(miscItem.FormKey);

        return mod;
    }

    // Excludes the mod itself, mirroring Synthesis's PostRunProcessor, which trims the load order before writing.
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

    [Theory, MutagenModAutoData]
    public void WriteWithSplit_WithLoadOrder_ExcludingOutputMod(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputDir = existingOutputDirectory.Path;

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        var param = CreateParamsWithLoadOrder(mod, fileSystem);

        var sut = new AutoSplitModWriter(new MultiModFileSplitter());
        sut.Write<ISkyrimMod, ISkyrimModGetter>(mod, outputPath, param);

        var split1 = SkyrimMod.CreateFromBinaryOverlay(
            outputPath, SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });
        var split2 = SkyrimMod.CreateFromBinaryOverlay(
            Path.Combine(outputDir, "Synthesis_2.esp"), SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });

        var allFormLists = split1.FormLists.Concat(split2.FormLists).ToList();
        var allMiscItems = split1.MiscItems.Concat(split2.MiscItems).ToList();

        allFormLists.Count.ShouldBe(2);
        allMiscItems.Count.ShouldBe(1);

        allFormLists.Select(f => f.EditorID).ToHashSet()
            .ShouldBe(new HashSet<string?> { "ClusterAList", "ClusterBList" });
        allMiscItems.First().EditorID.ShouldBe("CrossRefTarget");
    }

    [Theory, MutagenModAutoData]
    public void WriteWithSplit_SplitKeysInLoadOrder_CorrectOrder_MasterOrderPreserved(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputDir = existingOutputDirectory.Path;

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

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

        var split2 = SkyrimMod.CreateFromBinaryOverlay(
            Path.Combine(outputDir, "Synthesis_2.esp"), SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });

        var mastersList = split2.MasterReferences.Select(m => m.Master).ToList();
        mastersList.ShouldContain(modKey);

        var synthIndex = mastersList.IndexOf(modKey);
        synthIndex.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Theory, MutagenModAutoData]
    public void WriteWithSplit_SplitKeysInLoadOrder_ReversedOrder_Throws(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputDir = existingOutputDirectory.Path;

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        // Split keys reversed (Synthesis_2 before Synthesis): the base must load before its split siblings.
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

    [Theory, MutagenModAutoData]
    public void WriteWithSplit_OnlyBaseModInLoadOrder_SplitKeyMissing_StillSucceeds(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputDir = existingOutputDirectory.Path;

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        var externalMasters = CollectExternalMasters(mod);
        var loadOrderKeys = externalMasters.ToList();
        loadOrderKeys.Add(new ModKey("Synthesis", ModType.Plugin));
        // Synthesis_2.esp intentionally omitted — the writer must augment the load order with missing split keys.

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
    }

    [Theory, MutagenModAutoData]
    public void WriteWithSplit_SplitKeysInLoadOrder_CorrectOrder_ExternalMastersNotReordered(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputDir = existingOutputDirectory.Path;

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithCrossClusterReferences(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

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

        var split1 = SkyrimMod.CreateFromBinaryOverlay(
            outputPath, SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });

        var split1Masters = split1.MasterReferences.Select(m => m.Master).ToList();

        // Each master must appear in load-order position.
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

    [Theory, MutagenModAutoData]
    public void WriteWithSplit_ThreeSplits_2After3InLoadOrder_Throws(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputDir = existingOutputDirectory.Path;

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithThreeClusters(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

        // _3 placed before _2, so a later split sibling loads before an earlier one — must be rejected.
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

    [Theory, MutagenModAutoData]
    public void WriteWithSplit_ThreeSplits_CorrectOrder_Succeeds(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var outputDir = existingOutputDirectory.Path;

        var modKey = new ModKey("Synthesis", ModType.Plugin);
        var mod = CreateModWithThreeClusters(modKey);
        var outputPath = Path.Combine(outputDir, modKey.FileName);

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

