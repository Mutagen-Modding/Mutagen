using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using System.IO.Abstractions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

/// <summary>
/// Tests that split mod import handles duplicate parent records (CELL, WRLD, DIAL, QUST)
/// gracefully across all games, while still rejecting duplicate non-parent records.
///
/// When mods are split across files, GetOrAddAsOverride on child records (placed objects,
/// dialog responses) implicitly pulls parent container records into each split file,
/// causing the same parent record to appear in multiple files.
/// </summary>
public class SplitModDuplicateParentTests
{
    private static readonly ModKey TestModKey = new("SplitTest", ModType.Plugin);
    private static readonly ModKey SplitModKey2 = new("SplitTest_2", ModType.Plugin);
    private static readonly ModKey MasterModKey = new("Master", ModType.Master);
    private static readonly ModKey SplitModKey3 = new("SplitTest_3", ModType.Plugin);
    private static readonly FormKey SharedFormKey = new(MasterModKey, 0x3C);
    private static readonly FormKey SharedCellFormKey = new(MasterModKey, 0x3D);
    private static readonly FormKey ChildFormKey1 = new(TestModKey, 0x800);
    private static readonly FormKey ChildFormKey2 = new(TestModKey, 0x801);
    private static readonly FormKey ChildFormKey3 = new(TestModKey, 0x802);

    #region Skyrim - CELL, WRLD, DIAL

    [Theory, MutagenModAutoData]
    public void Skyrim_DuplicateCell_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        AddSkyrimCell(mod1, SharedFormKey, ChildFormKey1, "PlacedA");
        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        AddSkyrimCell(mod2, SharedFormKey, ChildFormKey2, "PlacedB");

        ImportShouldSucceed(mod1, mod2, GameRelease.SkyrimSE, existingOutputDirectory, fileSystem, result =>
        {
            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.ICellGetter>().ToList();
            cells.ShouldContain(c => c.FormKey == SharedFormKey);
            var placed = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.IPlacedObjectGetter>().ToList();
            placed.Count.ShouldBe(2);
            placed.ShouldContain(p => p.EditorID == "PlacedA");
            placed.ShouldContain(p => p.EditorID == "PlacedB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Skyrim_DuplicateWorldspace_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        mod1.Worldspaces.Add(new Mutagen.Bethesda.Skyrim.Worldspace(SharedFormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Skyrim.Cell(ChildFormKey1, SkyrimRelease.SkyrimSE) { EditorID = "TopCellA" }
        });
        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        mod2.Worldspaces.Add(new Mutagen.Bethesda.Skyrim.Worldspace(SharedFormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Skyrim.Cell(ChildFormKey2, SkyrimRelease.SkyrimSE) { EditorID = "TopCellB" }
        });

        ImportShouldSucceed(mod1, mod2, GameRelease.SkyrimSE, existingOutputDirectory, fileSystem, result =>
        {
            var worldspaces = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.IWorldspaceGetter>().ToList();
            worldspaces.Count.ShouldBe(1);
            worldspaces.ShouldContain(w => w.FormKey == SharedFormKey);
            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.ICellGetter>().ToList();
            cells.Count.ShouldBe(2);
            cells.ShouldContain(c => c.EditorID == "TopCellA");
            cells.ShouldContain(c => c.EditorID == "TopCellB");
        });
    }

    /// <summary>
    /// The key scenario: WorldspaceY contains CellX, CellX contains PlacedObjA and PlacedObjB.
    /// When A and B are written to different split files, both files must include WorldspaceY and CellX
    /// as parent containers. On re-import, CellX should not be duplicated within WorldspaceY.
    /// </summary>
    [Theory, MutagenModAutoData]
    public void Skyrim_WorldspaceWithSharedCell_CellNotDuplicated(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // mod1: WorldspaceY -> CellX -> PlacedObjA
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod1, SharedFormKey, SharedCellFormKey, ChildFormKey1, "PlacedA");

        // mod2: WorldspaceY -> CellX -> PlacedObjB
        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod2, SharedFormKey, SharedCellFormKey, ChildFormKey2, "PlacedB");

        ImportShouldSucceed(mod1, mod2, GameRelease.SkyrimSE, existingOutputDirectory, fileSystem, result =>
        {
            // WorldspaceY should exist exactly once
            var worldspaces = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.IWorldspaceGetter>().ToList();
            worldspaces.Count.ShouldBe(1);
            worldspaces.ShouldContain(w => w.FormKey == SharedFormKey);

            // CellX should exist exactly once (not duplicated)
            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.ICellGetter>().ToList();
            cells.Count(c => c.FormKey == SharedCellFormKey).ShouldBe(1);

            // Both placed objects should be accessible
            var placed = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.IPlacedObjectGetter>().ToList();
            placed.Count.ShouldBe(2);
            placed.ShouldContain(p => p.EditorID == "PlacedA");
            placed.ShouldContain(p => p.EditorID == "PlacedB");
        });
    }

    /// <summary>
    /// Three-way split: WorldspaceY -> CellX with placed objects spread across three files.
    /// </summary>
    [Theory, MutagenModAutoData]
    public void Skyrim_WorldspaceWithSharedCell_ThreeWaySplit_CellNotDuplicated(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod1, SharedFormKey, SharedCellFormKey, ChildFormKey1, "PlacedA");

        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod2, SharedFormKey, SharedCellFormKey, ChildFormKey2, "PlacedB");

        var mod3 = new SkyrimMod(SplitModKey3, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod3, SharedFormKey, SharedCellFormKey, ChildFormKey3, "PlacedC");

        ImportShouldSucceed3(mod1, mod2, mod3, GameRelease.SkyrimSE, existingOutputDirectory, fileSystem, result =>
        {
            var worldspaces = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.IWorldspaceGetter>().ToList();
            worldspaces.Count.ShouldBe(1);

            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.ICellGetter>().ToList();
            cells.Count(c => c.FormKey == SharedCellFormKey).ShouldBe(1);

            var placed = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.IPlacedObjectGetter>().ToList();
            placed.Count.ShouldBe(3);
            placed.ShouldContain(p => p.EditorID == "PlacedA");
            placed.ShouldContain(p => p.EditorID == "PlacedB");
            placed.ShouldContain(p => p.EditorID == "PlacedC");
        });
    }

    /// <summary>
    /// Verify that accessing cells through the Worldspace property hierarchy (not just
    /// EnumerateMajorRecords) also works correctly - CellX should appear once with all
    /// placed objects merged.
    /// </summary>
    [Theory, MutagenModAutoData]
    public void Skyrim_WorldspaceWithSharedCell_AccessViaHierarchy(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod1, SharedFormKey, SharedCellFormKey, ChildFormKey1, "PlacedA");

        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod2, SharedFormKey, SharedCellFormKey, ChildFormKey2, "PlacedB");

        ImportShouldSucceed(mod1, mod2, GameRelease.SkyrimSE, existingOutputDirectory, fileSystem, result =>
        {
            // Access through the property hierarchy via the Worldspaces group
            var skyrimResult = (Mutagen.Bethesda.Skyrim.ISkyrimModGetter)result;
            var worldspace = skyrimResult.Worldspaces
                .Single(w => w.FormKey == SharedFormKey);

            // Collect all cells reachable from the worldspace's SubCells
            var cellsInWorldspace = worldspace.SubCells
                .SelectMany(block => block.Items)
                .SelectMany(subBlock => subBlock.Items)
                .ToList();

            // CellX should not be duplicated
            cellsInWorldspace.Count(c => c.FormKey == SharedCellFormKey).ShouldBe(1);

            // The single CellX should have both placed objects
            var cell = cellsInWorldspace.Single(c => c.FormKey == SharedCellFormKey);
            var allPlaced = cell.Persistent
                .Concat(cell.Temporary)
                .OfType<Mutagen.Bethesda.Skyrim.IPlacedObjectGetter>()
                .ToList();
            allPlaced.Count.ShouldBe(2);
            allPlaced.ShouldContain(p => p.EditorID == "PlacedA");
            allPlaced.ShouldContain(p => p.EditorID == "PlacedB");
        });
    }

    /// <summary>
    /// The actual bug: when a Cell is overridden in multiple split files,
    /// EnumerateMajorRecordContexts returns the same Cell context multiple times (once per
    /// source mod that contains it). A patcher iterating contexts and calling GetOrAddAsOverride
    /// on each pulls the same Cell into its output mod twice, causing a duplicate FormKey error
    /// when the output mod is written.
    /// </summary>
    [Theory, MutagenModAutoData]
    public void Skyrim_SharedCellInMultipleSplitFiles_ContextsNotDuplicated(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Both split files override the same Cell hierarchy, each with a different placed object.
        // This simulates two patcher passes where each touched the same cell.
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod1, SharedFormKey, SharedCellFormKey, ChildFormKey1, "PlacedA");

        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod2, SharedFormKey, SharedCellFormKey, ChildFormKey2, "PlacedB");

        var file1 = WriteSplitFile(mod1, existingOutputDirectory, fileSystem, 1);
        var file2 = WriteSplitFile(mod2, existingOutputDirectory, fileSystem, 2);

        using var imported = ModFactory.ImportMultiFileGetter(
            TestModKey,
            new[] { (ModPath)file1, (ModPath)file2 },
            new[] { MasterModKey },
            GameRelease.SkyrimSE,
            new BinaryReadParameters() { FileSystem = fileSystem });

        var skyrimImported = (Mutagen.Bethesda.Skyrim.ISkyrimModGetter)imported;
        var cache = skyrimImported.ToImmutableLinkCache();

        // EnumerateMajorRecordContexts<ICell> should NOT return CellX twice
        var cellContexts = skyrimImported
            .EnumerateMajorRecordContexts<Mutagen.Bethesda.Skyrim.ICell, Mutagen.Bethesda.Skyrim.ICellGetter>(linkCache: cache)
            .ToList();

        var cellXContexts = cellContexts.Where(c => c.Record.FormKey == SharedCellFormKey).ToList();
        cellXContexts.Count.ShouldBe(1, "Cell context should be returned exactly once, not duplicated across split files");

        // Same check for simple contexts (used by some patchers)
        var simpleContexts = imported.EnumerateMajorRecordSimpleContexts().ToList();
        var dupes = simpleContexts.GroupBy(c => c.Record.FormKey).Where(g => g.Count() > 1).ToList();
        dupes.ShouldBeEmpty($"EnumerateMajorRecordSimpleContexts returned duplicate FormKeys: {string.Join(", ", dupes.Select(g => g.Key))}");
    }

    /// <summary>
    /// Reproduces the actual bug: import split files where WorldspaceY -> CellX has placed objects
    /// across both files. Use context.GetOrAddAsOverride on both placed objects (as a Synthesis
    /// patcher would). This pulls CellX into the output mod as a parent container — but if it gets
    /// pulled in twice (once from each source file's copy of the Worldspace), writing the output
    /// mod throws "Two records with the same FormKey" for CellX.
    /// </summary>
    [Theory, MutagenModAutoData]
    public void Skyrim_WorldspaceWithSharedCell_GetOrAddAsOverride_NoDuplicateCellOnWrite(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Set up split files: WorldspaceY -> CellX -> PlacedA in file1, PlacedB in file2
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod1, SharedFormKey, SharedCellFormKey, ChildFormKey1, "PlacedA");

        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        AddSkyrimWorldspaceCell(mod2, SharedFormKey, SharedCellFormKey, ChildFormKey2, "PlacedB");

        var file1 = WriteSplitFile(mod1, existingOutputDirectory, fileSystem, 1);
        var file2 = WriteSplitFile(mod2, existingOutputDirectory, fileSystem, 2);

        using var imported = ModFactory.ImportMultiFileGetter(
            TestModKey,
            new[] { (ModPath)file1, (ModPath)file2 },
            new[] { MasterModKey },
            GameRelease.SkyrimSE,
            new BinaryReadParameters() { FileSystem = fileSystem });

        var skyrimImported = (Mutagen.Bethesda.Skyrim.ISkyrimModGetter)imported;

        // Simulate a Synthesis patcher: create output mod, use context-based GetOrAddAsOverride
        // on both placed objects (which pulls in CellX as a parent each time)
        var outputMod = new SkyrimMod(new ModKey("Output", ModType.Plugin), SkyrimRelease.SkyrimSE);
        var cache = skyrimImported.ToImmutableLinkCache();

        var contexts = skyrimImported
            .EnumerateMajorRecordContexts<
                Mutagen.Bethesda.Skyrim.IPlacedObject,
                Mutagen.Bethesda.Skyrim.IPlacedObjectGetter>(linkCache: cache)
            .ToList();

        contexts.Count.ShouldBe(2);

        // GetOrAddAsOverride on each placed object — this is where the Cell parent gets pulled in
        foreach (var ctx in contexts)
        {
            ctx.GetOrAddAsOverride(outputMod);
        }

        // The output mod should have exactly one Cell with SharedCellFormKey
        var cells = outputMod.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.ICellGetter>().ToList();
        cells.Count(c => c.FormKey == SharedCellFormKey).ShouldBe(1,
            "CellX should appear exactly once in the output mod, not duplicated from split files");

        // Writing should not throw duplicate FormKey
        var outputPath = Path.Combine(existingOutputDirectory.Path, "Output.esp");
        Should.NotThrow(() =>
        {
            outputMod.WriteToBinary(outputPath, new BinaryWriteParameters
            {
                FileSystem = fileSystem,
                ModKey = ModKeyOption.NoCheck
            });
        });
    }

    [Theory, MutagenModAutoData]
    public void Skyrim_DuplicateDialogTopic_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        mod1.DialogTopics.Add(new Mutagen.Bethesda.Skyrim.DialogTopic(SharedFormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedTopic",
            Responses = { new Mutagen.Bethesda.Skyrim.DialogResponses(ChildFormKey1, SkyrimRelease.SkyrimSE) { EditorID = "ResponseA" } }
        });
        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        mod2.DialogTopics.Add(new Mutagen.Bethesda.Skyrim.DialogTopic(SharedFormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedTopic",
            Responses = { new Mutagen.Bethesda.Skyrim.DialogResponses(ChildFormKey2, SkyrimRelease.SkyrimSE) { EditorID = "ResponseB" } }
        });

        ImportShouldSucceed(mod1, mod2, GameRelease.SkyrimSE, existingOutputDirectory, fileSystem, result =>
        {
            var topics = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.IDialogTopicGetter>().ToList();
            topics.Count.ShouldBe(1);
            topics.ShouldContain(t => t.FormKey == SharedFormKey);
            var responses = result.EnumerateMajorRecords<Mutagen.Bethesda.Skyrim.IDialogResponsesGetter>().ToList();
            responses.Count.ShouldBe(2);
            responses.ShouldContain(r => r.EditorID == "ResponseA");
            responses.ShouldContain(r => r.EditorID == "ResponseB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Skyrim_DuplicateNpc_Throws(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        mod1.Npcs.Add(new Mutagen.Bethesda.Skyrim.Npc(SharedFormKey, SkyrimRelease.SkyrimSE) { EditorID = "SharedNpc" });
        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        mod2.Npcs.Add(new Mutagen.Bethesda.Skyrim.Npc(SharedFormKey, SkyrimRelease.SkyrimSE) { EditorID = "SharedNpc" });

        ImportShouldThrow(mod1, mod2, GameRelease.SkyrimSE, existingOutputDirectory, fileSystem);
    }

    #endregion

    #region Oblivion - CELL, WRLD, DIAL

    [Theory, MutagenModAutoData]
    public void Oblivion_DuplicateCell_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Oblivion.OblivionMod(TestModKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        AddOblivionCell(mod1, SharedFormKey, ChildFormKey1, "PlacedA");
        var mod2 = new Mutagen.Bethesda.Oblivion.OblivionMod(SplitModKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        AddOblivionCell(mod2, SharedFormKey, ChildFormKey2, "PlacedB");

        ImportShouldSucceed(mod1, mod2, GameRelease.Oblivion, existingOutputDirectory, fileSystem, result =>
        {
            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Oblivion.ICellGetter>().ToList();
            cells.ShouldContain(c => c.FormKey == SharedFormKey);
            var placed = result.EnumerateMajorRecords<Mutagen.Bethesda.Oblivion.IPlacedObjectGetter>().ToList();
            placed.Count.ShouldBe(2);
            placed.ShouldContain(p => p.EditorID == "PlacedA");
            placed.ShouldContain(p => p.EditorID == "PlacedB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Oblivion_DuplicateWorldspace_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Oblivion.OblivionMod(TestModKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        mod1.Worldspaces.Add(new Mutagen.Bethesda.Oblivion.Worldspace(SharedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Oblivion.Cell(ChildFormKey1, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "TopCellA" }
        });
        var mod2 = new Mutagen.Bethesda.Oblivion.OblivionMod(SplitModKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        mod2.Worldspaces.Add(new Mutagen.Bethesda.Oblivion.Worldspace(SharedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Oblivion.Cell(ChildFormKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "TopCellB" }
        });

        ImportShouldSucceed(mod1, mod2, GameRelease.Oblivion, existingOutputDirectory, fileSystem, result =>
        {
            var worldspaces = result.EnumerateMajorRecords<Mutagen.Bethesda.Oblivion.IWorldspaceGetter>().ToList();
            worldspaces.Count.ShouldBe(1);
            worldspaces.ShouldContain(w => w.FormKey == SharedFormKey);
            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Oblivion.ICellGetter>().ToList();
            cells.Count.ShouldBe(2);
            cells.ShouldContain(c => c.EditorID == "TopCellA");
            cells.ShouldContain(c => c.EditorID == "TopCellB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Oblivion_WorldspaceWithSharedCell_CellNotDuplicated(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Oblivion.OblivionMod(TestModKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        AddOblivionWorldspaceCell(mod1, SharedFormKey, SharedCellFormKey, ChildFormKey1, "PlacedA");

        var mod2 = new Mutagen.Bethesda.Oblivion.OblivionMod(SplitModKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        AddOblivionWorldspaceCell(mod2, SharedFormKey, SharedCellFormKey, ChildFormKey2, "PlacedB");

        ImportShouldSucceed(mod1, mod2, GameRelease.Oblivion, existingOutputDirectory, fileSystem, result =>
        {
            var worldspaces = result.EnumerateMajorRecords<Mutagen.Bethesda.Oblivion.IWorldspaceGetter>().ToList();
            worldspaces.Count.ShouldBe(1);

            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Oblivion.ICellGetter>().ToList();
            cells.Count(c => c.FormKey == SharedCellFormKey).ShouldBe(1);

            var placed = result.EnumerateMajorRecords<Mutagen.Bethesda.Oblivion.IPlacedObjectGetter>().ToList();
            placed.Count.ShouldBe(2);
            placed.ShouldContain(p => p.EditorID == "PlacedA");
            placed.ShouldContain(p => p.EditorID == "PlacedB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Oblivion_DuplicateDialogTopic_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Oblivion.OblivionMod(TestModKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        mod1.DialogTopics.Add(new Mutagen.Bethesda.Oblivion.DialogTopic(SharedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
        {
            EditorID = "SharedTopic",
            Items = { new Mutagen.Bethesda.Oblivion.DialogItem(ChildFormKey1, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "ItemA" } }
        });
        var mod2 = new Mutagen.Bethesda.Oblivion.OblivionMod(SplitModKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        mod2.DialogTopics.Add(new Mutagen.Bethesda.Oblivion.DialogTopic(SharedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
        {
            EditorID = "SharedTopic",
            Items = { new Mutagen.Bethesda.Oblivion.DialogItem(ChildFormKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "ItemB" } }
        });

        ImportShouldSucceed(mod1, mod2, GameRelease.Oblivion, existingOutputDirectory, fileSystem, result =>
        {
            var topics = result.EnumerateMajorRecords<Mutagen.Bethesda.Oblivion.IDialogTopicGetter>().ToList();
            topics.Count.ShouldBe(1);
            topics.ShouldContain(t => t.FormKey == SharedFormKey);
            var items = result.EnumerateMajorRecords<Mutagen.Bethesda.Oblivion.IDialogItemGetter>().ToList();
            items.Count.ShouldBe(2);
            items.ShouldContain(i => i.EditorID == "ItemA");
            items.ShouldContain(i => i.EditorID == "ItemB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Oblivion_DuplicateNpc_Throws(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Oblivion.OblivionMod(TestModKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        mod1.Npcs.Add(new Mutagen.Bethesda.Oblivion.Npc(SharedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "SharedNpc" });
        var mod2 = new Mutagen.Bethesda.Oblivion.OblivionMod(SplitModKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        mod2.Npcs.Add(new Mutagen.Bethesda.Oblivion.Npc(SharedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "SharedNpc" });

        ImportShouldThrow(mod1, mod2, GameRelease.Oblivion, existingOutputDirectory, fileSystem);
    }

    #endregion

    #region Fallout4 - CELL, WRLD, DIAL (via Quest), QUST

    [Theory, MutagenModAutoData]
    public void Fallout4_DuplicateCell_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(TestModKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        AddFallout4Cell(mod1, SharedFormKey, ChildFormKey1, "PlacedA");
        var mod2 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(SplitModKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        AddFallout4Cell(mod2, SharedFormKey, ChildFormKey2, "PlacedB");

        ImportShouldSucceed(mod1, mod2, GameRelease.Fallout4, existingOutputDirectory, fileSystem, result =>
        {
            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Fallout4.ICellGetter>().ToList();
            cells.ShouldContain(c => c.FormKey == SharedFormKey);
            var placed = result.EnumerateMajorRecords<Mutagen.Bethesda.Fallout4.IPlacedObjectGetter>().ToList();
            placed.Count.ShouldBe(2);
            placed.ShouldContain(p => p.EditorID == "PlacedA");
            placed.ShouldContain(p => p.EditorID == "PlacedB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Fallout4_DuplicateWorldspace_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(TestModKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        mod1.Worldspaces.Add(new Mutagen.Bethesda.Fallout4.Worldspace(SharedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Fallout4.Cell(ChildFormKey1, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "TopCellA" }
        });
        var mod2 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(SplitModKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        mod2.Worldspaces.Add(new Mutagen.Bethesda.Fallout4.Worldspace(SharedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Fallout4.Cell(ChildFormKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "TopCellB" }
        });

        ImportShouldSucceed(mod1, mod2, GameRelease.Fallout4, existingOutputDirectory, fileSystem, result =>
        {
            var worldspaces = result.EnumerateMajorRecords<Mutagen.Bethesda.Fallout4.IWorldspaceGetter>().ToList();
            worldspaces.Count.ShouldBe(1);
            worldspaces.ShouldContain(w => w.FormKey == SharedFormKey);
            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Fallout4.ICellGetter>().ToList();
            cells.Count.ShouldBe(2);
            cells.ShouldContain(c => c.EditorID == "TopCellA");
            cells.ShouldContain(c => c.EditorID == "TopCellB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Fallout4_WorldspaceWithSharedCell_CellNotDuplicated(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(TestModKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        AddFallout4WorldspaceCell(mod1, SharedFormKey, SharedCellFormKey, ChildFormKey1, "PlacedA");

        var mod2 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(SplitModKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        AddFallout4WorldspaceCell(mod2, SharedFormKey, SharedCellFormKey, ChildFormKey2, "PlacedB");

        ImportShouldSucceed(mod1, mod2, GameRelease.Fallout4, existingOutputDirectory, fileSystem, result =>
        {
            var worldspaces = result.EnumerateMajorRecords<Mutagen.Bethesda.Fallout4.IWorldspaceGetter>().ToList();
            worldspaces.Count.ShouldBe(1);

            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Fallout4.ICellGetter>().ToList();
            cells.Count(c => c.FormKey == SharedCellFormKey).ShouldBe(1);

            var placed = result.EnumerateMajorRecords<Mutagen.Bethesda.Fallout4.IPlacedObjectGetter>().ToList();
            placed.Count.ShouldBe(2);
            placed.ShouldContain(p => p.EditorID == "PlacedA");
            placed.ShouldContain(p => p.EditorID == "PlacedB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Fallout4_DuplicateQuest_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(TestModKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        mod1.Quests.Add(new Mutagen.Bethesda.Fallout4.Quest(SharedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
        {
            EditorID = "SharedQuest",
            DialogTopics = { new Mutagen.Bethesda.Fallout4.DialogTopic(ChildFormKey1, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "TopicA" } }
        });
        var mod2 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(SplitModKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        mod2.Quests.Add(new Mutagen.Bethesda.Fallout4.Quest(SharedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
        {
            EditorID = "SharedQuest",
            DialogTopics = { new Mutagen.Bethesda.Fallout4.DialogTopic(ChildFormKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "TopicB" } }
        });

        ImportShouldSucceed(mod1, mod2, GameRelease.Fallout4, existingOutputDirectory, fileSystem, result =>
        {
            var quests = result.EnumerateMajorRecords<Mutagen.Bethesda.Fallout4.IQuestGetter>().ToList();
            quests.Count.ShouldBe(1);
            quests.ShouldContain(q => q.FormKey == SharedFormKey);
            var topics = result.EnumerateMajorRecords<Mutagen.Bethesda.Fallout4.IDialogTopicGetter>().ToList();
            topics.Count.ShouldBe(2);
            topics.ShouldContain(t => t.EditorID == "TopicA");
            topics.ShouldContain(t => t.EditorID == "TopicB");
        });
    }

    [Theory, MutagenModAutoData]
    public void Fallout4_DuplicateNpc_Throws(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(TestModKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        mod1.Npcs.Add(new Mutagen.Bethesda.Fallout4.Npc(SharedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "SharedNpc" });
        var mod2 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(SplitModKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        mod2.Npcs.Add(new Mutagen.Bethesda.Fallout4.Npc(SharedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "SharedNpc" });

        ImportShouldThrow(mod1, mod2, GameRelease.Fallout4, existingOutputDirectory, fileSystem);
    }

    #endregion

    #region Starfield - CELL, WRLD, QUST
    // Starfield requires MasterFlagLookup for writing (SeparateMasterLoadOrders),
    // which makes simple write-and-reimport tests infeasible without additional setup.
    // The validation logic is game-agnostic and covered by the other game tests.

    [Theory(Skip = "Starfield WriteToBinary requires MasterFlagLookup"), MutagenModAutoData]
    public void Starfield_DuplicateCell_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Starfield.StarfieldMod(TestModKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        AddStarfieldCell(mod1, SharedFormKey, ChildFormKey1, "PlacedA");
        var mod2 = new Mutagen.Bethesda.Starfield.StarfieldMod(SplitModKey2, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        AddStarfieldCell(mod2, SharedFormKey, ChildFormKey2, "PlacedB");

        ImportShouldSucceed(mod1, mod2, GameRelease.Starfield, existingOutputDirectory, fileSystem, result =>
        {
            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Starfield.ICellGetter>().ToList();
            cells.ShouldContain(c => c.FormKey == SharedFormKey);
            var placed = result.EnumerateMajorRecords<Mutagen.Bethesda.Starfield.IPlacedObjectGetter>().ToList();
            placed.Count.ShouldBe(2);
            placed.ShouldContain(p => p.EditorID == "PlacedA");
            placed.ShouldContain(p => p.EditorID == "PlacedB");
        });
    }

    [Theory(Skip = "Starfield WriteToBinary requires MasterFlagLookup"), MutagenModAutoData]
    public void Starfield_DuplicateWorldspace_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Starfield.StarfieldMod(TestModKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        mod1.Worldspaces.Add(new Mutagen.Bethesda.Starfield.Worldspace(SharedFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Starfield.Cell(ChildFormKey1, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "TopCellA" }
        });
        var mod2 = new Mutagen.Bethesda.Starfield.StarfieldMod(SplitModKey2, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        mod2.Worldspaces.Add(new Mutagen.Bethesda.Starfield.Worldspace(SharedFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Starfield.Cell(ChildFormKey2, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "TopCellB" }
        });

        ImportShouldSucceed(mod1, mod2, GameRelease.Starfield, existingOutputDirectory, fileSystem, result =>
        {
            var worldspaces = result.EnumerateMajorRecords<Mutagen.Bethesda.Starfield.IWorldspaceGetter>().ToList();
            worldspaces.Count.ShouldBe(1);
            worldspaces.ShouldContain(w => w.FormKey == SharedFormKey);
            var cells = result.EnumerateMajorRecords<Mutagen.Bethesda.Starfield.ICellGetter>().ToList();
            cells.Count.ShouldBe(2);
            cells.ShouldContain(c => c.EditorID == "TopCellA");
            cells.ShouldContain(c => c.EditorID == "TopCellB");
        });
    }

    [Theory(Skip = "Starfield WriteToBinary requires MasterFlagLookup"), MutagenModAutoData]
    public void Starfield_DuplicateQuest_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Starfield.StarfieldMod(TestModKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        mod1.Quests.Add(new Mutagen.Bethesda.Starfield.Quest(SharedFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)
        {
            EditorID = "SharedQuest",
            DialogTopics = { new Mutagen.Bethesda.Starfield.DialogTopic(ChildFormKey1, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "TopicA" } }
        });
        var mod2 = new Mutagen.Bethesda.Starfield.StarfieldMod(SplitModKey2, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        mod2.Quests.Add(new Mutagen.Bethesda.Starfield.Quest(SharedFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)
        {
            EditorID = "SharedQuest",
            DialogTopics = { new Mutagen.Bethesda.Starfield.DialogTopic(ChildFormKey2, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "TopicB" } }
        });

        ImportShouldSucceed(mod1, mod2, GameRelease.Starfield, existingOutputDirectory, fileSystem, result =>
        {
            var quests = result.EnumerateMajorRecords<Mutagen.Bethesda.Starfield.IQuestGetter>().ToList();
            quests.Count.ShouldBe(1);
            quests.ShouldContain(q => q.FormKey == SharedFormKey);
            var topics = result.EnumerateMajorRecords<Mutagen.Bethesda.Starfield.IDialogTopicGetter>().ToList();
            topics.Count.ShouldBe(2);
            topics.ShouldContain(t => t.EditorID == "TopicA");
            topics.ShouldContain(t => t.EditorID == "TopicB");
        });
    }

    [Theory(Skip = "Starfield WriteToBinary requires MasterFlagLookup"), MutagenModAutoData]
    public void Starfield_DuplicateNpc_Throws(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Starfield.StarfieldMod(TestModKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        mod1.Npcs.Add(new Mutagen.Bethesda.Starfield.Npc(SharedFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "SharedNpc" });
        var mod2 = new Mutagen.Bethesda.Starfield.StarfieldMod(SplitModKey2, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        mod2.Npcs.Add(new Mutagen.Bethesda.Starfield.Npc(SharedFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "SharedNpc" });

        ImportShouldThrow(mod1, mod2, GameRelease.Starfield, existingOutputDirectory, fileSystem);
    }

    #endregion

    #region Helpers

    private static void AddSkyrimWorldspaceCell(
        SkyrimMod mod, FormKey worldspaceFormKey, FormKey cellFormKey, FormKey placedFormKey, string placedEditorId)
    {
        mod.Worldspaces.Add(new Mutagen.Bethesda.Skyrim.Worldspace(worldspaceFormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedWorld",
            SubCells =
            {
                new Mutagen.Bethesda.Skyrim.WorldspaceBlock()
                {
                    BlockNumberX = 0, BlockNumberY = 0,
                    GroupType = Mutagen.Bethesda.Skyrim.GroupTypeEnum.ExteriorCellBlock, LastModified = 4,
                    Items =
                    {
                        new Mutagen.Bethesda.Skyrim.WorldspaceSubBlock()
                        {
                            BlockNumberX = 0, BlockNumberY = 0,
                            GroupType = Mutagen.Bethesda.Skyrim.GroupTypeEnum.ExteriorCellSubBlock, LastModified = 4,
                            Items =
                            {
                                new Mutagen.Bethesda.Skyrim.Cell(cellFormKey, SkyrimRelease.SkyrimSE)
                                {
                                    EditorID = "SharedCell",
                                    Persistent =
                                    {
                                        new Mutagen.Bethesda.Skyrim.PlacedObject(placedFormKey, SkyrimRelease.SkyrimSE) { EditorID = placedEditorId }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
    }

    private static void AddOblivionWorldspaceCell(
        Mutagen.Bethesda.Oblivion.OblivionMod mod, FormKey worldspaceFormKey, FormKey cellFormKey, FormKey placedFormKey, string placedEditorId)
    {
        mod.Worldspaces.Add(new Mutagen.Bethesda.Oblivion.Worldspace(worldspaceFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
        {
            EditorID = "SharedWorld",
            SubCells =
            {
                new Mutagen.Bethesda.Oblivion.WorldspaceBlock()
                {
                    BlockNumberX = 0, BlockNumberY = 0,
                    GroupType = Mutagen.Bethesda.Oblivion.GroupTypeEnum.ExteriorCellBlock, LastModified = 4,
                    Items =
                    {
                        new Mutagen.Bethesda.Oblivion.WorldspaceSubBlock()
                        {
                            BlockNumberX = 0, BlockNumberY = 0,
                            GroupType = Mutagen.Bethesda.Oblivion.GroupTypeEnum.ExteriorCellSubBlock, LastModified = 4,
                            Items =
                            {
                                new Mutagen.Bethesda.Oblivion.Cell(cellFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
                                {
                                    EditorID = "SharedCell",
                                    Persistent =
                                    {
                                        new Mutagen.Bethesda.Oblivion.PlacedObject(placedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = placedEditorId }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
    }

    private static void AddFallout4WorldspaceCell(
        Mutagen.Bethesda.Fallout4.Fallout4Mod mod, FormKey worldspaceFormKey, FormKey cellFormKey, FormKey placedFormKey, string placedEditorId)
    {
        mod.Worldspaces.Add(new Mutagen.Bethesda.Fallout4.Worldspace(worldspaceFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
        {
            EditorID = "SharedWorld",
            SubCells =
            {
                new Mutagen.Bethesda.Fallout4.WorldspaceBlock()
                {
                    BlockNumberX = 0, BlockNumberY = 0,
                    GroupType = Mutagen.Bethesda.Fallout4.GroupTypeEnum.ExteriorCellBlock, LastModified = 4,
                    Items =
                    {
                        new Mutagen.Bethesda.Fallout4.WorldspaceSubBlock()
                        {
                            BlockNumberX = 0, BlockNumberY = 0,
                            GroupType = Mutagen.Bethesda.Fallout4.GroupTypeEnum.ExteriorCellSubBlock, LastModified = 4,
                            Items =
                            {
                                new Mutagen.Bethesda.Fallout4.Cell(cellFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
                                {
                                    EditorID = "SharedCell",
                                    Persistent =
                                    {
                                        new Mutagen.Bethesda.Fallout4.PlacedObject(placedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = placedEditorId }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
    }

    private static void AddSkyrimCell(SkyrimMod mod, FormKey cellFormKey, FormKey placedFormKey, string placedEditorId)
    {
        mod.Cells.Records.Add(new Mutagen.Bethesda.Skyrim.CellBlock()
        {
            BlockNumber = 0, GroupType = Mutagen.Bethesda.Skyrim.GroupTypeEnum.InteriorCellBlock, LastModified = 4,
            SubBlocks = { new Mutagen.Bethesda.Skyrim.CellSubBlock()
            {
                BlockNumber = 0, GroupType = Mutagen.Bethesda.Skyrim.GroupTypeEnum.InteriorCellSubBlock, LastModified = 4,
                Cells = { new Mutagen.Bethesda.Skyrim.Cell(cellFormKey, SkyrimRelease.SkyrimSE)
                {
                    EditorID = "SharedCell",
                    Persistent = { new Mutagen.Bethesda.Skyrim.PlacedObject(placedFormKey, SkyrimRelease.SkyrimSE) { EditorID = placedEditorId } }
                }}
            }}
        });
    }

    private static void AddOblivionCell(Mutagen.Bethesda.Oblivion.OblivionMod mod, FormKey cellFormKey, FormKey placedFormKey, string placedEditorId)
    {
        mod.Cells.Records.Add(new Mutagen.Bethesda.Oblivion.CellBlock()
        {
            BlockNumber = 0, GroupType = Mutagen.Bethesda.Oblivion.GroupTypeEnum.InteriorCellBlock, LastModified = 4,
            SubBlocks = { new Mutagen.Bethesda.Oblivion.CellSubBlock()
            {
                BlockNumber = 0, GroupType = Mutagen.Bethesda.Oblivion.GroupTypeEnum.InteriorCellSubBlock, LastModified = 4,
                Cells = { new Mutagen.Bethesda.Oblivion.Cell(cellFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
                {
                    EditorID = "SharedCell",
                    Persistent = { new Mutagen.Bethesda.Oblivion.PlacedObject(placedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = placedEditorId } }
                }}
            }}
        });
    }

    private static void AddFallout4Cell(Mutagen.Bethesda.Fallout4.Fallout4Mod mod, FormKey cellFormKey, FormKey placedFormKey, string placedEditorId)
    {
        mod.Cells.Records.Add(new Mutagen.Bethesda.Fallout4.CellBlock()
        {
            BlockNumber = 0, GroupType = Mutagen.Bethesda.Fallout4.GroupTypeEnum.InteriorCellBlock, LastModified = 4,
            SubBlocks = { new Mutagen.Bethesda.Fallout4.CellSubBlock()
            {
                BlockNumber = 0, GroupType = Mutagen.Bethesda.Fallout4.GroupTypeEnum.InteriorCellSubBlock, LastModified = 4,
                Cells = { new Mutagen.Bethesda.Fallout4.Cell(cellFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
                {
                    EditorID = "SharedCell",
                    Persistent = { new Mutagen.Bethesda.Fallout4.PlacedObject(placedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = placedEditorId } }
                }}
            }}
        });
    }

    private static void AddStarfieldCell(Mutagen.Bethesda.Starfield.StarfieldMod mod, FormKey cellFormKey, FormKey placedFormKey, string placedEditorId)
    {
        mod.Cells.Records.Add(new Mutagen.Bethesda.Starfield.CellBlock()
        {
            BlockNumber = 0, GroupType = Mutagen.Bethesda.Starfield.GroupTypeEnum.InteriorCellBlock, LastModified = 4,
            SubBlocks = { new Mutagen.Bethesda.Starfield.CellSubBlock()
            {
                BlockNumber = 0, GroupType = Mutagen.Bethesda.Starfield.GroupTypeEnum.InteriorCellSubBlock, LastModified = 4,
                Cells = { new Mutagen.Bethesda.Starfield.Cell(cellFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)
                {
                    EditorID = "SharedCell",
                    Persistent = { new Mutagen.Bethesda.Starfield.PlacedObject(placedFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = placedEditorId } }
                }}
            }}
        });
    }

    private static void ImportShouldSucceed(
        IMod mod1, IMod mod2, GameRelease release,
        DirectoryPath outputDir, IFileSystem fileSystem,
        Action<IModDisposeGetter> verify)
    {
        var file1 = WriteSplitFile(mod1, outputDir, fileSystem, 1);
        var file2 = WriteSplitFile(mod2, outputDir, fileSystem, 2);

        using var result = ModFactory.ImportMultiFileGetter(
            TestModKey,
            new[] { (ModPath)file1, (ModPath)file2 },
            new[] { MasterModKey },
            release,
            new BinaryReadParameters() { FileSystem = fileSystem });

        result.ShouldNotBeNull();
        result.ModKey.ShouldBe(TestModKey);
        verify(result);
    }

    private static void ImportShouldSucceed3(
        IMod mod1, IMod mod2, IMod mod3, GameRelease release,
        DirectoryPath outputDir, IFileSystem fileSystem,
        Action<IModDisposeGetter> verify)
    {
        var file1 = WriteSplitFile(mod1, outputDir, fileSystem, 1);
        var file2 = WriteSplitFile(mod2, outputDir, fileSystem, 2);
        var file3 = WriteSplitFile(mod3, outputDir, fileSystem, 3);

        using var result = ModFactory.ImportMultiFileGetter(
            TestModKey,
            new[] { (ModPath)file1, (ModPath)file2, (ModPath)file3 },
            new[] { MasterModKey },
            release,
            new BinaryReadParameters() { FileSystem = fileSystem });

        result.ShouldNotBeNull();
        result.ModKey.ShouldBe(TestModKey);
        verify(result);
    }

    private static void ImportShouldThrow(
        IMod mod1, IMod mod2, GameRelease release,
        DirectoryPath outputDir, IFileSystem fileSystem)
    {
        var file1 = WriteSplitFile(mod1, outputDir, fileSystem, 1);
        var file2 = WriteSplitFile(mod2, outputDir, fileSystem, 2);

        Should.Throw<InvalidOperationException>(() =>
        {
            using var result = ModFactory.ImportMultiFileGetter(
                TestModKey,
                new[] { (ModPath)file1, (ModPath)file2 },
                new[] { MasterModKey },
                release,
                new BinaryReadParameters() { FileSystem = fileSystem });
        });
    }

    private static string WriteSplitFile(
        IMod mod, DirectoryPath outputDir, IFileSystem fileSystem, int index)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(TestModKey.FileName);
        var extension = Path.GetExtension(TestModKey.FileName);

        var path = index == 1
            ? Path.Combine(outputDir.Path, TestModKey.FileName)
            : Path.Combine(outputDir.Path, $"{fileNameWithoutExtension}_{index}{extension}");

        mod.WriteToBinary(path, new BinaryWriteParameters()
        {
            FileSystem = fileSystem,
            ModKey = ModKeyOption.NoCheck
        });

        return path;
    }

    #endregion
}
