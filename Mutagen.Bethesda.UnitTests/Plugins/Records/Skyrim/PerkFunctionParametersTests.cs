using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class PerkFunctionParametersTests
{
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void AbilityWithAButtonLabel_RereadsIt(
        IFileSystem fileSystem,
        ModPath modPath,
        SkyrimMod mod,
        Perk perk,
        string buttonLabel)
    {
        perk.Effects.Add(new PerkAbilityEffect
        {
            ButtonLabel = buttonLabel,
        });

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        SingleEffect(reread)
            .ShouldBeAssignableTo<IPerkAbilityEffectGetter>().ShouldNotBeNull()
            .ButtonLabel.ShouldNotBeNull().String.ShouldBe(buttonLabel);
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void AbilityWithFlags_RereadsThem(
        IFileSystem fileSystem,
        ModPath modPath,
        SkyrimMod mod,
        Perk perk,
        ushort fragmentIndex)
    {
        perk.Effects.Add(new PerkAbilityEffect
        {
            Flags = new PerkScriptFlag
            {
                Flags = PerkScriptFlag.Flag.ReplaceDefault,
                FragmentIndex = fragmentIndex,
            },
        });

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        var flags = SingleEffect(reread).Flags.ShouldNotBeNull();

        flags.Flags.ShouldBe(PerkScriptFlag.Flag.ReplaceDefault);
        flags.FragmentIndex.ShouldBe(fragmentIndex);
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void AbilityWithoutFlags_RereadsWithoutFlags(
        IFileSystem fileSystem,
        ModPath modPath,
        SkyrimMod mod,
        Perk perk)
    {
        perk.Effects.Add(new PerkAbilityEffect());

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        SingleEffect(reread).Flags.ShouldBeNull();
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void ActivateChoiceWithFlags_RereadsThem(
        IFileSystem fileSystem,
        ModPath modPath,
        SkyrimMod mod,
        Perk perk,
        ushort fragmentIndex)
    {
        perk.Effects.Add(new PerkEntryPointAddActivateChoice
        {
            Flags = new PerkScriptFlag
            {
                Flags = PerkScriptFlag.Flag.RunImmediately,
                FragmentIndex = fragmentIndex,
            },
        });

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        var flags = SingleEffect(reread)
            .ShouldBeAssignableTo<IPerkEntryPointAddActivateChoiceGetter>().ShouldNotBeNull()
            .Flags.ShouldNotBeNull();
        flags.Flags.ShouldBe(PerkScriptFlag.Flag.RunImmediately);
        flags.FragmentIndex.ShouldBe(fragmentIndex);
    }

    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void ActivateChoiceWithoutFlags_RereadsWithoutFlags(
        IFileSystem fileSystem,
        ModPath modPath,
        SkyrimMod mod,
        Perk perk)
    {
        perk.Effects.Add(new PerkEntryPointAddActivateChoice());

        using var reread = WriteAndReread(fileSystem, modPath, mod);

        SingleEffect(reread)
            .ShouldBeAssignableTo<IPerkEntryPointAddActivateChoiceGetter>().ShouldNotBeNull()
            .Flags.ShouldBeNull();
    }

    private static ISkyrimModDisposableGetter WriteAndReread(IFileSystem fileSystem, ModPath modPath, SkyrimMod mod)
    {
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();

        return SkyrimMod.Create(mod.SkyrimRelease)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
    }

    private static IAPerkEffectGetter SingleEffect(ISkyrimModGetter mod) =>
        mod.Perks.ShouldHaveSingleItem().Effects.ShouldHaveSingleItem();
}
