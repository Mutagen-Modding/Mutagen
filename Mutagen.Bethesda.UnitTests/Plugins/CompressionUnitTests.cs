using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;
using StarfieldKeyword = Mutagen.Bethesda.Starfield.Keyword;
using StarfieldMod = Mutagen.Bethesda.Starfield.StarfieldMod;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class CompressionUnitTests
{
    [Theory, MutagenModAutoData]
    public void CompressedExport(
        IFileSystem fileSystem,
        ModPath filePath,
        SkyrimMod mod,
        Npc npc)
    {
        npc.EditorID = "Test123";
        npc.IsCompressed = true;
        mod.BeginWrite
            .ToPath(filePath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();
        using var reimport = SkyrimMod.Create(mod.SkyrimRelease)
            .FromPath(filePath)
            .WithFileSystem(fileSystem)
            .Construct();
        reimport.Npcs.Select(x => x.EditorID)
            .ShouldEqualEnumerable("Test123");
    }

    [Theory, MutagenModAutoData]
    public void DeletedCompressedExport(
        IFileSystem fileSystem,
        ModPath filePath,
        SkyrimMod mod,
        Npc npc)
    {
        npc.EditorID = "Test123";
        npc.IsCompressed = true;
        npc.IsDeleted = true;
        mod.BeginWrite
            .ToPath(filePath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();
        npc.IsCompressed.ShouldBeTrue();

        using var overlay = SkyrimMod.Create(mod.SkyrimRelease)
            .FromPath(filePath)
            .WithFileSystem(fileSystem)
            .Construct();
        var overlayNpc = overlay.Npcs.ShouldHaveSingleItem();
        overlayNpc.IsDeleted.ShouldBeTrue();
        overlayNpc.IsCompressed.ShouldBeFalse();

        var reimport = SkyrimMod.Create(mod.SkyrimRelease)
            .FromPath(filePath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        var reimportNpc = reimport.Npcs.ShouldHaveSingleItem();
        reimportNpc.IsDeleted.ShouldBeTrue();
        reimportNpc.IsCompressed.ShouldBeFalse();
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void DeletedCompressedExportStarfield(
        IFileSystem fileSystem,
        ModPath filePath,
        StarfieldMod mod,
        StarfieldKeyword keyword)
    {
        keyword.EditorID = "Test123";
        keyword.IsCompressed = true;
        keyword.IsDeleted = true;
        mod.BeginWrite
            .ToPath(filePath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();

        using var overlay = StarfieldMod.Create(mod.StarfieldRelease)
            .FromPath(filePath)
            .WithLoadOrderFromHeaderMasters()
            .WithNoDataFolder()
            .WithFileSystem(fileSystem)
            .Construct();
        var overlayKeyword = overlay.Keywords.ShouldHaveSingleItem();
        overlayKeyword.IsDeleted.ShouldBeTrue();
        overlayKeyword.IsCompressed.ShouldBeFalse();

        var reimport = StarfieldMod.Create(mod.StarfieldRelease)
            .FromPath(filePath)
            .WithLoadOrderFromHeaderMasters()
            .WithNoDataFolder()
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        var reimportKeyword = reimport.Keywords.ShouldHaveSingleItem();
        reimportKeyword.IsDeleted.ShouldBeTrue();
        reimportKeyword.IsCompressed.ShouldBeFalse();
    }
}