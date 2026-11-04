using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class PerkFunctionParametersTests
{
    private static IStarfieldModDisposableGetter WriteAndReread(
        IFileSystem fileSystem,
        ModPath modPath,
        StarfieldMod mod)
    {
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();
        return StarfieldMod.Create(mod.StarfieldRelease)
            .FromPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Construct();
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void ActivateChoiceWithoutFlags_RereadsWithoutFlags(
        IFileSystem fileSystem,
        ModPath modPath,
        StarfieldMod mod,
        Perk perk)
    {
        perk.Ranks.Add(new PerkRank
        {
            Effects =
            {
                new PerkEntryPointAddActivateChoice
                {
                    Flags = null,
                },
            },
        });

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        reread.Perks.ShouldHaveSingleItem().Ranks.ShouldHaveSingleItem().Effects.ShouldHaveSingleItem()
            .ShouldBeAssignableTo<IPerkEntryPointAddActivateChoiceGetter>().ShouldNotBeNull()
            .Flags.ShouldBeNull();
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void ActivateChoiceWithFlags_RereadsThem(
        IFileSystem fileSystem,
        ModPath modPath,
        StarfieldMod mod,
        Perk perk,
        string buttonLabel,
        APerkEffect.Flag flags)
    {
        perk.Ranks.Add(new PerkRank
        {
            Effects =
            {
                new PerkEntryPointAddActivateChoice
                {
                    ButtonLabel = buttonLabel,
                    Flags = flags,
                },
            },
        });

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        var choice = reread.Perks.ShouldHaveSingleItem().Ranks.ShouldHaveSingleItem().Effects.ShouldHaveSingleItem()
            .ShouldBeAssignableTo<IPerkEntryPointAddActivateChoiceGetter>().ShouldNotBeNull();
        choice.ButtonLabel.ShouldNotBeNull().String.ShouldBe(buttonLabel);
        choice.Flags.ShouldBe(flags);
    }

    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void AbilityCarryingFunctionParameters_RereadsThem(
        IFileSystem fileSystem,
        ModPath modPath,
        StarfieldMod mod,
        Perk perk,
        ushort perkEntryId,
        string buttonLabel,
        APerkEffect.Flag flags)
    {
        perk.Ranks.Add(new PerkRank
        {
            Effects =
            {
                new PerkAbilityEffect
                {
                    PerkEntryID = perkEntryId,
                    ButtonLabel = buttonLabel,
                    Flags = flags,
                },
            },
        });

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        var ability = reread.Perks.ShouldHaveSingleItem().Ranks.ShouldHaveSingleItem().Effects.ShouldHaveSingleItem()
            .ShouldBeAssignableTo<IPerkAbilityEffectGetter>().ShouldNotBeNull();
        ability.PerkEntryID.ShouldBe(perkEntryId);
        ability.ButtonLabel.ShouldNotBeNull().String.ShouldBe(buttonLabel);
        ability.Flags.ShouldBe(flags);
    }
}
