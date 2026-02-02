using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using System.IO.Abstractions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

/// <summary>
/// Tests for deep nested record structures (Cells, Worldspaces, DialogTopics) across split mod files.
/// These tests verify that multi-file overlays properly traverse nested structures like:
/// - Cells → CellBlocks → CellSubBlocks → Cells → PlacedObjects
/// - Worldspaces → SubCells (WorldspaceBlocks) → SubBlocks → Cells → PlacedObjects
/// - DialogTopics → Responses (major records)
/// </summary>
public class DeepNestedRecordTests
{
    [Theory, MutagenModAutoData]
    public void CellsWithPlacedObjects_AcrossSplitFiles(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Create a mod with Cells containing PlacedObjects
        var modKey = new ModKey("DeepNested", ModType.Plugin);
        var mod1 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);

        // Add a Cell with PlacedObjects to the first mod
        var placedObject1 = new PlacedObject(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedObject1.EditorID = "PlacedObject1";

        mod1.Cells.Records.Add(new CellBlock()
        {
            BlockNumber = 0,
            GroupType = GroupTypeEnum.InteriorCellBlock,
            LastModified = 4,
            SubBlocks =
            {
                new CellSubBlock()
                {
                    BlockNumber = 0,
                    GroupType = GroupTypeEnum.InteriorCellSubBlock,
                    LastModified = 4,
                    Cells =
                    {
                        new Cell(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE)
                        {
                            EditorID = "Cell1",
                            Temporary =
                            {
                                placedObject1
                            }
                        }
                    }
                }
            }
        });

        // Write the first split file
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        mod1.WriteToBinary(splitFile1, BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            ModKey = ModKeyOption.NoCheck
        });

        // Create a second mod with another Cell containing PlacedObjects
        var mod2 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var placedObject2 = new PlacedObject(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedObject2.EditorID = "PlacedObject2";

        mod2.Cells.Records.Add(new CellBlock()
        {
            BlockNumber = 0,
            GroupType = GroupTypeEnum.InteriorCellBlock,
            LastModified = 4,
            SubBlocks =
            {
                new CellSubBlock()
                {
                    BlockNumber = 0,
                    GroupType = GroupTypeEnum.InteriorCellSubBlock,
                    LastModified = 4,
                    Cells =
                    {
                        new Cell(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE)
                        {
                            EditorID = "Cell2",
                            Temporary =
                            {
                                placedObject2
                            }
                        }
                    }
                }
            }
        });

        // Write the second split file
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");
        mod2.WriteToBinary(splitFile2, BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            ModKey = ModKeyOption.NoCheck
        });

        // Import using ModFactory to create multi-file overlay
        var result = ModFactory<ISkyrimModDisposableGetter>.ImportMultiFileGetter(
            modKey,
            new[] { (ModPath)splitFile1, (ModPath)splitFile2 },
            Array.Empty<IModMasterStyledGetter>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        // Verify CellBlocks are merged (both have BlockNumber=0, so they merge into 1 block)
        // But the individual cells within should both be accessible
        Assert.Equal(1, result.Cells.Count);

        // Verify PlacedObjects are accessible through EnumerateMajorRecords
        var allRecords = result.EnumerateMajorRecords().ToList();
        var placedObjects = allRecords.OfType<IPlacedObjectGetter>().ToList();

        Assert.Equal(2, placedObjects.Count);
        Assert.Contains(placedObjects, p => p.EditorID == "PlacedObject1");
        Assert.Contains(placedObjects, p => p.EditorID == "PlacedObject2");
    }

    [Theory, MutagenModAutoData]
    public void WorldspacesWithSubCells_AcrossSplitFiles(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Create a mod with a Worldspace containing SubCells
        var modKey = new ModKey("WorldspaceNested", ModType.Plugin);
        var mod1 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);

        // Add a Worldspace with TopCell to the first mod
        var placedInTop = new PlacedObject(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedInTop.EditorID = "PlacedInTopCell";

        mod1.Worldspaces.Add(new Worldspace(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            EditorID = "Worldspace1",
            TopCell = new Cell(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE)
            {
                EditorID = "TopCell1",
                Temporary =
                {
                    placedInTop
                }
            }
        });

        // Write the first split file
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        mod1.WriteToBinary(splitFile1, BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            ModKey = ModKeyOption.NoCheck
        });

        // Create a second mod with another Worldspace
        var mod2 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var placedInTop2 = new PlacedObject(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedInTop2.EditorID = "PlacedInTopCell2";

        mod2.Worldspaces.Add(new Worldspace(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            EditorID = "Worldspace2",
            TopCell = new Cell(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE)
            {
                EditorID = "TopCell2",
                Temporary =
                {
                    placedInTop2
                }
            }
        });

        // Write the second split file
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");
        mod2.WriteToBinary(splitFile2, BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            ModKey = ModKeyOption.NoCheck
        });

        // Import using ModFactory to create multi-file overlay
        var result = ModFactory<ISkyrimModDisposableGetter>.ImportMultiFileGetter(
            modKey,
            new[] { (ModPath)splitFile1, (ModPath)splitFile2 },
            Array.Empty<IModMasterStyledGetter>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        // Verify both worldspaces are present
        Assert.Equal(2, result.Worldspaces.Count);

        // Verify nested Cells and PlacedObjects are accessible through EnumerateMajorRecords
        var allRecords = result.EnumerateMajorRecords().ToList();
        var worldspaces = allRecords.OfType<IWorldspaceGetter>().ToList();
        var cells = allRecords.OfType<ICellGetter>().ToList();
        var placedObjects = allRecords.OfType<IPlacedObjectGetter>().ToList();

        Assert.Equal(2, worldspaces.Count);
        Assert.Equal(2, cells.Count); // TopCell1 and TopCell2
        Assert.Equal(2, placedObjects.Count);
        Assert.Contains(placedObjects, p => p.EditorID == "PlacedInTopCell");
        Assert.Contains(placedObjects, p => p.EditorID == "PlacedInTopCell2");
    }

    [Theory, MutagenModAutoData]
    public void DialogTopicsWithResponses_AcrossSplitFiles(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Create a mod with DialogTopics containing Responses
        var modKey = new ModKey("DialogNested", ModType.Plugin);
        var mod1 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);

        // Add a DialogTopic with Responses to the first mod
        var response1 = new DialogResponses(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        response1.EditorID = "Response1";

        mod1.DialogTopics.Add(new DialogTopic(mod1.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            EditorID = "Topic1",
            Responses =
            {
                response1
            }
        });

        // Write the first split file
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        mod1.WriteToBinary(splitFile1, BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            ModKey = ModKeyOption.NoCheck
        });

        // Create a second mod with another DialogTopic
        var mod2 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var response2 = new DialogResponses(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        response2.EditorID = "Response2";

        mod2.DialogTopics.Add(new DialogTopic(mod2.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            EditorID = "Topic2",
            Responses =
            {
                response2
            }
        });

        // Write the second split file
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");
        mod2.WriteToBinary(splitFile2, BinaryWriteParameters.Default with
        {
            FileSystem = fileSystem,
            ModKey = ModKeyOption.NoCheck
        });

        // Import using ModFactory to create multi-file overlay
        var result = ModFactory<ISkyrimModDisposableGetter>.ImportMultiFileGetter(
            modKey,
            new[] { (ModPath)splitFile1, (ModPath)splitFile2 },
            Array.Empty<IModMasterStyledGetter>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        // Verify both dialog topics are present
        Assert.Equal(2, result.DialogTopics.Count);

        // Verify Responses are accessible through EnumerateMajorRecords
        var allRecords = result.EnumerateMajorRecords().ToList();
        var dialogTopics = allRecords.OfType<IDialogTopicGetter>().ToList();
        var responses = allRecords.OfType<IDialogResponsesGetter>().ToList();

        Assert.Equal(2, dialogTopics.Count);
        Assert.Equal(2, responses.Count);
        Assert.Contains(responses, r => r.EditorID == "Response1");
        Assert.Contains(responses, r => r.EditorID == "Response2");
    }
}
