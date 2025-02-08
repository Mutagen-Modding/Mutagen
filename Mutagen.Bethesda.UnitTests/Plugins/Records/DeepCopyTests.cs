using Shouldly;
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
        mod2.ModKey.ShouldBe(modKey);
        mod2.SkyrimRelease.ShouldBe(SkyrimRelease.EnderalSE);
        mod2.GameRelease.ShouldBe(GameRelease.EnderalSE);
    }
}