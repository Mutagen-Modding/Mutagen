using System.IO.Abstractions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class PerkFunctionParametersTests
{
    private static IFallout4ModDisposableGetter WriteAndReread(
        IFileSystem fileSystem,
        ModPath modPath,
        Fallout4Mod mod)
    {
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();
        return Fallout4Mod.Create(mod.Fallout4Release)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void ActivateChoiceWithoutFlags_RereadsWithoutFlags(
        IFileSystem fileSystem,
        ModPath modPath,
        Fallout4Mod mod,
        Perk perk)
    {
        perk.Effects.Add(new PerkEntryPointAddActivateChoice
        {
            Flags = null,
        });

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        reread.Perks.ShouldHaveSingleItem().Effects.ShouldHaveSingleItem()
            .ShouldBeAssignableTo<IPerkEntryPointAddActivateChoiceGetter>().ShouldNotBeNull()
            .Flags.ShouldBeNull();
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void ActivateChoiceWithFlags_RereadsThem(
        IFileSystem fileSystem,
        ModPath modPath,
        Fallout4Mod mod,
        Perk perk,
        string buttonLabel,
        APerkEffect.Flag flags)
    {
        perk.Effects.Add(new PerkEntryPointAddActivateChoice
        {
            ButtonLabel = buttonLabel,
            Flags = flags,
        });

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        var choice = reread.Perks.ShouldHaveSingleItem().Effects.ShouldHaveSingleItem()
            .ShouldBeAssignableTo<IPerkEntryPointAddActivateChoiceGetter>().ShouldNotBeNull();
        choice.ButtonLabel.ShouldNotBeNull().String.ShouldBe(buttonLabel);
        choice.Flags.ShouldBe(flags);
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void AbilityCarryingFunctionParameters_RereadsThem(
        IFileSystem fileSystem,
        ModPath modPath,
        Fallout4Mod mod,
        Perk perk,
        ushort perkEntryId,
        string buttonLabel,
        APerkEffect.Flag flags)
    {
        perk.Effects.Add(new PerkAbilityEffect
        {
            PerkEntryID = perkEntryId,
            ButtonLabel = buttonLabel,
            Flags = flags,
        });

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        var ability = reread.Perks.ShouldHaveSingleItem().Effects.ShouldHaveSingleItem()
            .ShouldBeAssignableTo<IPerkAbilityEffectGetter>().ShouldNotBeNull();
        ability.PerkEntryID.ShouldBe(perkEntryId);
        ability.ButtonLabel.ShouldNotBeNull().String.ShouldBe(buttonLabel);
        ability.Flags.ShouldBe(flags);
    }
}
