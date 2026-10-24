using System.Buffers.Binary;
using System.IO.Abstractions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;
using Fallout4Npc = Mutagen.Bethesda.Fallout4.Npc;
using Fallout4PerkPlacement = Mutagen.Bethesda.Fallout4.PerkPlacement;
using SkyrimNpc = Mutagen.Bethesda.Skyrim.Npc;
using SkyrimPerkPlacement = Mutagen.Bethesda.Skyrim.PerkPlacement;
using StarfieldNpc = Mutagen.Bethesda.Starfield.Npc;
using StarfieldPerkPlacement = Mutagen.Bethesda.Starfield.PerkPlacement;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class CountedListShortCountTests
{
    private const int PerksListed = 2;
    private static readonly RecordType PerkCount = new("PRKZ");

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void Skyrim_ACountBelowThePerksListed_ReadsEveryPerk(
        IFileSystem fileSystem,
        SkyrimMod mod,
        SkyrimNpc countsOne,
        SkyrimNpc countsNone,
        DirectoryPath existingDirectory)
    {
        countsOne.Perks = PerksOf<SkyrimPerkPlacement>();
        countsNone.Perks = PerksOf<SkyrimPerkPlacement>();
        var modPath = PathFor(mod, existingDirectory);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        OverwritePerkCount(fileSystem, modPath, mod.GameRelease, countsOne.FormKey, 1);
        OverwritePerkCount(fileSystem, modPath, mod.GameRelease, countsNone.FormKey, 0);

        var reimport = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        reimport.Npcs[countsOne.FormKey].Perks.ShouldNotBeNull().Count.ShouldBe(PerksListed);
        reimport.Npcs[countsNone.FormKey].Perks.ShouldNotBeNull().Count.ShouldBe(PerksListed);
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void Fallout4_ACountBelowThePerksListed_ReadsEveryPerk(
        IFileSystem fileSystem,
        Fallout4Mod mod,
        Fallout4Npc countsOne,
        Fallout4Npc countsNone,
        DirectoryPath existingDirectory)
    {
        countsOne.Perks = PerksOf<Fallout4PerkPlacement>();
        countsNone.Perks = PerksOf<Fallout4PerkPlacement>();
        var modPath = PathFor(mod, existingDirectory);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        OverwritePerkCount(fileSystem, modPath, mod.GameRelease, countsOne.FormKey, 1);
        OverwritePerkCount(fileSystem, modPath, mod.GameRelease, countsNone.FormKey, 0);

        var reimport = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        reimport.Npcs[countsOne.FormKey].Perks.ShouldNotBeNull().Count.ShouldBe(PerksListed);
        reimport.Npcs[countsNone.FormKey].Perks.ShouldNotBeNull().Count.ShouldBe(PerksListed);
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void Starfield_ACountBelowThePerksListed_ReadsEveryPerk(
        IFileSystem fileSystem,
        StarfieldMod mod,
        StarfieldNpc countsOne,
        StarfieldNpc countsNone,
        DirectoryPath existingDirectory)
    {
        countsOne.Perks = PerksOf<StarfieldPerkPlacement>();
        countsNone.Perks = PerksOf<StarfieldPerkPlacement>();
        var modPath = PathFor(mod, existingDirectory);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        OverwritePerkCount(fileSystem, modPath, mod.GameRelease, countsOne.FormKey, 1);
        OverwritePerkCount(fileSystem, modPath, mod.GameRelease, countsNone.FormKey, 0);

        var reimport = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        reimport.Npcs[countsOne.FormKey].Perks.ShouldNotBeNull().Count.ShouldBe(PerksListed);
        reimport.Npcs[countsNone.FormKey].Perks.ShouldNotBeNull().Count.ShouldBe(PerksListed);
    }

    private static ExtendedList<T> PerksOf<T>()
        where T : new()
    {
        return new ExtendedList<T>(Enumerable.Range(0, PerksListed).Select(_ => new T()));
    }

    private static ModPath PathFor(IModGetter mod, DirectoryPath directory) =>
        new(mod.ModKey, Path.Combine(directory, mod.ModKey.FileName));

    private static void OverwritePerkCount(IFileSystem fileSystem, ModPath modPath, GameRelease release, FormKey npc, uint count)
    {
        var bytes = fileSystem.File.ReadAllBytes(modPath);
        var recordStart = checked((int)RecordLocator.GetLocations(modPath, release, loadOrder: null, fileSystem: fileSystem)
            .GetRecord(npc).Location.Min);
        var perkCount = GameConstants.Get(release)
            .MajorRecord(new ReadOnlyMemorySlice<byte>(bytes).Slice(recordStart))
            .FindSubrecord(PerkCount);
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(recordStart + perkCount.ContentLocation), count);
        fileSystem.File.WriteAllBytes(modPath, bytes);
    }
}
