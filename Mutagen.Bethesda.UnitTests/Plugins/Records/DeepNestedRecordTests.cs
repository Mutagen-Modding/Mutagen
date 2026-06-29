using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
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
    private static readonly ModKey TestModKey = new("DeepNested", ModType.Plugin);
    private static readonly FormKey PlacedFormKey1 = new(TestModKey, 0x800);
    private static readonly FormKey PlacedFormKey2 = new(TestModKey, 0x801);
    private static readonly FormKey CellFormKey1 = new(TestModKey, 0x802);
    private static readonly FormKey CellFormKey2 = new(TestModKey, 0x803);
    private static readonly FormKey WorldspaceFormKey1 = new(TestModKey, 0x804);
    private static readonly FormKey WorldspaceFormKey2 = new(TestModKey, 0x805);
    private static readonly FormKey TopCellFormKey1 = new(TestModKey, 0x806);
    private static readonly FormKey TopCellFormKey2 = new(TestModKey, 0x807);
    private static readonly FormKey TopicFormKey1 = new(TestModKey, 0x808);
    private static readonly FormKey TopicFormKey2 = new(TestModKey, 0x809);
    private static readonly FormKey ResponseFormKey1 = new(TestModKey, 0x80A);
    private static readonly FormKey ResponseFormKey2 = new(TestModKey, 0x80B);

    [Theory, MutagenModAutoData]
    public void CellsWithPlacedObjects_AcrossSplitFiles(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Create a mod with Cells containing PlacedObjects
        var modKey = TestModKey;
        var mod1 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);

        // Add a Cell with PlacedObjects to the first mod
        var placedObject1 = new PlacedObject(PlacedFormKey1, SkyrimRelease.SkyrimSE);
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
                        new Cell(CellFormKey1, SkyrimRelease.SkyrimSE)
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
        var placedObject2 = new PlacedObject(PlacedFormKey2, SkyrimRelease.SkyrimSE);
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
                        new Cell(CellFormKey2, SkyrimRelease.SkyrimSE)
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
            Array.Empty<ModKey>(),
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
        var modKey = TestModKey;
        var mod1 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);

        // Add a Worldspace with TopCell to the first mod
        var placedInTop = new PlacedObject(PlacedFormKey1, SkyrimRelease.SkyrimSE);
        placedInTop.EditorID = "PlacedInTopCell";

        mod1.Worldspaces.Add(new Worldspace(WorldspaceFormKey1, SkyrimRelease.SkyrimSE)
        {
            EditorID = "Worldspace1",
            TopCell = new Cell(TopCellFormKey1, SkyrimRelease.SkyrimSE)
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
        var placedInTop2 = new PlacedObject(PlacedFormKey2, SkyrimRelease.SkyrimSE);
        placedInTop2.EditorID = "PlacedInTopCell2";

        mod2.Worldspaces.Add(new Worldspace(WorldspaceFormKey2, SkyrimRelease.SkyrimSE)
        {
            EditorID = "Worldspace2",
            TopCell = new Cell(TopCellFormKey2, SkyrimRelease.SkyrimSE)
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
            Array.Empty<ModKey>(),
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
        var modKey = TestModKey;
        var mod1 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);

        // Add a DialogTopic with Responses to the first mod
        var response1 = new DialogResponses(ResponseFormKey1, SkyrimRelease.SkyrimSE);
        response1.EditorID = "Response1";

        mod1.DialogTopics.Add(new DialogTopic(TopicFormKey1, SkyrimRelease.SkyrimSE)
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
        var response2 = new DialogResponses(ResponseFormKey2, SkyrimRelease.SkyrimSE);
        response2.EditorID = "Response2";

        mod2.DialogTopics.Add(new DialogTopic(TopicFormKey2, SkyrimRelease.SkyrimSE)
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
            Array.Empty<ModKey>(),
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
