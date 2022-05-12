using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class GetTopLevelGroupTests
{
    [Fact]
    public void GetTopLevelGroupByGenericDoesNotThrow()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var g = mod.GetTopLevelGroup<Npc>();
        var g2 = mod.GetTopLevelGroup<INpcGetter>();
    }
        
    // IGroup is not covariant, so this won't work
    [Fact]
    public void GetTopLevelGroupByGenericISetterThrows()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        Assert.Throws<InvalidCastException>(() =>
        {
            mod.GetTopLevelGroup<INpc>();
        });
    }
        
    [Fact]
    public void GetTopLevelGroupByParameterDoesNotThrow()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var g = mod.GetTopLevelGroup(typeof(Npc));
        var g2 = mod.GetTopLevelGroup(typeof(INpc));
        Assert.True(g == g2);
        var g3 = mod.GetTopLevelGroup(typeof(INpcGetter));
        Assert.True(g == g3);
    }
        
    [Fact]
    public void GetOverlayTopLevelGroupByGenericDoesNotThrow()
    {
        var mod = SkyrimMod.CreateFromBinaryOverlay(TestDataPathing.SkyrimTestMod, SkyrimRelease.SkyrimSE);
        mod.GetTopLevelGroup<INpcGetter>();
    }
        
    [Fact]
    public void GetOverlayTopLevelGroupByParameterDoesNotThrow()
    {
        var mod = SkyrimMod.CreateFromBinaryOverlay(TestDataPathing.SkyrimTestMod, SkyrimRelease.SkyrimSE);
        mod.GetTopLevelGroup(typeof(INpcGetter));
    }
}