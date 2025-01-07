using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class DeepCopyTests
{
    [Theory, MutagenModAutoData]
    public void DeepCopyModKey(ModKey modKey)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.EnderalSE);
        var mod2 = mod.DeepCopy();
        mod2.ModKey.Should().Be(modKey);
        mod2.SkyrimRelease.Should().Be(SkyrimRelease.EnderalSE);
        mod2.GameRelease.Should().Be(GameRelease.EnderalSE);
    }
}