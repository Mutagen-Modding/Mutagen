using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class CriticalDataFormVersionTests
{
    private const ushort LegendaryEditionFormVersion = 43;

    [Theory, MutagenModAutoData(GameRelease.SkyrimLE)]
    public void Overlay_CriticalDataBeforeFormVersion44_HasNoUnused3(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory,
        SkyrimMod mod,
        Weapon weapon,
        Spell spell)
    {
        weapon.FormVersion = LegendaryEditionFormVersion;
        weapon.Critical = new CriticalData();
        weapon.Critical.Effect.SetTo(spell);
        var modPath = new ModPath(mod.ModKey, Path.Combine(existingDirectory, mod.ModKey.FileName));
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();

        using var reread = SkyrimMod.Create(mod.SkyrimRelease)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();

        var critical = reread.Weapons.ShouldHaveSingleItem().Critical.ShouldNotBeNull();
        critical.Unused3.ShouldBe(0);
        critical.Effect.FormKey.ShouldBe(spell.FormKey);
    }
}
