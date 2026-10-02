using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;
using TempFile = Noggog.IO.TempFile;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class EnumerateAssetLinksShallowTests
{
    private static PlacedObject CreateScriptedPlacedObject(SkyrimMod mod, string scriptName)
    {
        return new PlacedObject(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE)
        {
            VirtualMachineAdapter = new VirtualMachineAdapter()
            {
                Scripts = new ExtendedList<ScriptEntry>()
                {
                    new ScriptEntry() { Name = scriptName }
                }
            }
        };
    }

    private static Cell CreateCellWithScriptedPlaced(SkyrimMod mod, string scriptName)
    {
        var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        cell.Persistent.Add(CreateScriptedPlacedObject(mod, scriptName));
        return cell;
    }

    private static string CompiledScriptPath(string scriptName) => Path.Combine("Scripts", $"{scriptName}.pex");

    private static void AddInteriorCell(SkyrimMod mod, Cell cell)
    {
        mod.Cells.Records.Add(new CellBlock()
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
                    Cells = { cell }
                }
            }
        });
    }

    [Theory, MutagenModAutoData]
    public void Shallow_IncludesOwnSubrecordAssets(
        SkyrimMod mod,
        string scriptName)
    {
        var placed = CreateScriptedPlacedObject(mod, scriptName);

        placed.EnumerateInferredAssetLinks(iterateNestedRecords: false)
            .Select(x => x.DataRelativePath.Path)
            .ShouldContain(CompiledScriptPath(scriptName));
    }

    [Theory, MutagenModAutoData]
    public void NestedRecordList_ExcludedWhenShallow_IncludedWhenDeep(
        SkyrimMod mod,
        string scriptName)
    {
        var cell = CreateCellWithScriptedPlaced(mod, scriptName);

        cell.EnumerateInferredAssetLinks(iterateNestedRecords: false)
            .ShouldBeEmpty();
        cell.EnumerateInferredAssetLinks(iterateNestedRecords: true)
            .Select(x => x.DataRelativePath.Path)
            .ShouldContain(CompiledScriptPath(scriptName));
    }

    [Theory, MutagenModAutoData]
    public void NestedRecordField_ExcludedWhenShallow_OwnListedAssetsRetained(
        SkyrimMod mod,
        string scriptName)
    {
        var worldspace = mod.Worldspaces.AddNew();
        worldspace.MapImage = new AssetLink<SkyrimTextureAssetType>("MapImage.dds");
        worldspace.TopCell = CreateCellWithScriptedPlaced(mod, scriptName);

        var shallowLinks = worldspace.EnumerateAssetLinks(
                AssetLinkQuery.Listed | AssetLinkQuery.Inferred,
                iterateNestedRecords: false)
            .Select(x => x.DataRelativePath.Path)
            .ToHashSet();

        shallowLinks.ShouldContain(worldspace.MapImage.DataRelativePath.Path);
        shallowLinks.ShouldNotContain(CompiledScriptPath(scriptName));
    }

    [Theory, MutagenModAutoData]
    public void Default_MatchesDeep(
        SkyrimMod mod,
        string scriptName)
    {
        var cell = CreateCellWithScriptedPlaced(mod, scriptName);

        cell.EnumerateInferredAssetLinks()
            .Select(x => x.DataRelativePath.Path)
            .ShouldBe(cell.EnumerateInferredAssetLinks(iterateNestedRecords: true)
                .Select(x => x.DataRelativePath.Path));
    }

    [Theory, MutagenModAutoData]
    public void ListedSetterEnumeration_ShallowIncludesOwnAssets(
        SkyrimMod mod)
    {
        var worldspace = mod.Worldspaces.AddNew();
        worldspace.MapImage = new AssetLink<SkyrimTextureAssetType>("MapImage.dds");

        IAssetLinkContainer container = worldspace;
        container.EnumerateListedAssetLinks(iterateNestedRecords: false)
            .Select(x => x.DataRelativePath.Path)
            .ShouldContain(worldspace.MapImage.DataRelativePath.Path);
    }

    [Theory, MutagenModAutoData]
    public void Mod_ShallowWalksGroupsButExcludesCellContents(
        SkyrimMod mod,
        string questScript,
        string placedScript)
    {
        var quest = mod.Quests.AddNew();
        quest.VirtualMachineAdapter = new QuestAdapter()
        {
            Scripts = new ExtendedList<ScriptEntry>()
            {
                new ScriptEntry() { Name = questScript }
            }
        };
        AddInteriorCell(mod, CreateCellWithScriptedPlaced(mod, placedScript));

        var shallowLinks = mod.EnumerateInferredAssetLinks(iterateNestedRecords: false)
            .Select(x => x.DataRelativePath.Path)
            .ToHashSet();
        var deepLinks = mod.EnumerateInferredAssetLinks(iterateNestedRecords: true)
            .Select(x => x.DataRelativePath.Path)
            .ToHashSet();

        shallowLinks.ShouldContain(CompiledScriptPath(questScript));
        shallowLinks.ShouldNotContain(CompiledScriptPath(placedScript));

        deepLinks.ShouldContain(CompiledScriptPath(questScript));
        deepLinks.ShouldContain(CompiledScriptPath(placedScript));
    }

    [Theory, MutagenModAutoData]
    public void Overlay_NestedRecordList_ExcludedWhenShallow_IncludedWhenDeep(
        SkyrimMod mod,
        string scriptName)
    {
        AddInteriorCell(mod, CreateCellWithScriptedPlaced(mod, scriptName));

        using var tmp = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath, suffix: ".esp");
        var path = new ModPath(mod.ModKey, tmp.File.Path);
        mod.BeginWrite
            .ToPath(path)
            .WithNoLoadOrder()
            .NoModKeySync()
            .Write();

        using var overlay = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimSE);
        var overlayCell = overlay.EnumerateMajorRecords<ICellGetter>().Single();

        overlayCell.EnumerateInferredAssetLinks(iterateNestedRecords: false)
            .ShouldBeEmpty();
        overlayCell.EnumerateInferredAssetLinks(iterateNestedRecords: true)
            .Select(x => x.DataRelativePath.Path)
            .ShouldContain(CompiledScriptPath(scriptName));
    }
}
