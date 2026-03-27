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
    private static readonly FormKey SharedFormKey = new(MasterModKey, 0x3C);

    #region Skyrim - CELL, WRLD, DIAL

    [Theory, MutagenModAutoData]
    public void Skyrim_DuplicateCell_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        AddSkyrimCell(mod1, SharedFormKey, "PlacedA");
        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        AddSkyrimCell(mod2, SharedFormKey, "PlacedB");

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
            TopCell = new Mutagen.Bethesda.Skyrim.Cell(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE) { EditorID = "TopCellA" }
        });
        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        mod2.Worldspaces.Add(new Mutagen.Bethesda.Skyrim.Worldspace(SharedFormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Skyrim.Cell(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE) { EditorID = "TopCellB" }
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

    [Theory, MutagenModAutoData]
    public void Skyrim_DuplicateDialogTopic_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        mod1.DialogTopics.Add(new Mutagen.Bethesda.Skyrim.DialogTopic(SharedFormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedTopic",
            Responses = { new Mutagen.Bethesda.Skyrim.DialogResponses(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE) { EditorID = "ResponseA" } }
        });
        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        mod2.DialogTopics.Add(new Mutagen.Bethesda.Skyrim.DialogTopic(SharedFormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedTopic",
            Responses = { new Mutagen.Bethesda.Skyrim.DialogResponses(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE) { EditorID = "ResponseB" } }
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
        AddOblivionCell(mod1, SharedFormKey, "PlacedA");
        var mod2 = new Mutagen.Bethesda.Oblivion.OblivionMod(SplitModKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        AddOblivionCell(mod2, SharedFormKey, "PlacedB");

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
            TopCell = new Mutagen.Bethesda.Oblivion.Cell(mod1.GetNextFormKey(), Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "TopCellA" }
        });
        var mod2 = new Mutagen.Bethesda.Oblivion.OblivionMod(SplitModKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        mod2.Worldspaces.Add(new Mutagen.Bethesda.Oblivion.Worldspace(SharedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Oblivion.Cell(mod2.GetNextFormKey(), Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "TopCellB" }
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
    public void Oblivion_DuplicateDialogTopic_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Oblivion.OblivionMod(TestModKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        mod1.DialogTopics.Add(new Mutagen.Bethesda.Oblivion.DialogTopic(SharedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
        {
            EditorID = "SharedTopic",
            Items = { new Mutagen.Bethesda.Oblivion.DialogItem(mod1.GetNextFormKey(), Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "ItemA" } }
        });
        var mod2 = new Mutagen.Bethesda.Oblivion.OblivionMod(SplitModKey2, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion);
        mod2.DialogTopics.Add(new Mutagen.Bethesda.Oblivion.DialogTopic(SharedFormKey, Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion)
        {
            EditorID = "SharedTopic",
            Items = { new Mutagen.Bethesda.Oblivion.DialogItem(mod2.GetNextFormKey(), Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = "ItemB" } }
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
        AddFallout4Cell(mod1, SharedFormKey, "PlacedA");
        var mod2 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(SplitModKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        AddFallout4Cell(mod2, SharedFormKey, "PlacedB");

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
            TopCell = new Mutagen.Bethesda.Fallout4.Cell(mod1.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "TopCellA" }
        });
        var mod2 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(SplitModKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        mod2.Worldspaces.Add(new Mutagen.Bethesda.Fallout4.Worldspace(SharedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Fallout4.Cell(mod2.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "TopCellB" }
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
    public void Fallout4_DuplicateQuest_ImportsSuccessfully(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        var mod1 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(TestModKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        mod1.Quests.Add(new Mutagen.Bethesda.Fallout4.Quest(SharedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
        {
            EditorID = "SharedQuest",
            DialogTopics = { new Mutagen.Bethesda.Fallout4.DialogTopic(mod1.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "TopicA" } }
        });
        var mod2 = new Mutagen.Bethesda.Fallout4.Fallout4Mod(SplitModKey2, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        mod2.Quests.Add(new Mutagen.Bethesda.Fallout4.Quest(SharedFormKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
        {
            EditorID = "SharedQuest",
            DialogTopics = { new Mutagen.Bethesda.Fallout4.DialogTopic(mod2.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = "TopicB" } }
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
        AddStarfieldCell(mod1, SharedFormKey, "PlacedA");
        var mod2 = new Mutagen.Bethesda.Starfield.StarfieldMod(SplitModKey2, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        AddStarfieldCell(mod2, SharedFormKey, "PlacedB");

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
            TopCell = new Mutagen.Bethesda.Starfield.Cell(mod1.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "TopCellA" }
        });
        var mod2 = new Mutagen.Bethesda.Starfield.StarfieldMod(SplitModKey2, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        mod2.Worldspaces.Add(new Mutagen.Bethesda.Starfield.Worldspace(SharedFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)
        {
            EditorID = "SharedWorld",
            TopCell = new Mutagen.Bethesda.Starfield.Cell(mod2.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "TopCellB" }
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
            DialogTopics = { new Mutagen.Bethesda.Starfield.DialogTopic(mod1.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "TopicA" } }
        });
        var mod2 = new Mutagen.Bethesda.Starfield.StarfieldMod(SplitModKey2, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        mod2.Quests.Add(new Mutagen.Bethesda.Starfield.Quest(SharedFormKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)
        {
            EditorID = "SharedQuest",
            DialogTopics = { new Mutagen.Bethesda.Starfield.DialogTopic(mod2.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = "TopicB" } }
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

    private static void AddSkyrimCell(SkyrimMod mod, FormKey cellFormKey, string placedEditorId)
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
                    Persistent = { new Mutagen.Bethesda.Skyrim.PlacedObject(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE) { EditorID = placedEditorId } }
                }}
            }}
        });
    }

    private static void AddOblivionCell(Mutagen.Bethesda.Oblivion.OblivionMod mod, FormKey cellFormKey, string placedEditorId)
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
                    Persistent = { new Mutagen.Bethesda.Oblivion.PlacedObject(mod.GetNextFormKey(), Mutagen.Bethesda.Oblivion.OblivionRelease.Oblivion) { EditorID = placedEditorId } }
                }}
            }}
        });
    }

    private static void AddFallout4Cell(Mutagen.Bethesda.Fallout4.Fallout4Mod mod, FormKey cellFormKey, string placedEditorId)
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
                    Persistent = { new Mutagen.Bethesda.Fallout4.PlacedObject(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4) { EditorID = placedEditorId } }
                }}
            }}
        });
    }

    private static void AddStarfieldCell(Mutagen.Bethesda.Starfield.StarfieldMod mod, FormKey cellFormKey, string placedEditorId)
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
                    Persistent = { new Mutagen.Bethesda.Starfield.PlacedObject(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield) { EditorID = placedEditorId } }
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
