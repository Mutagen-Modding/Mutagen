using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class NpcAttacksTests
{
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void NpcAttacksRoundTrip(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory,
        DirectoryPath existingRewriteDirectory,
        StarfieldMod mod)
    {
        var withAttacks = mod.Npcs.AddNew();
        var withoutAttacks = mod.Npcs.AddNew();
        withAttacks.Attacks.Add(new Attack
        {
            AttackData = new AttackData
            {
                DamageMult = 5f,
                Chance = 1f,
                Flags = AttackData.Flag.OverrideData,
                StrikeAngle = 70f,
                ActionPointsMult = 1f,
            },
            AttackEvent = "MeleeAttack05",
        });
        withAttacks.Attacks.Add(new Attack
        {
            AttackData = new AttackData { DamageMult = 2f, Chance = 0.5f },
            AttackEvent = "MeleeAttackPower01",
        });

        var path = new ModPath(mod.ModKey, Path.Combine(existingDirectory, mod.ModKey.FileName));
        mod.BeginWrite
            .ToPath(path)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();

        using var overlay = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(path)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Construct();
        ShouldHoldTheAttacks(overlay, withAttacks, withoutAttacks);

        var rewritePath = new ModPath(mod.ModKey, Path.Combine(existingRewriteDirectory, mod.ModKey.FileName));
        overlay.BeginWrite
            .ToPath(rewritePath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        var rewritten = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(rewritePath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        ShouldHoldTheAttacks(rewritten, withAttacks, withoutAttacks);
    }

    private static void ShouldHoldTheAttacks(
        IStarfieldModGetter mod,
        INpcGetter withAttacks,
        INpcGetter withoutAttacks)
    {
        var npcs = mod.Npcs.Records.ToDictionary(x => x.FormKey);
        var attacks = npcs[withAttacks.FormKey].Attacks;
        attacks.Count.ShouldBe(2);
        attacks[0].AttackEvent.ShouldBe("MeleeAttack05");
        var data = attacks[0].AttackData.ShouldNotBeNull();
        data.DamageMult.ShouldBe(5f);
        data.Chance.ShouldBe(1f);
        data.Flags.ShouldBe(AttackData.Flag.OverrideData);
        data.StrikeAngle.ShouldBe(70f);
        data.ActionPointsMult.ShouldBe(1f);
        attacks[1].AttackEvent.ShouldBe("MeleeAttackPower01");
        attacks[1].AttackData.ShouldNotBeNull().Chance.ShouldBe(0.5f);
        npcs[withoutAttacks.FormKey].Attacks.ShouldBeEmpty();
    }
}
