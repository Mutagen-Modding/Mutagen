using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Utility;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class MajorRecordInstantiatorTests
{
    [Fact]
    public void DirectGeneric()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator<Oblivion.Ammunition>.Activator(form, GameRelease.Oblivion);
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void SetterGeneric()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator<Oblivion.IAmmunition>.Activator(form, GameRelease.Oblivion);
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void GetterGeneric()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator<Oblivion.IAmmunitionGetter>.Activator(form, GameRelease.Oblivion);
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }
        
    [Fact]
    public void Direct()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator.Activator(form, GameRelease.Oblivion, typeof(Oblivion.Ammunition));
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void Setter()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator.Activator(form, GameRelease.Oblivion, typeof(Oblivion.IAmmunition));
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }

    [Fact]
    public void Getter()
    {
        Warmup.Init();
        var form = new FormKey(Mutagen.Bethesda.Oblivion.Constants.Oblivion, 0x123456);
        var ret = MajorRecordInstantiator.Activator(form, GameRelease.Oblivion, typeof(Oblivion.IAmmunitionGetter));
        Assert.IsType<Oblivion.Ammunition>(ret);
        Assert.Equal(form, ret.FormKey);
    }
}