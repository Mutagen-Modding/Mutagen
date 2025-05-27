﻿using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class OverriddenFormsWritingTests
{
    #region Helpers

    private void TestExpectedOverriddenForms(
        IFileSystem fileSystem,
        ModPath existingModPath,
        IModGetter mod,
        IModGetter master,
        params FormKey[] expectedFormkeys)
    {
        mod.BeginWrite
            .ToPath(existingModPath)
            .WithLoadOrderFromHeaderMasters()
            .WithKnownMasters(master)
            .WithFileSystem(fileSystem)
            .NoModKeySync()
            .WithOverriddenFormsOption(OverriddenFormsOption.Iterate)
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(existingModPath)
            .WithKnownMasters(master)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.ModHeader.OverriddenForms.ShouldNotBeNull();
        reimport.ModHeader.OverriddenForms!
            .Select(x => x.FormKey)
            .ShouldBe(expectedFormkeys);
    }

    private void ConstructCells(
        StarfieldMod mod,
        out Cell internalCell, out Cell worldspaceCell)
    {
        internalCell = new Cell(mod);
        mod.Cells.Add(new CellBlock()
        {
            SubBlocks = new ExtendedList<CellSubBlock>()
            {
                new CellSubBlock()
                {
                    Cells = new ExtendedList<Cell>()
                    {
                        internalCell
                    }
                }
            }
        });

        worldspaceCell = new Cell(mod);
        var worldspace = mod.Worldspaces.AddReturn(new Worldspace(mod));
        worldspace.SubCells.Add(new WorldspaceBlock()
        {
            Items = new ExtendedList<WorldspaceSubBlock>()
            {
                new WorldspaceSubBlock()
                {
                    Items = new ExtendedList<Cell>()
                    {
                        worldspaceCell
                    }
                }
            }
        });
    }

    #endregion

    [Theory, MutagenAutoData]
    public void OverriddenFormsNoCheck(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod2 = new StarfieldMod(TestConstants.PluginModKey2, StarfieldRelease.Starfield);
        var masterNpc = mod2.Npcs.AddNew();
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield);
        mod.Npcs.GetOrAddAsOverride(masterNpc);
        mod.ModHeader.OverriddenForms ??= new();
        var fk = new FormKey(TestConstants.PluginModKey2, 0x123456);
        mod.ModHeader.OverriddenForms.Add(fk);

        mod.BeginWrite
            .ToPath(existingModPath)
            .WithLoadOrderFromHeaderMasters()
            .WithKnownMasters(mod2)
            .WithFileSystem(fileSystem)
            .NoModKeySync()
            .WithOverriddenFormsOption(OverriddenFormsOption.NoCheck)
            .Write();

        using var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(existingModPath)
            .WithKnownMasters(mod2)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.ModHeader.OverriddenForms.ShouldNotBeNull();
        reimport.ModHeader.OverriddenForms!
            .Select(x => x.FormKey)
            .ShouldEqualEnumerable(fk);
    }

    /*[Theory, MutagenAutoData]
    public void OverriddenFormsDoesNotIncludeTopLevelRecord(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod2 = new StarfieldMod(TestConstants.PluginModKey2, StarfieldRelease.Starfield);
        var masterNpc = mod2.Npcs.AddNew();

        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield);
        mod.Npcs.GetOrAddAsOverride(masterNpc);

        TestExpectedOverriddenForms(fileSystem, existingModPath, mod);
    }

    [Theory, MutagenAutoData]
    public void OverriddenFormsIterateNukesExistingContent(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield);
        mod.ModHeader.OverriddenForms ??= new();
        var fk = new FormKey(TestConstants.PluginModKey2, 0x123456);
        mod.ModHeader.OverriddenForms.Add(fk);

        TestExpectedOverriddenForms(fileSystem, existingModPath, mod);
    }*/

    [Theory, MutagenAutoData]
    public void OverriddenFormsCellsPlacedIncluded(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod2 = new StarfieldMod(TestConstants.PluginModKey2, StarfieldRelease.Starfield);
        ConstructCells(mod2, out var internalCell, out var worldspaceCell);
        var masterPlaced = internalCell.Temporary.AddReturn(new PlacedObject(mod2));
        var worldspacePlaced = worldspaceCell.Temporary.AddReturn(new PlacedObject(mod2));

        var mod2LinkCache = mod2.ToImmutableLinkCache();

        var masterPlacedContext =
            mod2LinkCache.ResolveContext<IPlacedObject, IPlacedObjectGetter>(masterPlaced.FormKey);
        var masterWorldspacePlacedContext =
            mod2LinkCache.ResolveContext<IPlacedObject, IPlacedObjectGetter>(worldspacePlaced.FormKey);

        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield);
        masterPlacedContext.GetOrAddAsOverride(mod);
        masterWorldspacePlacedContext.GetOrAddAsOverride(mod);
        TestExpectedOverriddenForms(
            fileSystem,
            existingModPath,
            mod,
            mod2,
            masterPlaced.FormKey,
            worldspacePlaced.FormKey);
    }

    [Theory, MutagenAutoData]
    public void OverriddenFormsCellsDialogsIncluded(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod2 = new StarfieldMod(TestConstants.PluginModKey2, StarfieldRelease.Starfield);
        var quest = mod2.Quests.AddNew();
        var dialogTopic = quest.DialogTopics.AddReturn(new DialogTopic(mod2));
        var dialogResponse = dialogTopic.Responses.AddReturn(new DialogResponses(mod2));
        
        var mod2LinkCache = mod2.ToImmutableLinkCache();

        var dialogResponseContext =
            mod2LinkCache.ResolveContext<IDialogResponses, IDialogResponsesGetter>(dialogResponse.FormKey);

        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield);
        dialogResponseContext.GetOrAddAsOverride(mod);
        TestExpectedOverriddenForms(
            fileSystem,
            existingModPath,
            mod,
            mod2,
            dialogTopic.FormKey,
            dialogResponse.FormKey);
    }

    [Theory, MutagenAutoData]
    public void OverriddenFormsCellsScenesIncluded(
        IFileSystem fileSystem,
        ModPath existingModPath)
    {
        StarfieldMod mod2 = new StarfieldMod(TestConstants.PluginModKey2, StarfieldRelease.Starfield);
        var quest = mod2.Quests.AddNew();
        var scene = quest.Scenes.AddReturn(new Scene(mod2));
        
        var mod2LinkCache = mod2.ToImmutableLinkCache();

        var sceneContext =
            mod2LinkCache.ResolveContext<IScene, ISceneGetter>(scene.FormKey);

        StarfieldMod mod = new StarfieldMod(TestConstants.PluginModKey, StarfieldRelease.Starfield);
        sceneContext.GetOrAddAsOverride(mod);
        TestExpectedOverriddenForms(
            fileSystem,
            existingModPath,
            mod,
            mod2,
            scene.FormKey);
    }
}