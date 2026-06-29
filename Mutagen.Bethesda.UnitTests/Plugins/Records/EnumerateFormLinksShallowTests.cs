using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class EnumerateFormLinksShallowTests
{
    #region Cell Tests

    [Theory, MutagenModAutoData]
    public void Cell_ShallowEnumeration_IncludesDirectFormLinks(
        SkyrimMod mod,
        FormKey lightingKey,
        FormKey locationKey,
        FormKey waterKey)
    {
        var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        cell.LightingTemplate.SetTo(lightingKey);
        cell.Location.SetTo(locationKey);
        cell.Water.SetTo(waterKey);

        var shallowLinks = cell.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToHashSet();

        shallowLinks.ShouldContain(lightingKey);
        shallowLinks.ShouldContain(locationKey);
        shallowLinks.ShouldContain(waterKey);
    }

    [Theory, MutagenModAutoData]
    public void Cell_ShallowEnumeration_ExcludesPersistentPlacedLinks(
        SkyrimMod mod,
        FormKey lightingKey,
        FormKey placedBaseKey)
    {
        var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        cell.LightingTemplate.SetTo(lightingKey);

        var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedNpc.Base.SetTo(placedBaseKey);
        cell.Persistent.Add(placedNpc);

        var shallowLinks = cell.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToHashSet();

        shallowLinks.ShouldContain(lightingKey);
        shallowLinks.ShouldNotContain(placedBaseKey);
    }

    [Theory, MutagenModAutoData]
    public void Cell_ShallowEnumeration_ExcludesTemporaryPlacedLinks(
        SkyrimMod mod,
        FormKey placedBaseKey)
    {
        var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);

        var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedNpc.Base.SetTo(placedBaseKey);
        cell.Temporary.Add(placedNpc);

        var shallowLinks = cell.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToHashSet();

        shallowLinks.ShouldNotContain(placedBaseKey);
    }

    [Theory, MutagenModAutoData]
    public void Cell_DeepEnumeration_IncludesPlacedObjectLinks(
        SkyrimMod mod,
        FormKey lightingKey,
        FormKey placedBaseKey)
    {
        var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        cell.LightingTemplate.SetTo(lightingKey);

        var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedNpc.Base.SetTo(placedBaseKey);
        cell.Persistent.Add(placedNpc);

        var deepLinks = cell.EnumerateFormLinks(iterateNestedRecords: true)
            .Select(x => x.FormKey)
            .ToHashSet();

        deepLinks.ShouldContain(lightingKey);
        deepLinks.ShouldContain(placedBaseKey);
    }

    [Theory, MutagenModAutoData]
    public void Cell_DefaultParameter_MatchesDeepEnumeration(
        SkyrimMod mod,
        FormKey lightingKey,
        FormKey placedBaseKey)
    {
        var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        cell.LightingTemplate.SetTo(lightingKey);

        var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedNpc.Base.SetTo(placedBaseKey);
        cell.Temporary.Add(placedNpc);

        var defaultLinks = cell.EnumerateFormLinks()
            .Select(x => x.FormKey)
            .ToHashSet();
        var deepLinks = cell.EnumerateFormLinks(iterateNestedRecords: true)
            .Select(x => x.FormKey)
            .ToHashSet();

        defaultLinks.SetEquals(deepLinks).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void Cell_DeepHasMoreLinksThanShallow(
        SkyrimMod mod,
        FormKey lightingKey,
        FormKey placedBaseKey)
    {
        var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        cell.LightingTemplate.SetTo(lightingKey);

        var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedNpc.Base.SetTo(placedBaseKey);
        cell.Persistent.Add(placedNpc);

        var shallowCount = cell.EnumerateFormLinks(iterateNestedRecords: false).Count();
        var deepCount = cell.EnumerateFormLinks(iterateNestedRecords: true).Count();

        deepCount.ShouldBeGreaterThan(shallowCount);
    }

    #endregion

    #region DialogTopic Tests

    [Theory, MutagenModAutoData]
    public void DialogTopic_ShallowEnumeration_IncludesDirectFormLinks(
        SkyrimMod mod,
        FormKey branchKey,
        FormKey questKey)
    {
        var topic = new DialogTopic(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        topic.Branch.SetTo(branchKey);
        topic.Quest.SetTo(questKey);

        var shallowLinks = topic.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToHashSet();

        shallowLinks.ShouldContain(branchKey);
        shallowLinks.ShouldContain(questKey);
    }

    [Theory, MutagenModAutoData]
    public void DialogTopic_ShallowEnumeration_ExcludesResponseLinks(
        SkyrimMod mod,
        FormKey branchKey,
        FormKey speakerKey)
    {
        var topic = new DialogTopic(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        topic.Branch.SetTo(branchKey);

        var response = new DialogResponses(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        response.Speaker.SetTo(speakerKey);
        topic.Responses.Add(response);

        var shallowLinks = topic.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToHashSet();

        shallowLinks.ShouldContain(branchKey);
        shallowLinks.ShouldNotContain(speakerKey);
    }

    [Theory, MutagenModAutoData]
    public void DialogTopic_DeepEnumeration_IncludesResponseLinks(
        SkyrimMod mod,
        FormKey branchKey,
        FormKey speakerKey)
    {
        var topic = new DialogTopic(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        topic.Branch.SetTo(branchKey);

        var response = new DialogResponses(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        response.Speaker.SetTo(speakerKey);
        topic.Responses.Add(response);

        var deepLinks = topic.EnumerateFormLinks(iterateNestedRecords: true)
            .Select(x => x.FormKey)
            .ToHashSet();

        deepLinks.ShouldContain(branchKey);
        deepLinks.ShouldContain(speakerKey);
    }

    #endregion

    #region Mod-level Tests

    [Theory, MutagenModAutoData]
    public void Mod_ShallowEnumeration_ExcludesCellsFromSubBlocks(
        SkyrimMod mod,
        FormKey lightingKey,
        FormKey placedBaseKey)
    {
        var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        cell.LightingTemplate.SetTo(lightingKey);

        var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedNpc.Base.SetTo(placedBaseKey);
        cell.Persistent.Add(placedNpc);

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

        var shallowLinks = mod.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToHashSet();
        var deepLinks = mod.EnumerateFormLinks(iterateNestedRecords: true)
            .Select(x => x.FormKey)
            .ToHashSet();

        // Shallow: cells are major records within sub-blocks, so they and their
        // contents are entirely excluded from shallow enumeration at the mod level
        shallowLinks.ShouldNotContain(lightingKey);
        shallowLinks.ShouldNotContain(placedBaseKey);

        // Deep: everything is included
        deepLinks.ShouldContain(lightingKey);
        deepLinks.ShouldContain(placedBaseKey);
    }

    [Theory, MutagenModAutoData]
    public void Mod_DeepEnumeration_HasMoreLinksThanShallow(
        SkyrimMod mod,
        FormKey classKey,
        FormKey lightingKey,
        FormKey placedBaseKey)
    {
        var npc = mod.Npcs.AddNew();
        npc.Class.SetTo(classKey);

        var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        cell.LightingTemplate.SetTo(lightingKey);
        var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedNpc.Base.SetTo(placedBaseKey);
        cell.Persistent.Add(placedNpc);

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

        var shallowCount = mod.EnumerateFormLinks(iterateNestedRecords: false).Count();
        var deepCount = mod.EnumerateFormLinks(iterateNestedRecords: true).Count();

        deepCount.ShouldBeGreaterThan(shallowCount);
    }

    [Theory, MutagenModAutoData]
    public void Mod_NpcLinks_IncludedInBothShallowAndDeep(
        SkyrimMod mod,
        FormKey classKey)
    {
        var npc = mod.Npcs.AddNew();
        npc.Class.SetTo(classKey);

        var shallowLinks = mod.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToHashSet();
        var deepLinks = mod.EnumerateFormLinks(iterateNestedRecords: true)
            .Select(x => x.FormKey)
            .ToHashSet();

        // Npc has no nested major records, so its links appear in both modes
        shallowLinks.ShouldContain(classKey);
        deepLinks.ShouldContain(classKey);
    }

    #endregion

    #region Worldspace Tests

    [Theory, MutagenModAutoData]
    public void Worldspace_ShallowEnumeration_IncludesDirectFormLinks(
        SkyrimMod mod,
        FormKey climateKey,
        FormKey musicKey)
    {
        var worldspace = new Worldspace(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        worldspace.Climate.SetTo(climateKey);
        worldspace.Music.SetTo(musicKey);

        var shallowLinks = worldspace.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToHashSet();

        shallowLinks.ShouldContain(climateKey);
        shallowLinks.ShouldContain(musicKey);
    }

    [Theory, MutagenModAutoData]
    public void Worldspace_ShallowEnumeration_ExcludesTopCellLinks(
        SkyrimMod mod,
        FormKey climateKey,
        FormKey lightingKey,
        FormKey placedBaseKey)
    {
        var worldspace = new Worldspace(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        worldspace.Climate.SetTo(climateKey);

        var topCell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        topCell.LightingTemplate.SetTo(lightingKey);

        var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedNpc.Base.SetTo(placedBaseKey);
        topCell.Persistent.Add(placedNpc);

        worldspace.TopCell = topCell;

        var shallowLinks = worldspace.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToHashSet();

        shallowLinks.ShouldContain(climateKey);
        shallowLinks.ShouldNotContain(lightingKey);
        shallowLinks.ShouldNotContain(placedBaseKey);
    }

    [Theory, MutagenModAutoData]
    public void Worldspace_DeepEnumeration_IncludesTopCellLinks(
        SkyrimMod mod,
        FormKey climateKey,
        FormKey lightingKey,
        FormKey placedBaseKey)
    {
        var worldspace = new Worldspace(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        worldspace.Climate.SetTo(climateKey);

        var topCell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        topCell.LightingTemplate.SetTo(lightingKey);

        var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        placedNpc.Base.SetTo(placedBaseKey);
        topCell.Persistent.Add(placedNpc);

        worldspace.TopCell = topCell;

        var deepLinks = worldspace.EnumerateFormLinks(iterateNestedRecords: true)
            .Select(x => x.FormKey)
            .ToHashSet();

        deepLinks.ShouldContain(climateKey);
        deepLinks.ShouldContain(lightingKey);
        deepLinks.ShouldContain(placedBaseKey);
    }

    #endregion

    #region Edge Cases

    [Theory, MutagenModAutoData]
    public void Record_WithNoNestedRecords_ShallowAndDeepAreEqual(
        Npc npc,
        FormKey classKey)
    {
        npc.Class.SetTo(classKey);

        var shallowLinks = npc.EnumerateFormLinks(iterateNestedRecords: false)
            .Select(x => x.FormKey)
            .ToList();
        var deepLinks = npc.EnumerateFormLinks(iterateNestedRecords: true)
            .Select(x => x.FormKey)
            .ToList();

        shallowLinks.Count.ShouldBe(deepLinks.Count);
        shallowLinks.ToHashSet().SetEquals(deepLinks.ToHashSet()).ShouldBeTrue();
    }

    #endregion
}
