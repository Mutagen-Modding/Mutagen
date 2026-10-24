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
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();

        using var reread = SkyrimMod.Create(mod.SkyrimRelease)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();

        reread.Perks.ShouldHaveSingleItem().Effects.ShouldHaveSingleItem()
            .ShouldBeAssignableTo<IPerkAbilityEffectGetter>().ShouldNotBeNull()
            .ButtonLabel.ShouldNotBeNull().String.ShouldBe(buttonLabel);
    }
}
